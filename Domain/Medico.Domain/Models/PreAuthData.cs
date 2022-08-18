using System;

namespace Medico.Domain.Models
{
    public class PreAuthData : Entity
    {
       
        public string PreAuth { get; set; }
        public Guid  AppointmentId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
