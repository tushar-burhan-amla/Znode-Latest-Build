using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CustomFieldModel : BaseModel
    {
        public int? ProductId { get; set; }
        public string CustomCode { get; set; }
        public int CustomFieldId { get; set; }
        public string CustomKey { get; set; }
        public string CustomValue { get; set; }
        public List<CustomFieldLocaleModel> CustomFieldLocales { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
