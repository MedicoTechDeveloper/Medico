using Dapper;
using Dapper.Contrib.Extensions;
using KellermanSoftware.CompareNetObjects;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels.Audit;
using Medico.Application.ViewModels.Enums;
using Medico.Domain.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Services
{
    public class AuditTrailService : IAuditTrailService
    {
        #region DI
        private readonly IConfiguration _configuration;
        private IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public AuditTrailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        public async Task CreateAuditTrail(AuditActionType Action, string KeyFieldID, string KeyFieldName, object OldObject, object NewObject)
        {
            try
            {
                // get the differance
                CompareLogic compObjects = new CompareLogic();
                compObjects.Config.MaxDifferences = 99;
                ComparisonResult compResult = compObjects.Compare(OldObject, NewObject);
                List<AuditDelta> DeltaList = new List<AuditDelta>();
                foreach (var change in compResult.Differences)
                {
                    AuditDelta delta = new AuditDelta();
                    if (!string.IsNullOrEmpty(change.PropertyName))
                    {
                        if (change.PropertyName.Substring(0, 1) == ".")
                            delta.FieldName = change.PropertyName.Substring(1, change.PropertyName.Length - 1);
                    }
                   
                    delta.ValueBefore = change.Object1Value;
                    delta.ValueAfter = change.Object2Value;
                    DeltaList.Add(delta);
                }
                AuditTable audit = new AuditTable
                {
                    Id = Guid.NewGuid(),
                    AuditActionTypeENUM = (int)Action,
                    DataModel = KeyFieldName, //this.GetType().Name,
                    DateTimeStamp = DateTime.Now,
                    KeyFieldID = KeyFieldID,
                    ValueBefore = JsonConvert.SerializeObject(OldObject), // if use xml instead of json, can use xml annotation to describe field names etc better
                    ValueAfter = JsonConvert.SerializeObject(NewObject),
                    Changes = JsonConvert.SerializeObject(DeltaList)
                };

                using (IDbConnection con = Connection)
                {
                    con.Open();
                    string q = @"INSERT INTO [dbo].[AuditTable]
                       ([Id]
                       ,[KeyFieldID]
                       ,[DateTimeStamp]
                       ,[DataModel]
                       ,[ValueBefore]
                       ,[ValueAfter]
                       ,[Changes]
                       ,[AuditActionTypeENUM])
                 VALUES
                       (NewId()
                       ,@KeyFieldID
                       ,@DateTimeStamp
                       ,@DataModel
                       ,@ValueBefore
                       ,@ValueAfter
                       ,@Changes
                       ,@AuditActionTypeENUM)";

                    await con.ExecuteAsync(q, audit);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IOrderedEnumerable<AuditTable>> GetAll(string id, string dataModel)
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    con.Open();
                    string q = @"SELECT 
                        CAST(ID AS VARCHAR(50)) ID
                       ,[KeyFieldID]
                       ,[DateTimeStamp]
                       ,[DataModel]
                       ,[ValueBefore]
                       ,[ValueAfter]
                       ,[Changes]
                       ,[AuditActionTypeENUM] FROM [dbo].[AuditTable] WHERE CAST([KeyFieldID] AS VARCHAR(50)) = @Id AND DataModel = @DataModel";

                    var x = await con.QueryAsync<AuditTable>(q, new { id, dataModel });

                    return x.OrderByDescending(s => s.DateTimeStamp);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
