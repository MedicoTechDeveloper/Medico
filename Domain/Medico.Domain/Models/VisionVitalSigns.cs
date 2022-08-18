using System;

namespace Medico.Domain.Models
{
    public class VisionVitalSigns : Entity
    {
        public Guid PatientId { get; set; }

        public Patient Patient { get; set; }

        public bool WithGlasses { get; set; }

        public double Od { get; set; }

        public double Os { get; set; }

        public double Ou { get; set; }

        public DateTime CreateDate { get; set; }
    }
}