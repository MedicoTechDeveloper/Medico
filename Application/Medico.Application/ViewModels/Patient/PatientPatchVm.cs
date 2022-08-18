using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels.Patient
{
    public class PatientPatchVm
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public IList<string> UserIds { get; set; }
        public DateTime? ReminderDate { get; set; }
        public string Link { get; set; }
    }
}