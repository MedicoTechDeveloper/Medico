using System;

namespace Medico.Application.Tif.ViewModels
{
    public class PhoneNumber
    {
        public string formatted { get; set; }
        //public string number { get; set; }
    }

    public class Dob
    {
        public string formatted { get; set; }
    }

    public class ClaimantModelExt
    {
        public Guid companyId;
        public string rqid;
        public string id { get; set; }
        public string document_id { get; set; }
        public string remote_id { get; set; }
        public string file_name { get; set; }
        public string media_link { get; set; }
        public string media_link_original { get; set; }
        public string media_link_data { get; set; }
        public int page_count { get; set; }
        public DateTime uploaded_at { get; set; }
        public DateTime processed_at { get; set; }
        public string claimant_name { get; set; }
        public string address { get; set; }
        public PhoneNumber phone_number { get; set; }
        public string ss { get; set; }
        public Dob dob { get; set; }
        public string case_ { get; set; }
        public string allegations { get; set; }
        public string appt_date { get; set; }
        public string provider { get; set; }
        public string location { get; set; }
        public string additional_testing { get; set; }
        public string appt_time { get; set; }
        public string field_13 { get; set; }
        public object field_14 { get; set; }
        public object field_15 { get; set; }
        public object field_16 { get; set; }
    }
}
