using Medico.Application.ViewModels.Enums;
using System.Collections.Generic;

namespace Medico.Application.ViewModels.Audit
{
    public class AuditChange
    {
        public string DateTimeStamp { get; set; }
        public AuditActionType AuditActionType { get; set; }
        public string AuditActionTypeName { get; set; }
        public List<AuditDelta> Changes { get; set; }
        public AuditChange()
        {
            Changes = new List<AuditDelta>();
        }
    }
}
