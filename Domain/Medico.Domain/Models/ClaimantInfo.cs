using System.ComponentModel.DataAnnotations.Schema;

namespace Medico.Domain.Models
{
    [Table("ClaimantInfo")]
    public class ClaimantInfo
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
    }
}
