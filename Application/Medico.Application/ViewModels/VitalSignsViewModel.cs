using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Medico.Application.ViewModels
{
    public class VitalSignsViewModel : BaseViewModel
    {
        public Guid? AdmissionId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        public double? Pulse { get; set; }

        public double? SystolicBloodPressure { get; set; }

        public double? DiastolicBloodPressure { get; set; }

        public string BloodPressurePosition { get; set; }

        public string BloodPressureLocation { get; set; }

        public string OxygenSaturationAtRest { get; set; }

        public double? OxygenSaturationAtRestValue { get; set; }

        public double? RespirationRate { get; set; }

        // [Required]
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDelete { get; set; }
    }

    public class VitalSignsViewVM : BaseViewModel
    {
        public Guid? AdmissionId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        public double? Pulse { get; set; }

        public double? SystolicBloodPressure { get; set; }

        public double? DiastolicBloodPressure { get; set; }

        public string BloodPressurePosition { get; set; }

        public string BloodPressureLocation { get; set; }

        public string OxygenSaturationAtRest { get; set; }

        public double? OxygenSaturationAtRestValue { get; set; }

        public double? RespirationRate { get; set; }

        // [Required]
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDelete { get; set; }
    }

    public class UpdateIsDelete
    {

        [DefaultValue(false)]
        public bool IsDelete { get; set; }

    }

    public class VitalSignSearch: VitalSignsViewModel
    {
        public string CreatedByName { get; set; }
    }
}