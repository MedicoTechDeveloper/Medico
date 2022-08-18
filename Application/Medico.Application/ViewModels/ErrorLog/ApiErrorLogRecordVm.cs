using System;

namespace Medico.Application.ViewModels.ErrorLog
{
    public class ApiErrorLogRecordVm : BaseViewModel
    {
        public DateTime Date { get; set; }
        
        public string RequestedUrl { get; set; }
        
        public string HttpMethod { get; set; }

        public string UserName { get; set; }
        
        public string ErrorDetails { get; set; }
        
        public Guid? CompanyId { get; set; }
    }
}