using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels.PhraseUsage
{
    public class PhraseUsageVm
    {
        public Guid PhraseId { get; set; }

        public List<PatientChartNodeUsePhraseVm> PatientChartNodeUsePhrases { get; set; }

        public List<TemplateUsePhraseVm> TemplateUsePhrases { get; set; }
    }
}