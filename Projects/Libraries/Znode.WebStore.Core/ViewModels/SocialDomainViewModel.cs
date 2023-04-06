using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class SocialDomainViewModel
    {
        public int PortalId { get; set; }
        public int? DomainId { get; set; }

        public string DomainName { get; set; }

        public List<SocialTypeViewModel> SocialTypeList { get; set; }
    }
}
