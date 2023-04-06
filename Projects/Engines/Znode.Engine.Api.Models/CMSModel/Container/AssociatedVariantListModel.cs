using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AssociatedVariantListModel : BaseListModel
    {
        public List<AssociatedVariantModel> AssociatedVariants { get; set; }
    }
}
