using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Medico.Application.ViewModels.Admission
{
    public class UpdatePatientChartDocumentNodesVm
    {
        [Required]
        public Guid AdmissionId { get; set; }

        public IEnumerable<LookupStateViewModel> DocumentNodes { get; set; }
    }
}