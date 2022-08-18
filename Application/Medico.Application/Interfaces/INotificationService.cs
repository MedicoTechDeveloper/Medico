using Medico.Application.ViewModels;
using Medico.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medico.Application.Interfaces
{
    public interface INotificationService
    {
        #region Notification
        
        Task<int> Create(NotificationViewModel notificationViewModel);
        Task<IEnumerable<NotificationViewModel>> GetNotification(string currentUserId);
        Task<IEnumerable<NotificationViewModel>> GetNotificationReply(int ParentId, string currentUserId);
        Task<IEnumerable<NotificationType>> GetNotificationType();
        Task<IEnumerable<NotificationViewModel>> GetHeaderNotifications(string currentUserId);
        Task<int> MapParentNotification(int notifyId, IList<string> userIds);

        #endregion

        #region Notification Read

        /*Task<int> AddNotifyRead(NotificationReadViewModel notificationReadViewModel, int notifyId);*/
        Task<bool> EditNotifyRead(int id, NotificationReadViewModel1 notificationReadDto1, string currentUserId);
        Task<IEnumerable<NotificationReadViewModel>> GetNotificationRead(int notificationId);
        Task<NotificationCount> GetNotificationCount(string currentUserId);
        Task<IEnumerable<UserModel>> GetRecipients(int id);

        #endregion
    }
}
