using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medico.Application.Services.PatientChart;
using Newtonsoft.Json.Linq;

namespace Medico.Application.Report.ReportNodes
{
    public class AssessmentReportNodeBuilder : IReportNodeBuilder
    {
        public PatientChartNodeType NodeType => PatientChartNodeType.AssessmentNode;

        public Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId, Guid admissionId,
            int clientUtcOffset)
        {
            JArray assessments = patientChartNode.Value;

            if (assessments == null || !assessments.Any())
                return Task.FromResult(string.Empty);

            var assessmentsHtmlStringBuilder = new StringBuilder("<ul>");

            foreach (var assessment in assessments)
            {
                var diagnosis = assessment["diagnosis"].Value<string>();
                assessmentsHtmlStringBuilder.Append($"<li><div>{diagnosis}</div>");

                var assessmentNotes = assessment["notes"];
                if (assessmentNotes != null)
                {
                    var notes = assessmentNotes.Value<string>();
                    if (!string.IsNullOrEmpty(notes))
                        assessmentsHtmlStringBuilder.Append($"<div>{notes}</div>");
                }

                assessmentsHtmlStringBuilder.Append("</li>");
            }

            assessmentsHtmlStringBuilder.Append("</ul>");

            var patientChartReportNodeContent =
                string.Format(ReportNodeTemplates.RowTemplate, patientChartNode.Title, assessmentsHtmlStringBuilder);

            return Task.FromResult(patientChartReportNodeContent);
        }
    }
}