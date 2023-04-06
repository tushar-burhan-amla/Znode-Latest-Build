namespace Znode.Engine.Api.Models
{
    public class FileUploadResponse
    {
        public int StatusCode { get; set; }
        public string FileName { get; set; }
        public string MediaId { get; set; }
        public string ImagePath { get; set; }
        public bool IsDocumentRemove { get; set; }
    }
}
