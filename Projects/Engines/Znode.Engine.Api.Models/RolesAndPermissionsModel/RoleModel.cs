using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    //Role Model
    public class RoleModel : BaseModel
    {
        public string RoleId { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLengthRoleName)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredMessageRoleName)]
        public string Name { get; set; }

        [Display(Name = ZnodeApi_Resources.LabelIsActive, ResourceType = typeof(Api_Resources))]
        public bool? IsActive { get; set; }

        public bool IsSystemDefined { get; set; }

        public string TypeOfRole { get; set; }

        public List<DataObjectModel> RoleRights { get; set; }

        public bool IsAssociated { get; set; }
    }
}
