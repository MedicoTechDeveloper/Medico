using System;

namespace Medico.Application.ViewModels.Patient
{
    public class PatientUserVm
    {
        public bool IsAuthenticated { get; set; }
        
        public Guid PatientId { get; set; }
        
        public Guid CompanyId { get; set; }
    }
}