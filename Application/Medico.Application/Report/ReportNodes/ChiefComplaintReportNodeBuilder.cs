using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medico.Application.Services.PatientChart;
using Newtonsoft.Json.Linq;

namespace Medico.Application.Report.ReportNodes
{
    public class ChiefComplaintReportNodeBuilder : IReportNodeBuilder
    {
        public PatientChartNodeType NodeType =>
            PatientChartNodeType.ChiefComplaintNode;

        public Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var allegations = GetAllegationList(patientChartNode);

            if (string.IsNullOrEmpty(allegations))
                return Task.FromResult(string.Empty);

            var patientChartReportNodeContent =
                string.Format(ReportNodeTemplates.RowTemplate, "Allegations", allegations);

            return Task.FromResult(patientChartReportNodeContent);
        }

        private static string GetAllegationList(PatientChartNode patientChartNode)
        {
            var patientAdmissionSectionValue = (JObject) patientChartNode.Value;

            if (patientAdmissionSectionValue == null || !patientAdmissionSectionValue.HasValues)
                return string.Empty;

            IList<string> patientAllegationsSets =
                ((JArray) patientAdmissionSectionValue["patientAllegationsSets"]).Select(a => (string) a["allegations"])
                .ToList();

            if (!patientAllegationsSets.Any())
                return string.Empty;

            var allegationsStringBuilder = new StringBuilder();

            foreach (var allegations in patientAllegationsSets)
            {
                if (string.IsNullOrEmpty(allegations))
                    continue;

                var allegationsHtmlString = $"<li>{allegations}</li>";
                allegationsStringBuilder.Append(allegationsHtmlString);
            }

            return $"<ul>{allegationsStringBuilder}</ul>";
        }
    }
}