namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeFamilyLocaleModel : BaseModel
    {
        public int GlobalAttributeFamilyLocaleId { get; set; }
        public int GlobalAttributeFamilyId { get; set; }
        public int? LocaleId { get; set; }
        public string AttributeFamilyName { get; set; }
        public string Description { get; set; }
    }
}
