namespace Medico.Application.ViewModels
{
    public class ReportOccupationalHistoryVm
    {
        public string OccupationalType { get; set; }

        public string WorkingDaysNumber { get; set; }

        public string DisabilityClaimDetails { get; set; }

        public string WorkersCompensationClaimDetails { get; set; }

        public string EmploymentStatus { get; set; }
        
        public string Notes { get; set; }
        
        public bool IncludeNotesInReport { get; set; }
    }
}