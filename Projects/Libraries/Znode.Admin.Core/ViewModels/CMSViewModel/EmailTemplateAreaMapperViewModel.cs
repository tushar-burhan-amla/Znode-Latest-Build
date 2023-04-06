using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class EmailTemplateAreaMapperViewModel : BaseViewModel
    {
        public int EmailTemplateMapperId { get; set; }
        public int EmailTemplateId { get; set; }
        public int EmailTemplateAreasId { get; set; }
        public int? PortalId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSMSNotificationActive { get; set; }
        public bool IsEnableBcc { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredTemplateName)]
        public string EmailTemplateName { get; set; }
        public string EmailTemplateAreaName { get; set; }
        public bool IsAddMode { get; set; }
        public List<SelectListItem> EmailTemplateAreaList { get; set; }
    }
}