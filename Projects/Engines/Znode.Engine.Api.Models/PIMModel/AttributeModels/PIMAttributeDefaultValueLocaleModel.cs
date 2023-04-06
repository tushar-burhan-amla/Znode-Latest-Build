using System;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeDefaultValueLocaleModel : BaseModel
    {
        public int PimAttributeDefaultValueLocaleId { get; set; }
        public Nullable<int> LocaleId { get; set; }
        public Nullable<int> PimDefaultAttributeValueId { get; set; }
        public string DefaultAttributeValue { get; set; }
        public string Description { get; set; }
    }
}
