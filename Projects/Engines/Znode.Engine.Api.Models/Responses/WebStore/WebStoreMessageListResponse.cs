using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreMessageListResponse : BaseListResponse
    {
        public List<ManageMessageModel> Messages { get; set; }
    }
}
