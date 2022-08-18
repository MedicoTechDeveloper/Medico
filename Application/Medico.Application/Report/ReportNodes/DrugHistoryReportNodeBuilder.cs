using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class DrugHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IDrugHistoryService _drugHistoryService;

        public PatientChartNodeType NodeType => PatientChartNodeType.DrugHistoryNode;

        public DrugHistoryReportNodeBuilder(IDrugHistoryService drugHistoryService,
            IDefaultValueService defaultValueService) : base(defaultValueService)
        {
            _drugHistoryService = drugHistoryService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientDrugHistory = (await _drugHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientDrugHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var drugHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Status", true),
                new HistoryItemPropertyInfo("Type"),
                new HistoryItemPropertyInfo("Amount"),
                new HistoryItemPropertyInfo("Use"),
                new HistoryItemPropertyInfo("Route"),
                new HistoryItemPropertyInfo("Frequency"),
                new HistoryItemPropertyInfo("Length"),
                new HistoryItemPropertyInfo("Duration"),
                new HistoryItemPropertyInfo("Quit"),
                new HistoryItemPropertyInfo("StatusLength", false, "Quit"),
                new HistoryItemPropertyInfo("StatusLengthType", false, "Quit"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientDrugHistory, drugHistoryProperties, patientChartNode.Title);
        }
    }
}