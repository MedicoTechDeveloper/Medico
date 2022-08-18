using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Medico.Application.SelectableItemsManagement;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class TemplateReportNodeBuilder : IReportNodeBuilder
    {
        private readonly ISelectableItemsService _selectableItemsService;

        public PatientChartNodeType NodeType =>
            PatientChartNodeType.TemplateNode;

        public TemplateReportNodeBuilder(ISelectableItemsService selectableItemsService)
        {
            _selectableItemsService = selectableItemsService;
        }

        public Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var patientChartHtmlTemplateContent = GetTemplateContent(patientChartNode);
            var patientChartReportNodeContent =
                string.Format(ReportNodeTemplates.RowTemplate, patientChartNode.Title, patientChartHtmlTemplateContent);

            return Task.FromResult(patientChartReportNodeContent);
        }

        private string GetTemplateContent(PatientChartNode patientChartNode)
        {
            var templateContent = patientChartNode.Value;

            bool isDetailedTemplateUsed = templateContent.isDetailedTemplateUsed;

            string templateHtmlContent = isDetailedTemplateUsed
                ? templateContent.detailedTemplateHtml
                : templateContent.defaultTemplateHtml;

            if (isDetailedTemplateUsed)
                templateHtmlContent = _selectableItemsService.WrapAllSelectableListValuesToBoldTag(templateHtmlContent);

            templateHtmlContent = TrimEmptyLinesIfNeeded(templateHtmlContent);

            return templateHtmlContent;
        }

        private static string TrimEmptyLinesIfNeeded(string templateHtmlContent)
        {
            templateHtmlContent =
                Regex.Replace(templateHtmlContent, "(\r\n|\n|\r)", string.Empty);

            return Regex.Replace(templateHtmlContent, "(<p>&nbsp;<\\/p>)*$", string.Empty);
        }
    }
}