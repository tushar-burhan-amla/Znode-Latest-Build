using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        //Not required for Manage/Edit Order
        public string StoreName { get; set; }
        public string Store { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelOrderState, ResourceType = typeof(Admin_Resources))]
        public string OrderState { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPaymentStatus, ResourceType = typeof(Admin_Resources))]
        public string PaymentStatus { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPaymentMethod, ResourceType = typeof(Admin_Resources))]
        public string PaymentType { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelOrderDate, ResourceType = typeof(Admin_Resources))]
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BillingAddressHtml { get; set; }

        public string PaymentDisplayName { get; set; }

        public AddressViewModel BillingAddress { get; set; }
        public AddressViewModel ShippingAddress { get; set; }

        public GridModel OrderLineItemsGridModel { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelPurchaseOrderNumber, ResourceType = typeof(Admin_Resources))]
        public string PurchaseOrderNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPurchaseOrderDocument, ResourceType = typeof(Admin_Resources))]
        public string PurchaseOrderDocument { get; set; }
        public string PODocumentName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.ShippingType, ResourceType = typeof(Admin_Resources))]
        public string ShippingTypeName { get; set; }

        public List<OrderNotesViewModel> OrderNotes { get; set; }
        public OrderInfoViewModel OrderInformation { get; set; }
        public CustomerInfoViewModel CustomerInformation { get; set; }
        public CartViewModel CartInformation { get; set; }
        public ReturnOrderLineItemListViewModel ReturnOrderLineItems { get; set; }
        public string AdditionalNotes { get; set; }
        public string TrackingNumber { get; set; }
        public string CouponCode { get; set; }
        public string UpdatePageType { get; set; }
        public int AccountId { get; set; }
        public int? PaymentTypeId { get; set; }
        public string OrderNumber { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string OrderDateWithTime { get; set; }
        public List<SelectListItem> OrderPaymentStatusList { get; set; }
        public decimal OrderAmount { get; set; }
        public string ExternalId { get; set; }

        //Properties which will use only for Manage/Edit Order
        public int OmsOrderDetailsId { get; set; }
        public int PortalId { get; set; }
        public int PortalCatalogId { get; set; }
        public int UserId { get; set; }
        public int OmsOrderId { get; set; }
        public decimal Total { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelTransactionId, ResourceType = typeof(Admin_Resources))]
        public string PaymentTransactionToken { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTaxCost, ResourceType = typeof(Admin_Resources))]
        public decimal TaxCost { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelImportDuty, ResourceType = typeof(Admin_Resources))]
        public decimal ImportDuty { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingCost, ResourceType = typeof(Admin_Resources))]
        public decimal ShippingCost { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSubTotal, ResourceType = typeof(Admin_Resources))]
        public decimal SubTotal { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDiscountAmount, ResourceType = typeof(Admin_Resources))]
        public decimal DiscountAmount { get; set; }
        public List<OrderLineItemViewModel> OrderLineItems { get; set; }
        public string CurrencyCode { get; set; }
        public decimal GiftCardAmount { get; set; }
        public decimal CSRDiscountAmount { get; set; }
        public string OrderTotalWithCurrency { get; set; }
        public string Tax { get; set; }
        public string Shipping { get; set; }
        public string SubTotalAmount { get; set; }
        public int OmsPaymentStateId { get; set; }
        public bool IsInRMA { get; set; }
        public bool IsLineItemShipped { get; set; }
        public string CreditCardNumber { get; set; }

        public string CustomerPaymentGUID { get; set; }

        public string ReceiptHtml { get; set; }
        public bool IsEmailSend { get; set; }
        public bool IsTaxCostEdited { get; set; }
        public bool IsValidForRma { get; set; }
        public bool IsEmailNotificationForRma { get; set; }
        public int OmsOrderStateId { get; set; }

        public string CustomerServiceEmail { get; set; }
        public string CustomerServicePhoneNumber { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingTrackingUrl { get; set; }
        public DateTime OrderModifiedDate { get; set; }
        public string BillingPostalCode { get; set; }
        public string ShippingPostalCode { get; set; }
        public string StoreLogo { get; set; }
        public DateTime? ShipDate { get; set; }
        public bool IsCaptureDisable { get; set; }
        public decimal? EstimateShippingCost { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTransactionId, ResourceType = typeof(Admin_Resources))]
        public string TransactionId { get; set; }

        public string CultureCode { get; set; }
        public string PublishState { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelInHandsDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? InHandDate { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelJobName, ResourceType = typeof(Admin_Resources))]
        public string JobName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingConstraintsCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingConstraintCode { get; set; }
        public bool IsAnyPendingReturn { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public decimal ReturnCharges { get; set; }
        public bool IsAnOldOrder { get; set; }
        public decimal OrderTotalWithoutVoucher { get; set; }
        public bool IsOldOrder { get; set; }
        public List<TaxSummaryViewModel> TaxSummaryList { get; set; }
        public bool IsTradeCentricUser { get; set; }
    }
}