using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RecentViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public int PublishProductId { get; set; }
        public string ImageSmallPath { get; set; }
        public decimal? SalesPrice { get; set; }
        public string SEOUrl { get; set; }
        public string SKU { get; set; }
        public string ProductType { get; set; }
        public string CultureCode { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public string UOM { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal Rating { get; set; }
        public int TotalReviews { get; set; }
        public List<AttributesViewModel> Attributes { get; set; }
        public List<ProductPromotionViewModel> Promotions { get; set; }
        public string GroupProductPriceMessage { get; set; }
        public List<HighlightsViewModel> HighlightsList { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
    }
}
