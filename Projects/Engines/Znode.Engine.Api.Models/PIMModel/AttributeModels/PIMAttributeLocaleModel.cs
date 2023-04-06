using System;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeLocaleModel : BaseModel
    {
        public int PimAttributeLocaleId { get; set; }
        public Nullable<int> LocaleId { get; set; }
        public Nullable<int> PimAttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Description { get; set; }
    }
}
