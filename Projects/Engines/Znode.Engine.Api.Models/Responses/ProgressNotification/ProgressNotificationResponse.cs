using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProgressNotificationResponse : BaseResponse
    {
        public List<ProgressNotificationModel> ProgressNotifications { get; set; }

        public ProgressNotificationResponse()
        {
            ProgressNotifications = new List<ProgressNotificationModel>();
        }
    }
}
