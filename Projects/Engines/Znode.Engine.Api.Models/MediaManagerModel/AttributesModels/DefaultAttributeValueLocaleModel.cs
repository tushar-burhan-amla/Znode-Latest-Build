

namespace Znode.Engine.Api.Models
{
    public class DefaultAttributeValueLocaleModel : BaseModel
    {
        public int MediaAttributeDefaultValueLocaleId { get; set; }
        public int? MediaAttributeDefaultValueId { get; set; }
        public string DefaultAttributeValue { get; set; }
        public int? LocaleId { get; set; }
        public string Description { get; set; }
    }
}
