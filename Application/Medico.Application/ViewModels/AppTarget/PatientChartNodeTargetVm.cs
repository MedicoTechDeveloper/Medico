using System;

namespace Medico.Application.ViewModels.AppTarget
{
    public class PatientChartNodeTargetVm : BaseViewModel
    {
        public Guid DocumentId { get; set; }
        
        public Guid NodeId { get; set; }
    }
}