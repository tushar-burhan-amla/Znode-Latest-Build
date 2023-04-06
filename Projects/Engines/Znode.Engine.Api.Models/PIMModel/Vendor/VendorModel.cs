namespace Znode.Engine.Api.Models
{
    public class VendorModel : BaseModel
    {
        public int PimVendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public int? PimAttributeId { get; set; }
        public int? AddressId { get; set; }
        public string ExternalVendorNo { get; set; }
        public string Email { get; set; }
        public string NotificationEmailID { get; set; }
        public string EmailNotificationTemplate { get; set; }
        public string CompanyName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public AddressModel Address { get; set; }
    }
}
