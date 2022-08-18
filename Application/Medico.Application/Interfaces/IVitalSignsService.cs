using Medico.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IVitalSignsService
    {
        Task<VitalSignsViewModel> GetById(Guid id);

        Task<VitalSignsViewModel> Create(VitalSignsViewModel vitalSignsViewModel);

        Task<VitalSignsViewModel> Update(VitalSignsViewModel vitalSignsViewModel);

        Task Delete(Guid id);

        Task<IEnumerable<VitalSignSearch>> GetAll(PatientAdmissionDxOptionsViewModel vitalSignsDxOptionsViewModel);

        Task<IEnumerable<VitalSignsViewVM>> GetByPatientAndAdmissionIds(Guid patientId, Guid admissionId);

        Task<VitalSignsViewModel> GetLastPatientVitalSigns(Guid patientId, DateTime createDate);

        Task<bool> IsDelete(UpdateIsDelete data, Guid id);
        Task<VitalSignsViewModel> GetSingle(Guid id);
    }
}