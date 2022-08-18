using AutoMapper;
using Dapper;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Medico.Application.Services
{
    public class SettingService : ISettingService
    {
        #region DI
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public SettingService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        #endregion

        public async Task<EditorConfigVM> UpdateEditorConfig(string id, EditorConfigVM editorConfig)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();
                try
                {
                    string q = "UPDATE EditorConfig SET FontFamily = @FontFamily, FontSize = @FontSize WHERE ID = @Id";
                    var result = await con.ExecuteAsync(q,
                    new
                    {
                        id,
                        editorConfig.FontFamily,
                        editorConfig.FontSize
                    });

                    if (result > 0)
                    {
                        string query = "SELECT CAST(ID AS NVARCHAR(50)) ID, FontFamily, FontSize FROM EditorConfig WHERE ID = @ID";
                        var config = await con.QueryFirstAsync<EditorConfigVM>(query, new { id, });

                        return config;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<EditorConfigVM> GetEditorConfig()
        {
            using (IDbConnection con = Connection)
            {
                con.Open();
                try
                {
                    string query = "SELECT CAST(ID AS NVARCHAR(50)) ID, FontFamily, FontSize FROM EditorConfig ";
                    var config = await con.QueryFirstAsync<EditorConfigVM>(query);

                    return config;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
