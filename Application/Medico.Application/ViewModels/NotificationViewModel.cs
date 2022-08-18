using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Medico.Application.ViewModels
{
    public class NotificationInputModel : NotificationViewModel
    {
        public IList<string> UserIds { get; set; }
    }

    public class NotificationViewModel
    {
        public int Id { get; set; }
        [StringLength(500)] public string Title { get; set; }
        public string Description { get; set; }
        [StringLength(1000)] public string Link { get; set; }
        [MaxLength(50)] public string CreatedBy { get; set; }
        [Required] public DateTime CreatedOn { get; set; }
        [MaxLength(50)] public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatorName { get; set; }
        public bool? IsRead { get; set; }
        public int ParentId { get; set; }
        public int NotificationTypeId { get; set; }
        public string EntityStatus { get; set; }
        public DateTime? ReminderDate { get; set; }
        public IEnumerable<UserModel> UserModels { get; set; }
    }

    public class NotificationReadViewModel
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        [MaxLength(50)] public string UserId { get; set; }
        [DefaultValue(false)] public bool IsRead { get; set; }
        public IList<string> UserIds { get; set; }
    }

    public class NotificationReadViewModel1
    {
        [DefaultValue(false)]
        public bool IsRead { get; set; }
    }

    public class NotificationCount
    {
        public int TotalCount { get; set; }
    }
}
