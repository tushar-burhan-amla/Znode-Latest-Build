using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchProfileListModel : BaseListModel
    {
        public SearchProfileListModel()
        {
            SearchProfileList = new List<SearchProfileModel>();
        }
        public List<SearchProfileModel> SearchProfileList { get; set; }
        public int PortalId { get; set; }
        public int PublishCatalogId { get; set; }
        public string CatalogName { get; set; }
    }
}
