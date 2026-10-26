using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Notification history data access via <see cref="GenericRepository{T, TContext}"/>.
    /// </summary>
    public class NotificationHistoryRepository : GenericRepository<NotificationHistory, SMSDbContext>, INotificationHistoryRepository
    {
        public NotificationHistoryRepository(SMSDbContext context, ILogger<NotificationHistoryRepository> logger)
            : base(context, logger)
        {
        }
    }
}
