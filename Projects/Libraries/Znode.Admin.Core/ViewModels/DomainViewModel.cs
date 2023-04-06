using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class DomainViewModelss : BaseViewModel
    {
        public int DomainId { get; set; }
        public int PortalId { get; set; }

        [MaxLength(100)]
        [DisplayName(ZnodeApi_Resources.LabelURLName)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRequired)]
        [RegularExpression("^(http\\:\\/\\/[a-zA-Z0-9_\\-/]+(?:\\.[a-zA-Z0-9_\\-/]+)*)$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        public string DomainName { get; set; }
        public string ApiKey { get; set; }
        public bool IsActive { get; set; }
        public string StoreName { get; set; }
    }
}