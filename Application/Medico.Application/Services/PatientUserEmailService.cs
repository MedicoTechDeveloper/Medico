using System;
using Medico.Application.Interfaces;

namespace Medico.Application.Services
{
    public class PatientUserEmailService : IPatientUserEmailService
    {
        public Guid ExtractPatientIdFromEmail(string patientEmail)
        {
            var patientGuid = 
                patientEmail.Substring(0, patientEmail.IndexOf('@'));

            return Guid.Parse(patientGuid);
        }

        public string GeneratePatientUserEmailBasedOnPatientId(Guid patientId)
        {
            return $"{patientId}@mail.com";
        }
    }
}