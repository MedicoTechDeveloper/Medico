namespace Medico.Application.Services.PatientChart
{
    public class PatientChartNodeAttributes
    {
        public int? Order { get; set; }

        public bool IsActive { get; set; }

        public bool IsPredefined { get; set; }

        public bool IsNotShownInReport { get; set; }

        public bool SignedOffOnly { get; set; }

        public NodeSpecificAttributes NodeSpecificAttributes { get; set; }

        public static PatientChartNodeAttributes CreatePatientChartNodeAttributes(int? order,
            bool isActive, bool isNotShownInReport,
            bool signedOffOnly, bool isPredefined, dynamic nodeSpecificAttributes = null)
        {
            var patientChartNodeAttributes = new PatientChartNodeAttributes
            {
                Order = order,
                IsPredefined = isPredefined,
                IsActive = isActive,
                IsNotShownInReport = isNotShownInReport,
                SignedOffOnly = signedOffOnly,
                NodeSpecificAttributes = nodeSpecificAttributes == null ? new { } : nodeSpecificAttributes
            };

            return patientChartNodeAttributes;
        }
    }
}