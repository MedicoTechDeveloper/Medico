using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Medico.Application.Services
{
    public class VitalSignsService : BaseDeletableByIdService<VitalSigns, VitalSignsViewModel>,
        IVitalSignsService
    {
        private readonly IVitalSignsRepository _vitalSignsRepository;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public VitalSignsService(IVitalSignsRepository repository,
            IConfiguration configuration,
            IUserService userService,
            IVitalSignsRepository vitalSignsRepository, IMapper mapper)
            : base(repository, mapper)
        {
            _vitalSignsRepository = vitalSignsRepository;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<bool> IsHistoryExist(Guid patientId)
        {
            var tobaccoHistory = await Repository.GetAll()
                .FirstOrDefaultAsync();

            return tobaccoHistory != null;
        }

        public Task Delete(Guid id)
        {
            return DeleteById(id);
        }

        public async Task<IEnumerable<VitalSignSearch>> GetAll(PatientAdmissionDxOptionsViewModel historyDxOptionsViewModel)
        {
            var vitalSignsList = await Repository.GetAll()
                .Where(th => th.PatientId == historyDxOptionsViewModel.PatientId
                             && th.AdmissionId == historyDxOptionsViewModel.AdmissionId)
                .ProjectTo<VitalSignsViewModel>()
                .ToListAsync();

            var users = _userService.GetAll().Where(c => c.RoleName != "Patient");

            var vitalSigns = from vs in vitalSignsList
                             join u in users on vs.CreatedBy equals u.Id.ToString() into gj
                             from subpet in gj.DefaultIfEmpty()
                             select new VitalSignSearch
                             {
                                 AdmissionId = vs.AdmissionId,
                                 BloodPressureLocation = vs.BloodPressureLocation,
                                 CreatedByName = $"{subpet?.FirstName} {subpet?.LastName}" ?? string.Empty,
                                 CreatedDate = vs.CreatedDate,
                                 DiastolicBloodPressure = vs.DiastolicBloodPressure,
                                 ModifiedBy = vs.ModifiedBy,
                                 ModifiedDate = vs.ModifiedDate,
                                 OxygenSaturationAtRest = vs.OxygenSaturationAtRest,
                                 OxygenSaturationAtRestValue = vs.OxygenSaturationAtRestValue,
                                 Pulse = vs.Pulse,
                                 RespirationRate = vs.RespirationRate,
                                 BloodPressurePosition = vs.BloodPressurePosition,
                                 PatientId = vs.PatientId,
                                 Id = vs.Id,
                                 SystolicBloodPressure = vs.SystolicBloodPressure
                             };

            return vitalSigns;
        }

        public async Task<IEnumerable<VitalSignsViewVM>> GetByPatientAndAdmissionIds(Guid patientId, Guid admissionId)
        {
            try
            {
                var vitalSignsList = await Repository.GetAll()
                        .Where(vs => vs.PatientId == patientId && vs.AdmissionId == admissionId)
                        .OrderByDescending(vs => vs.CreatedDate)
                        .ProjectTo<VitalSignsViewVM>()
                        .ToListAsync();

                return vitalSignsList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<VitalSignsViewModel> GetLastPatientVitalSigns(Guid patientId, DateTime createDate)
        {
            var vitalSigns = await Repository.GetAll()
                .Where(a => a.PatientId == patientId && a.CreatedDate < createDate)
                .OrderByDescending(a => a.CreatedDate)
                .FirstOrDefaultAsync();

            return vitalSigns == null
                ? null
                : Mapper.Map<VitalSignsViewModel>(vitalSigns);
        }

        public async Task<bool> IsDelete(UpdateIsDelete data, Guid id)
        {
            var vitalSigns = await Repository.GetAll().FirstOrDefaultAsync(a => a.Id == id);
            if (vitalSigns != null)
            {
                vitalSigns.IsDelete = data.IsDelete;
                await Repository.SaveChangesAsync();
                return true;
            }
            return true;
        }

        public async Task<VitalSignsViewModel> GetSingle(Guid id)
        {
            try
            {
                using (IDbConnection con = Connection)
                {
                    con.Open();
                    string q = @"SELECT Pulse, SystolicBloodPressure, DiastolicBloodPressure, BloodPressurePosition,
                                    BloodPressureLocation, OxygenSaturationAtRest, OxygenSaturationAtRestValue,
                                    RespirationRate FROM [dbo].[VitalSigns] WHERE CAST([ID] AS VARCHAR(50)) = @Id";

                    return await con.QueryFirstOrDefaultAsync<VitalSignsViewModel>(q, new { id });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}