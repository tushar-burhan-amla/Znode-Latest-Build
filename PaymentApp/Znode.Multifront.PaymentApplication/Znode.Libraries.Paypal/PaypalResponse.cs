using System;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Libraries.Paypal
{
    /// <summary>
    /// Paypal response
    /// </summary>
    public class PaypalResponse
    {
        ///// <summary>
        /////  Represents the Gateway Response code as a string.
        ///// </summary>
        //private string _HostUrl = "https://{0}/cgi-bin/webscr?cmd=_express-checkout&token={1}";

        #region Public properties
        /// <summary>
        /// Gets or sets or retrieves the Gateway Response code
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets or retrieves the Gateway Response Text
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// Gets or sets or retrieves the Gateway TransactionID
        /// </summary>
        public string PayalToken { get; set; }

        /// <summary>
        /// Gets or sets or retrieves the Gateway TransactionID
        /// </summary>
        public string HostUrl { get; set; } = "https://{0}/cgi-bin/webscr?cmd=_express-checkout&token={1}";

        /// <summary>
        /// Gets or sets or retrives the EC Redirect URL.
        /// </summary>
        public string ECRedirectURL { get; set; }

        /// <summary>
        ///  Gets or sets or retrieves the Paypal Gateway payer ID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// Gets or sets or retrieves the Paypal-PaymentStatus
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Gets or sets or retrieves the Paypal-TransactionID
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Gets or sets or retrieves the Paypal-TransactionID
        /// </summary>
        public string ProfileID { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets or retrieves the PaypalResponse
        /// </summary>
        /// <returns>Payment Status</returns>
        public ZnodePaymentStatus GetPaymentStatus()
        {
            ZnodePaymentStatus result = ZnodePaymentStatus.PENDING;

            switch (this.PaymentStatus)
            {
                case "Completed":
                    {
                        result = ZnodePaymentStatus.CAPTURED;
                        break;
                    }
                case "Canceled-Reversal":
                    {
                        result = ZnodePaymentStatus.DECLINED;
                        break;
                    }
                case "Denied":
                case "Expired":
                case "Failed":
                case "In-Progress":
                case "Partially-Refunded":
                case "Pending":
                    {
                        result = ZnodePaymentStatus.PENDING;
                        break;
                    }
                case "Refunded":
                    {
                        result = ZnodePaymentStatus.REFUNDED;
                        break;
                    }
                case "Reversed":
                case "Processed":
                    {
                        result = ZnodePaymentStatus.AUTHORIZED;
                        break;
                    }
                case "Voided":
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the paypal billing period
        /// </summary>
        /// <param name="Unit">Unit of the paypal billing period</param>
        /// <returns>Returns the billing period</returns>
        public string GetPaypalBillingPeriod(string Unit)
        {
            if (Unit.Equals("DAY", StringComparison.OrdinalIgnoreCase))
            {
                return "Day";
            }
            else if (Unit.Equals("WEEK", StringComparison.OrdinalIgnoreCase))
            {
                return "Week";
            }
            else if (Unit.Equals("MONTH", StringComparison.OrdinalIgnoreCase))
            {
                return "Month";
            }
            else if (Unit.Equals("YEAR", StringComparison.OrdinalIgnoreCase))
            {
                return "Year";
            }

            return "NoBillingPeriodType";
        }
        #endregion
    }
}