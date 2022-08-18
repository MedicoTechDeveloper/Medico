using System;
using System.Threading.Tasks;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class AddendumReportNodeBuilder : IReportNodeBuilder
    {
        public PatientChartNodeType NodeType => PatientChartNodeType.AddendumNode;

        public Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId, Guid admissionId,
            int clientUtcOffset)
        {
            string addendumNotes = patientChartNode.Value;

            return Task.FromResult(string.IsNullOrEmpty(addendumNotes)
                ? string.Empty
                : string.Format(ReportNodeTemplates.RowTemplate, patientChartNode.Title, addendumNotes));
        }
    }
}