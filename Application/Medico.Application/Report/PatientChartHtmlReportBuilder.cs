using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medico.Application.Constants;
using Medico.Application.Extensions;
using Medico.Application.Helpers;
using Medico.Application.Interfaces;
using Medico.Application.Report.ReportNodes;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;
using Medico.Application.ViewModels.Company;
using Medico.Domain.Interfaces;
using Medico.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Medico.Application.Report
{
    public class PatientChartHtmlReportBuilder : IPatientChartReportBuilder
    {
        private readonly IAdmissionService _admissionService;
        private readonly ICompanyService _companyService;
        private readonly IPatientInsuranceService _patientInsuranceService;
        private readonly IAppointmentGridItemRepository _appointmentGridItemRepository;
        private readonly IEnumerable<IReportNodeBuilder> _reportNodeBuilders;
        private readonly MedicoSettingsViewModel _medicoSettings;

        public PatientChartHtmlReportBuilder(IOptions<MedicoSettingsViewModel> medicoSettings,
            IAdmissionService admissionService,
            ICompanyService companyService,
            IPatientInsuranceService patientInsuranceService,
            IAppointmentGridItemRepository appointmentGridItemRepository,
            IEnumerable<IReportNodeBuilder> reportNodeBuilders)
        {
            _admissionService = admissionService;
            _companyService = companyService;
            _patientInsuranceService = patientInsuranceService;
            _appointmentGridItemRepository = appointmentGridItemRepository;
            _reportNodeBuilders = reportNodeBuilders;
            _medicoSettings = medicoSettings.Value;
        }

        public async Task<string> BuildReportBasedOnPatientChart(Guid admissionId,
            Guid companyId, Guid patientId, int utcOffset)
        {
            var htmlReportContent = new StringBuilder();

            var company = await _companyService.GetById(companyId);
            var insurance = await _patientInsuranceService.GetByPatientId(patientId);
            var appointmentGridItem = await _appointmentGridItemRepository.GetAll()
                .FirstOrDefaultAsync(a => a.AdmissionId == admissionId);

            htmlReportContent.Append(BuildReportHeader(company, appointmentGridItem, insurance, utcOffset));

            var admission = await _admissionService.GetById(admissionId);

            var patientChart =
                JsonConvert.DeserializeObject<PatientChartNode>(admission.AdmissionData);

            var reportBodyStringBuilder = new StringBuilder();

            await BuildReportNode(patientId, patientChart, reportBodyStringBuilder, admissionId, utcOffset);

            htmlReportContent.Append(reportBodyStringBuilder.ToString());

            return htmlReportContent.ToString();
        }

        private async Task BuildReportNode(Guid patientId, PatientChartNode patientChartNode,
            StringBuilder reportBodyStringBuilder,
            Guid admissionId, int clientUtcOffset)
        {
            var patientChartNodeType = patientChartNode.Type;
            var patientChartNodeReportBuilder = _reportNodeBuilders
                .FirstOrDefault(b => b.NodeType == patientChartNodeType);

            if (patientChartNodeReportBuilder != null)
            {
                var patientChartNodeContent =
                    await patientChartNodeReportBuilder
                        .BuildContent(patientChartNode, patientId, admissionId, clientUtcOffset);

                if (!string.IsNullOrEmpty(patientChartNodeContent))
                    reportBodyStringBuilder.Append(patientChartNodeContent);
            }

            var children = patientChartNode.Children;

            if (children != null && children.Any())
            {
                foreach (var childPatientChartNode in children)
                {
                    await BuildReportNode(patientId, childPatientChartNode, reportBodyStringBuilder, admissionId,
                        clientUtcOffset);
                }
            }
        }

        private string BuildReportHeader(CompanyVm company, AppointmentGridItem appointmentGridItem,
            PatientInsuranceViewModel insurance, int utcOffset)
        {
            var state = new StateList()[company.State];

            var appointmentInfoTable =
                CreateAppointmentInfoTable(insurance, appointmentGridItem, utcOffset);

            return $@"
                    <div>
                        <div style=""overflow:hidden"">
                            <div style=""float:right;width:33.3%;text-align:right;"">
                                <img src=""{_medicoSettings.CompanyLogoUrl}"">
                            </div>
                        </div>
                        <div style=""overflow:hidden;margin-top:10px;"">
                            <div style=""line-height:1.1em;color:grey;font-size:0.8em;font-weight:bold;float:right;width:33.3%;text-align:right;"">
                                <span>{company.Address} - {company.SecondaryAddress}</span><br/>
                                <span>{company.City} - {state} - 85012</span><br/>
                                <span>Office {company.Phone}</span><br/>
                                <span>Fax {company.Fax} - 866 264 4120</span><br/>
                            </div>
                        </div>
                    </div>
                    <h2 style=""color:grey;clear:both;"">History and Physical</h2>
                    <hr/>
                    <div>{appointmentInfoTable}</div>
                ";
        }

        private static string CreateAppointmentInfoTable(PatientInsuranceViewModel patientInsurance,
            AppointmentGridItem appointmentGridItem, int utcOffset)
        {
            const string emptyValue = "---";

            var patientName = $"{appointmentGridItem.PatientFirstName} {appointmentGridItem.PatientLastName}";
            var patientDateOfBirth = appointmentGridItem.PatientDateOfBirth
                .AddHours(utcOffset).ToString("d");
            var patientAge = appointmentGridItem.PatientDateOfBirth.GetAge();

            var rqId = patientInsurance?.Rqid ?? emptyValue;
            var caseNumber = patientInsurance?.CaseNumber ?? emptyValue;
            var ssn = patientInsurance?.Ssn ?? emptyValue;

            var appointmentDate =
                appointmentGridItem.StartDate.AddHours(utcOffset)
                    .ToString("MM/dd/yyyy HH:mm");

            return HtmlReportHelper.CreateReportTable(new[]
            {
                new[]
                {
                    "<strong>Name:</strong>",
                    patientName,
                    "<strong>Date of Exam:</strong>",
                    appointmentDate
                },
                new[]
                {
                    "<strong>Date Of Birth / Age:</strong>",
                    $"{patientDateOfBirth} / {patientAge}",
                    "<strong>RQID:</strong>",
                    rqId
                },
                new[]
                {
                    "<strong>Case Number:</strong>",
                    caseNumber,
                    "<strong>Consultative Examiner:</strong>",
                    $"{appointmentGridItem.PhysicianFirstName} {appointmentGridItem.PhysicianLastName}"
                },
                new[]
                {
                    "<strong>Social Security</strong>:",
                    ssn,
                    "<strong>Location of Exam:</strong>",
                    appointmentGridItem.LocationName
                }
            });
        }
    }
}