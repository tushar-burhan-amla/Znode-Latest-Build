using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSPortalContentPageIndexViewModel : BaseViewModel
    {       
        public int CMSSearchIndexId { get; set; }

        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelIndexName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(200, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string IndexName { get; set; }

        public int CMSSearchIndexMonitorId { get; set; } 

        public CMSSearchIndexMonitorListViewModel CMSSearchIndexMonitorList { get; set; }

        public List<SelectListItem> StoreList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSelectStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        public string StoreName { get; set; }

        public string RevisionType { get; set; }

        public bool IsDisabledCMSPageResults { get; set; }

    }
}
