using System;
using System.ComponentModel.DataAnnotations;

namespace Medico.Application.ViewModels.Patient
{
    public class PatientLoginVm
    {
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required] public Guid CompanyId { get; set; }
    }
}