using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class UserAddressModel : BaseModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginName { get; set; }
        public int ProfileId { get; set; }
        public string CustomerPaymentGUID { get; set; }
        public AddressModel BillingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public bool UseSameAsBillingAddress { get; set; }
        public int? ReferralUserId { get; set; }
        public List<AddressModel> Addresses { get; set; }
    }
}
