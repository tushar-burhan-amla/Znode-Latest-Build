using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class SupplierTypeModel : BaseModel
    {

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredClassName)]
        public string ClassName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredName)]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredDescription)]
        public string Description { get; set; }
        public bool IsActive { get; set; }        
        public int SupplierTypeId { get; set; }
    }
}
