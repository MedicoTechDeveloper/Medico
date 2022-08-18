using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;

namespace Medico.Application.Report.ReportNodes
{
    public class AllergiesReportNodeBuilder : BaseHistoryReportNode, IReportNodeBuilder
    {
        private readonly IAllergyService _allergyService;
        public PatientChartNodeType NodeType => PatientChartNodeType.AllergiesNode;

        public AllergiesReportNodeBuilder(IAllergyService allergyService, IDefaultValueService defaultValueService)
            : base(defaultValueService)
        {
            _allergyService = allergyService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId, Guid admissionId,
            int clientUtcOffset)
        {
            var patientAllergies = (await _allergyService
                    .GetByPatientId(patientId))
                .ToList();

            if (!patientAllergies.Any())
                return await GetHistoryReportNodeDefaultString(patientChartNode.Type, patientChartNode.Title);

            var allergiesHtmlStringBuilder = new StringBuilder("<ul>");

            foreach (var patientAllergy in patientAllergies)
            {
                var medicationAndReactionHtmlString =
                    $"<li>{patientAllergy.Medication} - {patientAllergy.Reaction}";

                allergiesHtmlStringBuilder.Append(medicationAndReactionHtmlString);

                var patientAllergyNotes = patientAllergy.Notes;

                if (patientAllergy.IncludeNotesInReport && !string.IsNullOrEmpty(patientAllergyNotes))
                {
                    var allergiesNotesString = $" - {patientAllergyNotes}";
                    allergiesHtmlStringBuilder.Append(allergiesNotesString);
                }

                allergiesHtmlStringBuilder.Append("</li>");
            }

            allergiesHtmlStringBuilder.Append("</ul>");

            return
                string.Format(ReportNodeTemplates.RowTemplate, patientChartNode.Title, allergiesHtmlStringBuilder);
        }
    }
}