using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class UrlRedirectModel : BaseModel
    {
        public int CMSUrlRedirectId { get; set; }
        public int PortalId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredRedirectFrom)]
        public string RedirectFrom { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredRedirectTo)]
        public string RedirectTo { get; set; }
        public bool IsActive { get; set; }
    }
}
