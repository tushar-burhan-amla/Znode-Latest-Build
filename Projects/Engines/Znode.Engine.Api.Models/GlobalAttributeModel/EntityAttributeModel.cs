using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class EntityAttributeModel : BaseModel
    {
        public int EntityValueId { get; set; }
        public string EntityType { get; set; }
        public bool IsSuccess { get; set; }
        public List<EntityAttributeDetailsModel> Attributes { get; set; }
        public string FamilyCode { get; set; }
        public EntityAttributeModel()
        {
            Attributes = new List<EntityAttributeDetailsModel>();
        }
    }
}
