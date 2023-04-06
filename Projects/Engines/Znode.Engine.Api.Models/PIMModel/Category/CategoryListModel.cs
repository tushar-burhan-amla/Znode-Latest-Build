using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CategoryListModel : BaseListModel
    {
        public List<CategoryModel> Categories { get; set; }
        public List<CMSWidgetProductCategoryModel> CMSWidgetProductCategories { get; set; }
        public List<LocaleModel> Locale { get; set; }
        public Dictionary<string, object> AttrubuteColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }

        public CategoryListModel()
        {
            Categories = new List<CategoryModel>();
        }

        public string ItemType { get; set; }
    }
}
