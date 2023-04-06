using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Multifront.PaymentApplication.Models.Models
{
   public class AddressModel : BaseModel
    {
        public int AddressId { get; set; }
        public int AccountId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string StateCode { get; set; }

        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }
        public bool IsDefaultBilling { get; set; }
        public bool IsDefaultShipping { get; set; }
        public bool IsActive { get; set; }
        public bool IsGuest { get; set; }
        public bool IsShipping { get; set; }
        public bool IsBilling { get; set; }
        public bool IsShippingBillingDifferent { get; set; }
        public string FromBillingShipping { get; set; }
        public int AccountAddressId { get; set; }
        public int WarehouseId { get; set; }
        public int UserId { get; set; }
        public string AspNetUserId { get; set; }
        public int UserAddressId { get; set; }
        public string Mobilenumber { get; set; }
        public string AlternateMobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public string WarehouseName { get; set; }
        public string EmailAddress { get; set; }
        public string LTLAccountNumber { get; set; }
        public bool? IsUseWareHouseAddress { get; set; }
        public int PortalId { get; set; }
        public string ExternalId { get; set; }

        public string CompanyName { get; set; }
        public string CountryCode { get; set; }
        public bool DontAddUpdateAddress { get; set; }
        public int omsOrderId { get; set; }
        public int omsOrderShipmentId { get; set; }
        public int PublishStateId { get; set; }
        public bool IsAddressBook { get; set; }
        public string GatewayCode { get; set; }
    }
}
