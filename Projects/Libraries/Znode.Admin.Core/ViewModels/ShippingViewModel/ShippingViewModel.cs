using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ShippingViewModel : BaseViewModel
    {

        public ShippingViewModel()
        {
            ShippingServiceCodeList = new List<SelectListItem>();
        }
        public int PortalId { get; set; }
        public int ShippingTypeId { get; set; }
        public int ShippingId { get; set; }
        public int? ProfileId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.ShippingInternalCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingCode { get; set; }
        public string DestinationCountryCode { get; set; } = DefaultSettingHelper.DefaultCountry;

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredHandlingCharge)]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceName = ZnodeAdmin_Resources.HandlingChargeMustBeNumber, ErrorMessageResourceType = typeof(Admin_Resources))]
        public decimal HandlingCharge { get; set; }

        public string HandlingChargeBasedOn { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorDisplayNameRequired)]
        [Display(Name = ZnodeAdmin_Resources.ShippingDisplayName, ResourceType = typeof(Admin_Resources))]
        public string Description { get; set; }

        [RegularExpression("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        [Display(Name = ZnodeAdmin_Resources.LabelTrackingUrl, ResourceType = typeof(Admin_Resources))]
        public string TrackingUrl { get; set; }
        public bool IsActive { get; set; } = true;

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterDisplayOrder, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Range(1, 999999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidShippingDisplayOrder)]
        public int? DisplayOrder { get; set; } = 99;
        public string ExternalId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterShippingName)]
        [Display(Name = ZnodeAdmin_Resources.LabelShippingName, ResourceType = typeof(Admin_Resources))]
        public string ShippingName { get; set; }
        public string ShippingTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ShippingServiceCodeRequiredMessage)]
        public int ShippingServiceCodeId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseSelectShippingType, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string ClassName { get; set; }

        [MaxLength(5000, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ZipCodeLengthErrorMessage)]
        public string ZipCode { get; set; }

        public int? CurrencyId { get; set; }

        public List<SelectListItem> ShippingTypeList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }
        public List<SelectListItem> ShippingServiceCodeList { get; set; }

        public ShippingSKUViewModel ShippingSKU { get; set; }

        public HttpPostedFileBase ImportFile { get; set; }
        public string ImportedSkus { get; set; }

        public bool IsAmount { get; set; } = true;
        public bool IsSubtotal { get; set; }
        public bool IsShipping { get; set; }
        public string StateCode { get; set; }
        public string CountyFIPS { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> CityList { get; set; }
        public string FormattedShippingRate { get; set; }
        public decimal? ShippingRate { get; set; }
        public int PromotionId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelDeliveryTimeFrame, ResourceType = typeof(Admin_Resources))]
        public string DeliveryTimeframe { get; set; }
        public string PublishState { get; set; }
        public List<SelectListItem> PublishStateList { get; set; }
        public string FormattedShippingRateWithoutDiscount { get; set; }
        public decimal? ShippingRateWithoutDiscount { get; set; }
    }
}