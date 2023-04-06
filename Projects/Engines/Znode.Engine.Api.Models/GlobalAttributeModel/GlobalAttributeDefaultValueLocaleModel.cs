using System;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeDefaultValueLocaleModel : BaseModel
    {
        public int GlobalAttributeDefaultValueLocaleId { get; set; }
        public Nullable<int> LocaleId { get; set; }
        public Nullable<int> GlobalDefaultAttributeValueId { get; set; }
        public string DefaultAttributeValue { get; set; }
        public string Description { get; set; }
    }
}
