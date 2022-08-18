using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class SurgicalHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly ISurgicalHistoryService _surgicalHistoryService;

        public PatientChartNodeType NodeType => PatientChartNodeType.PreviousSurgicalHistoryNode;

        public SurgicalHistoryReportNodeBuilder(ISurgicalHistoryService surgicalHistoryService,
            IDefaultValueService defaultValueService) : base(defaultValueService)
        {
            _surgicalHistoryService = surgicalHistoryService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientSurgicalHistory = (await _surgicalHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientSurgicalHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var surgicalHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Diagnosis", true),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientSurgicalHistory, surgicalHistoryProperties, patientChartNode.Title);
        }
    }
}