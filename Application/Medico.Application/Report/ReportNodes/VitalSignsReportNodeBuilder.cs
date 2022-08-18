using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Medico.Application.Helpers;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;

namespace Medico.Application.Report.ReportNodes
{
    public class VitalSignsReportNodeBuilder : IReportNodeBuilder
    {
        private readonly IVitalSignsService _vitalSignsService;
        private readonly IBaseVitalSignsService _baseVitalSignsService;
        private readonly IVisionVitalSignsService _visionVitalSignsService;
        private readonly IVitalSignsNotesService _vitalSignsNotesService;
        public PatientChartNodeType NodeType => PatientChartNodeType.VitalSignsNode;

        public VitalSignsReportNodeBuilder(IVitalSignsService vitalSignsService,
            IBaseVitalSignsService baseVitalSignsService,
            IVisionVitalSignsService visionVitalSignsService,
            IVitalSignsNotesService vitalSignsNotesService)
        {
            _vitalSignsService = vitalSignsService;
            _baseVitalSignsService = baseVitalSignsService;
            _visionVitalSignsService = visionVitalSignsService;
            _vitalSignsNotesService = vitalSignsNotesService;
        }

        public async Task<string> BuildContent(PatientChartNode patientChartNode, Guid patientId,
            Guid admissionId, int clientUtcOffset)
        {
            var baseVitalSignsHtmlString =
                await GetBaseVitalSignsHtmlString(patientId);

            var vitalSignsHtmlString = await
                GetVitalSignsHtmlString(patientId, admissionId, clientUtcOffset);

            var visionVitalSignsHtmlString =
                await GetVisionVitalSignsHtmlString(patientId, clientUtcOffset);

            var vitalSignsNotesHtmlString =
                await GetVitalSignsNotesHtmlString(admissionId);

            return string.Format(ReportNodeTemplates.RowTemplate, "Vital Signs",
                $"{baseVitalSignsHtmlString}{vitalSignsHtmlString}{visionVitalSignsHtmlString}{vitalSignsNotesHtmlString}");
        }

        private async Task<string> GetVitalSignsNotesHtmlString(Guid admissionId)
        {
            var vitalSignsNotes = await _vitalSignsNotesService
                .GetByAdmissionId(admissionId);

            var shouldNotesBeShowNotesInReport = vitalSignsNotes != null &&
                                                 vitalSignsNotes.IncludeNotesInReport &&
                                                 !string.IsNullOrEmpty(vitalSignsNotes.Notes);

            return !shouldNotesBeShowNotesInReport
                ? string.Empty
                : $"{ReportConstants.EmptyLine}{vitalSignsNotes.Notes}";
        }

        private async Task<string> GetVisionVitalSignsHtmlString(Guid patientId, int clientUtcOffset)
        {
            var patientVisionVitalSigns = await _visionVitalSignsService
                .GetByPatientId(patientId);

            if (!patientVisionVitalSigns.Any())
                return string.Empty;

            var visionVitalSignsHtmlTable =
                CreateVisionVitalSignsHtmlTable(patientVisionVitalSigns, clientUtcOffset);

            return $"{ReportConstants.EmptyLine}{visionVitalSignsHtmlTable}";
        }

        private async Task<string> GetVitalSignsHtmlString(Guid patientId, Guid admissionId, int clientUtcOffset)
        {
            var patientVitalSigns = (await _vitalSignsService
                    .GetByPatientAndAdmissionIds(patientId, admissionId))
                .ToList();

            if (!patientVitalSigns.Any())
                return string.Empty;

            var admissionVitalSignsHtmlTable =
                CreateVitalSignsHtmlTable(patientVitalSigns, clientUtcOffset);

            return $"{ReportConstants.EmptyLine}{admissionVitalSignsHtmlTable}";
        }

        private async Task<string> GetBaseVitalSignsHtmlString(Guid patientId)
        {
            var patientBaseVitalSigns =
                await _baseVitalSignsService.GetByPatientId(patientId);

            if (patientBaseVitalSigns == null)
                return string.Empty;

            var baseVitalSignsHtmlTable1 =
                CreateBaseVitalSignsHtmlTable1(patientBaseVitalSigns);

            var baseVitalSignsHtmlTable2 =
                CreateBaseVitalSignsHtmlTable2(patientBaseVitalSigns);

            return
                $"{ReportConstants.EmptyLine}{baseVitalSignsHtmlTable1}{ReportConstants.EmptyLine}{baseVitalSignsHtmlTable2}";
        }

        private static string CreateVisionVitalSignsHtmlTable(List<VisionVitalSignsViewModel> patientVisionVitalSigns,
            int clientUtcOffset)
        {
            var visionVitalSignsTableColumnNames = new[]
                {"Date", "OS", "OD", "OU", "With Glasses"};

            var visionVitalSignsTableColumnValues = patientVisionVitalSigns
                .Select(vs =>
                {
                    var vitalSignsDateTime = vs.CreateDate
                        .AddHours(clientUtcOffset)
                        .ToString("MM/dd/yyyy");

                    const string visionValueTemplate = "{0} / {1}"; 
                    
                    return new[]
                    {
                        vitalSignsDateTime,
                        vs.Os == default ? string.Empty :  string.Format(visionValueTemplate, Constants.Report.VisionVitalSigns.VisualAcuityRating, vs.Os),
                        vs.Od == default ? string.Empty : string.Format(visionValueTemplate, Constants.Report.VisionVitalSigns.VisualAcuityRating, vs.Od),
                        vs.Ou == default ? string.Empty : string.Format(visionValueTemplate, Constants.Report.VisionVitalSigns.VisualAcuityRating, vs.Ou),
                        vs.WithGlasses ? "true" : "false"
                    };
                });

            return HtmlReportHelper
                .CreateReportHtmlTable(visionVitalSignsTableColumnNames, visionVitalSignsTableColumnValues);
        }

