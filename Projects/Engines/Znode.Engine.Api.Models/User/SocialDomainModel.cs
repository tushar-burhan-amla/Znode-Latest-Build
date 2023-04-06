using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SocialDomainModel
    {
        public int PortalId { get; set; }
        public int? DomainId { get; set; }
     
        public string DomainName { get; set; }

        public List<SocialTypeModel> SocialTypeList { get; set; }
    }
}
