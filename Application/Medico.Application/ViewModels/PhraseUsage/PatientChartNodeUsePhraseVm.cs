using System;

namespace Medico.Application.ViewModels.PhraseUsage
{
    public class PatientChartNodeUsePhraseVm
    {
        public Guid Id { get; set; }
        
        public Guid DocumentId { get; set; }    

        public Guid PatientChartNodeId { get; set; }
    }
}