using System.Configuration;
using System.Globalization;
namespace System
{
    public static class StringUtil
    {
        /// <summary>
        /// Function will convert string to ProperCase
        /// </summary>
        /// <param name="source">input string</param>
        /// <returns>Returns the string converted to proper case</returns>
        /// <example>
        /// Input String : firST lAst
        /// Output String : First Last
        /// </example>
        public static string ToProperCase(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source);
        }

        /// <summary>
        /// Function will convert string to integer
        /// </summary>
        /// <param name="source">input string</param>
        /// <returns>Returns the integer</returns>
        /// <example>
        /// Input String : "45"
        /// Output Integer : 45
        /// </example>
        public static int ToInteger(this string source)
        {
            int output;
            if (string.IsNullOrEmpty(source))
                return -1;

            Int32.TryParse(source, out output);
            return output;
        }

        /// <summary>
        /// Method to check whether the provide string is a Number or not.
        /// </summary>
        /// <param name="source">Referenced String Object.</param>
        /// <returns>Return true or false.</returns>
        public static bool IsNumeric(this string source)
        {
            double isNumber;
            return double.TryParse(source, out isNumber);
        }

        /// <summary>
        /// Function will get the value from AppSettings by given key.
        /// </summary>
        /// <param name="source">input string</param>
        /// <returns>Returns the corresponding AppSettings Value</returns>        
        public static string ToGetValueFromAppSettings(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            return ConfigurationManager.AppSettings.Get(source);
        }

        /// <summary>
        /// Convert to DateTime from String DateTime.
        /// </summary>
        /// <param name="textDate">DateTime in string format.</param>
        /// <example> 
        ///    This sample shows how to use ToDateFromText method.
        /// <code>
        ///   string dateTimeInString ="2016-02-16 14:35:16.750";
        ///   DateTime? dt = dateTimeInString.ToDateFromText();
        /// </code>
        /// </example>
        /// <returns>Return the DateTime.</returns>
        public static DateTime? ToDateFromText(this string textDate)
        {
            if (!string.IsNullOrEmpty(textDate))
            {
                DateTime convertedDate;
                DateTime.TryParse(textDate, out convertedDate);
                return (!Equals(convertedDate, DateTime.MinValue)) ? (DateTime?)convertedDate : null;
            }
            return null;
        }

        /// <summary>
        /// Convert to UTC DateTime Format from String DateTime.
        /// </summary>
        /// <param name="textDate">DateTime in string format.</param>
        /// <param name="timeZone">User defined TimeZone.</param>
        /// <example> 
        ///    This sample shows how to use ToUTCDateFromText method.
        /// <code>
        ///   string dateTimeInString ="2016-02-16 14:35:16.750";
        ///   DateTime? dt = dateTimeInString.ToUTCDateFromText();
        ///   DateTime? dt = dateTimeInString.ToUTCDateFromText("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return DateTime in UTC Format.</returns>
        public static DateTime? ToUTCDateFromText(this string textDate, string timeZone = "")
        {
            if (!string.IsNullOrEmpty(textDate))
            {
                DateTime convertedDate;
                DateTime.TryParse(textDate, out convertedDate);
                return (!Equals(convertedDate, DateTime.MinValue)) ? (DateTime?)DateTimeUtils.ToUtcDateTime(convertedDate, timeZone) : null;
            }
            return null;
        }

        /// <summary>
        /// Convert to UTC DateTime Format with Time from String DateTime.
        /// </summary>
        /// <param name="textDate">DateTime in string format.</param>
        /// <param name="timeZone">User defined TimeZone.</param>
        /// <example> 
        ///    This sample shows how to use ToUTCDateWithTimeFromText method.
        /// <code>
        ///   string dateTimeInString ="2016-02-16 14:35:16.750";
        ///   DateTime? dt = dateTimeInString.ToUTCDateWithTimeFromText();
        ///   DateTime? dt = dateTimeInString.ToUTCDateWithTimeFromText("India Standard Time");
        /// </code>
        /// </example>
        /// <returns>Return DateTime? in UTC Format with Time.</returns>
        public static DateTime? ToUTCDateWithTimeFromText(this string textDate, string timeZone = "")
        {
            if (!string.IsNullOrEmpty(textDate))
            {
                DateTime convertedDate;
                DateTime.TryParse(textDate, out convertedDate);
                if (!Equals(convertedDate, DateTime.MinValue))
                {
                    DateTimeUtils.AddSiteConfigTimeInDate(convertedDate, timeZone);
                    return (DateTime?)DateTimeUtils.ToUtcDateTime(convertedDate);
                }
                else
                    return null;
            }
            return null;
        }
    }
}
