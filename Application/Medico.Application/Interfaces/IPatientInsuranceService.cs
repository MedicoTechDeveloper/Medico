using System;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.ViewModels;

namespace Medico.Application.Interfaces
{
    public interface IPatientInsuranceService
    {
        IQueryable<PatientInsuranceViewModel> GetAll();

        int GetMaxId();

        Task<PatientInsuranceViewModel> GetByPatientId(Guid patientId);

        Task<PatientInsuranceViewModel> Create(PatientInsuranceViewModel patientInsuranceViewModel);

        Task<PatientInsuranceViewModel> Update(PatientInsuranceViewModel patientInsuranceViewModel);
    }
}