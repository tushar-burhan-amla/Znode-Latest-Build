using System.Collections.Generic;
namespace Znode.Engine.Api.Models
{
    public class ContentContainerDataModel : BaseModel
    {
        public string ContainerKey { get; set; }
        public int LocaleId { get; set; }
        public int ProfileId { get; set; }
        public int PortalId { get; set; }
        public int CMSContentContainerId { get; set; }   //Property is renamed from the ContentWidgetId to ContentContainerId
        public string Tags { get; set; }
        public int CMSContainerTemplateId { get; set; }
        public string ContentContainerName { get; set; }
        public int FamilyId { get; set; }
        public int CMSContainerProfileVariantId { get; set; }
        public string ContainerTemplateName { get; set; }

        public List<GlobalAttributeGroupModel> Groups { get; set; }
        public List<GlobalAttributeValuesModel> Attributes { get; set; }
    }
}
