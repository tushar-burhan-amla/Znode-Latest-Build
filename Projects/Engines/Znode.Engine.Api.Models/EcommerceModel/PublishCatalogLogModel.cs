using System;

namespace Znode.Engine.Api.Models
{
    public class PublishCatalogLogModel : BaseModel
    {
        public int PublishCatalogLogId { get; set; }
        public int? PublishCatalogId { get; set; }
        public bool? IsCatalogPublished { get; set; }
        public string PublishStatus { get; set; }
        public string UserName { get; set; }
        public int PublishCategoryCount { get; set; }
        public int PublishProductCount { get; set; }
        public string LastPublishedDate { get; set; }
        public int PimCatalogId { get; set; }
        public string PublishCategoryId { get; set; }
        public bool? IsCategoryPublished { get; set; }
        public string PublishProductId { get; set; }
        public bool? IsProductPublished { get; set; }
        public int? UserId { get; set; }
        public DateTime? LogDateTime { get; set; }
        public string Token { get; set; }
        public int? LocaleId { get; set; }
        public byte? PublishStateId { get; set; }
    }
}
