using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Medico.Application.Interfaces;
using Medico.Application.ViewModels;
using Medico.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Application.Services
{
    public class NotificationService : INotificationService
    {
        #region DI
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public NotificationService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        #endregion

        #region Methods
        public async Task<int> Create(NotificationViewModel notificationViewModel)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                Notification notification = _mapper.Map<Notification>(notificationViewModel);
                if (notification != null)
                {
                    int id = await con.InsertAsync(notification);
                    return id;
                }
                return 0;
            }
        }

        public async Task<IEnumerable<NotificationViewModel>> GetNotification(string currentUserId)
        {
            using (IDbConnection con = Connection)
            {
                //var dateRange = "today";
                //if (dateTime.HasValue)
                //{
                //    dateTime = dateTime.Value.AddHours(-7);
                //    dateRange = "previous";
                //}
                //else
                //{
                //    dateTime = DateTime.UtcNow.AddHours(-7);
                //}
                string query = @"GetNotifications";

                IEnumerable<NotificationViewModel> notificationDto = await con.QueryAsync<NotificationViewModel>(query,
                    new
                    {
                        currentUserId,
                    }, commandType: CommandType.StoredProcedure);

                return notificationDto;
            }
        }

        public async Task<IEnumerable<NotificationViewModel>> GetHeaderNotifications(string currentUserId)
        {
            using (IDbConnection con = Connection)
            {

                string query = @"GetHeaderNotifications";

                IEnumerable<NotificationViewModel> notificationDto = await con.QueryAsync<NotificationViewModel>(query,
                    new
                    {
                        currentUserId,
                    }, commandType: CommandType.StoredProcedure);

                return notificationDto;
            }
        }

        public async Task<IEnumerable<NotificationViewModel>> GetNotificationReply(int notificationId, string currentUserId)
        {
            using (IDbConnection con = Connection)
            {
                string query = @"[dbo].[GetNotificationReply]";

                IEnumerable<NotificationViewModel> notificationDto = await con.QueryAsync<NotificationViewModel>(query,
                    new { notificationId, currentUserId },
                    commandType: CommandType.StoredProcedure
                    );

                return notificationDto;
            }
        }

        public async Task<int> MapParentNotification(int notifyId, IList<string> userIds)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();

                foreach (var item in userIds)
                {
                    var exists = con.GetAll<Notification_User_Mapping>()
                        .Count(c => c.NotificationId == notifyId && c.UserId == item);

                    if (exists == 0)
                    {
                        var entity = new Notification_User_Mapping
                        {
                            NotificationId = notifyId,
                            UserId = item
                        };
                        int id = await con.InsertAsync(entity);
                    }
                }
                return 1;
            }
        }
        #endregion

        #region Notification Read

        /*public async Task<int> AddNotifyRead(NotificationReadViewModel notificationReadViewModel, int notifyId)
        {
            IDbConnection con = Connection;
            
            foreach (var item in notificationReadViewModel.UserIds)
            {
                notificationReadViewModel.UserId = item;
                notificationReadViewModel.NotificationId = notifyId;
                NotificationRead notificationRead = _mapper.Map<NotificationRead>(notificationReadViewModel);
                if (notificationRead != null)
                {
                    await con.InsertAsync(notificationRead);
                }
            }
            return 1;
        }*/

        public async Task<bool> EditNotifyRead(int id, NotificationReadViewModel1 notificationReadDto1, string currentUserId)
        {
            using (IDbConnection con = Connection)
            {
                /*string query = @"UPDATE dbo.NotificationReads SET IsRead = @IsRead Where UserId=@currentUserId And NotificationId=@id";
                int rowsAffected = await con.ExecuteAsync(query, new { id, currentUserId, IsRead = notificationReadDto1.IsRead });

                if(rowsAffected > 0)
                {*/
                    string query1 = @"UPDATE dbo.Notification_User_Mapping SET IsRead = @IsRead Where UserId=@currentUserId And NotificationId=@id";
                    int rowsAffected1 = await con.ExecuteAsync(query1, new { id, currentUserId, IsRead = notificationReadDto1.IsRead });
                //}
                return (rowsAffected1 > 0);
            }
        }

        public async Task<IEnumerable<NotificationReadViewModel>> GetNotificationRead(int notificationId)
        {
            using (IDbConnection con = Connection)
            {
                string query = @"SELECT UserId FROM NotificationReads where NotificationId=@notificationId";

                IEnumerable<NotificationReadViewModel> notificationDto = await con.QueryAsync<NotificationReadViewModel>(query, new { notificationId });

                return notificationDto;
            }
        }

        public async Task<NotificationCount> GetNotificationCount(string currentUserId)
        {
            using (IDbConnection con = Connection)
            {
                con.Open();
                var count = await con.QuerySingleAsync<NotificationCount>(@"GetUnreadNotificationCount",
                   new { currentUserId },
                   commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<IEnumerable<NotificationType>> GetNotificationType()
        {
            using (IDbConnection con = Connection)
            {
                string query = @"SELECT * FROM NotificationTypes";

                IEnumerable<NotificationType> notificationDto = await con.QueryAsync<NotificationType>(query, new { });

                return notificationDto;
            }
        }

        public async Task<IEnumerable<UserModel>> GetRecipients(int notificationId)
        {
            using (IDbConnection con = Connection)
            {
                string query = @"SELECT a.Id, ISNULL(m.FirstName, 'SuperAdmin') as FirstName, 
                                m.LastName FROM Notification_user_mapping umap 
                                INNER JOIN AspNetUsers a ON umap.UserId = a.Id
                                LEFT JOIN MedicoApplicationUser m ON m.Email = a.Email
                                WHERE umap.NotificationId = @NotificationId";

                var list = await con.QueryAsync<UserModel>(query, new { notificationId });

                return list;
            }
        }

        #endregion
    }
}