using System;

namespace Medico.Domain.Models
{
    public class AppointmentPatientChartDocument
    {
        public Guid AppointmentId { get; set; }

        public Appointment Appointment { get; set; }

        public Guid PatientChartDocumentNodeId { get; set; }
        
        public PatientChartDocumentNode PatientChartDocumentNode { get; set; }
    }
}