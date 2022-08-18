using System;

namespace Medico.Application.ViewModels
{
    public class SubTaskUserViewModel: BaseViewModel
    {
        public Guid SubTaskId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
    }
}
