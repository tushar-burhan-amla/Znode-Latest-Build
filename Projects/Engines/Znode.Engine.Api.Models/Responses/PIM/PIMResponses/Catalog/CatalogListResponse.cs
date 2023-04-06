using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CatalogListResponse : BaseListResponse
    {
        public List<CatalogModel> Catalogs { get; set; }

        public List<CatalogAssociateCategoryModel> AssociateCategories { get; set; }

        public Dictionary<string, object> AttributeColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }
    }
}