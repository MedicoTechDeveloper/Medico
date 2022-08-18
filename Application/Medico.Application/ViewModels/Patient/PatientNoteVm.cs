using System;

namespace Medico.Application.ViewModels.Patient
{
    public class PatientNoteVm
    {
        public string Id { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
    }
}
