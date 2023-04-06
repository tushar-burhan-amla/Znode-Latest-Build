using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchGlobalProductCategoryBoostListViewModel : BaseViewModel
    {
        public List<SearchGlobalProductCategoryBoostViewModel> SearchGlobalProductCategoryList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSelectCatalog, ResourceType = typeof(Admin_Resources))]       
        public int CatalogId { get; set; }
        public string CatalogName { get; set; }

        public SearchGlobalProductCategoryBoostListViewModel()
        {
            SearchGlobalProductCategoryList = new List<SearchGlobalProductCategoryBoostViewModel>();
        }
        public GridModel Grid { get; set; }
    }
}