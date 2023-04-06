using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.ECommerce.Entities
{
    [Serializable()]
    public class ZnodeOrder
    {
        #region Private Variables      
        protected IZnodeShoppingCartEntities shoppingCart = null;
        private OrderModel _Order = new OrderModel();
        private string _AdditionalInstructions = string.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ZNodeOrder class
        /// </summary>
        public ZnodeOrder(IZnodeShoppingCartEntities znodeCart)
        {
            shoppingCart = znodeCart;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the Billing Address object.
        /// </summary>
        public AddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Address object.
        /// </summary>
        public AddressModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the tax cost for this order
        /// </summary>
        public decimal VAT { get; set; }
        
        /// <summary>
        /// Gets or sets the Import Duty for this order
        /// </summary>
        public decimal ImportDuty { get; set; }    

        /// <summary>
        /// Gets or sets the payment trancation token for this order
        /// </summary>
        public string PaymentTransactionToken { get; set; }

        /// <summary>
        /// Gets or sets the tax cost for this order
        /// </summary>
        public decimal SalesTax { get; set; }

        /// <summary>
        /// Gets or sets the referral UserId for this order
        /// </summary>
        public int? ReferralUserId { get; set; }

        /// <summary>
        /// Gets or sets the tax cost for this order
        /// </summary>
        public decimal HST { get; set; }

        /// <summary>
        /// Gets or sets the tax cost for this order
        /// </summary>
        public decimal GST { get; set; }

        /// <summary>
        /// Gets or sets the tax cost for this order
        /// </summary>
        public decimal PST { get; set; }

        /// <summary>
        /// Gets or sets the card type for this order
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// Gets or sets the credit card exp month for this order
        /// </summary>
        public int? CreditCardExpMonth { get; set; }

        /// <summary>
        /// Gets or sets the credit card exp year for this order
        /// </summary>
        public int? CreditCardExpYear { get; set; }
        /// <summary>
        /// Gets or sets the estimate cost for order
        /// </summary>
        public decimal? EstimateShippingCost { get; set; }
        
        /// <summary>
        /// Gets or sets the list of orderline items for this order
        /// </summary>
        public string PaymentDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the payment external id for this order
        /// </summary>
        public string PaymentExternalId { get; set; }

        /// <summary>
        /// Gets or sets the list of order line items for this order
        /// </summary>
        public List<OrderLineItemModel> OrderLineItems { get; set; } = new List<OrderLineItemModel>();

        /// <summary>
        /// Gets or sets the Order object.
        /// </summary>
        public OrderModel Order
        {
            get { return this._Order; }
            set { this._Order = value; }
        }

        /// <summary>
        /// Gets the gift card amount.
        /// </summary>
        public decimal GiftCardAmount
        {
            get
            {
                return this.shoppingCart.GiftCardAmount;
            }
            set { this.shoppingCart.GiftCardAmount = value; }
        }

        /// <summary>
        /// Gets the gift card number.
        /// </summary>
        public string GiftCardNumber
        {
            get
            {
                return this.shoppingCart.GiftCardNumber;
            }
            set
            {
                this.shoppingCart.GiftCardNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the account mail Id.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the _Order id for this case
        /// </summary>
        public int OrderID
        {
            get { return this._Order.OmsOrderId; }
            set { this._Order.OmsOrderId = value; }
        }

        /// <summary>
        /// Gets or sets the site portal id
        /// </summary>
        public int PortalId
        {
            get { return this._Order.PortalId; }
            set { this._Order.PortalId = value; }
        }

        /// <summary>
        /// Gets or sets the account Id
        /// </summary>   
        public int UserID
        {
            get { return this._Order.UserId; }
            set { this._Order.UserId = value; }
        }

        /// <summary>
        /// Gets or sets the status for this order
        /// </summary>
        public int OrderStateID
        {
            get { return this._Order.OmsOrderStateId; }
            set { this._Order.OmsOrderStateId = value; }
        }

        /// <summary>
        /// Gets or sets the shipping type for this order
        /// </summary>
        public int ShippingId
        {
            get { return this._Order.ShippingId; }
            set { this._Order.ShippingId = value; }
        }

        public string CreditCardNumber
        {
            get { return this._Order.CreditCardNumber; }
            set { this._Order.CreditCardNumber = value; }
        }
        /// <summary>
        /// Gets or sets the payment type for this order
        /// </summary>
        public int? PaymentTypeId
        {
            get { return this._Order.PaymentTypeId; }
            set { this._Order.PaymentTypeId = value; }
        }

        /// <summary>
        /// Gets or sets the payment setting for this order
        /// </summary>
        public int? PaymentSettingID
        {
            get { return this._Order.PaymentSettingId; }
            set { this._Order.PaymentSettingId = value; }
        }

        /// <summary>
        /// Gets or sets the purchase _Order number applied by the customer,
        /// if Purchase _Order selected for this order
        /// </summary>
        public string PurchaseOrderNumber
        {
            get { return this._Order.PurchaseOrderNumber; }
            set { this._Order.PurchaseOrderNumber = value; }
        }

        /// <summary>
        /// Gets or sets the purchase _Order document applied by the customer,
        /// if Purchase _Order selected for this order
        /// </summary>
        public string PODocument
        {
            get { return this._Order.PoDocument; }
            set { this._Order.PoDocument = value; }
        }

        /// <summary>
        /// To get set currency code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// To get set culture code
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        /// To get set the ip address
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// To get set the In hand date
        /// </summary>
        public DateTime? InHandDate { get; set; }

        /// <summary>
        /// To get set the Job Name
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// To get set the Shipping Constraint Code
        /// </summary>
        public string ShippingConstraintCode { get; set; }

        /// <summary>
        /// To get set publishStateId
        /// </summary>
        public int PublishStateId { get; set; }

        /// <summary>
        /// Gets or sets the type of the card
        /// </summary>
        public int? PaymentStatusID
        {
            get { return this._Order.OmsPaymentStateId; }
            set { this._Order.OmsPaymentStateId = value; }
        }

        /// <summary>
        /// Gets or sets the tax cost for this order
        /// </summary>
        public decimal TaxCost
        {
            get { return this._Order.TaxCost; }
            set { this._Order.TaxCost = value; }
        }

        /// <summary>
        ///  Gets or sets the coupon discount amount for this order
        /// </summary>
        public decimal DiscountAmount
        {
            get { return this.Discount; }
            set { this._Order.DiscountAmount = value; }
        }

        /// <summary>
        ///  Gets the coupon discount for this order
        /// </summary>
        public decimal Discount
        {
            get
            {
                decimal disc = 0;
                if (this.shoppingCart != null)
                {
                    disc = this.shoppingCart.Discount;
                }

                return disc;
            }
        }

        /// <summary>
        /// Gets the csr discount amount.
        /// </summary>
        public decimal CSRDiscountAmount
        {
            get { return this.shoppingCart.CSRDiscountAmount; }
            set { this._Order.CSRDiscountAmount = value; }
        }

        /// <summary>
        /// Gets the csr discount description.
        /// </summary>
        public string CSRDiscountDescription
        {
            get
            {
                return this.shoppingCart.CSRDiscountDescription;
            }
        }

        /// <summary>
        ///  Gets or sets the Additional customer Instructions for this order
        /// </summary>
        public string AdditionalInstructions
        {
            get { return this._Order.AdditionalInstructions; }
            set { this._Order.AdditionalInstructions = value; }
        }

        /// <summary>
        ///  Gets or sets the coupon codes applied by the customer for this _Order
        /// </summary>
        public string CouponCode
        {
            get { return this._Order.CouponCode; }
            set { this._Order.CouponCode = value; }
        }

        /// <summary>
        /// Gets or sets the shipping cost for this order
        /// </summary>
        public decimal ShippingCost
        {
            get { return this._Order.ShippingCost; }
            set { this._Order.ShippingCost = value; }
        }

        /// <summary>
        /// Gets or sets the shipping difference for this order
        /// </summary>
        public decimal ShippingDifference
        {
            get { return this._Order.ShippingDifference; }
            set { this._Order.ShippingDifference = value; }
        }

        /// <summary>
        /// Gets or sets the sub-total amount for this order
        /// </summary>
        public decimal SubTotal
        {
            get { return this._Order.SubTotal; }
            set { this._Order.SubTotal = value; }
        }

        /// <summary>
        /// Gets or sets the total amount for this order
        /// </summary>
        public decimal Total
        {
            get { return this._Order.Total; }
            set { this._Order.Total = value; }
        }

        /// <summary>
        /// Gets or sets the OrderTotalWithoutVoucher for the order
        /// </summary>
        public decimal OrderTotalWithoutVoucher
        {
            get { return this._Order.OrderTotalWithoutVoucher; }
            set { this._Order.OrderTotalWithoutVoucher = value; }
        }

        /// <summary>
        /// Gets or sets the ordered date
        /// </summary>
        public DateTime OrderDate
        {
            get { return this._Order.OrderDate; }
            set { this._Order.OrderDate = value; }
        }

        /// <summary>
        /// Gets or sets the ordered date with time stamp.
        /// </summary>
        public string OrderDateWithTime
        {
            get { return this._Order.OrderDateWithTime; }
            set { this._Order.OrderDateWithTime = value; }
        }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        public DateTime Created
        {
            get { return this._Order.CreatedDate; }
            set { this._Order.CreatedDate = value; }
        }

        /// <summary>
        /// Gets or sets the modified date
        /// </summary>
        public DateTime Modified
        {
            get { return this._Order.ModifiedDate; }
            set { this._Order.ModifiedDate = value; }
        }

        /// <summary>
        ///  Gets or sets the custom3 data for this _Order
        /// </summary>
        public int CreatedBy
        {
            get { return this._Order.CreatedBy; }
            set { this._Order.CreatedBy = value; }
        }

        /// <summary>
        ///  Gets or sets the custom3 data for this _Order
        /// </summary>
        public int ModifiedBy
        {
            get { return this._Order.ModifiedBy; }
            set { this._Order.ModifiedBy = value; }
        }

        /// <summary>
        /// Gets the shipping discount.
        /// </summary>
        public decimal ShippingDiscount
        {
            get
            {
                return this.shoppingCart.Shipping?.ShippingDiscount ?? 0;
            }
        }

        /// <summary>
        /// Gets the shipping discount description.
        /// </summary>
        public string ShippingDiscountDescription
        {
            get
            {
                return this.shoppingCart.Shipping.ShippingDiscountDescription;
            }
        }

        /// <summary>
        /// Gets the shipping name.
        /// </summary>
        public string ShippingName
        {
            get
            {
                return this.shoppingCart.Shipping.ShippingName;
            }
        }

        /// <summary>
        /// Gets the shipping discount type
        /// </summary>
        public int ShippingDiscountType
        {
            get
            {
                return this.shoppingCart.Shipping.ShippingDiscountType;
            }
        }

        /// <summary>
        /// To get set order over due amount
        /// </summary>
        public decimal OrderOverDueAmount { get; set; }

        /// <summary>
        /// to get set shipping cost edited 
        /// </summary>
        public bool IsShippingCostEdited { get; set; }

        /// <summary>
        /// to get set tax cost edited 
        /// </summary>
        public bool IsTaxCostEdited { get; set; }
        /// <summary>
        /// check Email status
        /// </summary>
        public bool IsEmailSend { get; set; }
        /// <summary>
        /// to get set external Id for erp data.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// to get set Custom1 for custom field
        /// </summary>
        public string Custom1 { get; set; }

        /// <summary>
        /// to get set Custom2 for custom field
        /// </summary>
        public string Custom2 { get; set; }

        /// <summary>
        /// to get set Custom3 for custom field
        /// </summary>
        public string Custom3 { get; set; }

        /// <summary>
        /// to get set Custom4 for custom field
        /// </summary>
        public string Custom4 { get; set; }

        /// <summary>
        /// to get set Custom5 for custom field
        /// </summary>
        public string Custom5 { get; set; }

        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        /// <summary>
        /// Gets the shipping discount.
        /// </summary>
        public decimal ShippingHandlingCharges
        {
            get
            {
                return this.shoppingCart.Shipping?.ShippingHandlingCharge ?? 0;
            }
        }
        public decimal ReturnCharges { get; set; }

        public bool? IsCalculateTaxAfterDiscount { get; set; }
       
        public bool? IsPricesInclusiveOfTaxes { get; set; }
      
        /// <summary>
        /// Gets or sets the tax message list.
        /// </summary>
        public List<string> TaxMessageList { get; set; }

        /// <summary>
        /// Gets or sets the tax summary list.
        /// </summary>
        public List<TaxSummaryModel> TaxSummaryList { get; set; }

        #endregion
    }
}
