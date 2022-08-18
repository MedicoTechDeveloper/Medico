using System;
using System.Collections.Generic;
using System.Linq;

namespace Medico.Application.ViewModels.PatientChartDocument
{
    public class PatientChartDocumentFilterVm : SearchFilterVm
    {
        public string PatientChartDocumentNodes { get; set; }

        public List<Guid> PatientChartDocumentNodeList => string.IsNullOrEmpty(PatientChartDocumentNodes)
            ? Enumerable.Empty<Guid>().ToList()
            : PatientChartDocumentNodes.Split(",")
                .Select(Guid.Parse)
                .ToList();

        public bool RestrictByPatientChartDocumentNodes { get; set; }
    }
}