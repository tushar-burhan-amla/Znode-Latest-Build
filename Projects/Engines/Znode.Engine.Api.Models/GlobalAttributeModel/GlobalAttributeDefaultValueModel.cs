using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeDefaultValueModel : BaseModel
    {
        public int GlobalAttributeDefaultValueId { get; set; }
        public Nullable<int> GlobalAttributeId { get; set; }
        public Nullable<bool> IsEditable { get; set; }
        public List<GlobalAttributeDefaultValueLocaleModel> ValueLocales { get; set; }
        public string AttributeDefaultValueCode { get; set; }
        public int? DisplayOrder { get; set; }
        public GlobalAttributeDefaultValueModel()
        {
            ValueLocales = new List<GlobalAttributeDefaultValueLocaleModel>();
        }
        public Nullable<bool> IsDefault { get; set; }
        public string SwatchText { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }

        public Nullable<bool> IsSwatch { get; set; }
    }
}
