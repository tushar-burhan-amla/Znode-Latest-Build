using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishHistoryListModel : BaseListModel
    {
        public List<PublishHistoryModel> PublishHistoryList { get; set; }

        public PublishHistoryListModel()
        {
            PublishHistoryList = new List<PublishHistoryModel>();
        }
    }
}
