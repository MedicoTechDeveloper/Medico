using System;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IPatientSecurityService
    {
        Task<bool> DoesPatientUserRequestHisOwnInfo(Guid patientId);
    }
}