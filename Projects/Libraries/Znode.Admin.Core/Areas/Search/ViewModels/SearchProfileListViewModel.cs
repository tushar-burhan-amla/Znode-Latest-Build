using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchProfileListViewModel : BaseViewModel
    {
        public SearchProfileListViewModel()
        {
            SearchProfileList = new List<SearchProfileViewModel>();
        }
        public List<SearchProfileViewModel> SearchProfileList { get; set; }
        public int PortalId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCatalogName, ResourceType = typeof(Admin_Resources))]
        public int PublishCatalogId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCatalogRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelCatalogName, ResourceType = typeof(Admin_Resources))]
        public string CatalogName { get; set; }
        public GridModel GridModel { get; set; }
    }
}
