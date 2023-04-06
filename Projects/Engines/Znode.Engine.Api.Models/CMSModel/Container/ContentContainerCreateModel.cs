using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ContentContainerCreateModel : BaseModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.ErrorContentContainerKey, ErrorMessageResourceType = typeof(ZnodeApi_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.ContainerKeyError, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.ContainerKeyLimit, ErrorMessageResourceType = typeof(Api_Resources))]
        public string ContainerKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorIsGlobalContentWidget)]
        public bool IsGlobalContentWidget { get; set; }

        [MaxLength(1000)]
        public string Tags { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredFamilyCode)]
        public string FamilyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredContentContainerName)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.ContainerNameLimit, ErrorMessageResourceType = typeof(Api_Resources))]
        public string Name { get; set; }
        public string TemplateName { get; set; }

        public int? PortalId { get; set; }
        public int? ContainerTemplateId { get; set; }
        public int FamilyId { get; set; }
        public int GlobalEntityId { get; set; }
        public string StoreCode { get; set; }
        //Added PublishStateId property to maintain the publishstate of container
        public int PublishStateId { get; set; }
        public bool IsActive { get; set; }// This property is added in oder to contain the IsActive status of container variant. 
    }
}
