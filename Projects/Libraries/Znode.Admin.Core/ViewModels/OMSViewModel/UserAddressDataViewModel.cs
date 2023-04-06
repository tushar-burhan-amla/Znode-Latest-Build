using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class UserAddressDataViewModel : BaseViewModel
    {
        public AddressViewModel ShippingAddress { get; set; }
        public AddressViewModel BillingAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "ErrorRequired")]
        public string Email { get; set; }

        public int? AccountId { get; set; }
        public int UserId { get; set; }
        public int LocaleId { get; set; }
        public string MediaServerUrl { get; set; }
        public string MediaServerThumbnailUrl { get; set; }
        public string CurrencyCode { get; set; }
        public int PortalId { get; set; }
        public int PortalCatalogId { get; set; }
        public bool CreateLogin { get; set; }
        public bool UseSameAsBillingAddress { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public int UserBillingAddressId { get; set; }
        public int UserShippingAddressId { get; set; }
        public int ProfileId { get; set; }
        public string FullName { get; set; }
        public decimal? BudgetAmount { get; set; }
        public string PermissionCode { get; set; }
        public bool IsOrderQuote { get; set; }
        public List<SelectListItem> UsersAddressNameList { get; set; }

        public string CustomerPaymentGUID { get; set; }
        public bool EnableAddressValidation { get; set; }
        public bool RequireValidatedAddress { get; set; }
        public bool IsMultipleCouponCodeAllowed { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public string StateCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAddEditAddressAllow { get; set; }
    }
}