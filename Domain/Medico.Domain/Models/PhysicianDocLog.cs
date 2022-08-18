using System;
using System.Collections.Generic;

namespace Medico.Domain.Models
{
    public class DocumentField: Entity
    {
        public string DocumentKey { get; set; }
    }

    public class PhysicianDocLog: Entity
    {
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public string DocumentId { get; set; }
        public DateTime? DocParserProcessDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public Guid? ProcessedBy { get; set; }
        public Guid? CompanyId { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string ParserId { get; set; }
        public string DocContent { get; set; }
        public string media_link_original { get; set; }
        public List<DocumentField> DocumentField { get; set; }
        public bool IsDeleted { get; set; }
    }
}
