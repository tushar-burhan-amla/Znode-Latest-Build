using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProfileCatalogListResponse : BaseListResponse
    {
        public List<ProfileCatalogModel> ProfileCatalogs { get; set; }
    }
}
