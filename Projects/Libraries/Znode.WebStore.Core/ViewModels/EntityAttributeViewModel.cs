using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class EntityAttributeViewModel : BaseViewModel
    {
        public int EntityValueId { get; set; }
        public string EntityType { get; set; }
        public bool IsSuccess { get; set; }
        public int LocaleId { get; set; }
        public int PortalId { get; set; }

        public List<EntityAttributeDetailsViewModel> Attributes { get; set; }

        public EntityAttributeViewModel()
        {
            Attributes = new List<EntityAttributeDetailsViewModel>();
        }
    }
}
