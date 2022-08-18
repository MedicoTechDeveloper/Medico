using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;

namespace Medico.Application.Report.ReportNodes
{
    public class OccupationalHistoryReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IOccupationalHistoryService _occupationalHistoryService;

        public OccupationalHistoryReportNodeBuilder(IDefaultValueService defaultValueService,
            IOccupationalHistoryService occupationalHistoryService) : base(
            defaultValueService)
        {
            _occupationalHistoryService = occupationalHistoryService;
        }

        public PatientChartNodeType NodeType =>
            PatientChartNodeType.OccupationalHistoryNode;

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId, Guid admissionId,
            int clientUtcOffset)
        {
            var patientOccupationalHistory = (await _occupationalHistoryService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientOccupationalHistory.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var patientReportOccupationalHistoryItems = patientOccupationalHistory
                .Select(oh =>
                {
                    var reportOccupationalHistory = new ReportOccupationalHistoryVm();

                    var startDate = oh.Start;
                    var endDate = oh.End;

                    if (startDate == null || endDate == null)
                        reportOccupationalHistory.WorkingDaysNumber = string.Empty;
                    else
                    {
                        var workingDaysNumber = (endDate.Value - startDate.Value).TotalDays;

                        reportOccupationalHistory.WorkingDaysNumber =
                            Math.Round(workingDaysNumber, 0)
                                .ToString(CultureInfo.InvariantCulture);
                    }

                    reportOccupationalHistory.OccupationalType = oh.OccupationalType;
                    reportOccupationalHistory.DisabilityClaimDetails = oh.DisabilityClaimDetails;
                    reportOccupationalHistory.WorkersCompensationClaimDetails = oh.WorkersCompensationClaimDetails;
                    reportOccupationalHistory.EmploymentStatus = oh.EmploymentStatus;
                    reportOccupationalHistory.Notes = oh.Notes;
                    reportOccupationalHistory.IncludeNotesInReport = oh.IncludeNotesInReport;

                    return reportOccupationalHistory;
                });

            var occupationalHistoryProperties = new List<HistoryItemPropertyInfo>
            {
                new HistoryItemPropertyInfo("OccupationalType", true),
                new HistoryItemPropertyInfo("EmploymentStatus"),
                new HistoryItemPropertyInfo("WorkingDaysNumber"),
                new HistoryItemPropertyInfo("DisabilityClaimDetails"),
                new HistoryItemPropertyInfo("WorkersCompensationClaimDetails"),
                new HistoryItemPropertyInfo("Notes", false, "IncludeNotesInReport")
            };

            return GetHistoryReportNodeString(patientReportOccupationalHistoryItems, occupationalHistoryProperties,
                patientChartNode.Title);
        }
    }
}