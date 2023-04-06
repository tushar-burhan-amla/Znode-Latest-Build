using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class LogMessageListModel : BaseListModel
    {
        public List<LogMessageModel> LogMessageList { get; set; }

        public LogMessageListModel()
        {
            LogMessageList = new List<LogMessageModel>();
        }
    }
}
