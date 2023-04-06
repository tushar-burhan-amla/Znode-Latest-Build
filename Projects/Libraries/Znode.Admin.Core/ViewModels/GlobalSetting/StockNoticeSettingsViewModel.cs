using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class StockNoticeSettingsViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelDeleteAlreadySentEmails, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorStockNotice)]
        [RegularExpression("^(?:[1-9][0-9]{3}|[1-9][0-9]{2}|[1-9][0-9]|[1-9])$",ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorStockNotice, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string DeleteAlreadySentEmails { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDeletePendingEmails, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorStockNotice)]
        [RegularExpression("^(?:[1-9][0-9]{3}|[1-9][0-9]{2}|[1-9][0-9]|[1-9])$", ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorStockNotice, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string DeletePendingEmails { get; set; }
    }
}
