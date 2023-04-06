using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributesDefaultValueModel : BaseModel
    {
        public int DefaultAttributeValueId { get; set; }
        public Nullable<int> AttributeId { get; set; }
        public Nullable<bool> IsEditable { get; set; }
        public string AttributeDefaultValueCode { get; set; }
        public List<DefaultAttributeValueLocaleModel> ValueLocales { get; set; }

    }
}
