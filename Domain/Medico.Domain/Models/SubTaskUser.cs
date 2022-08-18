using System;

namespace Medico.Domain.Models
{
    public class SubTaskUser: Entity
    {
        public Guid SubTaskId { get; set; }
        public string UserId { get; set; }
    }
}
