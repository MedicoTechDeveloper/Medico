using Medico.Domain.Models;
using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels
{
    public class PatientOrderViewModel : BaseViewModel
    {
        public Guid PatientId { get; set; }
        public Guid AppointmentId { get; set; }
        public string OrderNumber { get; set; }
        public string Notes { get; set; }
        public int? AttachmentId { get; set; }
        public short OrderStatus { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ReferenceNo { get; set; }
        public Guid PhysicianId { get; set; }
        public Guid? InsuranceId { get; set; }
        public int? VendorId { get; set; }
        public DateTime DateOrdered { get; set; }
        public IEnumerable<PatientOrderItem> PatientOrderItems { get; set; }
        public IList<string> UserIds { get; set; }
        public DateTime? ReminderDate { get; set; }
    }

    public class PatientOrderSearch: PatientOrderViewModel
    {
        public string PatientName { get; set; }
        public string Physician { get; set; }
        public string Vendor { get; set; }
        public string Insurance { get; set; }
        public string ItemString { get; set; }
    }

    public class PatientOrderGrouped
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string PatientName { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ItemString { get; set; }
        public Guid PatientId { get; internal set; }
        public Guid PhysicianId { get; internal set; }
        public string Insurance { get; set; }
        public IEnumerable<PatientOrderItemModel1> OrderItems { get; internal set; }
        public int VendorId { get; internal set; }
        public string ReferenceNo { get; internal set; }
        public DateTime DateOrdered { get; internal set; }
        public string Notes { get; internal set; }
        public Guid InsuranceId { get; internal set; }
    }

    public class PatientOrderItemModel
    {
        public Guid PatientOrderId { get; set; }
        public Guid LabTestId { get; set; }
        public string LabTestName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }
        public string LabTestCode { get; internal set; }
        public decimal LabTestFee { get; internal set; }
        public short Status { get; set; }
        public Guid Id { get; set; }
    }

    public class PatientOrderItemModel1
    {
        public Guid PatientOrderId { get; set; }
        public Guid LabTestId { get; set; }
        public string LabTestName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }
        public string LabTestCode { get; internal set;}
        public string Category { get; internal set; }
        public decimal LabTestFee { get; internal set; }
        public short Status { get; set; }
        public Guid Id { get; set; }
    }

    public class PatientOrderUpdateModel 
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
        public short OrderStatus { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ReferenceNo { get; set; }
        public Guid PhysicianId { get; set; }
        public Guid? InsuranceId { get; set; }
        public int? VendorId { get; set; }
        public DateTime DateOrdered { get; set; }
        public IEnumerable<PatientOrderItemModel> PatientOrderItems { get; set; }
    }

}
