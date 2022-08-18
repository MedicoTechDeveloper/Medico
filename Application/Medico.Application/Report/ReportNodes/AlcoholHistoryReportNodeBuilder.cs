using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class AlcoholHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IAlcoholHistoryService _alcoholHistoryService;

        public PatientChartNodeType NodeType => PatientChartNodeType.AlcoholHistoryNode;

        public AlcoholHistoryReportNodeBuilder(IAlcoholHistoryService alcoholHistoryService,
            IDefaultValueService defaultValueService) : base(defaultValueService)
        {
            _alcoholHistoryService = alcoholHistoryService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientAlcoholHistory = (await _alcoholHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientAlcoholHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var alcoholHistoryProperties = new List<HistoryItemPropertyInfo>
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

            return GetHistoryReportNodeString(patientAlcoholHistory, alcoholHistoryProperties, patientChartNode.Title);
        }
    }
}