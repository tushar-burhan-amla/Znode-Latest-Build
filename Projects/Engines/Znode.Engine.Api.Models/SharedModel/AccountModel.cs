using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class AccountModel : BaseModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int AccountId { get; set; }
        public string ExternalId { get; set; }
        public int? ParentAccountId { get; set; }
        public string ParentAccountName { get; set; }
        public string AccountAddress { get; set; }
        public AddressModel Address { get; set; }
        public int? PortalId { get; set; }
        public string StoreName { get; set; }
        public string ShippingPostalCode { get; set; }
        public string BillingPostalCode { get; set; }
        public int? PublishCatalogId { get; set; }
        public string CatalogName { get; set; }
        public int LocaleId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorAccountCodeRequired)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.AccountCodeMaxLength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string AccountCode { get; set; }
        public int SalesRepId { get; set; }
        public string SalesRepUserName { get; set; }
        public string SalesRepFullName { get; set; }
    }
}
