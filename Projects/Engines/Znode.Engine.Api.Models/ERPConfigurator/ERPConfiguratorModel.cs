using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ERPConfiguratorModel : BaseModel
    {
        public int ERPConfiguratorId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredClassName)]
        public string ClassName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredName)]
        public string ERPName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool Status { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredEmailID)]
        public string Email { get; set; }

        public string JsonSetting { get; set; }
    }
}
