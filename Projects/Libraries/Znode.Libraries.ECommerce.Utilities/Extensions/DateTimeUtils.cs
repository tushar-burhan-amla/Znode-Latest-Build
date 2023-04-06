
namespace System
{
    public static class DateTimeUtils
    {
        #region Public Methods

        /// <summary>
        /// Convert DateTime to Local/User defined Time Zone DateTime.
        /// </summary> 
        /// <param name="dateTimeFromDatabase">DateTime Format.</param>
        /// <param name="timeZone">User defined TimeZone</param>
        /// <example> 
        ///    This sample shows how to use ToSiteConfigDateTime method.
        /// <code>
        ///   DateTime dt = DateTime.UtcNow.ToSiteConfigDateTime();
        ///   DateTime dt = DateTime.UtcNow.ToSiteConfigDateTime("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return DateTime in user defined Time zone format.</returns>
        public static DateTime ToSiteConfigDateTime(this DateTime dateTimeFromDatabase, string timeZone = "")
        {
            if (!Equals(dateTimeFromDatabase, DateTime.MinValue))
            {
                TimeZoneInfo userTimeZone = GetTimeZoneInformation(timeZone);
                return (Equals(dateTimeFromDatabase.Kind, DateTimeKind.Local))
                 ? TimeZoneInfo.ConvertTime(dateTimeFromDatabase, TimeZoneInfo.Local, userTimeZone)
                 : TimeZoneInfo.ConvertTimeFromUtc(dateTimeFromDatabase, userTimeZone);
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// Convert DateTime to UTC format as defined TimeZone DateTime.
        /// </summary> 
        /// <param name="dateTimeFromDatabase">DateTime Format.</param>
        /// <param name="timeZone">User defined TimeZone</param>
        /// <example> 
        ///    This sample shows how to use ToSiteConfigUtcDateTime method.
        /// <code>
        ///   DateTime dt = DateTime.ToSiteConfigUtcDateTime();
        ///   DateTime dt = DateTime.ToSiteConfigUtcDateTime("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return UTC DateTime in user defined Time zone format.</returns>
        public static DateTime ToSiteConfigUtcDateTime(this DateTime dateTimeFromDatabase, string timeZone = "")
              => Equals(dateTimeFromDatabase, DateTime.MinValue) ? DateTime.MinValue : TimeZoneInfo.ConvertTimeToUtc(dateTimeFromDatabase, GetTimeZoneInformation(timeZone));

        /// <summary>
        /// Convert DateTime to Locale format as defined TimeZone DateTime.
        /// </summary> 
        /// <param name="dateTimeFromDatabase">DateTime Format.</param>
        /// <param name="timeZone">User defined TimeZone</param>
        /// <example> 
        ///    This sample shows how to use ToSiteConfigLocaleDateTime method.
        /// <code>
        ///   DateTime dt = DateTime.ToSiteConfigLocaleDateTime();
        ///   DateTime dt = DateTime.ToSiteConfigLocaleDateTime("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return Locale DateTime in user defined Time zone format.</returns>
        public static DateTime ToSiteConfigLocaleDateTime(this DateTime dateTimeFromDatabase, string timeZone = "")
          => Equals(dateTimeFromDatabase, DateTime.MinValue) ? DateTime.MinValue : TimeZoneInfo.ConvertTimeFromUtc(dateTimeFromDatabase, GetTimeZoneInformation(timeZone));

        /// <summary>
        /// Convert DateTime to UTC Time Format based on Local/User defined Time Zone.
        /// </summary>
        /// <param name="dateTimeFromUser">DateTime Format</param>
        /// <param name="timeZone">User defined TimeZone</param>
        /// <example> 
        ///    This sample shows how to use ToUtcDateTime method.
        /// <code>
        ///   DateTime dt = DateTime.UtcNow.ToUtcDateTime();
        ///   DateTime dt = DateTime.UtcNow.ToUtcDateTime("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return UTC Time Format based on Local/User defined Time Zone</returns>
        public static DateTime ToUtcDateTime(this DateTime dateTimeFromUser, string timeZone = "")
        {
            if (!Equals(dateTimeFromUser, DateTime.MinValue))
            {
                TimeZoneInfo userTimeZone = GetTimeZoneInformation(timeZone);
                return (Equals(dateTimeFromUser.Kind, DateTimeKind.Local))
                      ? TimeZoneInfo.ConvertTimeToUtc(dateTimeFromUser)
                      : TimeZoneInfo.ConvertTimeToUtc(dateTimeFromUser, userTimeZone);
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Get the Local/User Defined Time Zone Name.
        /// </summary>
        /// <param name="timeZone">User defined TimeZone.</param>
        /// <example> 
        ///    This sample shows how to use GetSiteConfigTimeZoneDisplayName method.
        /// <code>
        ///   string timeZoneName = GetSiteConfigTimeZoneDisplayName();
        ///   string timeZoneName = GetSiteConfigTimeZoneDisplayName("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return the Local/User Defined Time Zone Name.</returns>
        public static string GetSiteConfigTimeZoneDisplayName(string timeZone = "")
            => GetTimeZoneInformation(timeZone).DisplayName;

        /// <summary>
        /// Get the Local/User defined Time Zone DateTime.
        /// </summary>
        /// <param name="timeZone">User defined TimeZone.</param>
        /// <example> 
        ///    This sample shows how to use GetSiteConfigDateTimeNow method.
        /// <code>
        ///   DateTime dt = GetSiteConfigDateTimeNow();
        ///   DateTime dt = GetSiteConfigDateTimeNow("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return the Local/User defined Time Zone DateTime.</returns>
        public static DateTime GetSiteConfigDateTimeNow(string timeZone = "")
            => ToSiteConfigDateTime(DateTime.UtcNow, timeZone);

        /// <summary>
        /// Add Time Details within the DateTime Object.
        /// </summary>
        /// <param name="convertedDate">Referenced DateTime Object.</param>
        /// <param name="timeZone">User defined TimeZone.</param>
        /// <example> 
        ///    This sample shows how to use AddSiteConfigTimeInDate method.
        /// <code>
        ///   DateTime dt = DateTime.Now.AddSiteConfigTimeInDate();
        ///   DateTime dt = DateTime.Now.AddSiteConfigTimeInDate("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return Referenced DateTime Object.</returns>
        public static DateTime AddSiteConfigTimeInDate(this DateTime convertedDate, string timeZone = "")
        {
            convertedDate = convertedDate.AddHours(GetSiteConfigDateTimeNow(timeZone).Hour);
            convertedDate = convertedDate.AddMinutes(GetSiteConfigDateTimeNow(timeZone).Minute);
            return convertedDate;
        }

        /// <summary>
        /// Get Next Day DateTime Information.
        /// </summary>
        /// <param name="dateTimeA">Referenced DateTime Object.</param>
        /// <example> 
        ///    This sample shows how to use GetTomorrow method.
        /// <code>
        ///   DateTime dt = GetToday().GetTomorrow();
        ///   DateTime dt = GetToday("India Standard Time").GetTomorrow();
        /// </code>
        /// </example>
        /// <returns>Return Next Day DateTime Information.</returns>
        public static DateTime GetTomorrow(this DateTime dateTimeA)
            => dateTimeA.AddDays(1);

        /// <summary>
        /// Get Previous Day DateTime Information in UTC.
        /// </summary>
        /// <param name="dateTimeA">Referenced DateTime Object.</param>
        /// <example> 
        ///    This sample shows how to use GetYesterday method.
        /// <code>
        ///   DateTime dt = GetToday().GetYesterday();
        ///   DateTime dt = GetToday("India Standard Time").GetYesterday();
        /// </code>
        /// </example>
        /// <returns>Return Previous Day DateTime Information.</returns>
        public static DateTime GetYesterday(this DateTime dateTimeA)
            => dateTimeA.AddDays(-1);

        /// <summary>
        /// Get Current Day DateTime Information in UTC.
        /// </summary>
        /// <param name="timeZone">User defined TimeZone.</param>
        /// <example> 
        ///    This sample shows how to use GetToday method.
        /// <code>
        ///   DateTime dt = GetToday();
        ///   DateTime dt = GetToday("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return Current Day DateTime Information.</returns>
        public static DateTime GetToday(string timeZone = "")
        {
            DateTime userCurrentDate = GetSiteConfigDateTimeNow(timeZone);
            return ToUtcDateTime(new DateTime(userCurrentDate.Year, userCurrentDate.Month, userCurrentDate.Day, 00, 00, 00, 00));
        }

        /// <summary>
        /// Get Last Seventh Day DateTime Information in UTC.
        /// </summary>
        /// <param name="dateTimeA">Referenced DateTime Object.</param>
        /// <example> 
        ///    This sample shows how to use GetLast7Days method.
        /// <code>
        ///   DateTime dt = GetToday().GetLast7Days();
        ///   DateTime dt = GetToday("India Standard Time").GetLast7Days();
        /// </code>
        /// </example>
        /// <returns>Return Last Seventh Day DateTime Information.</returns>
        public static DateTime GetLast7Days(this DateTime dateTimeA)
            => dateTimeA.AddDays(-7);

        /// <summary>
        /// Get Last thirtieth Day DateTime Information in UTC.
        /// </summary>
        /// <param name="dateTimeA">Referenced DateTime Object.</param>
        /// <example> 
        ///    This sample shows how to use GetLast30Days method.
        /// <code>
        ///   DateTime dt = GetToday().GetLast30Days();
        ///   DateTime dt = GetToday("India Standard Time").GetLast30Days();
        /// </code>
        /// </example>
        /// <returns>Return Last thirtieth Day DateTime Information.</returns>
        public static DateTime GetLast30Days(this DateTime dateTimeA)
            => dateTimeA.AddDays(-30);

        /// <summary>
        /// Get First Day of Current Month DateTime Information.
        /// </summary>
        /// <param name="dateTimeA">Referenced DateTime Object.</param>
        /// <example> 
        ///    This sample shows how to use GetFirstDayofMonth method.
        /// <code>
        ///   DateTime dt = GetToday().GetFirstDayofMonth();
        ///   DateTime dt = GetToday("India Standard Time").GetFirstDayofMonth();
        /// </code>
        /// </example>
        /// <returns>Return First Day of Current Month DateTime Information.</returns>
        public static DateTime GetFirstDayofMonth(this DateTime dateTimeA)
            => new DateTime(dateTimeA.Year, dateTimeA.Month, 1);


        /// <summary>
        /// Get Last Day of Current Month DateTime Information.
        /// </summary>
        /// <param name="dateTimeA">Referenced DateTime Object.</param>
        /// <example> 
        ///    This sample shows how to use GetLastDayofMonth method.
        /// <code>
        ///   DateTime dt = GetToday().GetLastDayofMonth();
        ///   DateTime dt = GetToday("India Standard Time").GetLastDayofMonth();
        /// </code>
        /// </example>
        /// <returns>Return Last Day of Current Month DateTime Information.</returns>
        public static DateTime GetLastDayofMonth(this DateTime dateTimeA)
        {
            dateTimeA = dateTimeA.AddMonths(1);
            return new DateTime(dateTimeA.Year, dateTimeA.Month, 1).AddDays(-1);
        }

        /// <summary>
        /// Compare the DateTime objects & Retuns the Small Date Time.
        /// </summary>
        /// <param name="date1">DateTime Object.</param>
        /// <param name="date2">DateTime Object.</param>
        /// <returns>Retuns the Small Date Time.</returns>
        public static DateTime GetSmallerDate(DateTime date1, DateTime date2)
            => Equals(DateTime.Compare(date1, date2), 1) ? date1 : date2;

        /// <summary>
        /// Compare the DateTime objects & Retuns the Large Date Time.
        /// </summary>
        /// <param name="date1">DateTime Object.</param>
        /// <param name="date2">DateTime Object.</param>
        /// <returns>Retuns the Large Date Time.</returns>
        public static DateTime GetGreaterDate(DateTime date1, DateTime date2)
            => Equals(DateTime.Compare(date1, date2), -1) ? date1 : date2;

        /// <summary>
        /// Method to check whether the provide string is DateTime or not.
        /// </summary>
        /// <param name="textDate">Referenced String Object.</param>
        /// <returns>Return true or false.</returns>
        public static bool IsDateTime(this string textDate)
        {
            DateTime datetime;
            DateTime.TryParse(textDate, out datetime);
            return !Equals(datetime, DateTime.MinValue);
        }

        /// <summary>
        /// Equals Extensions to compare two date time 
        /// </summary>
        /// <param name="dateTimeA"></param>
        /// <param name="dateTimeB"></param>
        /// <returns>true if both datetime are equals</returns>
        public static bool Equals(this DateTime dateTimeA, DateTime dateTimeB)
            => DateTime.Compare(dateTimeA, dateTimeB) == 0;


        #endregion

        #region Private Methods

        /// <summary>
        /// Get the TimeZone Infomation based on user defined Time Zone.
        /// </summary>
        /// <param name="timeZone">User defined TimeZone.</param>
        /// <returns>Return the Time Zone Information.</returns>
        /// In Case of configuring the Default Time Zone, set it from Web.Config or Database.
        private static TimeZoneInfo GetTimeZoneInformation(string timeZone)
            => TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(timeZone) ? "Central Standard Time" : timeZone);
        #endregion
    }
}
