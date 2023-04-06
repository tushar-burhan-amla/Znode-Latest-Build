using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalCatalogListViewModel : BaseViewModel
    {
        public PortalCatalogListViewModel()
        {
            PortalCatalogs = new List<PortalCatalogViewModel>();
        }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public List<PortalCatalogViewModel> PortalCatalogs { get; set; }
        public GridModel GridModel { get; set; }
        public string CatelogIds { get; set; }
        public int PromotionId { get; set; }
    }
}