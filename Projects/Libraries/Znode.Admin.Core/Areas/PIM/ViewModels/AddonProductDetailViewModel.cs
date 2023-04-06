namespace Znode.Engine.Admin.ViewModels
{
    public class AddonProductDetailViewModel : BaseViewModel
    {
        public int PimAddOnProductDetailId { get; set; }
        public int PimAddOnProductId { get; set; }
        public int PimChildProductId { get; set; }
        public int DisplayOrder { get; set; }
        public bool? IsDefault { get; set; }

        public int? AddOnDisplayOrder { get; set; }
    }
}