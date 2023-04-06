using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PIMAttributeListResponse : BaseListResponse
    {
        public List<PIMAttributeModel> Attributes { get; set; }
        public List<PIMAttributeTypeModel> AttributeTypes { get; set; }

        public List<PIMAttributeInputValidationModel> InputValidations { get; set; }

        
    }
}
