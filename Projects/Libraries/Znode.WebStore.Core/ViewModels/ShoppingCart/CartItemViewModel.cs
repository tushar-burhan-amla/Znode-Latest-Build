using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CartItemViewModel : BaseViewModel
    {
        public CartItemViewModel()
        {
            ExternalId = System.Guid.NewGuid().ToString();
        }

        public string ProductId { get; set; }
        public int ParentProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal Quantity { get; set; }
        public string GroupProductsQuantity { get; set; }
        public string SKU { get; set; }
        public string RoleName { get; set; }
        public decimal QuantityOnHand { get; set; }
        public string SeoPageName { get; set; }
        public string ProductCode { get; set; }
        public int UOMId { get; set; }
        public bool TrackInventory { get; set; }
        public bool AllowBackOrder { get; set; }
        public bool InsufficientQuantity { get; set; }
        public string ImagePath { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExtendedPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Total { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public decimal QuoteOrderTotal { get; set; }

        public int UserId { get; set; }
        public int SelectedAccountUserId { get; set; }
        public string CartDescription { get; set; }
        public string BundleProductSKUs { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string ExternalId { get; set; }
        public string GroupProductSKUs { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public string PersonalisedCodes { get; set; }
        public string PersonalisedValues { get; set; }
        public string OrderStatus { get; set; }
        public string OutOfStockMessage { get; set; }
        public string AutoAddonSKUs { get; set; }

        public int OmsOrderLineItemsId { get; set; }
        public int OmsQuoteId { get; set; }
        public int OmsQuoteLineItemId { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public int Sequence { get; set; }
        public int GroupSequence { get; set; }
        public int QuoteLineItemCount { get; set; }
        public string CustomText { get; set; }
        public string CartAddOnDetails { get; set; }
        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingId { get; set; }

        public string GroupId { get; set; }
        public List<AssociatedProductModel> GroupProducts { get; set; }

        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<AssociatedProductModel> AssociatedProducts { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }

        public string UOM { get; set; }
        public bool IsAllowedTerritories { get; set; }
        public Dictionary<string, decimal> AdditionalCost { get; set; }
        public bool PermissionDraft
        {
            get
            {
                if (string.Equals(this.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    || (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                    return true;
                else
                    return false;
            }
        }

        public List<PublishAttributeModel> ProductAttributes { get; set; }

        public int? OmsSavedcartLineItemId { get; set; }

        public int? ParentOmsSavedcartLineItemId { get; set; }

        public bool IsProductEdit { get; set; }
        public bool PermissionReject
        {
            get
            {
                if (string.Equals(this.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    || (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else
                    return false;
            }
        }
        public string CultureCode { get; set; }
    }
}