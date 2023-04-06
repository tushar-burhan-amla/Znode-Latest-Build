using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishStateMappingListModel : BaseListModel
    {
        public List<PublishStateMappingModel> PublishStateMappingList { get; set; }

        public PublishStateMappingListModel()
        {
            PublishStateMappingList = new List<PublishStateMappingModel>();
        }
    }
}
