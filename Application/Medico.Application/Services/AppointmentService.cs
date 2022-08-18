using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using DevExtreme.AspNet.Data;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Patient;
using Medico.Data.Context;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Medico.Application.Services
{
    public class AppointmentService : BaseDeletableByIdService<Appointment, AppointmentViewModel>, IAppointmentService
    {
        #region DI
        private readonly IAppointmentGridItemRepository _appointmentGridItemRepository;
        private readonly IAppointmentPatientChartDocumentRepository _appointmentPatientChartDocumentRepository;
        private readonly IPatientChartDocumentNodeRepository _patientChartDocumentNodeRepository;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly IUserService _userService;
        private readonly IPatientService _patientService;
        public IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public AppointmentService(IAppointmentGridItemRepository appointmentGridItemRepository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper,
            IConfiguration configuration,
            IHostingEnvironment env,
            IUserService userService,
            IPatientService patientService,
            IAppointmentPatientChartDocumentRepository appointmentPatientChartDocumentRepository,
            IPatientChartDocumentNodeRepository patientChartDocumentNodeRepository) : base(appointmentRepository,
            mapper)
        {
            _appointmentGridItemRepository = appointmentGridItemRepository;
            _appointmentPatientChartDocumentRepository = appointmentPatientChartDocumentRepository;
            _patientChartDocumentNodeRepository = patientChartDocumentNodeRepository;
            _configuration = configuration;
            _env = env;
            _userService = userService;
            _patientService = patientService;
        }
        #endregion

        public override async Task<AppointmentViewModel> Create(AppointmentViewModel viewModel)
        {
            var appointment = Mapper.Map<Appointment>(viewModel);
            await Repository.AddAsync(appointment);

            var patientChartDocumentNodeIds = viewModel.PatientChartDocumentNodes == null
                ? Enumerable.Empty<Guid>().ToList()
                : viewModel.PatientChartDocumentNodes.ToList();

            if (!patientChartDocumentNodeIds.Any())
            {
                //if patient chart document nodes are not specified during appointment creation,
                //all available company patient chart nodes will be added to the patient chart
                patientChartDocumentNodeIds = _patientChartDocumentNodeRepository.GetAll()
                    .Where(d => d.CompanyId == viewModel.CompanyId)
                    .Select(d => d.Id)
                    .ToList();
            }

            var appointmentId = appointment.Id;

            var patientChartDocumentNodes =
                patientChartDocumentNodeIds.Select(patientChartDocumentNodeId => new AppointmentPatientChartDocument
                { PatientChartDocumentNodeId = patientChartDocumentNodeId, AppointmentId = appointmentId });

            appointment.AddPatientChartDocumentNodes(patientChartDocumentNodes);

            await Repository.SaveChangesAsync();

            viewModel.Id = appointmentId;
            return viewModel;
        }

        public override async Task<AppointmentViewModel> Update(AppointmentViewModel viewModel)
        {
            var appointment = await Repository.GetAll()
                .Include(t => t.PatientChartDocumentNodes)
                .FirstOrDefaultAsync(t => t.Id == viewModel.Id);

            Mapper.Map(viewModel, appointment);

            Repository.Update(appointment);

            var appointmentId = appointment.Id;

            var patientChartDocumentNodeIds = viewModel.PatientChartDocumentNodes == null
                ? Enumerable.Empty<Guid>().ToList()
                : viewModel.PatientChartDocumentNodes.ToList();

            var patientChartDocumentNodes =
                patientChartDocumentNodeIds.Select(patientChartDocumentNodeId => new AppointmentPatientChartDocument
                { PatientChartDocumentNodeId = patientChartDocumentNodeId, AppointmentId = appointmentId });

            appointment.AddPatientChartDocumentNodes(patientChartDocumentNodes);

            await Repository.SaveChangesAsync();

            return viewModel;
        }

        public override async Task<AppointmentViewModel> GetById(Guid id)
        {
            var appointment = await Repository.GetAll()
                .Include(a => a.PatientChartDocumentNodes)
                .FirstOrDefaultAsync(a => a.Id == id);

            var appointmentViewModel = appointment == null
                ? null
                : Mapper.Map<AppointmentViewModel>(appointment);

            if (appointmentViewModel == null)
                return null;

            var appointmentPatientChartDocuments =
                appointment.PatientChartDocumentNodes;

            if (!appointmentPatientChartDocuments.Any())
                return appointmentViewModel;

            appointmentViewModel.PatientChartDocumentNodes =
                appointmentPatientChartDocuments.Select(d => d.PatientChartDocumentNodeId);

            return appointmentViewModel;
        }

        public async Task<AppointmentViewModel> GetByAdmissionId(Guid admissionId)
        {
            var appointment = await Repository.GetAll()
                .FirstOrDefaultAsync(a => a.AdmissionId == admissionId);

            return appointment == null
                ? null
                : Mapper.Map<AppointmentViewModel>(appointment);
        }

        public async Task<AppointmentGridItemViewModel> GetAppointmentGridItemById(Guid id)
        {
            var appointmentGridItem = await _appointmentGridItemRepository
                .GetAll()
                .FirstOrDefaultAsync(gi => gi.Id == id);

            return appointmentGridItem == null
                ? null
                : Mapper.Map<AppointmentGridItemViewModel>(appointmentGridItem);
        }

        public IQueryable<AppointmentGridItemViewModel> GetAllAppointmentGridItems(
            AppointmentDxOptionsViewModel dxOptions)
        {
            var companyId = dxOptions.CompanyId;
            if (companyId == Guid.Empty)
                return Enumerable.Empty<AppointmentGridItemViewModel>().AsQueryable();

            var query = _appointmentGridItemRepository.GetAll()
                .Where(a => a.CompanyId == companyId);

            query = ApplyIntervalFilter(dxOptions.StartDate, dxOptions.EndDate, query, out var isIntervalFilterApplied);
            query = ApplyFilter(dxOptions, query);

            dxOptions.Sort = new[]
            {
                new SortingInfo
                {
                    Desc = false,
                    Selector = "startDate"
                }
            };

            return query.ProjectTo<AppointmentGridItemViewModel>();
        }

        public IQueryable<AppointmentGridItemViewModel> GetAll(AppointmentDxOptionsViewModel dxOptions)
        {
            var companyId = dxOptions.CompanyId;
            if (companyId == Guid.Empty)
                return Enumerable.Empty<AppointmentGridItemViewModel>().AsQueryable();

            var query = _appointmentGridItemRepository.GetAll()
                .Where(a => a.CompanyId == companyId);

            query = ApplyIntervalFilter(dxOptions.StartDate, dxOptions.EndDate, query, out var isIntervalFilterApplied);
            query = ApplyFilter(dxOptions, query);

            var patientChartDocumentNodes =
                _appointmentPatientChartDocumentRepository.GetAll();

            return query.GroupJoin(patientChartDocumentNodes,
                appointmentGridItem => appointmentGridItem.Id,
                appointmentPatientChartDocumentNode => appointmentPatientChartDocumentNode.AppointmentId,
                (appointmentGridItem, appointmentPatientChartDocumentNodes) => new AppointmentGridItemViewModel
                {
                    Id = appointmentGridItem.Id,
                    Allegations = appointmentGridItem.Allegations,
                    AdmissionId = appointmentGridItem.AdmissionId,
                    CompanyId = appointmentGridItem.CompanyId,
                    AllegationsNotes = appointmentGridItem.AllegationsNotes,
                    StartDate = appointmentGridItem.StartDate,
                    Date = appointmentGridItem.Date,
                    EndDate = appointmentGridItem.EndDate,
                    AppointmentStatus = appointmentGridItem.AppointmentStatus,
                    LocationName = appointmentGridItem.LocationName,
                    LocationId = appointmentGridItem.LocationId,
                    RoomId = appointmentGridItem.RoomId,
                    RoomName = appointmentGridItem.RoomName,
                    PatientId = appointmentGridItem.PatientId,
                    PatientFirstName = appointmentGridItem.PatientFirstName,
                    PatientLastName = appointmentGridItem.PatientLastName,
                    PatientDateOfBirth = appointmentGridItem.PatientDateOfBirth,
                    PhysicianId = appointmentGridItem.PhysicianId,
                    PhysicianFirstName = appointmentGridItem.PhysicianFirstName,
                    PhysicianLastName = appointmentGridItem.PhysicianLastName,
                    NurseId = appointmentGridItem.NurseId,
                    NurseFirstName = appointmentGridItem.NurseFirstName,
                    NurseLastName = appointmentGridItem.NurseLastName,
                    TotalNumberOfPatientAppointments = appointmentGridItem.TotalNumberOfPatientAppointments,
                    SigningDate = appointmentGridItem.SigningDate,
                    PreviousAppointmentDate = appointmentGridItem.PreviousAppointmentDate,
                    PatientChartDocumentNodes =
                        appointmentPatientChartDocumentNodes.Select(d => d.PatientChartDocumentNodeId)
                });
        }

        public Task Delete(Guid id)
        {
            return DeleteById(id);
        }

        public async Task<AppointmentViewModel> GetPatientLastVisit(Guid patientId, DateTime currentDate)
        {
            var appointment = await Repository.GetAll()
                .Where(a => a.PatientId == patientId && a.StartDate < currentDate)
                .OrderByDescending(a => a.StartDate)
                .FirstOrDefaultAsync();
            return appointment == null
                ? null
                : Mapper.Map<AppointmentViewModel>(appointment);
        }

        public async Task<IEnumerable<AppointmentGridItemViewModel>> GetPatientPreviousVisits(Guid patientId,
            DateTime currentDate)
        {
            var previousAppointments = await _appointmentGridItemRepository.GetAll()
                .Where(a => a.PatientId == patientId && a.StartDate < currentDate)
                .OrderByDescending(a => a.StartDate).ProjectTo<AppointmentGridItemViewModel>()
                .ToListAsync();

            return previousAppointments;
        }

        public async Task<AppointmentViewModel> GetByLocationId(Guid locationId)
        {
            var appointment = await Repository.GetAll()
                .FirstOrDefaultAsync(a => a.LocationId == locationId);

            return appointment == null
                ? null
                : Mapper.Map<AppointmentViewModel>(appointment);
        }

        public async Task<AppointmentViewModel> GetByRoomId(Guid roomId)
        {
            var appointment = await Repository.GetAll()
                .FirstOrDefaultAsync(a => a.RoomId == roomId);

            return appointment == null
                ? null
                : Mapper.Map<AppointmentViewModel>(appointment);
        }

        public async Task<AppointmentViewModel> GetByUserId(Guid userId)
        {
            var appointment = await Repository.GetAll()
                .FirstOrDefaultAsync(a => a.PhysicianId == userId || a.NurseId == userId);

            return appointment == null
                ? null
                : Mapper.Map<AppointmentViewModel>(appointment);
        }

        public async Task<IEnumerable<PatientAppointmentVm>> GetByPatientAndCompanyId(Guid patientId, Guid companyId)
        {
            var patientAppointments = await _appointmentGridItemRepository.GetAll()
                .Where(a => a.PatientId == patientId && a.CompanyId == companyId)
                .OrderByDescending(a => a.StartDate)
                .ProjectTo<PatientAppointmentVm>()
                .ToListAsync();

            return patientAppointments;
        }

        public async Task<AppointmentHistory> PutStatus(AppointmentStatusVM appointmentStatus)
        {
            var appointment = await Repository.GetAll()
               .FirstOrDefaultAsync(a => a.Id == appointmentStatus.Id);

            appointment.AppointmentStatus = appointmentStatus.Status;
            Repository.Update(appointment);

            await Repository.SaveChangesAsync();

            return await InsertStatus(appointmentStatus);
        }

        public async Task<IEnumerable<AppointmentStatusSearch>> GetAppointmentStatus(Guid appointmentId)
        {
            try
            {
                MedicoContext context = new MedicoContext(_env);

                var appointment = await Repository.GetAll().FirstOrDefaultAsync(c => c.Id == appointmentId);

                var apppointmentHistory = await context.AppointmentHistory.Where(c => c.AppointmentId == appointmentId)
                    .OrderByDescending(c => c.CreatedOn).ToListAsync();

                var users = _userService.GetAll().Where(c => c.RoleName != "Patient");

                TimeSpan timeSpan = new TimeSpan();
                if (apppointmentHistory.Count() > 0)
                {
                    timeSpan = apppointmentHistory.FirstOrDefault().CreatedOn
                     .Subtract(appointment.StartDate);
                }

                var data = from vs in apppointmentHistory
                           join u in users on vs.CreatedBy equals u.Id.ToString() into gj
                           from subpet in gj.DefaultIfEmpty()
                           select new AppointmentStatusSearch
                           {
                               CreatedOn = vs.CreatedOn,
                               CreatedBy = vs.CreatedBy,
                               CreatedByName = subpet == null ? "Admin" : $"{subpet.FirstName} {subpet.LastName}",
                               Notes = vs.Notes,
                               Status = vs.Status,
                               TimeElapsed = $"{timeSpan.Days}d {timeSpan.Hours}h  {timeSpan.Minutes}m"
                           };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<AppointmentHistory> InsertStatus(AppointmentStatusVM appointmentStatus)
        {
            try
            {
                MedicoContext context = new MedicoContext(_env);
                var entity = await context.AppointmentHistory.AddAsync(new AppointmentHistory
                {
                    AppointmentId = appointmentStatus.Id,
                    CreatedBy = appointmentStatus.CreatedBy,
                    CreatedOn = DateTime.UtcNow,
                    Notes = appointmentStatus.Notes,
                    Status = appointmentStatus.Status
                });
                await context.SaveChangesAsync();
                return entity.Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static IQueryable<AppointmentGridItem> ApplyFilter(AppointmentDxOptionsViewModel dxOptions,
            IQueryable<AppointmentGridItem> query)
        {
            var locationId = dxOptions.LocationId;
            if (locationId != Guid.Empty)
            {
                query = query.Where(a => a.LocationId == locationId);
            }

            var patientId = dxOptions.PatientId;
            if (patientId != Guid.Empty)
            {
                query = query.Where(a => a.PatientId == patientId);
            }

            var physicianId = dxOptions.PhysicianId;
            if (physicianId != Guid.Empty)
            {
                query = query.Where(a => a.PhysicianId == physicianId);
            }

            var appointmentStatusesString = dxOptions.AppointmentStatuses;
            if (string.IsNullOrEmpty(appointmentStatusesString))
                return query;

            var appointmentStatuses =
                appointmentStatusesString.Split(",");

            var filterType = dxOptions.FilterType;
            if (filterType == null)
                throw new NullReferenceException(nameof(filterType));

            var filterTypeValue = filterType.Value;

            return filterTypeValue == FilterType.Equal
                ? query.Where(a => appointmentStatuses.Contains(a.AppointmentStatus))
                : query.Where(a => !appointmentStatuses.Contains(a.AppointmentStatus));
        }

        public async Task<IEnumerable<AppointmentStatusPatient>> GetAppointmentStatusByCompany(Guid companyId)
        {
            try
            {
                MedicoContext context = new MedicoContext(_env);

                var appointment = await Repository.GetAll().Where(c => c.CompanyId == companyId).ToListAsync();

                var apppointmentHistory = await context.AppointmentHistory.OrderByDescending(c => c.CreatedOn).ToListAsync();

                var users = _userService.GetAll().Where(c => c.RoleName != "Patient" && c.CompanyId == companyId);

                var patients = _patientService.GetAll().Where(c => c.CompanyId == companyId);

                var data = from vs in apppointmentHistory
                           join app in appointment on vs.AppointmentId equals app.Id
                           join pat in patients on app.PatientId equals pat.Id
                           join u in users on vs.CreatedBy equals u.Id.ToString() into gj
                           from subpet in gj.DefaultIfEmpty()
                           select new AppointmentStatusPatient
                           {
                               PatientId = pat.Id,
                               PatientName = $"{pat.Name}",
                               DateOfBirth = pat.DateOfBirth,
                               CreatedOn = vs.CreatedOn,
                               CreatedBy = vs.CreatedBy,
                               CreatedByName = subpet == null ? "Admin" : $"{subpet.FirstName} {subpet.LastName}",
                               Notes = vs.Notes,
                               Status = vs.Status,
                           };

                var list = new List<AppointmentStatusPatient>();

                foreach (var item in data)
                {
                    var patientId = item.PatientId;
                    string patientName = item.PatientName;

                    TimeSpan timeSpan = new TimeSpan();
                    var startDate = appointment.FirstOrDefault(c => c.PatientId == item.PatientId).StartDate;
                    var endDate = item.CreatedOn.Value;
                    timeSpan = endDate.Subtract(startDate);

                    list.Add(new AppointmentStatusPatient
                    {
                        PatientId = item.PatientId,
                        PatientName = item.PatientName,
                        TimeElapsed = $"{timeSpan.Days}d {timeSpan.Hours}h  {timeSpan.Minutes}m"
                    });

                    patientId = new Guid();
                    patientName = string.Empty;
                }

                return list.OrderBy(c => c.PatientName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<AppointmentStatusPieChart>> GetAppointmentStatusPieChart()
        {
            IDbConnection con = Connection;
            con.Open();
            var chartData =
              await con.QueryAsync<AppointmentStatusPieChart>(@"
select AppointmentStatus as Status,count(AppointmentStatus) as StatusCount
                                                                from Appointment 
																where month(StartDate) = 7 and year(StartDate)=2021
																group by AppointmentStatus",
              new
              { }, commandType: CommandType.Text);
            return chartData;
        }

        public async Task<IEnumerable<AppointmentPatientChartDocumentModel>> GetAppointmentPatientChartDocument(string admissionId)
        {
            IDbConnection con = Connection;
            con.Open();
            var chartData =
              await con.QueryAsync<AppointmentPatientChartDocumentModel>(@"[dbo].[GetAppointmentpatientChartNames]",
              new
              { admissionId }, commandType: CommandType.StoredProcedure);

            return chartData;
        }
    }
}