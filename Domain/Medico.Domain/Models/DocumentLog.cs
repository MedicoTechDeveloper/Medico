using System;

namespace Medico.Domain.Models
{
    public class DocumentLog: Entity
    {
        public string ClaimantName { get; set; }
        public string ClaimantSsn { get; set; }
        public string PhoneNumber { get; set; }
        public string ApptDate { get; set; }
        public string ApptTime { get; set; }
        public string Physician { get; set; }
        public DateTime? DocParserProcessDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public Guid? ProcessedBy { get; set; }
        public Guid? CompanyId { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string ParserId { get; set; }
        public bool IsDeleted { get; set; }
        public string media_link_original { get; set; }
    }
}
