using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProfileCatalogListModel : BaseListModel
    {
        public ProfileCatalogListModel()
        {
            ProfileCatalogs = new List<ProfileCatalogModel>();
        }
        public List<ProfileCatalogModel> ProfileCatalogs { get; set; }
    }
}
