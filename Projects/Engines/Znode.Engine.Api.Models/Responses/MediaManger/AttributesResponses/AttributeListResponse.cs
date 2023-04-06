using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributeListResponse : BaseListResponse
    {
        public List<AttributesDataModel> Attributes { get; set; }

        public List<AttributeTypeDataModel> AttributeTypes { get; set; }

        public List<AttributeInputValidationDataModel> InputValidations { get; set; }

        public List<AttributesDefaultValueModel> DefaultValues { get; set; }

    }
}
