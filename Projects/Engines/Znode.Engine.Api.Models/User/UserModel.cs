using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Models
{
    public class UserModel : BaseModel
    {
        public int UserId { get; set; }
        public string AspNetUserId { get; set; }

        public string AspNetZnodeUserId { get; set; }
        public string Email { get; set; }
        public string StoreCode { get; set; }
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginName { get; set; }
        public bool IsEmailSentFailed { get; set; }
        public LoginUserModel User { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int? PortalId { get; set; }
        public bool? IsLock { get; set; }
        public int ProfileId { get; set; }

        public string CompanyName { get; set; }
        public bool EmailOptIn { get; set; }
        public bool SMSOptIn { get; set; }
        public string ExternalId { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Source { get; set; }

        public string Accountname { get; set; }
        public string AccountCode { get; set; }
        public string StoreName { get; set; }
        public string PermissionsName { get; set; }
        public string DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
        public int? AccountId { get; set; }
        public int? AccountPermissionAccessId { get; set; }
        public int? DepartmentUserId { get; set; }
        public string[] PortalIds { get; set; }
        public int? ApprovalUserId { get; set; }
        public string PermissionCode { get; set; }
        public decimal? BudgetAmount { get; set; }
        public string ApproverName { get; set; }
        public int? AccountUserOrderApprovalId { get; set; }
        public int? AccountUserPermissionId { get; set; }
        public string CustomerPaymentGUID { get; set; }
        public List<AddressModel> Addresses { get; set; }
        public List<ProfileModel> Profiles { get; set; }
        public List<WishListModel> WishList { get; set; }
        public bool IsAdminUser { get; set; }
        public bool IsGuestUser { get; set; }
        public decimal? ReferralCommission { get; set; }
        public int? ReferralCommissionTypeId { get; set; }
        public bool IsWebStoreUser { get; set; }
        public int? ReferralUserId { get; set; }
        public int LocaleId { get; set; }
        public int QuoteApproverUserId { get; set; }
        public ExternalLoginInfo ExternalLoginInfo { get; set; }
        public bool IsSocialLogin { get; set; }
        public string ShippingPostalCode { get; set; }
        public string BillingPostalCode { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public int? PublishCatalogId { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public decimal PerOrderLimit { get; set; }
        public decimal AnnualOrderLimit { get; set; }
        public decimal AnnualBalanceOrderAmount { get; set; }
        public string BillingAccountNumber { get; set; }
        public int SalesRepId { get; set; }
        public string SalesRepUserName { get; set; }
        public string SalesRepFullName { get; set; }

        public IEnumerable<GlobalAttributeValuesModel> UserGlobalAttributes { get; set; }
        public UserVerificationTypeEnum UserVerificationTypeCode { get; set; }
        public string UserVerificationType { get; set; }
        public bool IsVerified { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; } = "";
        public string BusinessIdentificationNumber { get; set; }
        //IsInvalidCredential is used to identify the alert mail for registration attempt with existing email. 
        public bool IsInvalidCredential { get; set; }
        public bool IsTradeCentricUser { get; set; }
    }
}
