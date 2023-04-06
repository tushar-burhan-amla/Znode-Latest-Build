using Newtonsoft.Json;
using Znode.Engine.Promotions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api
{
    public static class ApiHelper
    {
        //Caches all active promotions in the application cache.
        public static void CacheActivePromotions() => ZnodePromotionManager.CacheActivePromotions();

        //This method respects the http header value to optimize JSON response from APIs
        //Sets the Newtonsoft JSON conversion setting, to minify the serialized JSON to ignore null and default value properties.
        //So each API response which we are serializing using this method will use these settings.
        public static string ToJson(object data)
        {
            bool minifiedJsonResponse = ZnodeApiSettings.MinifiedJsonResponse;
            return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
            {
                DefaultValueHandling = minifiedJsonResponse ? DefaultValueHandling.Ignore : DefaultValueHandling.Include
            });
        }

        // Create Schedular to call the voucher expiration email API. 
        public static void CreateVoucherReminderEmailScheduler() => ServiceHelper.CreateVoucherReminderEmailScheduler();

        // Create Schedular to call the clear user registration attempt detail API. 
        public static void CreateClearExistingUserRegistrationAttemptScheduler() => ServiceHelper.CreateClearUserRegistrationAttemptScheduler();

        // Create Schedular to call delete payment token API. 
        public static void CreateDeletePaymentTokenScheduler() => ServiceHelper.CreateDeletePaymentAuthTokenScheduler();

        // Create Scheduler to send stock notification.
        public static void SendStockNotification() => ServiceHelper.StockNoticeNotification();

        // Create Schedular to call delete Export API. 
        public static void CreateDeleteExportFileScheduler() => ServiceHelper.CreateDeleteExportScheduler();
    }
}
