using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ConfigurableAttributeModel : BaseModel
    {
        public string AttributeValue { get; set; }
        public string ImagePath { get; set; }
        public string SwatchText { get; set; }
        public bool IsDisabled { get; set; }
        public List<AttributesSelectValuesModel> SelectValues { get; set; }
        public int DisplayOrder { get; set; }
    }
}
