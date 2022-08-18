using System;
using System.Threading.Tasks;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public abstract class ContainerReportNodeBuilder : IReportNodeBuilder
    {
        public abstract PatientChartNodeType NodeType { get; }

        public Task<string> BuildContent(PatientChartNode patientChartNode,
            Guid patientId, Guid admissionId, int clientUtcOffset)
        {
            return Task.FromResult(string.Format(ReportNodeTemplates.TitleTemplate, patientChartNode.Title));
        }
    }

    public class TemplateListReportNodeBuilder : ContainerReportNodeBuilder
    {
        public override PatientChartNodeType NodeType =>
            PatientChartNodeType.TemplateListNode;
    }
    
    public class DocumentReportNodeBuilder : ContainerReportNodeBuilder
    {
        public override PatientChartNodeType NodeType =>
            PatientChartNodeType.DocumentNode;
    }
    
    public class SectionReportNodeBuilder : ContainerReportNodeBuilder
    {
        public override PatientChartNodeType NodeType =>
            PatientChartNodeType.SectionNode;
    }
}