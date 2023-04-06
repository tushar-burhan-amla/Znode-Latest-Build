namespace Znode.Engine.Api.Models
{
    public class WebStoreAttributeValueSwatchModel:BaseModel
    {
        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
    }
}
