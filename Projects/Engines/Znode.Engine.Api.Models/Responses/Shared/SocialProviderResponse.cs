using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SocialProviderResponse : BaseResponse
    {
        public List<SocialDomainModel> SocialDomainList { get; set; }
    }
}
