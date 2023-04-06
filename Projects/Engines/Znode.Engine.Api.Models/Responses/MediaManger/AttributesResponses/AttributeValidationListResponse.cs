using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributeValidationListResponse : BaseListResponse
    {
        public List<AttributeValidationDataModel> AttributeValidations { get; set; }
    }
}
