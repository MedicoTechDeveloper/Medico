using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class MedicalHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IMedicalHistoryService _medicalHistoryService;

        public PatientChartNodeType NodeType => PatientChartNodeType.PreviousMedicalHistoryNode;

        public MedicalHistoryReportNodeBuilder(IMedicalHistoryService medicalHistoryService,
            IDefaultValueService defaultValueService) : base(defaultValueService)
        {
            _medicalHistoryService = medicalHistoryService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientMedicalHistory = (await _medicalHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientMedicalHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var medicalHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("Diagnosis", true),
                new HistoryItemPropertyInfo("Notes", false,  "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientMedicalHistory, medicalHistoryProperties, patientChartNode.Title);
        }
    }
}