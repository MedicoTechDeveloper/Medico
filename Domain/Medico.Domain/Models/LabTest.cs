namespace Medico.Domain.Models
{
    public class LabTest: Entity
    {
        public string TestName { get; set; }
        public string Description { get; set; }
        public string TestCode { get; set; }
        public string CodeType { get; set; }
        public decimal TestFee { get; set; }
        public bool IsAvailableInHouse { get; set; }
        public bool IsActive { get; set; }
        public string Category { get; set; }
        public int? VendorId { get; set; }
        public string Procedure { get; set; }
        public string MedicoCode { get; set; }
        public string ReferenceNo { get; set; }
    }
}
