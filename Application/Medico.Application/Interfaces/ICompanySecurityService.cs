using System;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface ICompanySecurityService
    {
        Task<bool> DoesUserHaveAccessToCompany(Guid companyId);

        Task<bool> DoesUserHaveAccessToCompanyPatient(Guid patientId);

        Task<bool> DoesUserHaveAccessToCompanyAdmission(Guid admissionId);

        Task<bool> DoesUserHaveAccessToCompanyLocation(Guid locationId);

        Task<bool> DoesUserHaveAccessToCompanyRoom(Guid roomId);

        Task<bool> DoesUserHaveAccessToCompanyEmployee(Guid roomId);

        Task<bool> DoesUserHaveAccessToCompanyDocument(Guid roomId);
    }
}
