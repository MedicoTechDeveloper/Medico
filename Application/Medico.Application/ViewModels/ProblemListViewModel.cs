using System;

namespace Medico.Application.ViewModels
{
    public class ProblemListViewModel: BaseViewModel
    {
        public Guid AppointmentId { get; set; }
        public string Assessment { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
