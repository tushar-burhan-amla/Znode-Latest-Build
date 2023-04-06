namespace Znode.Engine.Admin.ViewModels
{
    public class AssociatedProductViewModel
    {
        public int ParentProductId { get; set; }
        public string AssociatedProductIds { get; set; }
        public int AttributeId { get; set; }
        public string AttributeCode { get; set; }
        public int? DisplayOrder { get; set; } = 99;
        public bool IsDefault { get; set; }
    }
}