using System.ComponentModel.DataAnnotations;

namespace Medico.Application.ViewModels
{
    public class InsuranceCompanyViewModel: BaseViewModel
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

    public class InsuranceCompanyProjectionViewModel
        : BaseViewModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

    }
}
