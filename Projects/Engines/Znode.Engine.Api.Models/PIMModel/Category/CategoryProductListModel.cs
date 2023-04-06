using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CategoryProductListModel : BaseListModel
    {
        public CategoryProductListModel()
        {
            CategoryProducts = new List<CategoryProductModel>();
        }
        public List<CategoryProductModel> CategoryProducts { get; set; }
    }
}
