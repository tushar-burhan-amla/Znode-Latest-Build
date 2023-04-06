using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ParentAccountListResponse : BaseListResponse
    {
        public List<ParentAccountModel> ParentAccount { get; set; }

        public ParentAccountListResponse()
        {
            ParentAccount = new List<ParentAccountModel>();
        }
    }
}
