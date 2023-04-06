using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CatalogAssociateCategoryListViewModel : BaseViewModel
    {
        public List<CatalogAssociateCategoryViewModel> CatalogAssociateCategories { get; set; }
        public int PimCatalogId { get; set; }
        public int PimCategoryId { get; set; }
        public int PimCategoryHierarchyId { get; set; }
        public int? ProfileCatalogId { get; set; }
        public int? ProfileId { get; set; }
        public GridModel GridModel { get; set; }

        public Dictionary<string, object> AttributeColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }
    }
}