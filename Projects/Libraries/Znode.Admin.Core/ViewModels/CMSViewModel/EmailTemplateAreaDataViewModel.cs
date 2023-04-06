using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class EmailTemplateAreaDataViewModel : BaseViewModel
    {
        public List<EmailTemplateAreaMapperViewModel> MappedEmailTemplateList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        public List<SelectListItem> Portals { get; set; }
    }
}