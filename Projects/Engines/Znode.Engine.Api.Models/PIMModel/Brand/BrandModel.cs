namespace Znode.Engine.Api.Models
{
    public class BrandModel : BaseModel
    {
        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public int? MediaId { get; set; }
        public string WebsiteLink { get; set; }
        public string Description { get; set; }
        public string SEOTitle { get; set; }
        public string SEOKeywords { get; set; }
        public string SEODescription { get; set; }
        public string SEOFriendlyPageName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string MediaPath { get; set; }
        public int BrandDetailLocaleId { get; set; }
        public int? CMSSEODetailId { get; set; }
        public int? CMSSEODetailLocaleId { get; set; }
        public int LocaleId { get; set; }
        public int PromotionId { get; set; }
        public int? PortalId { get; set; }

        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public string OriginalImagepath { get; set; }
        public string ImageName { get; set; }

        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int CMSWidgetBrandId { get; set; }

    }
}
