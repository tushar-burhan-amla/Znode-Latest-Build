using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class DomainModel : BaseModel
    {
        public string ApiKey { get; set; }
        public int DomainId { get; set; }

        [MaxLength(100)]
        [DisplayName(ZnodeApi_Resources.LabelURLName)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorRequired)]
        [RegularExpression("^((https|http)\\:\\/\\/[a-zA-Z0-9_\\-/]+(?:\\.[a-zA-Z0-9_\\-/]+)*)$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ValidationUrl)]
        public string DomainName { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public bool IsDefault { get; set; }
        public int PortalId { get; set; }
        public string ApplicationType { get; set; }
        public string DomainIds { get; set; }
        public string StoreName { get; set; }
        public string CloudflareZoneId { get; set; }
    }
}
