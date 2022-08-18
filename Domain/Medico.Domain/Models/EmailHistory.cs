using System;
using System.Collections.Generic;
using System.Text;

namespace Medico.Domain.Models
{
    public class EmailHistory : Entity
    {
        public string EmailType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
