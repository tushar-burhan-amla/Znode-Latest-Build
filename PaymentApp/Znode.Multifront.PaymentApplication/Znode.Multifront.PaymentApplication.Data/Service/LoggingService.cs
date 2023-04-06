namespace Znode.Multifront.PaymentApplication.Data
{
    public class LoggingService
    {
        /// <summary>
        /// Log an activity to the database.
        /// </summary>
        /// <param name="PaymentSettingId">Payment setting id.\</param>
        public static void LogActivity(int? paymentSettingId)
            => LogActivity(paymentSettingId, null, null, null, null, null);

        /// <summary>
        /// Log an activity to the database.
        /// </summary>
        /// <param name="PaymentSettingId">Payment setting id.\</param>
        /// <param name="Data1">Custom data to log</param>
        public static void LogActivity(int? paymentSettingId, string Data)
            => LogActivity(paymentSettingId, Data, null, null, null, null);

        /// <summary>
        /// Log an activity to the database.
        /// </summary>
        /// <param name="PaymentSettingId">Payment setting id.\</param>
        /// <param name="Data1">Custom data to log</param>
        /// <param name="Status">Status message for this activity</param>
        public static void LogActivity(int? paymentSettingId, string Data, string Status)
            => LogActivity(paymentSettingId, Data, null, null, Status, null);

        /// <summary>
        /// Log an activity to the database.
        /// </summary>
        /// <param name="PaymentSettingId">Payment setting id.\</param>
        /// <param name="Data1">Custom data to log</param>
        /// <param name="Data2">Custom data to log</param>
        /// <param name="Data3">Custom data to log</param>
        /// <param name="Status">Status message for this activity</param>
        /// <param name="LongData">Custom data to log. Can be up to 4000 characters.</param>
        public static void LogActivity(int? paymentSettingId, string Data1, string Data2, string Data3, string Status, string LongData)
            => ActivityLogService.InsertActivityLog(paymentSettingId, Data1, Data2, Data3, Status, LongData);

        /// <summary>
        /// Log an activity to the database.
        /// </summary>
        /// <param name="PaymentSettingId">Payment Setting id</param>
        /// <param name="Data1">custom data to log</param>
        /// <param name="Data2">custom data to log</param>
        /// <param name="Data3">custom data to log</param>
        /// <param name="Status">status message for this activity</param>
        /// <param name="LongData">Custom data to log, can be up to 4000 characters</param>
        /// <param name="Source">source</param>
        /// <param name="Target">target</param>
        public static void LogActivity(int? paymentSettingId, string Data1, string Data2, string Data3, string Status, string LongData, string Source, string Target)
            => ActivityLogService.InsertActivityLog(paymentSettingId, Data1, Data2, Data3, Status, LongData, Source, Target);
    }
}
