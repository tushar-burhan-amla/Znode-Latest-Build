namespace Znode.Engine.Api.Models
{
    public class RMAEmailDetailsModel : BaseModel
    {
        public string ReturnNumber { get; set; }
        public string ReturnStatus { get; set; }
        public string UserFullName { get; set; }
        public string EmailId { get; set; }
        public int PortalId { get; set; }
        public string EmailTemplateCode { get; set; }
        public string ReturnDetailsUrl { get; set; }
        public string CustomerServiceEmail { get; set; }
        public string CustomerServicePhoneNumber { get; set; }
    }
}
