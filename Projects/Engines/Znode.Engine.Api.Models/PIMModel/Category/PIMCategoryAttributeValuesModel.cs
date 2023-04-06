namespace Znode.Engine.Api.Models
{
    public class PIMCategoryValuesListModel : BaseModel
    {
        public int PimCategoryId { get; set; }
        public int PimAttributeId { get; set; }
        public int? PimAttributeValueId { get; set; }
        public int? PimAttributeDefaultValueId { get; set; }
        public int PimAttributeFamilyId { get; set; }
        public int LocaleId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
    }
}