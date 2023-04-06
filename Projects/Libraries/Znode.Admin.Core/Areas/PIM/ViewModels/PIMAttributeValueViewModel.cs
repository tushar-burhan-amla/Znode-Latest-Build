namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeValueViewModel : BaseViewModel
    {
        public int PimAttributeValueId { get; set; }
        public int? PimAttributeFamilyId { get; set; }
        public int? PimProductId { get; set; }
        public int? PimAttributeId { get; set; }
        public int? PimAttributeDefaultValueId { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeCode { get; set; }
    }
}