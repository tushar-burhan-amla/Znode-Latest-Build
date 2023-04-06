using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeEntityDetailsModel : BaseModel
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public bool HasChildAccount { get; set; }
        public List<GlobalAttributeGroupModel> Groups { get; set; }
        public List<GlobalAttributeValuesModel> Attributes { get; set; }
        public string FamilyCode { get; set; }
    }
}
