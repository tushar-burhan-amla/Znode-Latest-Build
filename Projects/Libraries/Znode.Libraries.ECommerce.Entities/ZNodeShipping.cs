using System;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Entities
{
    /// <summary>
    /// Property bag for settings used by the Shipping Rules.
    /// </summary>
    [Serializable()]
    public class ZnodeShipping : ZnodeBusinessBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ZNodeShipping class
        /// </summary>
        public ZnodeShipping()
        {
        }
        #endregion

        #region Public Properties

        // Gets or sets the shipping option Id
        public int ShippingID { get; set; }

        // Gets or sets the shipping option name
        public string ShippingName { get; set; }

        // Gets or sets the discount shipping cost
        public decimal ShippingDiscount { get; set; }

        // Gets or sets the discount shipping cost
        public string ShippingDiscountDescription { get; set; }

        // Gets or sets the shipping discount type
        public int ShippingDiscountType { get; set; }

        // Gets or sets the handling charge for shipping
        public decimal ShippingHandlingCharge { get; set; }

        // Gets or sets the shipping option response code
        public string ResponseCode { get; set; } = "0";

        // Gets or sets the shipping option response description
        public string ResponseMessage { get; set; }

        // Gets or sets the shipping option response shipping country code
        public string ShippingCountryCode { get; set; } = string.Empty;

        // Gets or sets the shipping setting
        public bool IsValidShippingSetting { get; set; } = true;

        // Gets or sets the shipping discount applied
        public bool ShippingDiscountApplied { get; set; }

        //Get set ShippingCode
        public string ShippingCode { get; set; }

        public bool IsExpedited { get; set; }
        #endregion
    }
}
