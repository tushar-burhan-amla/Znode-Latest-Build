using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ManageMessageListResponse : BaseListResponse
    {
        public List<ManageMessageModel> ManageMessages { get; set; }
    }
}
