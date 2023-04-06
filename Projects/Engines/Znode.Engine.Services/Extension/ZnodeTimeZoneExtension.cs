using System;


namespace Znode.Engine.Services
{
    public static class ZnodeTimeZoneExtension
    {
        //Convert the Date format to UTC format in DefaultGlobalConfigSetting
        public static DateTime ToUtcTimeZoneFormat(this DateTime? dateTime)
        {
            DateTime dt;
            DateTime.TryParse(Convert.ToString(dateTime), out dt);
            return dt.ToSiteConfigUtcDateTime(DefaultGlobalConfigSettingHelper.DefaultTimeZone);
        }

        //Convert the Date format to UTC format in DefaultGlobalConfigSetting
        public static DateTime ToUtcTimeZoneFormat(this DateTime dateTime)
        {
            DateTime dt;
            DateTime.TryParse(Convert.ToString(dateTime), out dt);
            return dt.ToSiteConfigUtcDateTime(DefaultGlobalConfigSettingHelper.DefaultTimeZone);
        }

        //Convert the Date format to locale format in DefaultGlobalConfigSetting
        public static DateTime ToLocaleTimeZoneFormat(this DateTime dateTime)
        {
            DateTime dt;
            DateTime.TryParse(Convert.ToString(dateTime), out dt);
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dt.ToSiteConfigLocaleDateTime(DefaultGlobalConfigSettingHelper.DefaultTimeZone);
        }

        //Convert the Date format to locale format in DefaultGlobalConfigSetting
        public static DateTime ToLocaleTimeZoneFormat(this DateTime? dateTime)
        {
            DateTime dt;
            DateTime.TryParse(Convert.ToString(dateTime), out dt);
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dt.ToSiteConfigLocaleDateTime(DefaultGlobalConfigSettingHelper.DefaultTimeZone);
        }
    }
}
