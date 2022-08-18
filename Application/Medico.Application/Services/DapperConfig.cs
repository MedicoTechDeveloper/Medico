using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Medico.Application.Services
{
    public class DapperConfig
    {
        private SqlConnection con = new SqlConnection("Server=DESKTOP-4MCPOOO;Database=medico_staging;");
        public SqlConnection Dapper()
        {
            return con;
        }
    }
}
