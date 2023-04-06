﻿using System;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.WebStore
{
    public static class WebStoreExtention
    {
        #region Date Format Extention
        //Convert the Date format to default format in DefaultGlobalConfigSetting
        public static string ToDateTimeFormat(this DateTime? dateTime)
            => IsNotNull(dateTime) ? GetDateTimeFormat(Convert.ToString(dateTime)) : dateTime.ToString();

        //Convert the Date format to default format in DefaultGlobalConfigSetting
        public static string ToDateTimeFormat(this DateTime dateTime)
            => GetDateTimeFormat(Convert.ToString(dateTime));

        //Convert the Date format to default format in DefaultGlobalConfigSetting
        public static string ToDateTimeFormat(this string dateTime)
       => !string.IsNullOrEmpty(dateTime)
                 ? GetDateTimeFormat(dateTime) : string.Empty;
        #endregion

        #region Display Unit
        //Convert the Size to default format in DefaultGlobalConfigSetting
        public static string ToDisplayUnitFormat(this string Size)
        {
            string DisplayUnit = DefaultSettingHelper.DefaultDisplayUnit;
            long size = Convert.ToInt64(Size);
            switch (DisplayUnit)
            {
                case "KB":
                    return Convert.ToString(Math.Round((size / 1024M), 2)) + Admin_Resources.KBUnit;
                case "MB":
                    return Convert.ToString(Math.Round((size / 1024000M), 2)) + Admin_Resources.MBUnit;
                default:
                    return Size;
            }
        }
        #endregion

        #region Price Round Off
        //Round off the decimal values.
        public static string ToPriceRoundOff(this decimal Size)
        => GetPriceRoundOff(Convert.ToString(Size));

        //Round off the nullable decimal values.
        public static string ToPriceRoundOff(this decimal? Size)
            => IsNotNull(Size) ? GetPriceRoundOff(Convert.ToString(Size)) : Size.ToString();

        //Round off the string values.
        public static string ToPriceRoundOff(string Size)
        => !string.IsNullOrEmpty(Size) ? GetPriceRoundOff(Size) : string.Empty;
        #endregion

        #region Inventory Round Off
        //Round off the decimal values.
        public static string ToInventoryRoundOff(this decimal Size)
        => GetInventoryRoundOff(Convert.ToString(Size));

        //Round off the nullable decimal values.
        public static string ToInventoryRoundOff(this decimal? Size)
         => IsNotNull(Size) ? GetInventoryRoundOff(Convert.ToString(Size)) : Size.ToString();


        //Round off the string values.
        public static string ToInventoryRoundOff(this string Size)
        => !string.IsNullOrEmpty(Size) ? GetInventoryRoundOff(Size) : string.Empty;

        #endregion

        #region Private Methods

        //Get Date format to default format in DefaultGlobalConfigSetting
        private static string GetDateTimeFormat(string dateTime)
        {
            DateTime dt;
            DateTime.TryParse(Convert.ToString(dateTime), out dt);
            return dt.ToString(DefaultSettingHelper.DefaultDateFormat);
        }

        //Get Round off value for price.
        private static string GetPriceRoundOff(string Size)
        {
            string roundOff = DefaultSettingHelper.DefaultPriceRoundOff;
            decimal size = Convert.ToDecimal(Size);
            return Convert.ToString(Math.Round((size), Convert.ToInt32(roundOff), MidpointRounding.AwayFromZero));
        }
        //Get Round off value for inventory.
        private static string GetInventoryRoundOff(string Size)
        {
            string roundOff = DefaultSettingHelper.DefaultInventoryRoundOff;
            decimal size = Convert.ToDecimal(Size);
            return Convert.ToString(Math.Round((size), Convert.ToInt32(roundOff), MidpointRounding.AwayFromZero));
        }
        #endregion
    }
}

