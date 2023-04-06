using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public static class PaymentConstant
    {
        #region card connect respnce
        //These are the responses returned by CardConnect
        public const string ApproveResponseCode = "A";
        public const string RetryResponseCode = "B";
        public const string DeclineResponseCode = "C";
        public const string CyberSource = "cybersource";
        public const string AuthorizeNet = "authorizenet";
        #endregion
    }
}
