
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Helpers
{
    public static class DateRangePickerHelper
    {

        #region Public Methods
        //Format filters containing date time range.
        public static void FormatFilterForDateTimeRange(FilterCollection filters, string defaultFilter, string emptyValuefilter)
        {
            if (string.IsNullOrEmpty(HelperMethods.GetDateTimeRangeFilterTuple(filters)?.FilterValue))
                AddDateTimeRangeInFilter(filters, defaultFilter);

            //Get date time range filter in filter tuple.
            string dateTimeRangeValue = HelperMethods.GetDateTimeRangeFilterTuple(filters)?.FilterValue;
            ZnodeLogging.LogMessage("Date time range value:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { dateTimeRangeValue = dateTimeRangeValue });

            //Remove date time range filter from filter collection.
            RemoveDateTimeRangeFilter(filters);

            //Update date time range in filters if _dateTimeRange is not null.
            if (!string.IsNullOrEmpty(dateTimeRangeValue))
            {
                if (Enum.GetNames(typeof(DateTimeRange)).ToList().Contains(dateTimeRangeValue) && dateTimeRangeValue != emptyValuefilter)
                    AddDateTimeRangeInFilter(filters, CalculateDateTimeRange(dateTimeRangeValue));
                else if (dateTimeRangeValue != emptyValuefilter)
                    SetCustomDateRange(filters, dateTimeRangeValue);

                ZnodeLogging.LogMessage("Updated filter value:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            }
        }

        //Remove date time range filter from session.
        public static void RemoveDateTimeRangeFiltersFromCookies(string listType, FilterCollectionDataModel model)
        {
            if (model.Filters?.Count == 0 && SessionHelper.GetDataFromSession<string>(DynamicGridConstants.listTypeSessionKey) != listType && CookieHelper.IsCookieExists(listType))
            {                
                NameValueCollection cookieFilter = CookieHelper.GetCookieValues(listType);
                if (cookieFilter?.Count > 0)
                {
                    FilterCollection filterCollection = Newtonsoft.Json.JsonConvert.DeserializeObject<FilterCollection>(cookieFilter[0]);
                    if (filterCollection?.Exists(x => x.Item1.ToLower() == AdminConstants.DateTimeRange.ToLower()) == true)
                    {
                        filterCollection.RemoveAll(x => string.Equals(x.FilterName, AdminConstants.DateTimeRange, StringComparison.InvariantCultureIgnoreCase));
                        string filters = new JavaScriptSerializer().Serialize(filterCollection);
                        CookieHelper.SetCookie(listType, filters, (ZnodeConstant.MinutesInAHour * Convert.ToDouble(ZnodeAdminSettings.CookieExpiresValueForFilter)));
                    }                       
                }
            }
        }

        #endregion

        #region Private Methods

        //Calculate date time range for date time range codes       
        private static string CalculateDateTimeRange(string dateTimeRangeValue)
        {
            int numberOfHours = (int)Enum.Parse(typeof(DateTimeRange), dateTimeRangeValue);
            DateTime currentDateTime = HelperUtility.GetDateWithTime();
            return ("'" + currentDateTime.AddHours(-numberOfHours).ToString(HelperMethods.GetStringDateTimeFormat()) + "' AND '" + currentDateTime.ToString(HelperMethods.GetStringDateTimeFormat()) + "'");

        }

        //Set custom range filter value
        private static void SetCustomDateRange(FilterCollection filters, string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Contains("to"))
            {
                string[] dateTimeRangeValue = value?.Split(new string[] { "to" }, StringSplitOptions.None);
                value = "'" + dateTimeRangeValue[0]?.Trim() + "' AND '" + dateTimeRangeValue[1]?.Trim() + "'";
                AddDateTimeRangeInFilter(filters, value);
            }
        }

        //Remove date time range filter from filter collection
        private static void RemoveDateTimeRangeFilter(FilterCollection filters) =>
            filters.RemoveAll(x => string.Equals(x.FilterName, AdminConstants.DateTimeRange, StringComparison.InvariantCultureIgnoreCase));

        //Add date time range filter in filter collection
        private static void AddDateTimeRangeInFilter(FilterCollection filters, string dateTimeRangeValue) =>
            filters.Add(new FilterTuple(AdminConstants.DateTimeRange, FilterOperators.Between, dateTimeRangeValue));
        #endregion         
    }
}
