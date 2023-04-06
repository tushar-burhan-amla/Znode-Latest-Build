using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CategoryListResponse : BaseListResponse
    {
        public List<CategoryModel> Categories { get; set; }
        public CategoryListModel CategoriesList { get; set; }
        public List<CMSWidgetProductCategoryModel> CMSWidgetProductCategories { get; set; }
        public List<LocaleModel> Locale { get; set; }
    }
}
