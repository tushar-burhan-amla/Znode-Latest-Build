using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ContentContainerDataViewModel : BaseViewModel
    {
        public string ContainerKey { get; set; }
        public int LocaleId { get; set; }
        public int ProfileId { get; set; }
        public int PortalId { get; set; }
        public int ContentContainerId { get; set; }
        public string Tags { get; set; }
        public int TemplateId { get; set; }
        public string ContainerName { get; set; }
        public int FamilyId { get; set; }
        public int ContainerVariantId { get; set; }

        public List<GlobalAttributeGroupViewModel> Groups { get; set; }
        public List<GlobalAttributeValuesViewModel> Attributes { get; set; }
    }
}
