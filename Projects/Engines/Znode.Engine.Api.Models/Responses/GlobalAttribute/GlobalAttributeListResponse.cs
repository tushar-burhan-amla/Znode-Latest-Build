using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class GlobalAttributeListResponse : BaseListResponse
    {
        public List<GlobalAttributeModel> Attributes { get; set; }
        public List<GlobalAttributeTypeModel> AttributeTypes { get; set; }

        public List<GlobalAttributeInputValidationModel> InputValidations { get; set; }
    }
}
