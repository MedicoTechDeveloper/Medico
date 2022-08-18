using System;

namespace Medico.Application.ViewModels
{
    public class AppointmentDxOptionsViewModel : CompanyDxOptionsViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid LocationId { get; set; }

        public Guid PatientId { get; set; }

        public Guid PhysicianId { get; set; }
        
        public string AppointmentStatuses { get; set; }
        
        public FilterType? FilterType { get; set; }
    }
}
