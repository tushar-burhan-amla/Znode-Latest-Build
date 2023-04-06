using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class HelperUtility
    {
        #region Private Variables
        private delegate bool ParseDelegate<T>(string s, out T result);

        #endregion

        #region Public Method
        /// <summary>
        /// Generated the unique Password string
        /// </summary>
        /// <param name="maxSize">Length for Max password size</param>
        /// <returns>Return the Unique key combination</returns>
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            string strCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = strCharacters.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte bytedata in data)
            {
                result.Append(chars[bytedata % (chars.Length - 1)]);
            }
            SetNumberInUniqueKey(result);
            return result.ToString();
        }

        /// <summary>
        /// Generate XML string from data object 
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="t">Data object</param>
        /// <returns>XMLstring of object. and empty string is any Exception Occurs</returns>
        /// <exception cref="">Returns Empty string if exception occurs</exception>
        /// <example>string data = ToXML<List<PIMAttributeGroupViewModel>>(attributeFamilyDetails.Groups);</example>
        public static string ToXML<T>(T t) where T : class
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();

                //Omit Xml Declaration from xml as it was not required in stored Procedure
                settings.OmitXmlDeclaration = true;

                //Used to Indent the xml.
                settings.Indent = true;
                settings.CheckCharacters = false;

                System.IO.StringWriter stringwriter = new System.IO.StringWriter();
                XmlWriter xmlWriter = XmlWriter.Create(stringwriter, settings);

                //used to set the namespace in XML to empty string as it was not required in stored Procedure
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(xmlWriter, t, namespaces);
                return Convert.ToString(stringwriter);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return string.Empty;
            }
        }

        //Get compressed xml
        public static string GetCompressedXml<T>(T model) where T : class
        {
            try
            {
                XDocument documents = XDocument.Parse(ToXML(model));
                documents.Descendants().Where(x => (bool?)x.Attribute("{http://www.w3.org/2001/XMLSchema-instance}nil") == true).Remove();
                return documents.ToString(SaveOptions.DisableFormatting);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage($"failed in compressing xml with error message {ex.Message}", string.Empty, TraceLevel.Error);
                return string.Empty;
            }
        }

        //map Xml to Model 
        public static T ConvertXMLStringToModel<T>(string xmlString) where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(xmlString)) return null;

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader rdr = new StringReader(Convert.ToString(XDocument.Parse(xmlString)));
                T model = (T)serializer.Deserialize(rdr);
                return model;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }

        //Map list of Xml to list of Model.
        public static List<T> ConvertListOfXMLStringToListModel<T>(IEnumerable<string> xmlString) where T : class
        {
            try
            {
                List<T> xmlList = new List<T>();
                foreach (string xml in xmlString)
                {
                    if (!string.IsNullOrEmpty(xml))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        StringReader rdr = new StringReader(Convert.ToString(XDocument.Parse(xml)));
                        T model = (T)serializer.Deserialize(rdr);
                        xmlList.Add(model);
                    }
                    else {
                        ZnodeLogging.LogMessage("While publishing catalog/category, some products xml getting null.", ZnodeLogging.Components.PIM.ToString(),TraceLevel.Error);
                    }
                }
                return xmlList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }


        /// <summary>
        /// Convert Tuple list into XMLString.
        /// </summary>
        /// <param name="filterTuples"> List of filter tuples</param>
        /// <returns></returns>
        public static string FilterTupleToXMLString(List<Tuple<string, string>> filterTuples)
        {
            XDocument xmlDocument = new XDocument(new XElement("filters"));

            foreach (Tuple<string, string> filter in filterTuples)
            {
                XElement filterElement = new XElement("filter",
                new XElement("key", filter.Item1),
                new XElement("condition", filter.Item2)
                );
                xmlDocument.Root.Add(filterElement);
            }
            return XDocument.Parse(xmlDocument.ToString()).ToString();
        }

        //Split bulk records /  collections into chunks.Here we need to provide the list of collections and chunk size.
        public static List<List<T>> SplitCollectionIntoChunks<T>(List<T> collection, int chunkSize)
        {
            List<List<T>> chunks = new List<List<T>>();
            int chunkCount = collection.Count() / chunkSize;

            if (collection.Count % chunkSize > 0)
                chunkCount++;

            for (var index = 0; index < chunkCount; index++)
                chunks.Add(collection.Skip(index * chunkSize).Take(chunkSize).ToList());

            return chunks;
        }

        //Returns true if the passed value is not null, else return false.
        public static bool IsNotNull(object value)
            => !Equals(value, null);

        //Returns true if the passed value is null else false.
        public static bool IsNull(object value)
            => Equals(value, null);

        //Method to replace token with message text.
        public static string ReplaceTokenWithMessageText(string key, string replaceValue, string resourceText)
        {
            Regex rgx = new Regex(key, RegexOptions.IgnoreCase);
            return rgx.Replace(resourceText, string.IsNullOrEmpty(replaceValue) ? string.Empty : replaceValue);
        }


        /// <summary>
        /// Method to replace multiple tokens with respective message text.
        /// </summary>
        /// <param name="keyValueDictionary"></param>
        /// <param name="resourceText"></param>
        /// <returns>return message with updated values.</returns>
        public static string ReplaceMultipleTokenWithMessageText(IDictionary<string, string> keyValueDictionary, string resourceText)
        {
            foreach (var keyvaluePair in keyValueDictionary)
            {
                Regex rgx = new Regex(keyvaluePair.Key, RegexOptions.IgnoreCase);
                resourceText = rgx.Replace(resourceText, string.IsNullOrEmpty(keyvaluePair.Value) ? string.Empty : keyvaluePair.Value);
            }
            return resourceText;
        }

        /// <summary>
        /// Check number exists between the given range
        /// </summary>
        /// <param name="num">number to check between range</param>
        /// <param name="minNumber">lower limit</param>
        /// <param name="MaxNumber">upper limit</param>
        /// <param name="inclusive">true if want to include min and max number in comparison</param>
        /// <returns>true or false</returns>
        public static bool Between(int num, int minNumber, int MaxNumber, bool inclusive = false)
        {
            return inclusive
                ? minNumber <= num && num <= MaxNumber
                : minNumber < num && num < MaxNumber;
        }

        /// <summary>
        /// Check number exists between the given range.
        /// </summary>
        /// <param name="num">number to check between range</param>
        /// <param name="minNumber">lower limit</param>
        /// <param name="MaxNumber">upper limit</param>
        /// <param name="inclusive">true if want to include min and max number in comparison</param>
        /// <returns>true or false</returns>
        public static bool Between(decimal num, decimal minNumber, decimal MaxNumber, bool inclusive = false)
        {
            return inclusive
                ? minNumber <= num && num <= MaxNumber
                : minNumber < num && num < MaxNumber;
        }
        public enum StoreFeature
        {
            Taxes_In_Product_Price,
            Persistent_Cart,
            Address_Validation,
            Customer_Based_Pricing,
            Allow_multiple_coupons,
            Allow_Single_Page_Checkout,
            Allow_Global_Level_User_Creation,
            Require_Validated_Address,
            Enable_Profile_Based_Search,
            Enable_Product_Compare,
            Enable_Voice_Based_Search,
            Enable_CMS_Page_Results,
            Enable_Barcode_Scanner,
            Enable_Save_For_Later,
            Enable_Product_Inheritance,
            Enable_Add_To_Cart_Option_For_Product_Sliders
        }

        //Convert datetime string to sqldatetime format.
        public static string ConvertStringToSqlDateFormat(string dateValue)
        {
            DateTime utcDateValue = new DateTime();
            if (DateTime.TryParse(dateValue, out utcDateValue))
                return utcDateValue.ToString(ZnodeConstant.SQLDateFormat);
            else
                return dateValue;
        }

        /// <summary>
        /// To get file path with url
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFilePath(string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                StringBuilder filePath = new StringBuilder();
                filePath.Append(ZnodeWebstoreSettings.ZnodeApiRootUri);
                filePath.Append(file);
                return filePath.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Get current datetime.
        /// </summary>
        /// <returns>Returns current date and time.</returns>
        public static DateTime GetDateTime() => DateTime.Now.Date;

        /// <summary>
        /// Get current datetime.
        /// </summary>
        /// <returns>Returns current date and time.</returns>
        public static DateTime GetDateWithTime() => DateTime.Now;

        /// <summary>
        /// Get current date.
        /// </summary>
        /// <returns>Returns current date</returns>
        public static DateTime GetDate() => DateTime.Now.Date;
        /// <summary>
        /// Check Property Exist in dynamic object
        /// </summary>
        /// <param name="settings">dynamic object</param>
        /// <param name="name">propertyname</param>
        /// <returns>bool - true if object has property</returns>
        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }

        /// <summary>
        /// Convert Data Table to Json.
        /// </summary>
        /// <param name="dataTable">dataTable</param>
        /// <returns>Return json string.</returns>
        public static string ToJson(this DataTable dataTable)
            => JsonConvert.SerializeObject(dataTable);

        /// <summary>
        /// To create HtppCookies with HttpOnly and Secure flag true
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// This method is obsolete instead of this method Use CookieHelper.SetCookie() method
        [Obsolete]
        public static HttpCookie CreateHttpCookies(string name, string value = "")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            HttpCookie httpCookie = new HttpCookie(name, value);

            return httpCookie;
        }

        /// <summary>
        /// Method to get the http cookies
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// This method is obsolete instead of this method Use CookieHelper.GetCookie() method
        [Obsolete]
        public static HttpCookie GetHttpCookies(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];

            //If cookie was not available in Request object, then check if it's being created in CURRENT response context and use that
            if (IsNull(cookie))
            {                
                cookie = HttpContext.Current.Response.Cookies[name];
            }

            if (cookie != null)
            {
                cookie.HttpOnly = ZnodeApiSettings.IsCookieHttpOnly;
                cookie.Secure = ZnodeApiSettings.IsCookieSecure;
            }
            return cookie;
        }

        /// <summary>
        /// Generate JSON string from data object 
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="t">Data object</param>
        /// <returns>JSONstring of object. and empty string if any Exception Occurs</returns>
        /// <exception cref="">Returns Empty string if exception occurs</exception>
        /// <example>string data = ToJSON(objectToSerialize);</example>
        public static string ToJSON(object t)
        {
            try
            {
                return JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return string.Empty;
            }
        }

        //Get PortalId from SiteConfig
        public static int GetPortalId()
        {
            string domainName = HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST")?.Trim() ?? GetPortalDomainName();
            return ZnodeConfigManager.GetSiteConfig(domainName)?.PortalId ?? 0;
        }

        //Get Domain Name by Request Headers
        public static string GetPortalDomainName()
        {
            const string headerDomainName = "Znode-DomainName";
            var headers = HttpContext.Current.Request.Headers;
            string domainName = headers[headerDomainName];
            return domainName;
        }

        public static string GetStringDateFormat() => GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString());

        //Try parse integer value 
        public static int TryParseInt32(this string value)
        {
            return TryParse<int>(value, int.TryParse);
        }

        //Get the header value against headername.
        public static string GetHeaderValue(string headerName)
        {
            NameValueCollection headers = HttpContext.Current.Request.Headers;
            return headers[headerName];
        }

        //Try parse integer value 
        public static Int16 TryParseInt16(this string value)
        {
            return TryParse<Int16>(value, Int16.TryParse);
        }

        //Try parse integer value 
        public static Int64 TryParseInt64(this string value)
        {
            return TryParse<Int64>(value, Int64.TryParse);
        }

        //Try parse byte value 
        public static byte TryParseByte(this string value)
        {
            return TryParse<byte>(value, byte.TryParse);
        }

        //Try parse boolean value 
        public static bool TryParseBoolean(this string value)
        {
            return TryParse<bool>(value, bool.TryParse);
        }

        //Try parse single value 
        public static Single TryParseSingle(this string value)
        {
            return TryParse<Single>(value, Single.TryParse);
        }

        //Try parse double value 
        public static Double TryParseDouble(this string value)
        {
            return TryParse<Double>(value, Double.TryParse);
        }

        //Try parse decimal value
        public static Decimal TryParseDecimal(this string value)
        {
            return TryParse<Decimal>(value, Decimal.TryParse);
        }

        //Try parse datetime value
        public static DateTime TryParseDateTime(this string value)
        {
            return TryParse<DateTime>(value, DateTime.TryParse);
        }

        /// <summary>
        /// Get provided Enum all fields names and descriptions and returns in Dictionary format
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns>IDictionary<string, string></returns>
        public static IDictionary<string, string> GetNamesAndDescriptionsFromEnum(Enum value)
        {
            IDictionary<string, string> enumFieldsDescriptions = new Dictionary<string, string>();

            foreach (FieldInfo fieldInfo in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumFieldsDescriptions.Add(fieldInfo.Name, GetDescriptionValue(fieldInfo));
            }

            return enumFieldsDescriptions;
        }

        /// <summary>
        /// Get description of the Enum field from FieldInfo object
        /// </summary>
        /// <param name="fieldInfo">FieldInfo</param>
        /// <returns>string</returns>
        public static string GetDescriptionValue(FieldInfo fieldInfo)
        {
            DescriptionAttribute[] descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (descriptionAttributes != null && descriptionAttributes.Length > 0)
            {
                return descriptionAttributes[0].Description;
            }

            return fieldInfo.Name.ToString();
        }

        /// <summary>
        /// Get description of the passed Enum field from Enum value
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns>string</returns>
        public static string GetEnumDescriptionValue(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString(), BindingFlags.Static | BindingFlags.Public);

            DescriptionAttribute[] descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (descriptionAttributes != null && descriptionAttributes.Length > 0)
            {
                return descriptionAttributes[0].Description;
            }

            return fieldInfo.Name.ToString();
        }

        // Perform a deep copy of the object, using Json as a serialization method.
        // In case of error in case of serialization, then it will return reference type originalObject as it
        public static T CreateDeepCloneObject<T>(T originalObject)
        {
            try
            {
                if (Object.ReferenceEquals(originalObject, null))
                {
                    return default(T);
                }

                // initialize inner objects individually. For example in default constructor some list property initialized with some values,
                // but in 'source' these items are cleaned. Without ObjectCreationHandling.Replace default constructor values will be added to result
                var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

                T deepObject = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(originalObject), deserializeSettings);

                return deepObject;
            }
            catch (Exception exception)
            {
                ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Error);
                return originalObject;
            }
        }

        //Get Random Characters
        public static string GetRandomCharacters()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const int randomSuffixLength = 3;
            string randomSuffix = new string(Enumerable.Repeat(characters, randomSuffixLength).Select(s => s[random.Next(s.Length)]).ToArray());
            return randomSuffix;
        }

        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source?.IndexOf(value, comparisonType) >= 0;
        }

        /// <summary>
        /// Validate ID Passed in QueryString with ID saved in Session
        /// </summary>
        /// <param name="idInRequestUrl">ID Passed In Query String e.g. If Someone Tamper Requested Query String,Need To Verify Query String Data With Data Saved In Database</param>
        /// <param name="idInDatabase">ID Exist In Session</param>
        /// <returns>Returns True If The Passed Values Is Equal, Else Return False</returns>
        public static bool IsValidIdInQueryString(int idInRequestUrl, int idInDatabase)
        {
            bool isIdValid = false;
            if (idInRequestUrl > 0 && idInDatabase > 0)
            {
                if (Equals(idInRequestUrl, idInDatabase))
                    isIdValid = true;
            }
            return isIdValid;
        }

        /// <summary>
        /// Replaces the "client_ip" header in current HttpRequest object with the "x-forwarded-for" header value
        /// </summary>
        public static void ReplaceProxyToClientIp()
        {
            try
            {
                // If a request is coming from a proxy server, "client_ip" header will have "proxy server's ip", in such case, replace it with actual IP received in "x-forwarded-for" header.
                var fwdIp = HttpContext.Current.Request.Headers["x-forwarded-for"];
                if (fwdIp != null)
                {
                    HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] = fwdIp;
                }
            }
            catch 
            {
                // Do not Throw any exception or  add a logs here 
            }

        }
        #endregion

        #region Private Method
        /// <summary>
        /// Set the Number at the end of the autogenerated unique key.
        /// As for User, it is mandatory to have atlease one number in the password
        /// </summary>
        /// <param name="result"></param>
        private static void SetNumberInUniqueKey(StringBuilder result)
        {
            Random rnd = new Random();
            int dice = rnd.Next(1, 9);
            result.Append(Convert.ToString(dice));
        }

        //Get domain data by domain id
        private static ZnodeDomain GetDomain(int domainId)
        {
            ZnodeRepository<ZnodeDomain> _domainRepository = new ZnodeRepository<ZnodeDomain>();
            ZnodeDomain domain = _domainRepository.GetById(domainId);
            return domain;
        }

        //Get cached keys.
        private static List<string> GetCachedItemsKeys(ZnodeDomain domain)
        {
            List<string> cacheItemsToRemove = new List<string>();
            cacheItemsToRemove.Add("SliderBannerKey_" + domain?.PortalId);
            cacheItemsToRemove.Add("ProductListKey_" + domain?.PortalId);
            cacheItemsToRemove.Add("LinkKey_" + domain?.PortalId);
            cacheItemsToRemove.Add("CategoryListKey_" + domain?.PortalId);
            cacheItemsToRemove.Add("TextWidgetKey_" + domain?.PortalId);
            cacheItemsToRemove.Add("TagManager_" + domain?.PortalId);
            cacheItemsToRemove.Add("SpecialsNavigation" + domain?.PortalId);
            cacheItemsToRemove.Add("BrandDropDownNavigation" + domain?.PortalId);
            cacheItemsToRemove.Add("PriceNavigation" + domain?.PortalId);
            cacheItemsToRemove.Add("BrandListKey_" + domain?.PortalId);
            return cacheItemsToRemove;
        }

        public static string GetDefaultGlobalDateTimeFormat(string key) => GetDefaultGlobalSetting(key);

        //Replace the token with amount string value having currency symbol.
        public static string ReplaceTokenWithCurrencyText(string key, string replaceValue, string resourceText, string cultureCode)
        {
            char currencySymbol = '$'; int charCount = 0;
            if (!string.IsNullOrEmpty(cultureCode))
            {
                System.Globalization.CultureInfo info = new System.Globalization.CultureInfo(cultureCode);
                currencySymbol = !string.IsNullOrEmpty(info?.NumberFormat?.CurrencySymbol) && info.NumberFormat.CurrencySymbol.Length > 0 ? info.NumberFormat.CurrencySymbol.First() : currencySymbol;
            }
            if (!string.IsNullOrEmpty(replaceValue))
                charCount = replaceValue.Count(f => f == currencySymbol);
            if (charCount == 1)
                replaceValue = currencySymbol + replaceValue;
            return ReplaceTokenWithMessageText(key, replaceValue, resourceText);
        }

        private static string GetDefaultGlobalSetting(string key)
        {
            DefaultGlobalConfigHelper defaultGlobalConfigHelper = new DefaultGlobalConfigHelper();
            string featureValue = defaultGlobalConfigHelper.GetDefaultGlobalFeatures(key);
            return featureValue;

        }

        /// <summary>
        /// Returns the Default Global Setting Values based on the key from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetDefaultGlobalSettingCacheData(string key)
        {
            //Get data from cache.           
            string globalSettingsFeature = string.Empty;
            if (!Equals(HttpRuntime.Cache["DefaultGlobalConfigCache"], null))
            {
                object defaultGlobalConfigCache = (object)HttpRuntime.Cache.Get("DefaultGlobalConfigCache");
                var _model = defaultGlobalConfigCache.GetType().GetProperty("DefaultGlobalConfigs").GetValue(defaultGlobalConfigCache, null);

                var defaultGlobalConfigsJson = JsonConvert.SerializeObject(_model);
                List<Dictionary<object, object>> defaultGlobalConfigsDictionary = JsonConvert.DeserializeObject<List<Dictionary<object, object>>>(defaultGlobalConfigsJson);
                var defaultGlobalConfigsRow = defaultGlobalConfigsDictionary.FirstOrDefault(pair => pair.ContainsKey("FeatureName") && pair.ContainsValue(key));

                if (!Equals(defaultGlobalConfigsRow, null))
                {
                    globalSettingsFeature = Convert.ToString(defaultGlobalConfigsRow.FirstOrDefault(pair => pair.Key.Equals("FeatureValues")).Value);
                }

            }
            return globalSettingsFeature;

        }

        /// <summary>
        /// Returns the Default Global Setting Values based on the key.
        /// </summary>
        /// <param name="key">Key in Global settings</param>
        /// <returns>Returns the Default Global Setting Values.</returns>
        private static string GetDefaultGlobalSettingData(string key)
        {
            DefaultGlobalConfigHelper defaultGlobalConfigHelper = new DefaultGlobalConfigHelper();
            //Get data from cache.           
            string globalSettingsFeature = string.Empty;
            globalSettingsFeature = GetDefaultGlobalSettingCacheData(key);

            //If cache is null then get from database
            if (Equals(globalSettingsFeature, null))
            {
                globalSettingsFeature = GetDefaultGlobalDateTimeFormat(key);
            }

            return globalSettingsFeature;

        }

        //Method to perform tryparse
        private static T TryParse<T>(this string value, ParseDelegate<T> parse) where T : struct
        {
            T result;
            parse(value, out result);
            return result;
        }

        //Function to encode data using base64 encryption
        public static string EncodeBase64(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        //Function to decode data using base64 decryption
        public static string DecodeBase64(string encodedValue) => Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));
        #endregion
    }
}
