using System.Collections.Generic;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IProgressNotificationService
    {
        /// <summary>
        /// Get all the progress notifications.
        /// </summary>
        /// <returns></returns>
        Task<List<ProgressNotificationModel>> GetProgressNotifications();
    }
}
