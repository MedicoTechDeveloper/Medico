using System;
using System.ComponentModel;

namespace Medico.Domain.Models
{
    public class VitalSigns : Entity
    {
        public Guid? AdmissionId { get; set; }

        public Admission Admission { get; set; }

        public Guid PatientId { get; set; }

        public Patient Patient { get; set; }

        public double? Pulse { get; set; }

        public double? SystolicBloodPressure { get; set; }

        public double? DiastolicBloodPressure { get; set; }

        public string BloodPressurePosition { get; set; }

        public string BloodPressureLocation { get; set; }

        public string OxygenSaturationAtRest { get; set; }

        public double? OxygenSaturationAtRestValue { get; set; }

        public double? RespirationRate { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDelete { get; set; }
    }
}