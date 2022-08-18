using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels.PhraseUsage
{
    public class PhraseUsageReadVm
    {
        public Guid PhraseId { get; set; }

        public string PhraseName { get; set; }

        public List<PatientChartNodeUsePhraseReadVm> PatientChartNodeUsePhrases { get; set; }

        public List<TemplateUsePhraseReadVm> TemplateUsePhrases { get; set; }
    }
}