using Medico.Application.Interfaces;
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Medico.Application.ViewModels;
using Medico.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Medico.Application.Services
{
    public class VendorDataService : IVendorDataService
    {
        #region DI
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public VendorDataService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        #endregion

        #region Methods

        public async Task<int> Create(VendorDataViewModel vendorDataViewModel)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();


                VendorData vendorData = _mapper.Map<VendorData>(vendorDataViewModel);
                if (vendorData != null)
                {
                    int id = await con.InsertAsync(vendorData);
                    return id;
                }
                return 0;
            }
        }

        public async Task<bool> Update(int id,VendorDataViewModel vendorDataViewModel)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                VendorData vendorData = await con.GetAsync<VendorData>(id);
                vendorData = _mapper.Map<VendorData>(vendorDataViewModel);
                if (vendorData != null)
                {
                    bool result = await con.UpdateAsync(vendorData);
                    return result;
                }
                return false;
            }
        }

        public async  Task<IEnumerable<VendorDataViewModel>> GetVendorDdl()
        {
            using (IDbConnection con = Connection)
            {
                string query = @"SELECT * FROM VendorDatas order by VendorName ASC";

                IEnumerable<VendorDataViewModel> vendor = await con.QueryAsync<VendorDataViewModel>(query, new {});

                return vendor;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (IDbConnection con = Connection)
            {
                //Entity for Delete
                VendorData vendorData = await con.GetAsync<VendorData>(id);
                if (vendorData == null)
                {
                    throw new Exception("invalid record");
                }
                bool result = await con.DeleteAsync(vendorData);
                return result;
            }
        }
        #endregion
    }
}
