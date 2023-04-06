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
using System.Web.Caching;
using System.Web.Mvc;

using Znode.Admin.Core.Helpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Helpers
{
    public static class HelperMethods
    {
        #region Public Methods
        public static int GridPagingStartValue { get; } = string.IsNullOrEmpty(ZnodeAdminSettings.GridPagingStartValue) ? 10 : Convert.ToInt32(ZnodeAdminSettings.GridPagingStartValue);

        public static string DatePickDateFormat() => DefaultSettingHelper.DefaultDateFormat;

        public static string ConvertStringToDate(string date) => !string.IsNullOrEmpty(date) ? SearchDateFormat(GetDateTime(date)) : string.Empty;

        public static string GetStringDateFormat() => DefaultSettingHelper.DefaultDateFormat;

        public static string GetStringTimeFormat() => DefaultSettingHelper.DefaultTimeFormat;

        public static string GetStringDateTimeFormat() => DefaultSettingHelper.DefaultDateFormat + " " + DefaultSettingHelper.DefaultTimeFormat;

        public static string GetDefaultTimeZnode() => DefaultSettingHelper.DefaultTimeZone;

        public static string GetDomainUrl() => (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority))) ? HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty;

        public static DateTime GetDateTime(string dateTime)
        {
            DateTime date;
            DateTime.TryParse(dateTime, out date);
            return date;
        }


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

            if (!Equals(isEnabled, null))
            {
                if (isEnabled)
                {
                    icon = "z-ok";
                }
                else
                {
                    icon = "z-close";
                }
            }
            return icon;
        }

        /// <summary>
        /// Generate Media path
        /// </summary>
        /// <returns>Media path</returns>
        public static string GetMediaPath()
        {
            string mediaUrl = SessionHelper.GetDataFromSession<string>("MediaPath");

            if (Equals(mediaUrl, null))
            {
                mediaUrl = GetService<IDependencyHelper>().GetDefaultMediaConfiguration().URL;
                SessionHelper.SaveDataInSession<string>("MediaPath", mediaUrl);
            }

            return mediaUrl;

        }

        /// <summary>
        /// Get date format as per global setting.
        /// </summary>
        /// <returns>Returns date format.</returns>
        public static string GetDateFormat() => DefaultSettingHelper.DefaultDateFormat.Contains("MMM") ? DefaultSettingHelper.DefaultDateFormat.Replace("MMM", "M") : DefaultSettingHelper.DefaultDateFormat.Contains("MM") ? DefaultSettingHelper.DefaultDateFormat.Replace("MM", "mm") : DefaultSettingHelper.DefaultDateFormat;

        /// <summary>
        /// Generate Thumbnail image path
        /// </summary>
        /// <returns>Thumbnail image path</returns>
        public static string GetThumbnailImagePath()
        {
            var mediaConfiguration = GetService<IDependencyHelper>().GetDefaultMediaConfiguration();
            return $"{mediaConfiguration.URL}{mediaConfiguration.ThumbnailFolderName}/";
        }

        //Get Resource value by value        
        public static string GetResourceNameByValue(string value)
        {
            Assembly asm = Assembly.Load("Znode.Libraries.Resources");
            try
            {
                return new System.Resources.ResourceManager("Znode.Libraries.Resources.Admin_Resources.Admin_Resources", asm).GetObject(value).ToString();
            }
            catch (Exception)
            {
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
            => Equals(price, null) ? 0 : Math.Round(price.Value, 2);

        //For Price according to currency
        public static string FormatPriceWithCurrency(decimal? price, string currencyName, string UOM = "")
        {
            if (HelperUtility.IsNotNull(price))
            {
                string currencyValue;
                if (!string.IsNullOrEmpty(currencyName))
                {
                    CultureInfo info = new CultureInfo(currencyName);
                    info.NumberFormat.CurrencyNegativePattern = 0;
                    info.NumberFormat.CurrencyDecimalDigits = Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff);
                    currencyValue = $"{price.GetValueOrDefault().ToString("c", info.NumberFormat)}";
                }
                else
                    currencyValue = Convert.ToString(price);

                return !string.IsNullOrEmpty(UOM) ? $"{currencyValue} / {UOM}" : currencyValue;
            }
            return null;
        }

        /// <summary>
        /// Sets the filter for IsCategory property.
        /// </summary>
        /// <param name="filters">Filters to set for IsCategory.</param>
        /// <param name="isCategory">Value to set for IsCategory.</param>
        public static void SetIsCategoryFilters(FilterCollection filters, string isCategory)
        {
            if (!Equals(filters, null))
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
            if (!Equals(filters, null))
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
            if (!Equals(filters, null) && portalId > 0)
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

        /// <summary>
        /// Gets the value of the  key from BindData model.
        /// </summary>
        /// <param name="model">BindData Model.</param>
        /// <param name="key">String key.</param>
        /// <returns>Value of the key in BindDataModel.</returns>
        public static object GetValue(this BindDataModel model, string key)
        {
            object value = null;
            model.ControlsData.TryGetValue(key, out value);
            return value;
        }

        //Get the list of pricing details.
        public static DataTable GetImportDetails(HttpPostedFileBase importFile)
        {
            DataTable dataTable = new DataTable();
            string path = string.Empty;
            try
            {
                string type = string.Empty;
                string fileExtension = Path.GetExtension(importFile.FileName).ToLower();
                path = Path.Combine(HttpContext.Current.Server.MapPath("~/Data/SampleExcel"), $"{GetDateTime().ToString("ddMMyyyyhhmmss")}_{importFile.FileName}");
                importFile.SaveAs(path);

                if (!Equals(fileExtension.ToLower(), AdminConstants.CSV))
                    type = GetAllSheetNameFromExcel(path).FirstOrDefault();

                if (Equals(fileExtension.ToLower(), AdminConstants.CSV))
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

        // Check whether the Columns are valid or not.
        public static bool IsValidColumns(DataTable importPriceData, List<PropertyInfo> properties)
        {
            //Get the column Names from the DataTable.
            string[] columnNamesInFile = (from column in importPriceData.Columns.Cast<DataColumn>()
                                          select column.ColumnName).ToArray();

            // Check whether the Column sequence match with the Mapping.
            return IsValidColumnSequence(properties, columnNamesInFile);
        }

        //Convert File Stream to DataTable.       
        public static DataTable GetDataFromCSV(string filePath)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet("Temp");
            using (OleDbConnection oleDbConnection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path.GetDirectoryName(filePath) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\""))
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

        //Mapping For Validation Of Attribute Dynamic Control
        public static Dictionary<string, object> GetValidation(string ControlName, string type, string attributeTypeName)
        {
            Dictionary<string, object> validations = new Dictionary<string, object>();
            switch (ControlName)
            {
                case AdminConstants.Number:
                    if (type.Equals("MaxCharacters"))
                    {
                        validations.Add(AdminConstants.AllowNegative, "false");
                        validations.Add(AdminConstants.AllowDecimals, "false");
                    }
                    else if (type.Equals("MaxFileSize"))
                    {
                        validations.Add(AdminConstants.AllowNegative, "false");
                    }
                    else
                    {
                        validations.Add(AdminConstants.AllowNegative, "true");
                        validations.Add(AdminConstants.AllowDecimals, "true");
                    }
                    if (attributeTypeName.Equals("Image") || attributeTypeName.Equals("File") || attributeTypeName.Equals("Audio"))
                    {
                        validations.Add(AdminConstants.MinNumber, "0");
                        validations.Add(AdminConstants.MaxNumber, ZnodeAdminSettings.MaxFileSize);
                    }
                    else if (attributeTypeName.Equals("Video"))
                    {
                        validations.Add(AdminConstants.MinNumber, "0");
                        validations.Add(AdminConstants.MaxNumber, ZnodeAdminSettings.MaxSizeForVideo);
                    }
                    return validations;
                case AdminConstants.Date:
                    if (type == AdminConstants.MinDate)
                    {
                        validations.Add(AdminConstants.Group, AdminConstants.DateGroup);
                        validations.Add(AdminConstants.MinDate, string.Empty);
                    }
                    else
                    {
                        validations.Add(AdminConstants.Group, AdminConstants.DateGroup);
                        validations.Add(AdminConstants.MaxDate, string.Empty);
                    }
                    return validations;
                default:
                    break;
            }
            return validations;
        }

        //To Check whether File or Directory exists.    
        public static bool FileOrDirectoryExists(string filePath)
            => (Directory.Exists(filePath) || File.Exists(filePath));

        //List of default file types for export.
        public static List<SelectListItem> GetFileTypesForExport()
        {
            List<SelectListItem> fileTypes = new List<SelectListItem>();
            fileTypes.Add(new SelectListItem { Value = Convert.ToInt32(FileTypes.Excel).ToString(), Text = (FileTypes.Excel).ToString() });
            fileTypes.Add(new SelectListItem { Value = Convert.ToInt32(FileTypes.CSV).ToString(), Text = (FileTypes.CSV).ToString() });
            return fileTypes;
        }

        //Get the type of file. 
        public static string GetFileFormat(string exportFileTypeId)
            => Equals(exportFileTypeId, "1") ? "xls" : "csv";

        public static List<SelectListItem> GetAddonTypes(string selectedAddonType)
        {
            return Enum.GetNames(typeof(AddonType)).Select(e => new SelectListItem { Text = e, Value = e, Selected = Equals(e.ToLower(), selectedAddonType) }).ToList();
        }

        //Get regex for default round off value for price.
        public static string GetRegexForDefaultRoundOffValForPrice()
       => GetRoundOffRegex(false);

        //Get regex for default round off value for inventory.
        public static string GetRegexForDefaultRoundOffValForInventory()
        => GetRoundOffRegex(true);

        public static string GetPriceFormatErorrMessage(string errorMessageForRoundOff, string errorMessage)
        {
            string message = string.Empty;
            int defaultRoundOffVal = Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff);
            return GetRoundOffMessage(errorMessageForRoundOff, errorMessage, out message, defaultRoundOffVal);
        }

        //Get error message for quantity or re-order level.
        public static string GetInventoryFormatErrorMessage(string errorMessageForRoundOff, string errorMessage)
        {
            string message = string.Empty;
            int defaultRoundOffVal = Convert.ToInt32(DefaultSettingHelper.DefaultInventoryRoundOff);
            return GetRoundOffMessage(errorMessageForRoundOff, errorMessage, out message, defaultRoundOffVal);
        }

        public static AjaxHeadersModel GetAjaxHeaders()
        {
            return new AjaxHeadersModel();
        }

        //Get default inventory round off value. 
        public static int GetDefaultInventoryRoundOff()
         => Convert.ToInt32(DefaultSettingHelper.DefaultInventoryRoundOff);

        public static string GetImportTemplateFilePath(string format, string sampleFilePath)
            => !string.IsNullOrEmpty(format) ? $"{sampleFilePath}{format.ToLower()}" : string.Empty;

        /// <summary>
        ///Dropdown for Approval Status. 
        /// </summary>
        /// <returns>List of approval status.</returns>
        public static List<SelectListItem> GetApprovalStatusDictionary()
        {
            List<SelectListItem> approvalStatus = new List<SelectListItem>();
            approvalStatus.Add(new SelectListItem { Value = "I", Text = Admin_Resources.TextInactive });
            approvalStatus.Add(new SelectListItem { Value = "N", Text = Admin_Resources.TextPendingApproval });
            approvalStatus.Add(new SelectListItem { Value = "A", Text = Admin_Resources.TextApproved });
            approvalStatus.Add(new SelectListItem { Value = "D", Text = Admin_Resources.TextDeclined });
            return approvalStatus;
        }

        //Get List of Active Countries.
        public static List<SelectListItem> GetCountries()
        {
            ICountryClient _countryClient = new CountryClient();
            List<SelectListItem> countriesSelectList = new List<SelectListItem>();
            FilterCollection filters = SetFiltersForCountry();
            SortCollection sorts = SetSortsForCountry();
            CountryListModel countries = _countryClient.GetCountryList(null, filters, sorts);

            if (countries?.Countries?.Count > 0)
            {
                //Set default country on top in dropdown as per in Global setting.
                SetDefaultCountry(countries);
                foreach (CountryModel country in countries.Countries)
                    countriesSelectList.Add(new SelectListItem() { Text = country.CountryName, Value = country.CountryCode });
            }
            return countriesSelectList;
        }

        //Get List of Active Countries.
        public static List<SelectListItem> GetPortalAssociatedCountries(int portalId)
        {
            IPortalCountryClient _portalCountryClient = new PortalCountryClient();

            FilterCollection filters = SetFiltersForAssociatedCountry(portalId);
            SortCollection sorts = SetSortsForCountry();

            //Get list of associated category.
            CountryListModel countries = _portalCountryClient.GetAssociatedCountryList(null, filters, sorts, null, null);

            List<SelectListItem> countriesSelectList = new List<SelectListItem>();
            if (countries?.Countries?.Count > 0)
            {
                //Set default country on top in dropdown as per in Global setting.
                SetDefaultCountry(countries);
                foreach (CountryModel country in countries.Countries.Distinct())
                    countriesSelectList.Add(new SelectListItem() { Text = country.CountryName, Value = country.CountryCode });

                countriesSelectList = countriesSelectList.GroupBy(x => x.Value).Select(x => x.First()).ToList();
            }
            return countriesSelectList;
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

        public static SortCollection SalesRepSort(ref SortCollection sortCollection)
        {
            if (HelperUtility.IsNull(sortCollection) || sortCollection.Count == 0)
            {
                sortCollection = new SortCollection();
                sortCollection.Add(View_AdminUserDetailEnum.CreatedDate.ToString(), DynamicGridConstants.DESCKey);
                sortCollection.Add(View_AdminUserDetailEnum.FullName.ToString(), DynamicGridConstants.ASCKey);
                sortCollection.Add(View_AdminUserDetailEnum.UserName.ToString(), DynamicGridConstants.ASCKey);

            }
            return sortCollection;
        }

        public static SortCollection StoreExperienceSort(SortCollection sortCollection)
        {
            if (HelperUtility.IsNull(sortCollection) || sortCollection.Count == 0)
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodePortalEnum.StoreName.ToString(), DynamicGridConstants.ASCKey);
                sortCollection.Add(ZnodePortalEnum.StoreCode.ToString(), DynamicGridConstants.ASCKey);
                sortCollection.Add(AdminConstants.PublishStatus, DynamicGridConstants.ASCKey);
            }
            return sortCollection;
        }

        //Set the sort collection for user id desc.
        public static void SortUserIdDesc(ref SortCollection sortCollection)
        {
            if (HelperUtility.IsNull(sortCollection) || sortCollection.Count == 0)
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeUserEnum.UserId.ToString(), DynamicGridConstants.DESCKey);
                sortCollection.Add(ZnodeConstant.FullName.ToString(), DynamicGridConstants.ASCKey);
                sortCollection.Add(ZnodeUserEnum.Email.ToString(), DynamicGridConstants.DESCKey);
            }
        }

        //Set the sort collection for user id desc.
        public static void SortCustomerListDesc(ref SortCollection sortCollection)
        {
            if (HelperUtility.IsNull(sortCollection) || sortCollection.Count == 0)
            {
                sortCollection = new SortCollection();
                sortCollection.Add(View_GetPriceListUsersEnum.UserId.ToString(), DynamicGridConstants.DESCKey);
                sortCollection.Add(View_GetPriceListUsersEnum.FullName.ToString(), DynamicGridConstants.ASCKey);
                sortCollection.Add(View_GetPriceListUsersEnum.EmailId.ToString(), DynamicGridConstants.DESCKey);
            }
        }

        public static string GetVideoTag(string url)
        => $"<div class=\"embed-responsive grid-video-edit\">{GetVideoSource(url)}</div>";

        public static string GetAudioTag(string url)
        => $"<div class=\"grid-audio-edit\" ><audio controls \"><source src = '{url}'  /></audio></div>";

        private static string GetVideoSource(string url)
        {
            string _ext = Path.GetExtension(url);
            string tag = string.Empty;
            if (!string.IsNullOrEmpty(_ext))
            {
                _ext = _ext.Remove(0, 1);
                tag = $"<video class=\"embed-responsive-item\" controls><source src='{url}' preload=\"metadata\" /></video>";
            }
            else
            {
                YouTubeUrl = url;
                tag = !string.IsNullOrEmpty(YouTubeVideoId) ? $"<embed width=\"420\" height=\"315\" src=\"https://www.youtube.com/embed/{YouTubeVideoId}?controls=1\" />" : string.Empty;
            }
            return tag;
        }

        public static string YouTubeUrl { get; set; }

        public static string YouTubeVideoId
        {
            get
            {
                var youtubeMatch =
                    new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)")
                    .Match(YouTubeUrl);
                return youtubeMatch.Success ? youtubeMatch.Groups[1].Value : string.Empty;
            }
        }

        //Set the desc sort collection.
        public static SortCollection SortDesc(string sortKey, SortCollection sortCollection)
        {
            if (HelperUtility.IsNull(sortCollection) || sortCollection.Count == 0 && !string.IsNullOrEmpty(sortKey))
            {
                sortCollection = new SortCollection();
                sortCollection.Add(sortKey, DynamicGridConstants.DESCKey);
            }
            return sortCollection;
        }

        public static DateTime GetDateTime() => DateTime.Now;

        public static DateTime GetDateTimeByAddedYear(int AddYear) => DateTime.Now.AddYears(AddYear);


        //Set the asc sort collection.
        public static SortCollection SortAsc(string sortKey, SortCollection sortCollection)
        {
            if (HelperUtility.IsNull(sortCollection) || sortCollection.Count == 0 && !string.IsNullOrEmpty(sortKey))
            {
                sortCollection = new SortCollection();
                sortCollection.Add(sortKey, DynamicGridConstants.ASCKey);
            }
            return sortCollection;
        }

        //Get the list of global search.
        public static List<SelectListItem> GetGlobalSearchList()
        {
            List<SelectListItem> globalFilters = new List<SelectListItem>();
            if (SessionProxyHelper.IsSalesRepUser() == false)
            {
                globalFilters.Add(new SelectListItem { Text = "Products", Value = DynamicGridConstants.ProductFilter });
                globalFilters.Add(new SelectListItem { Text = "Catalogs", Value = DynamicGridConstants.CatalogFilter });
                globalFilters.Add(new SelectListItem { Text = "Categories", Value = DynamicGridConstants.CategoryFilter });
            }
            globalFilters.Add(new SelectListItem { Text = "Orders", Value = DynamicGridConstants.OrderFilter });
            globalFilters.Add(new SelectListItem { Text = "Users", Value = DynamicGridConstants.UserFilter });
            return globalFilters;
        }

        //Get the list of available publish states.
        public static List<PopupInputFieldDataItem> GetAvailablePublishStates()
        {
            IPublishPopupHelper helper = GetService<IPublishPopupHelper>();
            return helper.GetAvailablePublishStates();
        }

        public static List<PopupInputFieldDataItem> GetPublishContentChoices()
        {
            return new List<PopupInputFieldDataItem>(){
                new PopupInputFieldDataItem
                {
                    DisplayName = "Store Settings",
                    Disabled = true,
                    IsChecked = true,
                    Value = ZnodePublishContentTypeEnum.StoreSettings.ToString()
                },
                new PopupInputFieldDataItem
                {
                    DisplayName = "CMS Content",
                    Disabled = false,
                    IsChecked = false,
                    Value = ZnodePublishContentTypeEnum.CmsContent.ToString()
                },
                new PopupInputFieldDataItem
                {
                    DisplayName = "Catalog",
                    Disabled = false,
                    IsChecked = false,
                    Value = ZnodePublishContentTypeEnum.Catalog.ToString()
                }
            };
        }

        //Get publish status code details 
        public static List<PopupInputFieldDataItem> GetPublishStatusContentChoices(bool isDisabled = false,string applicationType=ZnodeConstant.Store)
        {
            return new List<PopupInputFieldDataItem>(){
                new PopupInputFieldDataItem
                {
                    DisplayName = ZnodeConstant.PublishDraftProductsOnly,
                    Disabled = isDisabled,
                    IsChecked = false,
                    ApplicationType=applicationType,
                    Value = ZnodePublishStatesEnum.DRAFT.ToString()
                }
            };
        }

        //Gets date time filter tuple present in Filter Collection.
        public static FilterTuple GetDateTimeRangeFilterTuple(FilterCollection filters) =>
             filters.FirstOrDefault(x => string.Equals(x.FilterName, AdminConstants.DateTimeRange, StringComparison.InvariantCultureIgnoreCase));

        public static string GetValuesInDefaultFormat(string value)
        {
            if (!string.IsNullOrEmpty(value) && value.ToLower().Contains("to"))
            {
                string dateTimeFormat = HelperMethods.GetStringDateTimeFormat();
                string[] dates = value.Split(new string[] { "to" }, StringSplitOptions.None);
                return Convert.ToDateTime(dates[0].Trim()).ToString(dateTimeFormat) + "to" + Convert.ToDateTime(dates[1].Trim()).ToString(dateTimeFormat);
            }

            return string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        //Round off the quantity of cart item/cartcount.
        public static string GetRoundOffQuantity(string quantity)
        {
            string productQuantity = quantity;
            if (!string.IsNullOrEmpty(productQuantity) && productQuantity != "0")
            {
                productQuantity = Convert.ToDecimal(productQuantity).ToInventoryRoundOff();
                if (productQuantity.Contains("."))
                {
                    decimal count = Convert.ToDecimal(productQuantity.Split('.')[1]);
                    if (!(count > 0))
                        productQuantity = productQuantity.Split('.')[0];
                }
            }
            return productQuantity;
        }

        //Helper method to get data from cache.
        public static T GetFromCache<T>(string cacheKey)
        {
            var model = HttpRuntime.Cache[cacheKey];
            return (T)Convert.ChangeType(model, typeof(T));
        }

        //Helper method to add data into cache.
        public static void AddIntoCache<T>(T model, string cacheKey, string cacheDuration)
        {
            HttpRuntime.Cache.Insert(cacheKey, model, null, DateTime.UtcNow.AddMinutes(Convert.ToDouble(cacheDuration)), Cache.NoSlidingExpiration);
        }

        //Remove data from cache on basis of cache key.
        public static void ClearCache(string cacheKey)
     => HttpContext.Current.Cache.Remove(cacheKey);
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
            catch
            {
                oleDbConnection.Close();
            }
        }

        //Set sorts for country.
        private static SortCollection SetSortsForCountry()
        {
            SortCollection sortCollection = new SortCollection();
            sortCollection.Add(ZnodeCountryEnum.CountryName.ToString(), DynamicGridConstants.ASCKey);
            return sortCollection;
        }

        //Set filters for country.
        private static FilterCollection SetFiltersForCountry()
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCountryEnum.IsActive.ToString(), FilterOperators.Equals, "true"));
            return filters;
        }

        //Set filters for portal associated country.
        private static FilterCollection SetFiltersForAssociatedCountry(int portalId)
        {
            FilterCollection filters = new FilterCollection();
            if (portalId > 0)
                filters.Add(new FilterTuple(ZnodePortalCountryEnum.PortalId.ToString(), FilterOperators.Equals, Convert.ToString(portalId)));
            return filters;
        }

        //Get connection string to read excel file.
        private static string GetConnectionString(string filePath)
          => !Path.GetExtension(filePath).ToLower().Equals(AdminConstants.XLSX)
            ? $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'"
            : $"Provider = Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0;IMEX=1;HDR=YES'";

        //Get the list of pricing data from excel sheet.
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

        //Method to set default country on top in dropdown as per in Global setting.
        private static void SetDefaultCountry(CountryListModel countries)
        {
            int defaultCountryId = countries.Countries.FindIndex(x => x.CountryId == countries.Countries.FirstOrDefault(i => i.IsDefault == true)?.CountryId);
            CountryModel defaultCountry = countries.Countries[defaultCountryId];
            countries.Countries[defaultCountryId] = countries.Countries[0];
            countries.Countries[0] = defaultCountry;
        }

        //Gets the regular expression for decimal value.
        private static string GetRoundOffRegex(bool isInventory)
        {
            int defaultRoundOffVal = isInventory ? Convert.ToInt32(DefaultSettingHelper.DefaultInventoryRoundOff) : Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff);
            Regex regex = new Regex("^\\d{0,}(\\.\\d{0," + defaultRoundOffVal + "})?$", RegexOptions.IgnoreCase);
            return Convert.ToString(regex);
        }

        //Set debug mode.
        public static bool IsDebugMode()
        => ZnodeAdminSettings.IsDebugMode;

        //Is Add/Edit Address Allowed.
        public static bool IsAddEditAddressAllow(string roleName)
        {
            bool isAddEditAddressAllow = true;

            if (!string.IsNullOrEmpty(roleName))
                isAddEditAddressAllow = !(string.Equals(roleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(roleName, ZnodeRoleEnum.User.ToString(), StringComparison.OrdinalIgnoreCase));
            return isAddEditAddressAllow;
        }

        public static decimal GetFormattedRoundOffPrice(decimal? price)
            => Equals(price, null) ? 0 : Math.Round(price.Value, Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff));

        //Give roundoff format based on PriceRoundOff
        public static string GetFormattedRoundOffPrice()
        {
            return "{0:n" + DefaultSettingHelper.DefaultPriceRoundOff + "}";
        }

        //Get Scheduler Title to show on mouse hover
        public static string GetSchedulerTitle(string ConnectorTouchPoints, string schedulerCallFor = "")
        {
            IERPTaskSchedulerHelper helper = GetService<IERPTaskSchedulerHelper>();
            return helper.GetSchedulerTitle(ConnectorTouchPoints, schedulerCallFor);
        }

        #endregion

        #region Time Zone
        //Convert Datetime to timezone which is set in global setting.
        public static DateTime ToGlobalTimeZone(this DateTime dateTime)
        {
            return dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(GetDefaultTimeZnode()) ? "Central Standard Time" : GetDefaultTimeZnode()));
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

        public static string GetServerTimeZone() => TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName.ToString()).ToString();
            

        #endregion
    }
}
