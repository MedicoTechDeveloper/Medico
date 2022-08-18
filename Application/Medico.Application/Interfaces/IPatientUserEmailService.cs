using System;

namespace Medico.Application.Interfaces
{
    public interface IPatientUserEmailService
    {
        Guid ExtractPatientIdFromEmail(string patientEmail);

        string GeneratePatientUserEmailBasedOnPatientId(Guid patientId);
    }
}