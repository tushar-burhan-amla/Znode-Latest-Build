using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductAttributeCodeValueListModel : BaseListModel
    {
        public List<ProductAttributeCodeValueModel> AttributeCodeValueList { get; set; }

        public bool Status { get; set; }
    }
}
