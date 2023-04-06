namespace Znode.Engine.Api.Models
{
    public class MediaDetailModel 
    {
        public int MediaId { get; set; }      
        public string FileName { get; set; }
        public bool IsImage { get; set; }
        public string MediaServerPath { get; set; }
        public string Path { get; set; }
        public string  Version { get; set; }
    }
}
