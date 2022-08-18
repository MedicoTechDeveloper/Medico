using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class TobaccoHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly ITobaccoHistoryService _tobaccoHistoryService;

        public PatientChartNodeType NodeType =>
            PatientChartNodeType.TobaccoHistoryNode;

        public TobaccoHistoryReportNodeBuilder(ITobaccoHistoryService tobaccoHistoryService,
            IDefaultValueService defaultValueService) : base(defaultValueService)
        {
            _tobaccoHistoryService = tobaccoHistoryService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientTobaccoHistory = (await _tobaccoHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientTobaccoHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var tobaccoHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Status", true),
                new HistoryItemPropertyInfo("Type"),
                new HistoryItemPropertyInfo("Amount"),
                new HistoryItemPropertyInfo("Use"),
                new HistoryItemPropertyInfo("Frequency"),
                new HistoryItemPropertyInfo("Length"),
                new HistoryItemPropertyInfo("Duration"),
                new HistoryItemPropertyInfo("Quit"),
                new HistoryItemPropertyInfo("StatusLength", false, "Quit"),
                new HistoryItemPropertyInfo("StatusLengthType", false, "Quit"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientTobaccoHistory, tobaccoHistoryProperties, patientChartNode.Title);
        }
    }
}