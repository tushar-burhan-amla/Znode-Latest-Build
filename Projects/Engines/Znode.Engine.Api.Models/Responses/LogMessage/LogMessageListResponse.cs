using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class LogMessageListResponse : BaseListResponse
    {
        public List<LogMessageModel> LogMessageList { get; set; }
    }
}
