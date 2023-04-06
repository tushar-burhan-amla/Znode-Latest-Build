using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishedConfigurableProductEntityModel : BaseModel
    {
        public int VersionId { get; set; }
        public int ZnodeProductId { get; set; }
        public int ZnodeCatalogId { get; set; }
        public int AssociatedZnodeProductId { get; set; }
        public int AssociatedProductDisplayOrder { get; set; }
        public List<string> ConfigurableAttributeCodes { get; set; }
        public List<PublishedSelectValuesEntityModel> SelectValues { get; set; }

        public PublishedConfigurableProductEntityModel()
        {
            SelectValues = new List<PublishedSelectValuesEntityModel>();
        }
    }
}
