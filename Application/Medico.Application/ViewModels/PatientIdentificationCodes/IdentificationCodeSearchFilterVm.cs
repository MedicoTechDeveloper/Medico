using System;

namespace Medico.Application.PatientIdentificationCodes.ViewModels
{
    public class IdentificationCodeSearchFilterVm
    {
        public Guid PatientId { get; set; }
        
        public int IdentificationCodeType { get; set; }
    }
}