using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class AssociatedVariantModel : BaseModel
    {
        public int? ProfileId { get; set; }
        public int? PortalId { get; set; }
        public int? LocaleId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorContentContainerId)]
        public int ContentContainerId { get; set; }
        public string LocaleName { get; set; }
        public string ProfileName { get; set; }
        public string ProfileCode { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public string ContainerKey { get; set; }
        public int ContainerProfileVariantId { get; set; }
        public int? ContainerTemplateId { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public bool IsDefaultVariant { get; set; }
        public string PublishStatus { get; set; }
        public int PublishStateId { get; set; } // added the PublishStateId property to track the publishstatus of container variant.
        public bool IsActive { get; set; } // This property is added in oder to contain the IsActive status of container variant. 
    }
}
