using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeDataModel : BaseModel
    {
        public PIMAttributeModel AttributeModel { get; set; }
        public PIMFrontPropertiesModel FrontProperties { get; set; }
        public List<PIMAttributeValidationModel> ValidationRule { get; set; }
    }
}