        private static string CreateVitalSignsHtmlTable(IEnumerable<VitalSignsViewVM> patientVitalSigns,
            int clientUtcOffset)
        {
            var vitalSignsTableColumnNames = new[]
                {"Time", "BP, mm Hg", "Position", "Pulse, bmp", "Resp, rpm", "O2 Sat, %", "Activity"};

            var vitalSignsTableColumnValues = patientVitalSigns.Select(vs =>
            {
                var vitalSignsTime = vs.CreatedDate.AddHours(clientUtcOffset)
                    .ToString("HH:mm");

                var systolicBloodPressure = vs?.SystolicBloodPressure;
                var diastolicBloodPressure = vs?.DiastolicBloodPressure;
                var bloodPressure = $"{systolicBloodPressure} / {diastolicBloodPressure}";

                var pulse = vs?.Pulse.ToString();

                var respirationRate = vs?.RespirationRate.ToString();

                var o2Sat = vs?.OxygenSaturationAtRestValue.ToString();

                return new[]
                {
                    vitalSignsTime, bloodPressure, $"{vs.BloodPressurePosition} / {vs.BloodPressureLocation}", pulse,
                    respirationRate, o2Sat,
                    vs.OxygenSaturationAtRest
                };
            });

            return HtmlReportHelper
                .CreateReportHtmlTable(vitalSignsTableColumnNames, vitalSignsTableColumnValues);
        }

        private static string CreateBaseVitalSignsHtmlTable1(BaseVitalSignsViewModel patientBaseVitalSigns)
        {
            var patientWeight = patientBaseVitalSigns.Weight;
            var isPatientWeightSet = patientWeight != null;
            var patientWeightString = isPatientWeightSet
                ? patientWeight.ToString()
                : string.Empty;

            var patientHeight = patientBaseVitalSigns.Height;
            var isPatientHeightSet = patientHeight != null;
            var patientHeightString = isPatientHeightSet
                ? patientHeight.ToString()
                : string.Empty;

            var bmi = !isPatientWeightSet || !isPatientHeightSet
                ? string.Empty
                : MedicalCalculationHelper.CalculateBmi(patientHeight.Value, patientWeight.Value)
                    .ToString(CultureInfo.InvariantCulture);

            var dominantHand = patientBaseVitalSigns.DominantHand;

            var oxygen = !string.IsNullOrWhiteSpace(patientBaseVitalSigns.OxygenUse) ||
                         patientBaseVitalSigns.OxygenAmount != null
                ? $"{patientBaseVitalSigns.OxygenUse} / {patientBaseVitalSigns.OxygenAmount}"
                : string.Empty;

            var baseVitalSignsTable1ColumnNames = new[]
                {"Weight, lbs", "Height, inches", "BMI, %", "Dominant Hand", "Oxygen"};

            var baseVitalSignsTable1ColumnValues =
                new[] {new[] {patientWeightString, patientHeightString, bmi, dominantHand, oxygen}};

            return HtmlReportHelper
                .CreateReportHtmlTable(baseVitalSignsTable1ColumnNames, baseVitalSignsTable1ColumnValues);
        }

        private static string CreateBaseVitalSignsHtmlTable2(BaseVitalSignsViewModel patientBaseVitalSigns)
        {
            var baseVitalSignsTable2ColumnNames = new[]
                {"Location", "Calf, cm", "Thigh, cm", "Forearm, cm", "Bicep, cm"};

            var baseVitalSignsTable2ColumnValues =
                new[]
                {
                    new[]
                    {
                        "Right",
                        patientBaseVitalSigns.RightCalf == null
                            ? string.Empty
                            : patientBaseVitalSigns.RightCalf.ToString(),
                        patientBaseVitalSigns.RightThigh == null
                            ? string.Empty
                            : patientBaseVitalSigns.RightThigh.ToString(),
                        patientBaseVitalSigns.RightForearm == null
                            ? string.Empty
                            : patientBaseVitalSigns.RightForearm.ToString(),
                        patientBaseVitalSigns.RightBicep == null
                            ? string.Empty
                            : patientBaseVitalSigns.RightBicep.ToString(),
                    },
                    new[]
                    {
                        "Left",
                        patientBaseVitalSigns.LeftCalf == null
                            ? string.Empty
                            : patientBaseVitalSigns.LeftCalf.ToString(),
                        patientBaseVitalSigns.LeftThigh == null
                            ? string.Empty
                            : patientBaseVitalSigns.LeftThigh.ToString(),
                        patientBaseVitalSigns.LeftForearm == null
                            ? string.Empty
                            : patientBaseVitalSigns.LeftForearm.ToString(),
                        patientBaseVitalSigns.LeftBicep == null
                            ? string.Empty
                            : patientBaseVitalSigns.LeftBicep.ToString(),
                    }
                };

            return HtmlReportHelper
                .CreateReportHtmlTable(baseVitalSignsTable2ColumnNames, baseVitalSignsTable2ColumnValues);
        }
    }
}