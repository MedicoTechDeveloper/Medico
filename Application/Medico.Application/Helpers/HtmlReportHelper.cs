using System.Collections.Generic;
using System.Text;

namespace Medico.Application.Helpers
{
    public static class HtmlReportHelper
    {
        public static string CreateReportHtmlTable(IEnumerable<string> header, IEnumerable<string[]> body)
        {
            var htmlTableStringBuilder = new StringBuilder();

            htmlTableStringBuilder
                .Append(
                    "<table style='border-collapse:collapse;'><thead><tr>");

            foreach (var columnName in header)
            {
                htmlTableStringBuilder.Append(
                    $"<th style='border:solid 1px #999;padding:4px 8px;'>{columnName}</th>");
            }

            htmlTableStringBuilder.Append("</tr></thead><tbody>");

            foreach (var rowValues in body)
            {
                htmlTableStringBuilder.Append("<tr>");

                foreach (var value in rowValues)
                {
                    htmlTableStringBuilder.Append(
                        $"<td style='border:solid 1px #999;padding:4px 8px;text-align:center;'>{value}</td>");
                }

                htmlTableStringBuilder.Append("</tr>");
            }

            htmlTableStringBuilder.Append("</tbody></table>");

            return htmlTableStringBuilder.ToString();
        }

        public static string CreateReportTable(IEnumerable<string[]> table)
        {
            var htmlTableString = new StringBuilder("");

            htmlTableString
                .Append("<table style='width:100%'>");

            foreach (var row in table)
            {
                htmlTableString.Append("<tr>");

                foreach (var column in row)
                {
                    htmlTableString.Append($"<td style = 'padding:4px 8px;'>{column}</td >");
                }

                htmlTableString.Append("</tr>");
            }

            htmlTableString.Append("</tbody></table>");
            return htmlTableString.ToString();
        }
    }
}