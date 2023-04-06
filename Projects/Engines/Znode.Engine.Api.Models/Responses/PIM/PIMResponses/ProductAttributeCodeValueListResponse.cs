using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductAttributeCodeValueListResponse : BaseListResponse
    {
        public List<ProductAttributeCodeValueModel> AttributeCodeValueList { get; set; }

        public bool Status { get; set; }
    }
}
