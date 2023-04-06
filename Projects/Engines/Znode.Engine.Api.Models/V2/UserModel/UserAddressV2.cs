using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class UserAddressV2 : BaseModel
    {
        public int AddressId { get; set; }
        public int AccountId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        [MaxLength(600)]
        public string DisplayName { get; set; }
        [MaxLength(300)]
        public string FirstName { get; set; }
        [MaxLength(300)]
        public string LastName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string StateCode { get; set; }
        [MaxLength(50)]
        public string PostalCode { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public bool IsDefaultBilling { get; set; }
        public bool IsDefaultShipping { get; set; }
        public bool IsActive { get; set; }
        public bool IsGuest { get; set; }

    }
}
