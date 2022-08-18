using System;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Domain.Interfaces;

namespace Medico.Application.Services
{
    public class CompanySecurityService : ICompanySecurityService
    {
        private readonly IUser _user;

        public CompanySecurityService(IUser user)
        {
            _user = user;
        }

        public Task<bool> DoesUserHaveAccessToCompany(Guid companyId)
        {
            return _user.HasAccessToCompany(companyId);
        }

        public Task<bool> DoesUserHaveAccessToCompanyPatient(Guid patientId)
        {
            return _user.HasAccessToCompanyPatient(patientId);
        }

        public Task<bool> DoesUserHaveAccessToCompanyAdmission(Guid admissionId)
        {
            return _user.HasAccessToCompanyAdmission(admissionId);
        }

        public Task<bool> DoesUserHaveAccessToCompanyLocation(Guid locationId)
        {
            return _user.HasAccessToCompanyLocation(locationId);
        }
            
        public Task<bool> DoesUserHaveAccessToCompanyRoom(Guid roomId)
        {   
            return _user.HasAccessToCompanyRoom(roomId);
        }

        public Task<bool> DoesUserHaveAccessToCompanyEmployee(Guid employeeId)
        {
            return _user.HasAccessToCompanyEmployee(employeeId);
        }   

        public Task<bool> DoesUserHaveAccessToCompanyDocument(Guid documentId)
        {
            return _user.HasAccessToCompanyDocument(documentId);
        }
    }
}
