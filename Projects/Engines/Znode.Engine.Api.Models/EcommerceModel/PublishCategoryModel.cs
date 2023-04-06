using System.Collections.Generic;
using System;

namespace Znode.Engine.Api.Models
{
    public class PublishCategoryModel : BaseModel
    {
        public int PublishCategoryId { get; set; }
        public int[] ZnodeParentCategoryIds { get; set; }
        public int ZnodeCatalogId { get; set; }
        public string PublishedCatalogId { get; set; }
        public int[] ProductIds { get; set; }
        public string Version { get; set; }
        public int LocaleId { get; set; }
        public string Name { get; set; }
        public SEODetailsModel SEODetails { get; set; }

        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }

        public List<PublishAttributeModel> Attributes { get; set; }
        public List<PublishProductModel> products { get; set; }
        public int PromotionId { get; set; }
        public List<PublishCategoryModel> ParentCategory { get; set; }
        public bool ? IsActive { get; set; }
        public string CatalogName { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }
        public string PublishStatus { get; set; }
     
        public DateTime? ActivationDate { get; set; }
        
        public DateTime? ExpirationDate { get; set; }

        public string CategoryName { get; set; }
        public string SEOCode { get; set; }
        public string IsPublish { get; set; }
        public string CategoryCode { get; set; }
    }
}
