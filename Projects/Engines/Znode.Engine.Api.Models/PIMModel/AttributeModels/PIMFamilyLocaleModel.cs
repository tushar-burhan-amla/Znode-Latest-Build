namespace Znode.Engine.Api.Models
{
    public class PIMFamilyLocaleModel : BaseModel
    {
        public int PimFamilyLocaleId { get; set; }
        public int? LocaleId { get; set; }
        public int? PimAttributeFamilyId { get; set; }
        public string AttributeFamilyName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}
