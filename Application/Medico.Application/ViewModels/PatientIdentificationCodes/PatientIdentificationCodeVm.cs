using System;
using Medico.Application.ViewModels;

namespace Medico.Application.PatientIdentificationCodes.ViewModels
{
    public class PatientIdentificationCodeVm : BaseViewModel
    {
        public string Prefix { get; set; }

        public string LetterCode { get; set; }

        public string IdentificationCodeString { get; set; }

        public int NumericCode { get; set; }

        public int Type { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public Guid PatientId { get; set; }
    }
}