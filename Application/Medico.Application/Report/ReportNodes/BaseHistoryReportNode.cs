using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class BaseHistoryReportNode
    {
        private readonly IDefaultValueService _defaultValueService;

        protected BaseHistoryReportNode(IDefaultValueService defaultValueService)
        {
            _defaultValueService = defaultValueService;
        }

        protected virtual string HistoricalReportNodeContentTemplate => "<ul>{0}</ul>";

        protected async Task<string> GetHistoryReportNodeDefaultString(PatientChartNodeType patientChartNodeType,
            string reportNodeTitle)
        {
            var defaultReportNodeValue = await _defaultValueService
                .GetPatientChartNodeType(patientChartNodeType);

            var defaultReportNodeValueHtmlString =
                $"<li>{defaultReportNodeValue.Value}</li>";

            var historicalReportNodeContent =
                string.Format(HistoricalReportNodeContentTemplate, defaultReportNodeValueHtmlString);

            return string.Format(ReportNodeTemplates.RowTemplate, reportNodeTitle,
                historicalReportNodeContent);
        }

        protected string GetHistoryReportNodeString(IEnumerable<object> historyItems,
            IList<HistoryItemPropertyInfo> historyItemPropertiesInfos, string reportNodeTitle)
        {
            var historyStringBuilder = new StringBuilder();

            foreach (var historyItem in historyItems)
            {
                var historyItemProperties =
                    historyItem.GetType().GetProperties();

                historyStringBuilder.Append("<li>");

                foreach (var historyItemPropertyInfo in historyItemPropertiesInfos)
                {
                    var historyItemPropertyValue =
                        historyItemProperties
                            .Single(hi => hi.Name == historyItemPropertyInfo.Name)
                            .GetValue(historyItem);

                    var historyItemPropertyValueString = historyItemPropertyValue == null
                        ? string.Empty
                        : historyItemPropertyValue.ToString()
                            .Trim();

                    var historyItemKeyPropertyValueName = historyItemPropertyInfo.DependsOn;
                    if (!string.IsNullOrEmpty(historyItemKeyPropertyValueName))
                    {
                        var historyItemKeyPropertyValue = historyItemProperties
                            .Single(hi => hi.Name == historyItemKeyPropertyValueName)
                            .GetValue(historyItem);

                        if (historyItemKeyPropertyValue is bool value)
                        {
                            if (value && !string.IsNullOrEmpty(historyItemPropertyValueString))
                            {
                                historyStringBuilder.Append(historyItemPropertyInfo.IsFirstItem
                                    ? historyItemPropertyValue
                                    : $" - {historyItemPropertyValue}");
                            }
                        }
                        else
                            throw new InvalidOperationException("The depends on property should be a type of bool");
                    }

                    else
                    {
                        if (!string.IsNullOrEmpty(historyItemPropertyValueString))
                            historyStringBuilder.Append(historyItemPropertyInfo.IsFirstItem
                                ? historyItemPropertyValue
                                : $" - {historyItemPropertyValue}");
                    }
                }

                historyStringBuilder.Append("</li>");
            }

            var historicalReportNodeContent =
                string.Format(HistoricalReportNodeContentTemplate, historyStringBuilder);

            return string.Format(ReportNodeTemplates.RowTemplate, reportNodeTitle, historicalReportNodeContent);
        }
    }
}