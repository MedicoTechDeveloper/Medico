using System;

namespace Medico.Application.ViewModels
{
    public class VitalSignsLookUpViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string VarModifier { get; set; }
        public decimal MinVar { get; set; }
        public decimal MaxVar { get; set; }
        public string Gender { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string ValUnits { get; set; }
        public string AVID { get; set; }
    }

    public class VitalSignsLookUpProjectionViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string VarModifier { get; set; }
        public decimal MinVar { get; set; }
        public decimal MaxVar { get; set; }
        public string Gender { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string ValUnits { get; set; }
        public string AVID { get; set; }
    }
}
