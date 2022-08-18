using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels
{
    public class PreAuthDataViewModel : BaseViewModel
    {
        public string PreAuth { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class PreAuthDataProjectionViewModel : BaseViewModel
    {
        public string PreAuth { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
