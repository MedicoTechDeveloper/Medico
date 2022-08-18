using Medico.Application.ViewModels.Patient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface IPatientNoteService
    {
        Task<IEnumerable<PatientNoteVm>> Create(PatientPatchVm patientVm);
        Task<IEnumerable<PatientNoteVm>> GetNotes(string patientId, string fromDate, string toDate, string subject, string status, string employee);
        Task<bool> EditNotes(string id, PatientPatchVm patientVm);
        Task<PatientNoteVm> GetNotesById(Guid id);

        Task<bool> DeleteNote(string id);
    }
}
