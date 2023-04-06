using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class RequestStatusViewModel : BaseViewModel
    {
        public int RmaRequestStatusId { get; set; }

        public int RmaReasonForReturnId { get; set; }

        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.NameRequired, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(250, ErrorMessageResourceName = ZnodeAdmin_Resources.NameMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.ReasonRequired, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(250, ErrorMessageResourceName = ZnodeAdmin_Resources.ReasonMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Reason { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCustomerMessage, ResourceType = typeof(Admin_Resources))]
        public string CustomerNotification { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAdminNotification, ResourceType = typeof(Admin_Resources))]
        public string AdminNotification { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }
        public string RequestCode { get; set; }

        public List<SelectListItem> IsActiveList { get; set; }
    }
}