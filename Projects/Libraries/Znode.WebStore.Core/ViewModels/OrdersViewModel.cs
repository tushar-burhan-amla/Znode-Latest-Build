using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public string StoreName { get; set; }
        public int UserId { get; set; }
        public int OmsOrderId { get; set; }
        public string Store { get; set; }
        public string OrderState { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDisplayName { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public decimal? Total { get; set; }
        public decimal? RemainingOrderAmount { get; set; }
        public string TotalWithCurrencyCode { get; set; }
        public string UserName { get; set; }

        public decimal TaxCost { get; set; }
        public decimal ImportDuty { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public List<OrderLineItemViewModel> OrderLineItems { get; set; }

        public string BillingAddressHtml { get; set; }
        public string OrderItem { get; set; }
        public int ItemCount { get; set; }
        public string CurrencyCode { get; set; }
        public string TrackingNumber { get; set; }
        public string PayPalExpressResponseText { get; set; }
        public string PayPalExpressResponseToken { get; set; }
        public decimal GiftCardAmount { get; set; }
        public string ShippingTypeName { get; set; }
        public string CouponCode { get; set; }
        public decimal CSRDiscountAmount { get; set; }
        public string PODocumentPath { get; set; }

        public string ComissionJunctionURL { get; set; }
        public bool IsInventoryAndMinMaxQuantityAvailable { get; set; }
        public ReturnOrderLineItemListModel ReturnItemList { get; set; }

        public string OrderLineItemQueryString { get; set; }

        public string CustomerServiceEmail { get; set; }
        public string CustomerServicePhoneNumber { get; set; }
        public string TransactionId { get; set; }
        public decimal TotalAdditionalCost { get; set; }
        public string Comments { get; set; }
        public string CultureCode { get; set; }
        public List<QuoteApprovalViewModel> QuoteApproverComments { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public DateTime? InHandDate { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }
        public bool IsOrderEligibleForReturn { get; set; }
        public bool IsOrderEligibleForReturnReceipt { get; set; }
        public string GiftCardNumber { get; set; }
        public decimal OrderTotalWithoutVoucher { get; set; }

        //Returns the list of purchased products to be sent to Ecommerce Sales Performance report.
        public virtual EcommercePurchaseDataViewModel GetEcommercePurchaseData()
        {
            List<EcommercePurchasedProductsViewModel> purchasedProducts = OrderLineItems?
                .Select(x => new EcommercePurchasedProductsViewModel() { Id = x.OmsOrderLineItemsId, Sku = x.Sku, ProductName = x.ProductName, Description = x.Description, Quantity = x.Quantity, Price = x.Price })
                .ToList();
            EcommercePurchaseDataViewModel ecommercePurchaseDataViewModel = new EcommercePurchaseDataViewModel() { OrderNumber = OrderNumber, Total = Total, ShippingCost = ShippingCost, TaxCost = TaxCost, CouponCode = CouponCode, PurchasedProducts = purchasedProducts };
            return ecommercePurchaseDataViewModel;
        }
        public decimal ShippingDiscount { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public decimal ReturnCharges { get; set; }
        public List<TaxSummaryViewModel> TaxSummaryList { get; set; }
        public int? OmsPaymentStateId { get; set; }//get and set for order payment status id for paypal express payment method
        public string OrderTotalWithCurrency { get; set; }//get and set account orders amount with currency 
        public string RoleName { get; set; }//get and set current user rolename
        public string CustomerPaymentGUID { get; set; }
        public bool IsUserForOfflinePayment { get; set; }
        public string PaymentApiResponseHeader { get; set; }
       
        public PaymentHistoryListViewModel PaymentHistoryList { get; set; }
    }
}