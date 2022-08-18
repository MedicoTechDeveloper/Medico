using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;

namespace Medico.Application.Report.ReportNodes
{
    public class ReviewedMedicalRecordsReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public PatientChartNodeType NodeType =>
            PatientChartNodeType.ReviewedMedicalRecordsNode;

        public ReviewedMedicalRecordsReportNodeBuilder(IMedicalRecordService medicalRecordService,
            IDefaultValueService defaultValueService) : base(defaultValueService)
        {
            _medicalRecordService = medicalRecordService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientMedicalRecords = (await _medicalRecordService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientMedicalRecords.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var patientMedicalReportRecords = patientMedicalRecords.Select(r => new MedicalRecordReportVm
            {
                FormattedCreateDate =
                    r.CreateDate.AddHours(clientUtcOffset).ToString("MM/dd/yyyy"),
                DocumentType = r.DocumentType,
                Notes = r.Notes,
                IncludeNotesInReport = r.IncludeNotesInReport
            });

            var medicalRecordProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("DocumentType", true),
                new HistoryItemPropertyInfo("FormattedCreateDate"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientMedicalReportRecords, medicalRecordProperties,
                patientChartNode.Title);
        }
    }
}