using System.Collections.Generic;
using Znode.Engine.WebStore;

namespace Znode.WebStore.Core.ViewModels
{
    public class SiteMapCategoryListViewModel : BaseViewModel
    {
        public List<SiteMapCategoryViewModel> CategoryList { set; get; }
        public List<SiteMapBrandViewModel> BrandList { get; set; }
    }
}
