using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishCategoryProductDetailModel : BaseModel
    {
        //Promotion Related Details
        public int? ProductPromotionId { get; set; }
        public int? PublishProductId { get; set; }
        public int? PromotionId { get; set; }
        public int? ReferralPublishProductId { get; set; }
        public int? PromotionProductQuantity { get; set; }

        public string PromotionType { get; set; }
        public string PromotionMessage { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ActivationDate { get; set; }

        public List<int> ProductIds { get; set; }

        //Inventory.
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal? AllLocationQuantity { get; set; }
        public string WarehouseName { get; set; }
        public bool IsDefaultWarehouse { get; set; }

        //Pricing.
        public string SKU { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? TierPrice { get; set; }
        public decimal? TierQuantity { get; set; }
        public decimal? RetailPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public string CultureCode { get; set; }
        //SEO and Product Reviews.
        public decimal? Rating { get; set; }
        public int? TotalReviews { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }
        public string ImageSmallPath { get; set; }

        public List<ProductPromotionModel> Promotions { get; set; }
        public List<WebStoreGroupProductModel> AssociatedGroupProducts { get; set; }
        public List<AssociatedProductsModel> AssociatedProducts { get; set; }
        public List<WebStoreAddOnModel> AddOns { get; set; }
    }
}
