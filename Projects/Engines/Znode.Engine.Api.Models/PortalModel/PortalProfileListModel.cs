using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalProfileListModel : BaseListModel
    {
     
        public ProfileModel Profile { get; set; }
        public List<PortalProfileModel> PortalProfiles { get; set; }

        public PortalProfileListModel()
        {
            PortalProfiles = new List<PortalProfileModel>();
        }
    }
}
