using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchBoostAndBuryRuleListViewModel : BaseViewModel
    {
        public List<SearchBoostAndBuryRuleViewModel> SearchBoostAndBuryRuleList { get; set; }
        public int PortalId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCatalogName, ResourceType = typeof(Admin_Resources))]
        public int PublishCatalogId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCatalogRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelCatalogName, ResourceType = typeof(Admin_Resources))]
        public string CatalogName { get; set; }
        public GridModel GridModel { get; set; }
    }
}
