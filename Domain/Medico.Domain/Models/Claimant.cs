using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medico.Domain.Models
{
    [Table("Claimant")]
    public class Claimant
    {
        public string Id { get; set; }
        public string DocumentId { get; set; }
        public string RemoteId { get; set; }
        public string FileName { get; set; }
        public string MediaLink { get; set; }
        public string MediaLinkOriginal { get; set; }
        public string MediaLinkData { get; set; }
        public int? PageCount { get; set; }
        public DateTime? UploadedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string RQID { get; set; }
        public string MedicoInfo { get; set; }
        public string ApptDate { get; set; }
        public string Service { get; set; }
        public string ClaimantSSN { get; set; }
        public string Birthday { get; set; }
        public string SpecialInstructions { get; set; }
        public string HPI { get; set; }
        public string ProblemList { get; set; }
        public string VariableHPI { get; set; }
        public string PhoneNumber { get; set; }
        public string AppointmentTime { get; set; }
        public string ClaimantName { get; set; }
        public string PhysicianName { get; set; }
        public string ExamLocation { get; set; }
        public string CaseNumber { get; set; }
    }
}
