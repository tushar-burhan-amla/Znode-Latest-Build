namespace Znode.Engine.Api.Models
{
    public class PortalShippingModel : BaseModel
    {
        public int ShippingPortalId { get; set; }
        public int PortalId { get; set; }

        public string ShippingOriginAddress1 { get; set; }
        public string ShippingOriginAddress2 { get; set; }
        public string ShippingOriginCity { get; set; }
        public string ShippingOriginCountryCode { get; set; }
        public string ShippingOriginPhone { get; set; }
        public string ShippingOriginStateCode { get; set; }
        public string ShippingOriginZipCode { get; set; }
        public bool? IsUseWarehouseAddress { get; set; }

        public string FedExAccountNumber { get; set; }
        public string FedExLTLAccountNumber { get; set; }
        public string FedExMeterNumber { get; set; }
        public string FedExProductionKey { get; set; }
        public string FedExSecurityCode { get; set; }
        public string FedExDropoffType { get; set; }
        public string FedExPackagingType { get; set; }
        public bool? FedExUseDiscountRate { get; set; }
        public bool? FedExAddInsurance { get; set; }

        public string UpsKey { get; set; }
        public string UpsPassword { get; set; }
        public string UpsUsername { get; set; }
        public string LTLUPSAccessLicenseNumber { get; set; }
        public string LTLUPSUsername { get; set; }
        public string LTLUPSPassword { get; set; }
        public string LTLUPSAccountNumber { get; set; }
        public string UPSDropoffType { get; set; }
        public string UPSPackagingType { get; set; }

        public string LTLUpsUserName { get; set; }
        public string LTLUpsPassword { get; set; }
        public string LTLUpsAccessLicenseNumber { get; set; }
        public string LTLUpsAccountNumber { get; set; }

        public string USPSShippingAPIURL { get; set; }
        public string USPSWebToolsUserID { get; set; }
        public decimal? PackageWeightLimit { get; set; }
        public int PublishStateId { get; set; }

        public PortalUnitModel portalUnitModel { get; set; }
    }
}
