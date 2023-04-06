using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class EntityAttributeViewModel : BaseViewModel
    {
        public int EntityValueId { get; set; }
        public string EntityType { get; set; }
        public bool IsSuccess { get; set; }
        public List<EntityAttributeDetailsViewModel> Attributes { get; set; }
 
        public string FamilyCode { get; set; }
        public EntityAttributeViewModel()
        {
            Attributes = new List<EntityAttributeDetailsViewModel>();
        }
    }
}
