using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ContentContainerListModel : BaseListModel
    {
        public List<ContentContainerResponseModel> ContainerList { get; set; }
    }
}
