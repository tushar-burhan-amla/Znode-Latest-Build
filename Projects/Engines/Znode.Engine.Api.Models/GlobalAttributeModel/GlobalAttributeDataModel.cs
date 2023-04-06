using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeDataModel : BaseModel
    {
        public GlobalAttributeModel AttributeModel { get; set; }
        public List<GlobalAttributeValidationModel> ValidationRule { get; set; }
    }
}
