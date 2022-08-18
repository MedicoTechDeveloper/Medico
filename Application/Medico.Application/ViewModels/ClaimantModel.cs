using System;
using System.Collections.Generic;
using System.Text;

namespace Medico.Application.ViewModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class PhoneNumber
    {
        public string number { get; set; }
    }

    public class ClaimantCityStateZip
    {
        public string key_0 { get; set; }
        public string key_1 { get; set; }
        public string key_2 { get; set; }
        public string key_3 { get; set; }
        public string key_4 { get; set; }
        public string key_5 { get; set; }
        public string key_6 { get; set; }
    }

    public class ClaimantModel
    {
        public Guid companyId;
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
        public string medico_info { get; set; }
        public string claimant_ssn { get; set; }
        public string birthday { get; set; }
        public string rqidbytext { get; set; }
        public string allegationsbytext { get; set; }
        public string special_instructions { get; set; }
        public string physicianbylocation { get; set; }
        public string claimant_name { get; set; }
        public PhoneNumber phone_number { get; set; }
        public string appointment_time { get; set; }
        public string case_number { get; set; }
        public string exam_location { get; set; }
        public List<ClaimantCityStateZip> claimant_city_state_zip { get; set; }
        public string examdate { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ClaimantCaseInformation
    {
        public string key_0 { get; set; }
    }

    public class Birthday
    {
        public string formatted { get; set; }
    }

    public class HomeAddress
    {
        public string key_0 { get; set; }
        public string key_1 { get; set; }
        public string key_2 { get; set; }
        public string key_3 { get; set; }
    }

    //public class ClaimantModel
    //{
    //    public Guid companyId;
    //    public string id { get; set; }
    //    public string document_id { get; set; }
    //    public string remote_id { get; set; }
    //    public string file_name { get; set; }
    //    public string media_link { get; set; }
    //    public string media_link_original { get; set; }
    //    public string media_link_data { get; set; }
    //    public int page_count { get; set; }
    //    public DateTime uploaded_at { get; set; }
    //    public DateTime processed_at { get; set; }
    //    public string rqid { get; set; }
    //    public string claimant { get; set; }
    //    public string @case { get; set; }
    //    public string date { get; set; }
    //    public string time { get; set; }
    //    public string allegations { get; set; }
    //    public string name { get; set; }
    //    public string claimant_information { get; set; }
    //    public string datetime2 { get; set; }
    //    public string evaluate { get; set; }
    //    public string special_instructions { get; set; }
    //    public string ssn { get; set; }
    //}


    public class ClaimantModel2
    {
        public Guid companyId;

        ClaimantModel2()
        {
            claimant_city_state_zip = new ClaimantCityStateZip();
        }

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
        public string medico_info { get; set; }
        public List<ClaimantCaseInformation> claimant_case_information { get; set; }
        public HomeAddress home_address { get; set; }
        public object appt_date { get; set; }
        public string service { get; set; }
        public string claimant_ssn { get; set; }
        public Birthday birthday { get; set; }
        public string rqidbytext { get; set; }
        public string allegationsbytext { get; set; }
        public string variable_hpi { get; set; }
        public string special_instructions { get; set; }
        public string physicianbylocation { get; set; }
        public string claimant_name { get; set; }
        public PhoneNumber phone_number { get; set; }
        public string appointment_time { get; set; }
        public string case_number { get; set; }
        public string exam_location { get; set; }
        public ClaimantCityStateZip claimant_city_state_zip { get; set; }
    }

    
}
