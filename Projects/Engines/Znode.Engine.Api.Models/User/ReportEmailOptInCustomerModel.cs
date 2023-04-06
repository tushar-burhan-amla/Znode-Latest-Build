namespace Znode.Engine.Api.Models
{
    public class ReportEmailOptInCustomerModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string StoreName { get; set; }
        public string CustomerType { get; set; }
    }
}
