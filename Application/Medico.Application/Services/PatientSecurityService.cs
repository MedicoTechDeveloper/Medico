using System;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Domain.Interfaces;

namespace Medico.Application.Services
{
    public class PatientSecurityService : IPatientSecurityService
    {
        private readonly IUser _user;
        private readonly IPatientUserEmailService _patientUserEmailService;

        public PatientSecurityService(IUser user, IPatientUserEmailService patientUserEmailService)
        {
            _user = user;
            _patientUserEmailService = patientUserEmailService;
        }
        
        public async Task<bool> DoesPatientUserRequestHisOwnInfo(Guid patientId)
        {
            var patientUserEmail = await _user.GetEmail();
            var patientUserSavedId = _patientUserEmailService
                .ExtractPatientIdFromEmail(patientUserEmail);

            return patientUserSavedId == patientId;
        }
    }
}