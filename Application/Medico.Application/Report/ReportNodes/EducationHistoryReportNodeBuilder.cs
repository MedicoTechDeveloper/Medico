using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class EducationHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IEducationHistoryService _educationHistoryService;

        public EducationHistoryReportNodeBuilder(IDefaultValueService defaultValueService,
            IEducationHistoryService educationHistoryService) : base(
            defaultValueService)
        {
            _educationHistoryService = educationHistoryService;
        }

        public PatientChartNodeType NodeType => PatientChartNodeType.EducationNode;

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId, Guid admissionId,
            int clientUtcOffset)
        {
            var patientEducationHistory = (await _educationHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientEducationHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var educationHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Degree", true),
                new HistoryItemPropertyInfo("YearCompleted"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientEducationHistory, educationHistoryProperties, patientChartNode.Title);
        }
    }
}