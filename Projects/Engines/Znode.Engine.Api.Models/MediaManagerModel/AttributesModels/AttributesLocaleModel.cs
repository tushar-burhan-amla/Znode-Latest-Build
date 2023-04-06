using System;

namespace Znode.Engine.Api.Models
{
    public class AttributesLocaleModel : BaseModel
    {
        public int MediaAttributeLocaleId { get; set; }
        public Nullable<int> LocaleId { get; set; }
        public Nullable<int> MediaAttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Description { get; set; }
    }
}
