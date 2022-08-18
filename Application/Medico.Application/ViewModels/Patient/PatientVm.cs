using System;
using System.ComponentModel.DataAnnotations;
using Medico.Application.DataAnnotation;
using Medico.Domain.Enums;

namespace Medico.Application.ViewModels.Patient
{
    public class PatientVmExtra : PatientVm
    {
        public string Physician { get; set; }
        public string ApptDate { get; set; }
        public string ApptTime { get; set; }
        public string StateName { get; set; }
        public string Allegations { get; set; }
        public string ExamLocation { get; set; }
        public string DocumentId { get; set; }
    }

    public class PatientVm : BaseViewModel
    {
        [Required] public Guid CompanyId { get; set; }
        
        [Required] public string FirstName { get; set; }
        
        [Required] public string LastName { get; set; }

        public string NameSuffix { get; set; }
        
        public string MiddleName { get; set; }
        
        [Required] public int Gender { get; set; }
        
        [Required] public DateTime DateOfBirth { get; set; }

        [Required] public int MaritalStatus { get; set; }
        
        [Required] public string Ssn { get; set; }
        
        [Required] public string PrimaryAddress { get; set; }
        
        public string SecondaryAddress { get; set; }
        
        [Required] public string City { get; set; }
        
        [Required] public string PrimaryPhone { get; set; }
        
        public string SecondaryPhone { get; set; }
        
        [NonRequiredEmailAddress] public string Email { get; set; }
        
        [Required] public string Zip { get; set; }
        
        [Required] public ZipCodeType ZipCodeType { get; set; }
        
        [Required] public int State { get; set; }

        public Guid? PatientInsuranceId { get; set; }
        
        public string Notes { get; set; }
        
        public string Password { get; set; }
        public string Rqid { get; set; }
        public string CaseNumber { get; set; }
        public string Mrn { get; set; }
        public string Fin { get; set; }
        public DateTime AdmissionDate { get; set; }
    }
}