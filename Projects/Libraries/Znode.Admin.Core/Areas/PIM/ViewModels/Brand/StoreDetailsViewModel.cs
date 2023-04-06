namespace Znode.Engine.Admin.ViewModels
{
    public class StoreDetailsViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public int? PublishCatalogId { get; set; }
        public int? LocaleId { get; set; }
        public int? CMSThemeId { get; set; }
        public string StoreName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
    }
}