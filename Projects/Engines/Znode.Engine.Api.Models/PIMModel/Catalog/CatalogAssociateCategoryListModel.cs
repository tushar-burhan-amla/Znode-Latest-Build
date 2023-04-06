using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CatalogAssociateCategoryListModel : BaseListModel
    {
        public List<CatalogAssociateCategoryModel> catalogAssociatedCategoryList { get; set; }

        public Dictionary<string, object> AttributeColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }
    }
}