using System;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeLocaleModel : BaseModel
    {
        public int GlobalAttributeLocaleId { get; set; }
        public Nullable<int> LocaleId { get; set; }
        public Nullable<int> GlobalAttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Description { get; set; }
    }
}
