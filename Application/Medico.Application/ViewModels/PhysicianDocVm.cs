using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels
{
    public class Field1
    {
        public string key_0 { get; set; }
    }

    public class PhysicianDocVm
    {
        public string document_id { get; set; }
        public string remote_id { get; set; }
        public string file_name { get; set; }
        public string media_link { get; set; }
        public string media_link_original { get; set; }
        public string media_link_data { get; set; }
        public int page_count { get; set; }
        public DateTime uploaded_at { get; set; }
        public DateTime processed_at { get; set; }
        public string DocContent { get; set; }
        public Guid PatientId { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsProcessed { get; set; }

        // for medical record
        public DateTime CreateDate { get; set; }
        public string DocumentType { get; set; }
        // public List<Field1> field_1 { get; set; }
    }
}
