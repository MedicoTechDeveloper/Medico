using System;
using System.Collections;

namespace Medico.Application.ViewModels
{
    public class EmailViewModel
    {
        public String ReportName { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public String To { get; set; }
        public String From { get; set; }
        public String CcList { get; set; }
        public String BccList { get; set; }
        public byte[] Bytes { get; set; }
        public string TemplateName { get; set; }
        public Hashtable HashValues { get; set; }
        public string FromName { get; set; }
        public string Message { get; set; }
    }
}
