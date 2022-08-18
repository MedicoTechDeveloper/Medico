using System;
using System.ComponentModel.DataAnnotations;
using Medico.Application.ViewModels.Patient;

namespace Medico.Application.ViewModels
{
    public class PatientInsuranceViewModel : PatientVm
    {
        [Required] public Guid PatientId { get; set; }
       
        public string MRN { get; set; }
        public string FIN { get; set; }
        public Guid PrimaryInsuranceCompany { get; set; }
        public Guid SecondaryInsuranceCompany { get; set; }
        public string PrimaryInsuranceGroupNumber { get; set; }
        public string SecondaryInsuranceGroupNumber { get; set; }
        public string PrimaryInsuranceNumber { get; set; }
        public string SecondaryInsuranceNumber { get; set; }
    }
}