using System;

namespace Medico.Application.ViewModels
{
    public class VitalSignsNotesViewModel : BaseViewModel
    {
        public Guid AdmissionId { get; set; }

        public string Notes { get; set; }
        
        public bool IncludeNotesInReport { get; set; }
    }
}
