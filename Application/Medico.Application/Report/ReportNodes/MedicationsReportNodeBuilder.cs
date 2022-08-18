using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class MedicationsReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IMedicationHistoryService _medicationHistoryService;

        public MedicationsReportNodeBuilder(IDefaultValueService defaultValueService,
            IMedicationHistoryService medicationHistoryService) : base(
            defaultValueService)
        {
            _medicationHistoryService = medicationHistoryService;
        }

        public PatientChartNodeType NodeType => PatientChartNodeType.MedicationsNode;

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId, Guid admissionId,
            int clientUtcOffset)
        {
            var patientMedicationHistory = (await _medicationHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientMedicationHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var medicationHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Medication", true),
                new HistoryItemPropertyInfo("Dose"),
                new HistoryItemPropertyInfo("Units"),
                new HistoryItemPropertyInfo("Route"),
                new HistoryItemPropertyInfo("DosageForm"),
                new HistoryItemPropertyInfo("Prn"),
                new HistoryItemPropertyInfo("MedicationStatus"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientMedicationHistory, medicationHistoryProperties,
                patientChartNode.Title);
        }
    }
}