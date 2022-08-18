using System.ComponentModel.DataAnnotations;

namespace Medico.Domain.Models
{
    public class InsuranceCompany: Entity
    {
        [Required] public string Name { get; set; }
       
        [Required] public string Address { get; set; }

        [Required] public string City { get; set; }

        [Required] public string Phone { get; set; }

        public string Email { get; set; }

        [Required] public string Zip { get; set; }

        [Required] public int State { get; set; }
        public string WebSiteUrl { get; set; }
    }
}
