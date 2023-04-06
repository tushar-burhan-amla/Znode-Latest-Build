using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ContentContainerResponseModel : BaseModel
    {
        public int ContentContainerId { get; set; }
        public int? PortalId { get; set; }
        public int? ProfileId { get; set; }
        public int? ContainerTemplateId { get; set; }
        public string StoreCode { get; set; }
        public string ContainerKey { get; set; }
        public string IsGlobalContentWidget { get; set; }
        public string Tags { get; set; }
        public string FamilyCode { get; set; }
        public string FamilyName { get; set; }
        public string StoreName { get; set; }
        public string ProfileName { get; set; }
        public int FamilyId { get; set; }
        public int GlobalEntityId { get; set; }
        public string Name { get; set; }
        public string TemplateName { get; set; }
        public int ContainerProfileVariantId { get; set; }
        public int? LocaleId { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public List<AssociatedVariantModel> ContainerVariants { get; set; }
        public string ContainerName { get; set; }
        public string PublishStatus { get; set; }
        //Added PublishStateId property to maintain the publishstate of container
        public int PublishStateId { get; set; }
        public bool IsActive { get; set; } // This property is added in oder to contain the IsActive status of container variant. 
        public string TargetPublishState { get; set; } //This property is added in order to contain the target publish state of the container
    }
}
