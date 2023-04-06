namespace Znode.Engine.Admin.ViewModels
{
    public class ProfileCatalogCategoryViewModel : BaseViewModel
    {
        public string ProfileName { get; set; }
        public int ProfileCatalogCategoryId { get; set; }
        public int ProfileCatalogId { get; set; }
        public int PimCategoryId { get; set; }
        public int PimProductId { get; set; }
    }
}