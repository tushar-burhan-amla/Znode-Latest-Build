using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchAttributesListViewModel : BaseViewModel
    {
        public List<SearchAttributesViewModel> SearchAttributeList { get; set; }
        public List<SelectListItem> StoreList { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TextStoreName, ResourceType = typeof(Admin_Resources))]
        public int PublishCatalogId { get; set; }
        public int SearchProfileId { get; set; }
        public string CatalogName { get; set; }
        public string SearchProfileName { get; set; }
        public GridModel GridModel { get; set; }
        public string AssociatedAttributes { get; set; }
    }
}
