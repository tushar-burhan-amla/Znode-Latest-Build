using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributeCodeValueModel
    {
        public string SKU { get; set; }
        public string LocaleCode { get; set; }
        public List<PIMAttributeCodeValueModel> PIMAttributeCodeValueList { get; set; }

        public bool IsPublish { get; set; } = false;
    }
}