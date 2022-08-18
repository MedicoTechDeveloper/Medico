using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Medico.Domain.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [StringLength(250)] public string Title { get; set; }
        public string Description { get; set; }
        [StringLength(1000)] public string Link { get; set; }
        [Required] [MaxLength(50)] public string CreatedBy { get; set; }
        [Required] public DateTime CreatedOn { get; set; }
        [MaxLength(50)] public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int ParentId { get; set; }
        public int NotificationTypeId  { get; set; }
        public string EntityStatus { get; set; }
        public DateTime? ReminderDate { get; set; }
        [Computed]
        public ICollection<Notification_User_Mapping> Notification_User_Mapping { get; set; }
    }

   
}
