using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SocialModel:BaseModel
    {
        public List<SocialDomainModel> SocialDomainList { get; set; }
    }
}
