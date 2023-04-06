using Znode.Engine.WebStore;

namespace Znode.WebStore.Core.ViewModels
{
    public class SiteMapProductViewModel : BaseViewModel
    {
        public string SEOUrl { get; set; }
        public int ZnodeProductId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
    }
}
