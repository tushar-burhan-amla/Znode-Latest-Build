namespace Znode.Engine.Api.Models
{
    public class WebStoreProductImagesModel : BaseModel
    {
        public int MediaId { get; set; }
        public string MediaPath { get; set; }
        public int DisplayOrder { get; set; }
    }
}
