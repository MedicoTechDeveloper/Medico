namespace Medico.Application.ViewModels
{
    public class LabTestViewModel: BaseViewModel
    {
        public string TestName { get; set; }
        public string TestCode { get; set; }
        public string CodeType { get; set; }
        public string MedicoCode { get; set; }
        public decimal TestFee { get; set; }
        public string Description { get; set; }
        public bool IsAvailableInHouse { get; set; }
        public string Category { get; set; }
        public string Procedure { get; set; }
        public int? VendorId { get; set; }
        public string ReferenceNo { get; set; }
    }

    public class LabTestGrouped
    {
        public string TestName { get; set; }
        public int Count { get; set; }
    }

    public class LabTestFee
    {
        public decimal TestFee { get; set; }
    }

    public class LabTest1 : LabTestViewModel
    {
        public string VendorName { get; set; }
    }

}
