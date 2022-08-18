using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class PrescriptionReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IMedicationPrescriptionService _medicationPrescriptionService;

        public PatientChartNodeType NodeType =>
            PatientChartNodeType.PrescriptionNode;

        public PrescriptionReportNodeBuilder(IMedicationPrescriptionService medicationPrescriptionService,
            IDefaultValueService defaultValueService) : base(defaultValueService)
        {
            _medicationPrescriptionService = medicationPrescriptionService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientPrescription = (await _medicationPrescriptionService
                    .GetByAdmissionId(admissionId))
                .ToList();

            if (!patientPrescription.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var prescriptionProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Medication", true),
                new HistoryItemPropertyInfo("Dose"),
                new HistoryItemPropertyInfo("Units"),
                new HistoryItemPropertyInfo("Route"),
                new HistoryItemPropertyInfo("DosageForm"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientPrescription, prescriptionProperties, patientChartNode.Title);
        }
    }
}