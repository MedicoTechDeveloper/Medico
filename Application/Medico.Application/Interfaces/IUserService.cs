using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Medico.Application.ViewModels;

namespace Medico.Application.Interfaces
{
    public interface IUserService
    {
        IQueryable<MedicoApplicationUserViewModel> GetAll();

        Task<MedicoApplicationUserViewModel> GetById(Guid id);

        Task<MedicoApplicationUserViewModel> GetByEmail(string email);

        Task<MedicoApplicationUserViewModel> GetByName(string firstName);

        Task<MedicoApplicationUserViewModel> Create(MedicoApplicationUserViewModel locationViewModel);

        Task<MedicoApplicationUserViewModel> Update(MedicoApplicationUserViewModel locationViewModel);

        Task Delete(Guid id);

        IQueryable<LookupViewModel> Lookup(UserDxOptionsViewModel loadOptions);

        IQueryable<MedicoApplicationUserViewModel> Grid(CompanyDxOptionsViewModel loadOptions);

        Task<MedicoApplicationUserViewModel> GetByUserId(Guid id);

        Task<IEnumerable<LookupViewModel>> GetUserCompanies(string email);

        Task<IEnumerable<LookupViewModel>> GetPatientCompanies(UserIdentificationInfoVm patientIdentificationInfo);

        Task<IList<MedicoApplicationUserViewModel>> GetPatientsUsers(string patientFirstName,
            string patientLastName, DateTime dateOfBirth, Guid companyId);

        Task<MedicoApplicationUserViewModel> GetFirstOrDefaultAsync(
           Expression<Func<MedicoApplicationUserViewModel, bool>> filter);

        Task<MedicoApplicationUserViewModel> GetProfile(string email);
    }
}