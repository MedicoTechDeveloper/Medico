using System;

namespace Medico.Application.ViewModels
{
    public class PhraseDxOptionsViewModel : CompanyDxOptionsViewModel
    {
        public Guid? PatientChartNodeId { get; set; }

        public Guid? TemplateId { get; set; }
    }
}