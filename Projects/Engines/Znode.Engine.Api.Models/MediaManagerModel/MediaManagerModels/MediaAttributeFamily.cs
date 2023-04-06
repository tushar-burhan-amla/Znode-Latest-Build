namespace Znode.Engine.Api.Models
{
    public class MediaAttributeFamily : BaseModel
    {
        public int FamilyLocaleId { get; set; }
        public int? LocaleId { get; set; }
        public int? AttributeFamilyId { get; set; }
        public string AttributeFamilyName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string MaxFileSize { get; set; }
    }
}
