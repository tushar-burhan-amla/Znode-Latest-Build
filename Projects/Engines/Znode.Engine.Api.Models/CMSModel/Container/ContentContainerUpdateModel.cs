using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ContentContainerUpdateModel : BaseModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.ErrorContentContainerKey, ErrorMessageResourceType = typeof(ZnodeApi_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.ContainerKeyError, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.ContainerKeyLimit, ErrorMessageResourceType = typeof(Api_Resources))]
        public string ContainerKey { get; set; }

        [MaxLength(1000)]
        public string Tags { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredContentContainerName)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.ContainerNameLimit, ErrorMessageResourceType = typeof(Api_Resources))]
        public string Name { get; set; }
    }
}
