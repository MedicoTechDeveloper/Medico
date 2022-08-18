using System.Collections.Generic;

namespace Medico.Application.ViewModels.Patient
{
    public class PatientLoginResponseVm
    {
        public PatientLoginResponseVm()
        {
            Errors = new List<string>();
            PatientUser = new PatientUserVm();
        }

        public PatientUserVm PatientUser { get; }
        
        public List<string> Errors { get; }
    }
}