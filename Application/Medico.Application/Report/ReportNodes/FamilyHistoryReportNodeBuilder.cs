using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class FamilyHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IFamilyHistoryService _familyHistoryService;

        public FamilyHistoryReportNodeBuilder(IDefaultValueService defaultValueService,
            IFamilyHistoryService familyHistoryService) : base(
            defaultValueService)
        {
            _familyHistoryService = familyHistoryService;
        }

        public PatientChartNodeType NodeType =>
            PatientChartNodeType.FamilyHistoryNode;

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId, Guid admissionId,
            int clientUtcOffset)
        {
            var patientFamilyHistory = (await _familyHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientFamilyHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var familyHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Diagnosis", true),
                new HistoryItemPropertyInfo("FamilyMember"),
                new HistoryItemPropertyInfo("FamilyStatus"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientFamilyHistory, familyHistoryProperties,
                patientChartNode.Title);
        }
    }
}