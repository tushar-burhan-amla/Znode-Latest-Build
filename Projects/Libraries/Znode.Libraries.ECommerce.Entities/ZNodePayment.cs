using System;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;
namespace Znode.Libraries.ECommerce.Entities
{
    /// <summary>
    /// Processes different payment types
    /// </summary>
    [Serializable()]
    public class ZnodePayment : ZnodeBusinessBase
    {     
        #region Public Properties       

        /// <summary>
        /// Gets or sets a value indicating whether the recurring billing exists.
        /// </summary>
        public bool IsRecurringBillingExists { get; set; }

        /// <summary>
        /// Gets or sets the PaymentSettingId
        /// </summary>
        public int? PaymentSettingId { get; set; }

        /// <summary>
        /// Gets or sets the paymentTypeId
        /// </summary>
        public int? PaymentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the payment name.
        /// </summary>
        public string PaymentName { get; set; }

        /// <summary>
        /// Gets or sets the payment type name.
        /// </summary>
        public string PaymentTypeName { get; set; }

        /// <summary>
        /// Gets or sets the payment ispreauthorize.
        /// </summary>
        public bool IsPreAuthorize { get; set; }
        /// <summary>
        /// Gets or sets the token Id.
        /// </summary>
        public int TokenId { get; set; } = 0;

        
        // Gets or sets the billing address 
        public AddressModel BillingAddress { get; set; }


        // Gets or sets the Shipping Address 
        public AddressModel ShippingAddress { get; set; }       

        /// <summary>
        /// Gets or sets the shopping cart.
        /// </summary>
        public ZnodeShoppingCart ShoppingCart { get; set; }       

        /// <summary>
        /// Gets or sets the session Id (used for worldpay gateway) 
        /// </summary>
        public string SessionId { get; set; }        

        /// <summary>
        /// Gets or sets the transaction Id.
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Gets or sets the authentication code.
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to save the credit card data for furter transaction.
        /// </summary>
        public bool SaveCardData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use token or not.
        /// </summary>
        public bool UseToken { get; set; }

        /// <summary>
        /// Gets or sets the subscription Id.
        /// </summary>
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the payment display name.
        /// </summary>
        public string PaymentDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the payment display name.
        /// </summary>
        public string PaymentExternalId { get; set; }

        #endregion
    }
}
