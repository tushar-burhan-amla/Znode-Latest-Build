using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchGlobalProductBoostListViewModel : BaseViewModel
    {
        public List<SearchGlobalProductBoostViewModel> SearchGlobalProductBoostList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSelectCatalog, ResourceType = typeof(Admin_Resources))]        
        public int CatalogId { get; set; }

        public string CatalogName { get; set; }

        public SearchGlobalProductBoostListViewModel()
        {
            SearchGlobalProductBoostList = new List<SearchGlobalProductBoostViewModel>();
        }
        public GridModel Grid { get; set; }
    }
}