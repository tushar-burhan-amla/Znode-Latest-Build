using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
namespace Znode.Multifront.PaymentApplication.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Routes.MapHttpRoute("script-znodeapijs", "script/znodeapijs", new { controller = "script", action = "znodeapijs" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("script-znodeapiACHjs", "script/znodeapijsforach", new { controller = "script", action = "znodeapijsforach" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-getgateways", "payment/getgateways", new { controller = "payment", action = "getgateways" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getachgateways", "payment/getachgateways", new { controller = "payment", action = "getachgateways" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("payment-paynow", "payment/paynow", new { controller = "payment", action = "paynow" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-paypal", "payment/paypal", new { controller = "payment", action = "paypal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-paypalexpress", "payment/paypalexpress", new { controller = "payment", action = "paypalexpress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-finalizepaypal", "payment/finalizepaypalprocess", new { controller = "payment", action = "finalizepaypalprocess" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Amazon Pay
            config.Routes.MapHttpRoute("payment-amazonpay", "payment/amazonpay", new { controller = "payment", action = "amazonpay" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-amazonaddress", "payment/getamazonpayaddress", new { controller = "payment", action = "getamazonpayaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-amazonpayrefund", "payment/amazonpayrefund", new { controller = "payment", action = "amazonrefund" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-amazonvoid", "payment/amazonvoid/{token}", new { controller = "payment", action = "amazonvoid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-amazonpaycapture", "payment/amazonpaycapture/{token}", new { controller = "payment", action = "amazonpaycapture" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("payment-updatepaymentsettings", "payment/updatepaymentsettings", new { controller = "payment", action = "updatepaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("payment-addpaymentsettings", "payment/addpaymentsettings", new { controller = "payment", action = "addpaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-capture", "payment/capture/{token}", new { controller = "payment", action = "capture" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-void", "payment/void/{token}", new { controller = "payment", action = "void" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-refund", "payment/refund", new { controller = "payment", action = "refund" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-getpaymentsettings", "payment/getpaymentsettings", new { controller = "payment", action = "getpaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-deletepaymentsettings", "payment/delete", new { controller = "payment", action = "deletepaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-getpaymentsettingdetails", "payment/getpaymentsettingdetails/{paymentsettingid}", new { controller = "payment", action = "getpaymentsettingdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getpaymentsettingcredentials", "payment/getpaymentsettingcredentials/{paymentSettingId}/{isTestMode}", new { controller = "payment", action = "getpaymentsettingcredentials" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getpaymenttypes", "payment/getpaymenttypes", new { controller = "payment", action = "getpaymenttypes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getpaymenttransaction", "payment/getpaymenttransaction/{transactionId}", new { controller = "payment", action = "getpaymenttransaction" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getpaymentsettingbypaymentcode", "payment/getpaymentsettingbypaymentcode/{paymentCode}", new { controller = "payment", action = "getpaymentsettingbypaymentcode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-deletepaymentsettingbypaymentcode", "payment/deletepaymentsettingbypaymentcode", new { controller = "payment", action = "deletepaymentsettingbypaymentcode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-getpaymentsettingcredentialsbypaymentcode", "payment/getpaymentsettingcredentialsbypaymentcode/{paymentCode}/{isTestMode}", new { controller = "payment", action = "getpaymentsettingcredentialsbypaymentcode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getsavedcarddetailsbycustomerguid", "payment/getsavedcarddetailsbycustomerguid/{customersGUID}", new { controller = "payment", action = "getsavedcarddetailsbycustomerguid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getpaymentcreditcarddetails", "payment/GetPaymentCreditCardDetails/{paymentSettingId}/{customersGUID}", new { controller = "payment", action = "getpaymentcreditcarddetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getSavedCreditCardCount", "payment/GetPaymentCreditCardCount/{paymentSettingId}/{customerGUID}", new { controller = "payment", action = "GetPaymentCreditCardCount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-deleteSavedCreditCard", "payment/DeleteSavedCreditCardDetail/{paymentGUID}", new { controller = "payment", action = "DeleteSavedCreditCardDetail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-gettransactiondetails", "payment/gettransactionstatusdetails/{transactionId}", new { controller = "payment", action = "GetTransactionStatusDetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-getrefundtransactionid", "payment/getrefundtransactionId/{transactionId}", new { controller = "payment", action = "GetRefundTransactionId" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("payment-getauthtoken", "authtoken/paymentaccesstokengenerator/{userOrSessionId}/{fromAdminApp}", new { controller = "authtoken", action = "generatetoken" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("payment-deleteauthtoken", "authtoken/deleteexpiredpaymentaccesstoken", new { controller = "authtoken", action = "deleteexpiredauthtoken" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("payment-generategatewaytoken", "payment/generategatewaytoken", new { controller = "Payment", action = "generategatewaytoken" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
        }
    }
}
