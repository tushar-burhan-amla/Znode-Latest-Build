using System.Diagnostics;
using System.Text;
using System.Web;
using Znode.Multifront.PaymentApplication.Helpers;

namespace Znode.Multifront.PaymentApplication.Data
{
    public static class ActivityLogService
    {
        /// <summary>
        /// Adds an entry in the ZnodeActivityLog table.
        /// </summary>
        /// <param name="ActivityLogId">The ActivityLogID from the ZnodeActivityLog table</param>
        /// <param name="PaymentSettingId">Payment setting id.\</param>
        /// <param name="Data1">Custom data to log</param>
        /// <param name="Data2">Custom data to log</param>
        /// <param name="Data3">Custom data to log</param>
        /// <param name="Status">Status message for this activity</param>
        /// <param name="LongData">Custom data to log. Can be up to 4000 characters.</param>
        public static int InsertActivityLog(int? paymentSettingId, string Data1, string Data2, string Data3, string Status, string LongData)
        => InsertActivityLog(paymentSettingId, Data1, Data2, Data3, Status, LongData, string.Empty, string.Empty);

        /// <summary>
        /// Adds an entry in the ZnodeActivityLog table.
        /// </summary>
        /// <param name="ActivityLogId">The ActivityLogID from the ZnodeActivityLog table</param>
        /// <param name="PaymentSettingId">Payment setting id.\</param>
        /// <param name="Data1">Custom data to log</param>
        /// <param name="Data2">Custom data to log</param>
        /// <param name="Data3">Custom data to log</param>
        /// <param name="Status">Status message for this activity</param>
        /// <param name="LongData">Custom data to log. Can be up to 4000 characters.</param>
        /// <param name="Source">Source</param>
        /// <param name="Target">Target</param>
        /// <returns>Log Id</returns>
        public static int InsertActivityLog(int? paymentSettingId, string Data1, string Data2, string Data3, string Status, string LongData, string Source, string Target)
        {
            int activityLogId = -1;

            try
            {
                // Do a quick check to see if Activity logging has been disabled.
                if (DatabaseLoggingEnabled())
                {
                    ZnodePaymentRepository<ZNodeActivityLog> activityLogRepository = new ZnodePaymentRepository<ZNodeActivityLog>();
                    ZNodeActivityLog znodeActivityLog = new ZNodeActivityLog();
                    znodeActivityLog.PaymentSettingId = paymentSettingId;
                    znodeActivityLog.Data1 = Data1;
                    znodeActivityLog.Data2 = Data2;
                    znodeActivityLog.Data3 = Data3;
                    znodeActivityLog.Status = Status;
                    znodeActivityLog.LongData = LongData;
                    znodeActivityLog.Source = Source;
                    znodeActivityLog.Target = Target;
                    znodeActivityLog.URL = HttpContext.Current.Request.Url.PathAndQuery;

                    activityLogRepository.Insert(znodeActivityLog);
                    activityLogId = znodeActivityLog.ActivityLogId;
                }
            }
            catch
            {
                StringBuilder sb = new StringBuilder("Error logging message to activity table in LogActivity. ");
                sb.Append("/nData1: " + Data1);
                sb.Append("/nData2: " + Data2);
                sb.Append("/nData3: " + Data3);
                sb.Append("/nStatus: " + Status);
                sb.Append("/nLongData: " + LongData);
                sb.Append("/nSource: " + Source);
                sb.Append("/nTarget: " + Target);
                Logging.LogMessage(sb.ToString(), Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            return activityLogId;
        }

        public static bool DatabaseLoggingEnabled()
        {
            bool enableLogging = false;
            string configSetting = System.Configuration.ConfigurationManager.AppSettings["EnableLogging"];

            if (Equals(configSetting, null))
                return enableLogging;

            configSetting = configSetting.Trim();

            // Even if the entry is not in the web.config we are going to default this value to on.           
            if (configSetting.Equals("1"))
                enableLogging = true;

            return enableLogging;
        }
    }
}