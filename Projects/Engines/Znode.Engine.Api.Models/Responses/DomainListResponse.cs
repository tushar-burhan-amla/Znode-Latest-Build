using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DomainListResponse : BaseListResponse
    {
        public List<DomainModel> Domains { get; set; }
    }
}
