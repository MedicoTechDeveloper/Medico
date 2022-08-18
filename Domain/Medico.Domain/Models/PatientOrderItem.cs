using System;

namespace Medico.Domain.Models
{
    public class PatientOrderItem: Entity
    {
        public Guid PatientOrderId { get; set; }
        public Guid LabTestId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }
        public short Status { get; set; }
    }
}
