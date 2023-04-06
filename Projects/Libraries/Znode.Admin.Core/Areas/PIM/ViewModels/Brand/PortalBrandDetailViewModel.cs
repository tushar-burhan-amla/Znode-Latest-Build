namespace Znode.Engine.Admin.ViewModels
{
   public class PortalBrandDetailViewModel : BaseViewModel
    {
        public int PortalBrandId { get; set; }
        public int PortalId { get; set; }
        public int BrandId { get; set; }       
        public int? DisplayOrder { get; set; }
    }
}
