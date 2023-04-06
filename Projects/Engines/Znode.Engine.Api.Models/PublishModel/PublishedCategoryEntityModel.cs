using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishedCategoryEntityModel : BaseModel
    { 
        public int VersionId { get; set; }
        public int ZnodeCategoryId { get; set; }
        public string Name { get; set; }
        public int ZnodeCatalogId { get; set; }
        public int[] ZnodeParentCategoryIds { get; set; }
        public int[] ProductIds { get; set; }
        public int LocaleId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<PublishedAttributeEntityModel> Attributes { get; set; }
        public string CatalogName { get; set; }
        public string CategoryCode { get; set; }
        public int CategoryIndex { get; set; }

        public PublishedCategoryEntityModel()
        {
            Attributes = new List<PublishedAttributeEntityModel>();
        }
    }
}
