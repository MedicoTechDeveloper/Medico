using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Patient;

namespace Medico.Application.Interfaces
{
    public interface IPatientService
    {
        Task<IList<PatientVm>> GetNotRegisteredAsync();
        
        IQueryable<PatientProjectionViewModel> GetAll();
        
        IQueryable<PatientProjectionViewModel> GetPatientsByAppointmentStatus(PatientDxOptionsViewModel loadOptions);

        Task<PatientVm> GetById(Guid id);

        Task<string> GetPatientEmail(Guid id);

        Task<PatientWithVitalSignsVm> GetByIdWithVitalSigns(Guid id);

        Task<PatientVm> Create(PatientVm patientVm);

        Task<PatientVm> Update(PatientVm patientVm);

        Task Delete(Guid id);

        IQueryable<PatientLookupVm> Lookup(DateRangeDxOptionsViewModel loadOptions);

        Task<List<PatientVm>> GetByFilter(PatientFilterVm patientSearchFilter);

        Task UpdatePatientNotes(PatientPatchVm patientNotesPatch);
        int GetMaxId();
    }
}