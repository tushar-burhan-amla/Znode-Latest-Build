namespace Znode.Engine.Api.Models
{
    public class CustomFieldLocaleModel : BaseModel
    {
        public int CustomFieldLocaleId { get; set; }
        public int? CustomFieldId { get; set; }
        public int? LocaleId { get; set; }
        public string CustomKey { get; set; }
        public string CustomKeyValue { get; set; }
        public string LocaleName { get; set; }
        public bool IsDefault { get; set; }
    }
}
