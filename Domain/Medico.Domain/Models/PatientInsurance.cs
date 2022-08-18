using System;
using Medico.Domain.Enums;

namespace Medico.Domain.Models
{
    public class PatientInsurance : Entity
    {
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public string CaseNumber { get; set; }
        public string RqId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Ssn { get; set; }
        public string Zip { get; set; }
        public ZipCodeType ZipCodeType { get; set; }
        public string PrimaryAddress { get; set; }
        public string SecondaryAddress { get; set; }
        public string City { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string Email { get; set; }
        public int State { get; set; }
        public Guid PrimaryInsuranceCompany { get; set; }
        public Guid SecondaryInsuranceCompany { get; set; }
        public string PrimaryInsuranceGroupNumber { get; set; }
        public string SecondaryInsuranceGroupNumber { get; set; }
        public string PrimaryInsuranceNumber { get; set; }
        public string SecondaryInsuranceNumber { get; set; }
        public string MRN { get; set; }
        public string FIN { get; set; }
    }
}