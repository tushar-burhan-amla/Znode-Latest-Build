using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CategoryValuesListModel : BaseModel
    {
        public int PimCategoryId { get; set; }

        public int LocaleId { get; set; }
        public int? PimCatalogId { get; set; }
        public int? PimParentCategoryId { get; set; }
        public List<PIMCategoryValuesListModel> AttributeValues { get; set; }

        public CategoryValuesListModel()
        {
            AttributeValues = new List<PIMCategoryValuesListModel>();
        }
    }
}