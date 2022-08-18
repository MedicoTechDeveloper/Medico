using System;

namespace Medico.Domain.Models
{
    public class PatientNote: Entity
    {
        public Guid PatientId { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
    }
}
