using Medico.Domain.Models;

namespace Medico.Domain.Interfaces
{
    public interface IPatientIdentificationCodeRepository 
        : IDeletableByIdRepository<PatientIdentificationCode>
    {
        
    }
}