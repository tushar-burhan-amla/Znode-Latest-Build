using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore.Helpers
{
    public static class HelperMethods
    {
        #region Public Methods
        public static int GridPagingStartValue { get; } = string.IsNullOrEmpty(ZnodeAdminSettings.GridPagingStartValue) ? 10 : Convert.ToInt32(ZnodeAdminSettings.GridPagingStartValue);

        public static string DatePickDateFormat() => DefaultSettingHelper.DefaultDateFormat;

        public static string ConvertStringToDate(string date) => !string.IsNullOrEmpty(date) ? SearchDateFormat(GetDateTime(date)) : string.Empty;

        public static string GetStringDateFormat() => DefaultSettingHelper.DefaultDateFormat;

        public static string GetStringDateTimeFormat() => DefaultSettingHelper.DefaultDateFormat + " " + DefaultSettingHelper.DefaultTimeFormat;

        public static string GetStringTimeFormat() => DefaultSettingHelper.DefaultTimeFormat;

        public static string GetDefaultTimeZnode() => DefaultSettingHelper.DefaultTimeZone;

        public static string GetDomainUrl() => (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority))) ? HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty;

        public static DateTime GetDateTime(string dateTime)
        {
            DateTime date;
            DateTime.TryParse(dateTime, out date);
            return date;
        }

        public static DateTime GetDateTime() => DateTime.Now;

        /// <summary>
        /// To get Format Date as per date formate define in database
        /// </summary>
        /// <param name="date">DateTime date</param>
        /// <returns>>returns string FormatDate</returns>
        public static string SearchDateFormat(DateTime date)
        {
            //Get the default time zone set for it.
            string timeZone = GetDefaultTimeZnode();
            //convert the time to UTC before search as the time is in UTC.
            date = TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(timeZone) ? "Central Standard Time" : timeZone));
            return date.ToString(DynamicGridConstants.DateFormatOfDatabase);
        }

        /// <summary>
        /// Create class instance 
        /// </summary>
        /// <param name="className"> Name of class </param>
        /// <param name="classNameSpace">Namespace of class</param>
        /// <returns></returns>
        public static object CreateAttributeValidationControlInstance(string className, string classNameSpace) => Activator.CreateInstance(Type.GetType(classNameSpace + "." + className));

        /// <summary>
        /// Get the enable/disable checkmark icon
        /// </summary>
        /// <param name="isEnabled">Indicates to icon enabled or disabled status.</param>
        /// <returns>Returns enabled/disabled checkmark icon.</returns>
        public static string GetCheckMark(bool isEnabled)
        {
            string icon = string.Empty;

            if (HelperUtility.IsNotNull(isEnabled))
            {
                if (isEnabled)
                    icon = "z-ok";
                else
                    icon = "z-close";
            }
            return icon;
        }


        /// <summary>
        /// Get date format as per global setting.
        /// </summary>
        /// <returns>Returns date format.</returns>
        public static string GetDateFormat() => DefaultSettingHelper.DefaultDateFormat.Contains("MMM") ? DefaultSettingHelper.DefaultDateFormat.Replace("MMM", "M") : DefaultSettingHelper.DefaultDateFormat.Contains("MM") ? DefaultSettingHelper.DefaultDateFormat.Replace("MM", "mm") : DefaultSettingHelper.DefaultDateFormat;

        //Get Resource value by value        
        public static string GetResourceNameByValue(string value)
        {
            Assembly asm = Assembly.Load("Znode.Libraries.Resources");
            try
            {
                return new System.Resources.ResourceManager("Znode.Libraries.Resources.Admin_Resources.Admin_Resources", asm).GetObject(value).ToString();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return "";
            }

        }

        //Returns integer value if the input string is parsed to integer else return null.
        public static int? GetIntegerValue(this string integerString)
        {
            int number = 0;
            bool success = int.TryParse(integerString, out number);
            return success ? number : (int?)null;
        }

        /// <summary>
        /// To get formatted price 
        /// </summary>
        /// <param name="price">decimal price</param>
        /// <returns>returns two decimal value</returns>
        public static decimal FormatPrice(decimal? price)
            => HelperUtility.IsNull(price) ? 0 : Math.Round(price.Value, 2);

        //For Price according to currency.
        public static string FormatPriceWithCurrency(decimal? price, string CurrencyName)
        {
            string currencyValue;
            if (HelperUtility.IsNotNull(CurrencyName))
            {
                CultureInfo info = new CultureInfo(CurrencyName);
                info.NumberFormat.CurrencyDecimalDigits = Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff);
                currencyValue = $"{price.GetValueOrDefault().ToString("c", info.NumberFormat)}";
            }
            else
                currencyValue = Convert.ToString(price);

            return currencyValue;
        }

        /// <summary>
        /// Sets the filter for IsCategory property.
        /// </summary>
        /// <param name="filters">Filters to set for IsCategory.</param>
        /// <param name="isCategory">Value to set for IsCategory.</param>
        public static void SetIsCategoryFilters(FilterCollection filters, string isCategory)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking For IsCategory already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == ZnodePimAttributeGroupEnum.IsCategory.ToString()))
                {
                    //If IsCategory Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == ZnodePimAttributeGroupEnum.IsCategory.ToString());

                    //Add New IsCategory Into filters
                    filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory));
                }
                else
                    filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory));
            }
        }

        /// <summary>
        /// Sets the filter for IsAccountCustomer property.
        /// </summary>
        /// <param name="filters">Filters to set for IsAccountCustomer.</param>
        /// <param name="isAccountCustomer">Value to set for IsAccountCustomer.</param>
        public static void SetIsAccountCustomerFilter(FilterCollection filters, int isAccountCustomer)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking For IsAccountCustomer already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == FilterKeys.IsAccountCustomer))
                {
                    //If IsAccountCustomer Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.IsAccountCustomer);

                    //Add New IsAccountCustomer Into filters
                    filters.Add(new FilterTuple(FilterKeys.IsAccountCustomer, FilterOperators.Equals, isAccountCustomer.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.IsAccountCustomer, FilterOperators.Equals, isAccountCustomer.ToString()));
            }
        }

        /// <summary>
        /// Sets the filter for accountId property.
        /// </summary>
        /// <param name="filters">Filters to set for accountId.</param>
        /// <param name="accountId">Value to set for accountId.</param>
        public static void SetAccountIdFilters(FilterCollection filters, int accountId)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking For AccountId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower()))
                {
                    //If AccountId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower());
                    //Add New AccountId Into filters
                    filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountId.ToString().ToLower(), FilterOperators.Equals, accountId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountId.ToString().ToLower(), FilterOperators.Equals, accountId.ToString()));
            }
        }

        /// <summary>
        /// Sets the filter for userId property.
        /// </summary>
        /// <param name="filters">Filters to set for userId.</param>
        /// <param name="userId">Value to set for userId.</param>
        public static void SetUserIdFilters(FilterCollection filters, int userId)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking for Id already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == ZnodeNoteEnum.UserId.ToString()))
                {
                    //If Id Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == ZnodeNoteEnum.UserId.ToString());

                    //Add New Id Into filter.
                    filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            }
        }

        /// <summary>
        /// Sets the filter for portalId property.
        /// </summary>
        /// <param name="filters">Filters to set for portalId.</param>
        /// <param name="portalId">Value to set for portalId.</param>
        public static void SetPortalIdFilters(FilterCollection filters, int portalId)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking For PortalId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower()))
                {
                    //If PortalId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower());
                    //Add New PortalId Into filters
                    filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString().ToLower(), FilterOperators.Equals, portalId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString().ToLower(), FilterOperators.Equals, portalId.ToString()));
            }
        }

        // Check whether the Columns are valid or not.
        public static bool IsValidColumns(DataTable importPriceData, List<PropertyInfo> properties)
        {
            //Get the column Names from the DataTable.
            string[] columnNamesInFile = (from column in importPriceData.Columns.Cast<DataColumn>()
                                          select column.ColumnName).ToArray();

            // Check whether the Column sequence match with the Mapping.
            return IsValidColumnSequence(properties, columnNamesInFile);
        }

        public static List<SelectListItem> GetEnumList<T>(string selectedString) where T : struct
        {
            var listItems = Enum.GetValues(typeof(T)).OfType<Enum>().Select(e =>
             new SelectListItem()
             {
                 Text = GetEnumDescription(e),
                 Value = e.ToString(),
                 Selected = e.Equals(selectedString)
             });

            return listItems.ToList();
        }

        public static string GetEnumDescription(Enum en)
        {
            Type type = en.GetType();
            System.Reflection.MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo?.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);
                if (attrs?.Length > 0)
                    return ((System.ComponentModel.DataAnnotations.DisplayAttribute)attrs[0]).GetName();
            }

            return en.ToString();
        }

        //Get regex for default round off value for price.
        public static string GetRegexForDefaultRoundOffValForPrice()
       => GetRoundOffRegex(false);

        public static decimal GetFormattedRoundOffPrice(decimal? price)
            => Equals(price, null) ? 0 : Math.Round(price.Value, Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff));

        public static DataTable GetImportDetails(HttpPostedFileBase importFile)
        {
            DataTable dataTable = new DataTable();
            string path = string.Empty;
            try
            {
                string type = string.Empty;
                string fileExtension = Path.GetExtension(importFile.FileName).ToLower();
                path = Path.Combine(HttpContext.Current.Server.MapPath("~/Data/QuickOrder"), $"{GetDateTime().ToString("ddMMyyyyhhmmss")}_{importFile.FileName}");
                importFile.SaveAs(path);

                if (!Equals(fileExtension.ToLower(), WebStoreConstants.CSV))
                    type = GetAllSheetNameFromExcel(path).FirstOrDefault();

                if (Equals(fileExtension.ToLower(), WebStoreConstants.CSV))
                    dataTable = GetDataFromCSV(path);
                else
                    dataTable = GetDataFromExcel(path, type);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            return dataTable;
        }

        //Get all Sheet Names from the Excel File.
        public static string[] GetAllSheetNameFromExcel(string filePath)
        {
            string TABLE_NAME = "table_name";
            string[] excelSheetNames = null;
            using (OleDbConnection oleDbConnection = new OleDbConnection(GetConnectionString(filePath)))
            {
                OpenConnection(oleDbConnection);
                DataTable dataTable = GetExcelDataSchema(oleDbConnection);


                if (dataTable?.Rows?.Count > 0)
                {
                    excelSheetNames = new string[dataTable.Rows.Count];

                    for (int sheetCount = 0; sheetCount < dataTable.Rows.Count; sheetCount++)
                        excelSheetNames[sheetCount] = dataTable.Rows[sheetCount][TABLE_NAME].ToString().Replace("'", string.Empty);
                }
            }
            return excelSheetNames;
        }

        //Convert File Stream to DataTable.       
        public static DataTable GetDataFromCSV(string filePath)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet("Temp");
            //using (OleDbConnection oleDbConnection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path.GetDirectoryName(filePath) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\""))
            using (OleDbConnection oleDbConnection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path.GetDirectoryName(filePath) + "; Extended Properties = \"Text;HDR=No;FMT=Delimited\""))
            {
                oleDbConnection.Open();
                try
                {
                    dataTable = GetExcelDataSchema(oleDbConnection);
                    if (dataTable?.Rows?.Count > 0)
                    {
                        try
                        {
                            OleDbDataAdapter adapter = new OleDbDataAdapter
                                ("SELECT * FROM [" + Path.GetFileName(filePath) + "]", oleDbConnection);

                            adapter.Fill(dataSet);
                            if (dataSet != null && dataSet.Tables.Count > 0)
                            {
                                dataSet.Tables[0].Rows.RemoveAt(0);
                            }
                        }
                        catch (Exception ex)
                        {
                            ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                            oleDbConnection.Close();
                            throw;
                        }
                    }
                    oleDbConnection.Close();
                }
                catch { }
                finally { oleDbConnection.Close(); }
            }
            return (HelperUtility.IsNotNull(dataSet) && HelperUtility.IsNotNull(dataSet.Tables[0])) ? dataSet.Tables[0] : null;
        }
        private static DataTable GetDataFromExcel(string filePath, string sheetName)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            using (OleDbConnection oleDbConnection = new OleDbConnection(GetConnectionString(filePath)))
            {
                oleDbConnection.Open();
                try
                {
                    dataTable = GetExcelDataSchema(oleDbConnection);
                    if (dataTable?.Rows?.Count > 0)
                    {
                        try
                        {
                            FillDataAdapter(oleDbConnection, string.Empty, sheetName, dataSet);
                        }
                        catch
                        {
                            oleDbConnection.Close();
                            throw;
                        }
                    }
                    oleDbConnection.Close();
                }
                catch (Exception)
                { }
                finally { oleDbConnection.Close(); }
            }
            return HelperUtility.IsNotNull(dataSet?.Tables[0]) ? dataSet.Tables[0] : null;
        }
        //To Bind the Data from Excel Schema to DataTable.
        private static void FillDataAdapter(OleDbConnection oleDbConnection, string condition, string sheetname, DataSet dataSet, bool isCSV = false, string filepath = "")
        {
            OleDbDataAdapter oleDbDataAdapter;
            if (!string.IsNullOrEmpty(condition))
                oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM [" + sheetname + "] where " + condition, oleDbConnection);
            else if (isCSV)
                oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM " + Path.GetFileName(filepath), oleDbConnection);
            else
                oleDbDataAdapter = new OleDbDataAdapter("SELECT  * FROM [" + sheetname + "]", oleDbConnection);
            oleDbDataAdapter.Fill(dataSet, sheetname);
        }

        //Get connection string to read excel file.
        private static string GetConnectionString(string filePath)
          => !Path.GetExtension(filePath).ToLower().Equals(WebStoreConstants.XLSX)
            ? $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'"
            : $"Provider = Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0;IMEX=1;HDR=YES'";


        #endregion

        #region Private Methods
        // Check whether the Column sequence match with the Mapping.
        private static bool IsValidColumnSequence(List<PropertyInfo> properties, string[] columnNamesInFile)
        {
            if (HelperUtility.IsNotNull(properties) && HelperUtility.IsNotNull(columnNamesInFile))
            {
                for (int index = 0; index < columnNamesInFile.Length; index++)
                {
                    if (!Equals(columnNamesInFile[index], properties[index].Name))
                        return false;
                }
            }
            return true;
        }

        //Get the schema to read data from Excel sheet.
        private static DataTable GetExcelDataSchema(OleDbConnection oleDbConnection)
            => oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

        //Open the Old Connection to read Excel file.
        private static void OpenConnection(OleDbConnection oleDbConnection)
        {
            try
            {
                oleDbConnection.Open();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                oleDbConnection.Close();
            }
        }

        //Gets the Round of message.
        private static string GetRoundOffMessage(string errorMessageForRoundOff, string errorMessage, out string message, int defaultRoundOffVal)
        {
            if (defaultRoundOffVal > 0)
            {
                string num = string.Empty;
                for (var i = 1; i <= defaultRoundOffVal; i++)
                    num = num + "" + i;
                message = string.Format(errorMessageForRoundOff, num);
            }
            else
                message = string.Format(errorMessage);

            return message;
        }

        //Gets the regular expression for decimal value.
        private static string GetRoundOffRegex(bool isInventory)
        {
            int defaultRoundOffVal = isInventory ? Convert.ToInt32(DefaultSettingHelper.DefaultInventoryRoundOff) : Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff);
            Regex regex = new Regex("^\\d{0,}(\\.\\d{0," + defaultRoundOffVal + "})?$", RegexOptions.IgnoreCase);
            return Convert.ToString(regex);
        }

        public static string GetPriceFormatErorrMessage(string errorMessageForRoundOff, string errorMessage)
        {
            string message = string.Empty;
            int defaultRoundOffVal = Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff);
            return GetRoundOffMessage(errorMessageForRoundOff, errorMessage, out message, defaultRoundOffVal);
        }

        // Gets the value of the  key from BindData model.
        public static object GetValue(this BindDataModel model, string key)
        {
            object value = null;
            model.ControlsData.TryGetValue(key, out value);
            return value;
        }
        #endregion

        # region Time Zone
        //Convert Datetime to timezone which is set in global setting.
        public static DateTime ToGlobalTimeZone(this DateTime dateTime)
        {
            return dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(GetDefaultTimeZnode()) ? "Central Standard Time" : GetDefaultTimeZnode()));
        }

        //Get Current Date And time and Set the Utc Off set Time Zone 
        public static string GetUtcOffsetForTimeZone()
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(GetDefaultTimeZnode()) ? "Central Standard Time" : GetDefaultTimeZnode());
            return Convert.ToString(timeZoneInfo);
        }
        //Convert to Datetime format which is set in global setting.
        public static string ToTimeZoneDateTimeFormat(this DateTime dateTime)
        {
            return (dateTime != null && dateTime != DateTime.MinValue) ? ToGlobalTimeZone(dateTime).ToString(GetStringDateTimeFormat()) : null;
        }

        //Convert to Date format which is set in global setting.
        public static string ToTimeZoneDateFormat(this DateTime dateTime)
        {
            return (dateTime != null && dateTime != DateTime.MinValue) ? ToGlobalTimeZone(dateTime).ToString(GetStringDateFormat()) : null;
        }

        //Convert to Time format which is set in global setting.
        public static string ToTimeZoneTimeFormat(this DateTime dateTime)
        {
            return (dateTime != null && dateTime != DateTime.MinValue) ? ToGlobalTimeZone(dateTime).ToString(GetStringTimeFormat()) : null;
        }
        #endregion
    }
}
