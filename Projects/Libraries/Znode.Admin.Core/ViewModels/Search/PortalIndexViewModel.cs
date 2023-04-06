using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalIndexViewModel : BaseViewModel
    {
        public PortalIndexViewModel()
        {
            SearchIndexMonitorList = new SearchIndexMonitorListViewModel();
        }
        public int CatalogIndexId { get; set; }

        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelIndexName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(200, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string IndexName { get; set; }

        public int SearchCreateIndexMonitorId { get; set; }

        public SearchIndexMonitorListViewModel SearchIndexMonitorList { get; set; }

        public List<SelectListItem> StoreList { get; set; }

        public ERPTaskSchedulerViewModel SchedulerData { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSelectStore, ResourceType = typeof(Admin_Resources))]
        public int PublishCatalogId { get; set; }

        public string CatalogName { get; set; }

        public string RevisionType { get; set; }

        public bool DirectCalling { get; set; }
    }
}