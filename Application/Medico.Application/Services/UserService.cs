using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Medico.Application.Services
{
    public class UserService : BaseDeletableByIdService<MedicoApplicationUser, MedicoApplicationUserViewModel>,
        IUserService
    {
        private readonly IMapper _mapper;
        private readonly IMedicoApplicationUserViewRepository _medicoApplicationUserViewRepository;
        private readonly IAppointmentGridItemRepository _appointmentGridItemRepository;
        private readonly IDataSourceLoadOptionsHelper _dataSourceLoadOptionsHelper;
        private readonly LocalizationSettingsVm _localizationSettings;

        public UserService(IMedicoApplicationUserViewRepository medicoApplicationUserViewRepository,
            IMapper mapper, IMedicoApplicationUserRepository medicoApplicationUserRepository,
            IAppointmentGridItemRepository appointmentGridItemRepository,
            IDataSourceLoadOptionsHelper dataSourceLoadOptionsHelper,
            IOptions<LocalizationSettingsVm> localizationOptions) : base(medicoApplicationUserRepository, mapper)
        {
            _medicoApplicationUserViewRepository = medicoApplicationUserViewRepository;
            _mapper = mapper;
            _appointmentGridItemRepository = appointmentGridItemRepository;
            _dataSourceLoadOptionsHelper = dataSourceLoadOptionsHelper;
            _localizationSettings = localizationOptions.Value;
        }

        public IQueryable<MedicoApplicationUserViewModel> GetAll()
        {
            return _medicoApplicationUserViewRepository.GetAll()
                .ProjectTo<MedicoApplicationUserViewModel>();
        }

        public async Task<MedicoApplicationUserViewModel> GetByEmail(string email)
        {
            var applicationUser = await Repository.GetAll()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (applicationUser == null)
                throw new NullReferenceException(nameof(applicationUser));

            return Mapper.Map<MedicoApplicationUserViewModel>(applicationUser);
        }

        public Task Delete(Guid id)
        {
            return DeleteById(id);
        }

        public IQueryable<LookupViewModel> Lookup(UserDxOptionsViewModel loadOptions)
        {
            var companyId = loadOptions.CompanyId;
            if (companyId == Guid.Empty)
                return Enumerable.Empty<LookupViewModel>().AsQueryable();

            var startDate = loadOptions.StartDate;
            var endDate = loadOptions.EndDate;

            var searchString = _dataSourceLoadOptionsHelper.GetSearchString(loadOptions);
            var isSearchStringExist = !string.IsNullOrEmpty(searchString);

            //we have to remove native devextreme filter
            if (isSearchStringExist)
                loadOptions.Filter = null;

            var query = _appointmentGridItemRepository.GetAll()
                .Where(a => a.CompanyId == companyId);

            query = ApplyIntervalFilter(startDate, endDate, query, out var isIntervalFilterApplied);

            if (isIntervalFilterApplied)
            {
                if (isSearchStringExist)
                    query = query.Where(a =>
                        (a.PhysicianFirstName + " " + a.PhysicianLastName).Contains(searchString));

                return query.Select(a => new {a.PhysicianId, a.PhysicianFirstName, a.PhysicianLastName})
                    .Distinct()
                    .Select(p => new LookupViewModel
                    {
                        Id = p.PhysicianId,
                        Name = $"{p.PhysicianFirstName} {p.PhysicianLastName}"
                    });
            }

            var employeeType = loadOptions.EmployeeType;

            var medicoApplicationUserQuery = Repository.GetAll()
                .Where(u => u.EmployeeType == employeeType && u.CompanyId == companyId && u.IsActive);

            if (isSearchStringExist)
                medicoApplicationUserQuery = medicoApplicationUserQuery
                    .Where(u => (u.FirstName + " " + u.LastName).Contains(searchString));

            return medicoApplicationUserQuery.ProjectTo<LookupViewModel>();
        }

        public IQueryable<MedicoApplicationUserViewModel> Grid(CompanyDxOptionsViewModel loadOptions)
        {
            var companyId = loadOptions.CompanyId;
            if (companyId == Guid.Empty)
                return Enumerable.Empty<MedicoApplicationUserViewModel>().AsQueryable();

            return _medicoApplicationUserViewRepository.GetAll()
                .Where(u => u.CompanyId == companyId)
                .ProjectTo<MedicoApplicationUserViewModel>();
        }

        public async Task<MedicoApplicationUserViewModel> GetByUserId(Guid id)
        {
            var medicoApplicationUserView = await _medicoApplicationUserViewRepository
                .GetAll().FirstOrDefaultAsync(u => u.Id == id);

            return medicoApplicationUserView == null
                ? null
                : _mapper.Map<MedicoApplicationUserViewModel>(medicoApplicationUserView);
        }

        public async Task<IEnumerable<LookupViewModel>> GetUserCompanies(string email)
        {
            return await Repository.GetAll()
                .Where(u => u.Email == email)
                .Include(u => u.Company)
                .Where(u => u.Company.IsActive)
                .Select(u => u.Company)
                .ProjectTo<LookupViewModel>()
                .ToListAsync();
        }

        public async Task<IEnumerable<LookupViewModel>> GetPatientCompanies(
            UserIdentificationInfoVm patientIdentificationInfo)
        {
            return (await GetPatientsUsersInternally(patientIdentificationInfo.FirstName,
                    patientIdentificationInfo.LastName,
                    patientIdentificationInfo.DateOfBirth))
                .Select(pu => new LookupViewModel
                {
                    Id = pu.Company.Id,
                    Name = pu.Company.Name
                });
        }

        public async Task<IList<MedicoApplicationUserViewModel>> GetPatientsUsers(string patientFirstName,
            string patientLastName, DateTime dateOfBirth, Guid companyId)
        {
            var patientsUsers = (await GetPatientsUsersInternally(patientFirstName,
                    patientLastName, dateOfBirth))
                .Select(pu => Mapper.Map<MedicoApplicationUserViewModel>(pu))
                .ToList();

            return patientsUsers.Where(pu => pu.CompanyId == companyId).ToList();
        }

        private async Task<IList<MedicoApplicationUser>> GetPatientsUsersInternally(string patientFirstName,
            string patientLastName, DateTime dateOfBirth)
        {
            var availableTimeZoneOffsets = _localizationSettings.AvailableTimeZoneOffsets;

            //EmployeeType available values can be found on the client side patient type = 7
            Expression<Func<MedicoApplicationUser, bool>> filter =
                u => u.FirstName == patientFirstName &&
                     u.LastName == patientLastName
                     && u.IsActive && u.EmployeeType == 7 &&
                     u.Company.IsActive
                     && availableTimeZoneOffsets
                         .Any(offset => u.DateOfBirth.AddHours(offset) == dateOfBirth);

            return await Repository.GetAll()
                .Include(u => u.Company)
                .Where(filter)
                .ToListAsync();
        }

        public async Task<MedicoApplicationUserViewModel> GetFirstOrDefaultAsync(
            Expression<Func<MedicoApplicationUserViewModel, bool>> filter)
        {
            return await GetAll().FirstOrDefaultAsync(filter);
        }


        public async Task<MedicoApplicationUserViewModel> GetByName(string firstName)
        {
            var applicationUser = await Repository.GetAll()
                .FirstOrDefaultAsync(u => u.FirstName == firstName);

            //if (applicationUser == null)
            //    throw new NullReferenceException(nameof(applicationUser));

            return Mapper.Map<MedicoApplicationUserViewModel>(applicationUser);
        }


        public async Task<MedicoApplicationUserViewModel> GetProfile(string email)
        {
            var medicoApplicationUserView = await _medicoApplicationUserViewRepository
                .GetAll().FirstOrDefaultAsync(u => u.Email == email);

            return medicoApplicationUserView == null
                ? null
                : _mapper.Map<MedicoApplicationUserViewModel>(medicoApplicationUserView);
        }
    }
}