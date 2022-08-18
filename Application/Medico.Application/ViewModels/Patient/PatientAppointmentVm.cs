using System;

namespace Medico.Application.ViewModels.Patient
{
    public class PatientAppointmentVm
    {
        public Guid AppointmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        
        public DateTime Date { get; set; }

        public string AppointmentStatus { get; set; }
        
        public string LocationName { get; set; }

        public string RoomName { get; set; }
        
        public string Physician { get; set; }

        public string Nurse { get; set; }

        public bool IsPatientChartSignedIn { get; set; }
    }
}