namespace Znode.Engine.Admin.ViewModels
{
    public class ParentProductAssociationModel
    {
        public int ParentId { get; set; }
        public int PimProductId { get; set; }
        public string AssociatedIds { get; set; }
        public int AttributeId { get; set; }
        public int DisplayOrder { get; set; } = 999;
        public bool? IsDefault { get; set; } = false;
    }
}