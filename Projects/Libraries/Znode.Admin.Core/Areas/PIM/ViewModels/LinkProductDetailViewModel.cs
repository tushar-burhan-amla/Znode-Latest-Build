namespace Znode.Engine.Admin.ViewModels
{
    public class LinkProductDetailViewModel : BaseViewModel
    {
        public int PimLinkProductDetailId { get; set; }
        public int? PimParentProductId { get; set; }
        public int? PimProductId { get; set; }
        public int PimAttributeId { get; set; }
        public int? DisplayOrder { get; set; }=1;
        public virtual ProductViewModel ZnodePimProduct { get; set; }
    }
}