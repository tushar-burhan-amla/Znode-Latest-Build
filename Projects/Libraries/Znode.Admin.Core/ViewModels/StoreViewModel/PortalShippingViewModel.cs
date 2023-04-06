using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalShippingViewModel : BaseViewModel
    {
        public int ShippingPortalId { get; set; }
        public int PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingOriginAddress1, ResourceType = typeof(Admin_Resources))]
        public string ShippingOriginAddress1 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingOriginAddress2, ResourceType = typeof(Admin_Resources))]
        public string ShippingOriginAddress2 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingOriginCity, ResourceType = typeof(Admin_Resources))]
        public string ShippingOriginCity { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingOriginCountryCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingOriginCountryCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingOriginPhoneNumber, ResourceType = typeof(Admin_Resources))]
        public string ShippingOriginPhone { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingOriginStateCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingOriginStateCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingOriginZipCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingOriginZipCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string FedExAccountNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExLTLAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string FedExLTLAccountNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExMeterNumber, ResourceType = typeof(Admin_Resources))]
        public string FedExMeterNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExProductionKey, ResourceType = typeof(Admin_Resources))]
        public string FedExProductionKey { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExSecurityCode, ResourceType = typeof(Admin_Resources))]
        public string FedExSecurityCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExDropoffType, ResourceType = typeof(Admin_Resources))]
        public string FedExDropoffType { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExPackagingType, ResourceType = typeof(Admin_Resources))]
        public string FedExPackagingType { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExUseDiscountRate, ResourceType = typeof(Admin_Resources))]
        public bool? FedExUseDiscountRate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFedExAddInsurance, ResourceType = typeof(Admin_Resources))]
        public bool? FedExAddInsurance { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUpsKey, ResourceType = typeof(Admin_Resources))]
        public string UpsKey { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUpsPassword, ResourceType = typeof(Admin_Resources))]
        public string UpsPassword { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUpsUsername, ResourceType = typeof(Admin_Resources))]
        public string UpsUsername { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLTLAccessLicenseNumber, ResourceType = typeof(Admin_Resources))]
        public string LTLUPSAccessLicenseNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLTLUsername, ResourceType = typeof(Admin_Resources))]
        public string LTLUPSUsername { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLTLPassword, ResourceType = typeof(Admin_Resources))]
        public string LTLUPSPassword { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLTLAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string LTLUPSAccountNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsUseWareHouseAddress, ResourceType = typeof(Admin_Resources))]
        public bool? IsUseWarehouseAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUSPSShippingAPIURL, ResourceType = typeof(Admin_Resources))]
        public string USPSShippingAPIURL { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUSPSWebToolsUserID, ResourceType = typeof(Admin_Resources))]
        public string USPSWebToolsUserID { get; set; }

        [Range(0.00001, 999999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidPackageWeightLimit)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Display(Name = ZnodeAdmin_Resources.LabelPackageWeightLimit, ResourceType = typeof(Admin_Resources))]
        public decimal? PackageWeightLimit { get; set; } = 65;


        [Display(Name = ZnodeAdmin_Resources.LabelUPSDropoffType, ResourceType = typeof(Admin_Resources))]
        public string UPSDropoffType { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUPSPackagingType, ResourceType = typeof(Admin_Resources))]
        public string UPSPackagingType { get; set; }

        public int PublishStateId { get; set; }

        public string PortalName { get; set; }

        public List<SelectListItem> PackagingTypes { get; set; }
        public List<SelectListItem> FedexDropOffTypes { get; set; }
        public List<SelectListItem> countryList { get; set; }

        public List<SelectListItem> UPSPackagingTypes { get; set; }
        public List<SelectListItem> UPSDropOffTypes { get; set; }
    }
}