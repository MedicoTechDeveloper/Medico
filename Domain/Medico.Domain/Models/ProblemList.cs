using System;

namespace Medico.Domain.Models
{
    public class ProblemList : Entity
    {
        public Guid AppointmentId { get; set; }
        public string Assessment { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
