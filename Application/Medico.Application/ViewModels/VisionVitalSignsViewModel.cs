using System;
using System.ComponentModel.DataAnnotations;

namespace Medico.Application.ViewModels
{
    public class VisionVitalSignsViewModel : BaseViewModel
    {
        [Required]
        public Guid PatientId { get; set; }

        public bool WithGlasses { get; set; }

        public double Od { get; set; }

        public double Os { get; set; }

        public double Ou { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }
    }
}
