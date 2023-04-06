namespace Znode.Engine.Api.Models.Responses
{
    public class ImageModel
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public int LargeImgWidth { get; set; }
        public int MediumImgWidth { get; set; }
        public int SmallImgWidth { get; set; }
        public int CrossImgWidth { get; set; }
        public int ThumbImgWidth { get; set; }
        public int SmallThumbImgWidth { get; set; }
        public string BucketName { get; set; }
    }
}
