using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishCatalogModel : BaseModel
    {
        public int PublishCatalogId { get; set; }

        public string CatalogName { get; set; }

        public List<PublishCategoryModel> PublishCategories { get; set; }
        public List<PublishProductModel> PublishProducts { get; set; }
        public int PromotionId { get; set; }
    }
}
