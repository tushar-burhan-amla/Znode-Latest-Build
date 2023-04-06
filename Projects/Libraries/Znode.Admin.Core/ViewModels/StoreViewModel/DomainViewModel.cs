using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class DomainViewModel : BaseViewModel
    {
        public int DomainId { get; set; }
        public int PortalId { get; set; }

        [MaxLength(100)]
        [Display(Name = ZnodeAdmin_Resources.LabelURLName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/|www\.)+[a-z0-9][\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        public string DomainName { get; set; }
        public string ApiKey { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public bool Status { get; set; }
        public string ApplicationType { get; set; }
        public List<SelectListItem> ApplicationTypeList { get; set; }
        public List<SelectListItem> IsDefaultList { get; set; }
        public string PortalName { get; set; }
        public string StoreName { get; set; }
    }
}