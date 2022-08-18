using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medico.Domain.Models
{
    [Table("Notification_User_Mapping")]
    public class Notification_User_Mapping
    {
        public int Id { get; set; }
        [Required] public int NotificationId { get; set; }
        [Required, MaxLength(50)] public string UserId { get; set; }
        [DefaultValue(false)] public bool IsRead { get; set; }
    }
}
