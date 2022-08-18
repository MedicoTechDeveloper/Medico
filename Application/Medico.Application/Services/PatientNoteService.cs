using AutoMapper;
using Dapper;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels.Patient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Medico.Application.Services
{
    public class PatientNoteService : IPatientNoteService
    {
        #region DI
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public PatientNoteService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        #endregion

        public async Task<IEnumerable<PatientNoteVm>> Create(PatientPatchVm patientPatchVm)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                try
                {
                    string q = @"INSERT INTO PatientNote(Id, PatientId, Notes, CreatedOn, CreatedByName, Subject, [Status]) 
                                VALUES(NewId(), @PatientId, @Notes, GetUtcDate(), @CreatedByName, @Subject, @Status)";

                    int id = await con.ExecuteAsync(q,
                        new
                        {
                            PatientId = patientPatchVm.Id,
                            patientPatchVm.Notes,
                            patientPatchVm.CreatedByName,
                            patientPatchVm.Subject,
                            patientPatchVm.Status,
                        });

                    if (id > 0)
                    {
                        string query = @"SELECT CAST(ID AS NVARCHAR(50)) ID, Notes, CreatedOn, CreatedByName, Subject, [Status]
                                        FROM PatientNote WHERE PatientId = @PatientId ORDER BY CreatedOn DESC";

                        var result = await con.QueryAsync<PatientNoteVm>(query, new { PatientId = patientPatchVm.Id, });
                        return result;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<IEnumerable<PatientNoteVm>> GetNotes(string patientId, string fromDate, string toDate, string subject, string status, string employee)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();
                try
                {
                    string query = @"[dbo].[SearchPatientNotes]";

                    var result = await con.QueryAsync<PatientNoteVm>(query,
                        new
                        {
                            patientId,
                            fromDate,
                            toDate,
                            subject,
                            status,
                            employee
                        },
                        commandType: CommandType.StoredProcedure);
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<bool> EditNotes(string id, PatientPatchVm patientPatchVm)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                string q = @"UPDATE [dbo].[PatientNote]
                                   SET [Notes] = @Notes
                                      ,[Subject] = @Subject
                                      ,[Status] = @Status
                                      WHERE Id=@id";

                var data = await con.ExecuteAsync(q,
                        new
                        {
                            id,
                            patientPatchVm.Notes,
                            patientPatchVm.Subject,
                            patientPatchVm.Status
                        });
                return true;
            }
        }

        public async Task<PatientNoteVm> GetNotesById(Guid id)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();
                try
                {
                    string query = @"SELECT CAST(ID AS NVARCHAR(50)) ID, Notes, CreatedOn, CreatedBy, CreatedByName, Subject, [Status] FROM PatientNote 
                    WHERE ID = @id";

                    var result = await con.QuerySingleAsync<PatientNoteVm>(query, new { id });
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<bool> DeleteNote(string id)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                string q = @"Delete From [dbo].[PatientNote]
                                      WHERE Id=@id";

                var data = await con.ExecuteAsync(q,
                        new
                        {
                            id
                        });
                return true;
            }
        }
    }
}
