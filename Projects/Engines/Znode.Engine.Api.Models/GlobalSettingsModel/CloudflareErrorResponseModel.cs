namespace Znode.Engine.Api.Models
{
    public class CloudflareErrorResponseModel : BaseModel
    {
        public int DomainId { get; set; }
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
