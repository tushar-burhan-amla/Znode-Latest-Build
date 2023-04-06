using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalCatalogViewModel : BaseViewModel
    {
        public int PortalCatalogId { get; set; }
        public int PortalId { get; set; }
        public int PublishCatalogId { get; set; }
        public int? ThemeId { get; set; }
        public int? CssId { get; set; }

        public string CatalogName { get; set; }
        public List<PublishCatalogViewModel> PublishCatalogs { get; set; }

        public string PortalName { get; set; }
        public int PromotionId { get; set; }
    }
}