using System;
using System.Collections.Specialized;
using System.Web;

namespace Znode.Multifront.PaymentApplication.Helpers
{
    public class HelperMethods
    {
        /// <summary>
        /// Format Credit Card Number According to CreditCardFormat Key in Webconfig
        /// </summary>
        /// <param name="CreditCardLastFourDigit">string CreditCardLastFourDigit</param>
        /// <returns>Formated Credit Card Number</returns>
        public static string FormatCreditCardNumber(string CreditCardLastFourDigit)
            => $"{Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["CreditCardFormat"])}{CreditCardLastFourDigit}";

        /// <summary>
        /// Set paging parameters 
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="pagingStart">pagingStart</param>
        /// <param name="pagingLength">pagingLength</param>
        public static void SetPaging(NameValueCollection page, out int pagingStart, out int pagingLength)
        {
            // We use int.MaxValue for the paging length to ensure we always get total results back
            pagingStart = 1;
            pagingLength = int.MaxValue;
            if (!Equals(page, null) && page.HasKeys())
            {
                // Only do if both index and size are given
                if (!string.IsNullOrEmpty(page.Get(PageKeys.Index)) && !string.IsNullOrEmpty(page.Get(PageKeys.Size)))
                {
                    pagingStart = Convert.ToInt32(page.Get(PageKeys.Index));
                    pagingLength = Convert.ToInt32(page.Get(PageKeys.Size));
                }
            }
        }
        /// <summary>
        /// Replaces the "client_ip" header in current HttpRequest object with the "x-forwarded-for" header value
        /// </summary>
        public static void ReplaceProxyToClientIp()
        {
            try
            {
                // If a request is coming from a proxy server, "client_ip" header will have "proxy server's ip", in such case, replace it with actual IP received in "x-forwarded-for" header.
                var fwdIp = HttpContext.Current.Request.Headers["x-forwarded-for"];
                if (fwdIp != null)
                {
                    HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] = fwdIp;
                }
            }
            catch
            {
                // Do not Throw any exception or  add a logs here 
            }
        }
    }
}
