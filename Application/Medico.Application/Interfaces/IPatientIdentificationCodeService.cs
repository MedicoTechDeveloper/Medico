using System.Threading.Tasks;
using Medico.Application.PatientIdentificationCodes.ViewModels;
using Medico.Application.ViewModels;

namespace Medico.Application.Interfaces
{
    public interface IPatientIdentificationCodeService
    {
        Task<PatientIdentificationCodeVm> Get(IdentificationCodeSearchFilterVm searchFilter);

        Task<CreateUpdateResponseVm<PatientIdentificationCodeVm>> Save(PatientIdentificationCodeVm code);
        
        Task<int?> GetNextValidNumericCodeValue(IdentificationCodeSearchFilterVm searchFilter);
    }
}