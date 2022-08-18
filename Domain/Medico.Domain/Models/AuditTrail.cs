using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medico.Domain.Models
{
    [Table("AuditTable")]
    public partial class AuditTable :Entity
    {
        public string KeyFieldID { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public string DataModel { get; set; }
        public string ValueBefore { get; set; }
        public string ValueAfter { get; set; }
        public string Changes { get; set; }
        public int AuditActionTypeENUM { get; set; }
    }
}
