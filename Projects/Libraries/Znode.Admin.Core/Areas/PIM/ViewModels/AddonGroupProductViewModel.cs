namespace Znode.Engine.Admin.ViewModels
{
    public class AddonGroupProductViewModel : BaseViewModel
    {
        public int PimAddonGroupProductId { get; set; }
        public int PimAddonGroupId { get; set; }
        public int PimChildProductId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
