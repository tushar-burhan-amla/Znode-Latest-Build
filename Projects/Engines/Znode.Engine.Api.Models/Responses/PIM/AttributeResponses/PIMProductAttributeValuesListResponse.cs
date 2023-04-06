using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PIMProductAttributeValuesListResponse : BaseListResponse
    {
        public List<PIMProductAttributeValuesModel> AttributeValues { get; set; }
    }
}
