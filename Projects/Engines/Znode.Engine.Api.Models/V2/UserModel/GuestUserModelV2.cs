using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class GuestUserModelV2 : BaseModel
    {
        public int UserId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        [Required]
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailOptIn { get; set; }
        public string ExternalId { get; set; }
        public AddressModelV2 BillingAddress { get; set; }
        public AddressModelV2 ShippingAddress { get; set; }
    }
}
