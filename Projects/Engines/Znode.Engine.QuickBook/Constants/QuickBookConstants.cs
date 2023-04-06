using System;
using System.Collections.Specialized;
using System.Configuration;
using ZNode.Libraries.Framework.Business;

namespace Znode.Engine.QuickBook
{
    /// <summary>
    /// Constants used throughout the QuickBook engine
    /// </summary>
    public static class QuickBookConstants
    {
        private static NameValueCollection settings = ConfigurationManager.AppSettings;

        #region XML Attributes

        /// <summary>
        /// "Name", QB XML attribute for name
        /// </summary>
        public static string NameXMLAttribute { get; } = "Name";

        /// <summary>
        /// "RefNumber", QB XML attribute for sales order reference number
        /// </summary>
        public static string SOReferanceNumberXMLAttribute { get; } = "RefNumber";

        /// <summary>
        /// "CustomerAdd", QB XML attribute for adding customer
        /// </summary>
        public static string CustomerAddXMLAttribute { get; } = "CustomerAdd";

        /// <summary>
        /// "CustomerAddRq", QB XML attribute for adding customer request
        /// </summary>
        public static string CustomerAddRqXMLAttribute { get; } = "CustomerAddRq";

        /// <summary>
        /// "SalesOrderAddRq", QB XML attribute for adding sales order request
        /// </summary>
        public static string SalesOrderAddRqXMLAttribute { get; } = "SalesOrderAddRq";

        /// <summary>
        /// "CustomerAddRs", QB XML attribute for adding customer response from QB
        /// </summary>
        public static string CustomerAddXMLResponseAttribute { get; } = "CustomerAddRs";

        /// <summary>
        /// "ItemInventoryAddRs", QB XML attribute for adding inventory item response from QB
        /// </summary>
        public static string ItemInventoryAddXMLResponseAttribute { get; } = "ItemInventoryAddRs";

        /// <summary>
        /// ItemInventoryAddRq, QB XML attribute for adding inventory item request for QB
        /// </summary>
        public static string ItemInventoryAddRqXMLAttribute { get; } = "ItemInventoryAddRq";

        /// <summary>
        /// "SalesOrderAddRs", QB XML attribute for adding sales order response from QB
        /// </summary>
        public static string SalesOrderAddXMLResponseAttribute { get; } = "SalesOrderAddRs";

        #endregion XML Attributes

        #region Login Details

        /// <summary>
        /// Username from Znode used for accessing all records
        /// </summary>
        public static string QBAdminZnodeUsername { get; } = Convert.ToString(settings["QBAdminZnodeUsername"]);

        /// <summary>
        ///  Portal ID set in ZNode site configuration for accessing data
        /// </summary>
        public static int QBAdminZnodePortalId { get; } = ZNodeConfigManager.SiteConfig.PortalId;

        /// <summary>
        /// Username used for admin access in QB Web Connector
        /// </summary>
        public static string Username { get; } = Convert.ToString(settings["QBWebConnectorUsername"]);

        /// <summary>
        /// Password used for admin access in QB Web Connector
        /// </summary>
        public static string Password { get; } = Convert.ToString(settings["QBWebConnectorPassword"]);

        /// <summary>
        /// Conversion to the Base64 string for encoded UTF8 username+password i.e QuickBook login credentials.
        /// </summary>
        public static string ValidationToken { get; } = Convert.ToString(settings["QBWebConnectorValidationToken"]);

        /// <summary>
        /// Default path for QuickBook Authentication.
        /// </summary>
        public static string QBAuthenticationPath { get; } = Convert.ToString(settings["QBAuthenticationPath"]);

        #endregion Login Details

        #region Static constant data

        /// <summary>
        /// Gives "Status OK" string, this string is responded when requested xml is responded properly from QuickBook.
        /// </summary>
        public static string XMLQuickBookOKStatus { get; } = "Status OK";

        /// <summary>
        /// Gives "Error" string, this string is responded when requested xml is having some error for QuickBook.
        ///
        /// </summary>
        public static string XMLQuickBookErrorStatus { get; } = "Error";

        /// <summary>
        /// Gives "OK" string, this string is responded while closing connection from QuickBook web connector.
        /// </summary>
        public static string QuickBookOKResponseCode { get; } = "OK";

        /// <summary>
        /// "QuickBook" is set as default logging component.
        /// </summary>
        public static string QuickBookLoggingComponent { get; } = "QuickBook";

        /// <summary>
        /// "2.0.0.1", It's used for QB web connector
        /// </summary>
        public static string ServerVersion { get; } = "2.0.0.1";

        /// <summary>
        /// 1.5, It's used for QB web connector
        /// </summary>
        public static double RecommendedVersion { get; } = 1.5;

        /// <summary>
        /// 1.0, It's used for QB web connector
        /// </summary>
        public static double SupportedMinVersion { get; } = 1.0;

        #endregion Static constant data

        #region QuickBook Error Code

        /// <summary>
        /// 0x80040400 - QuickBooks found an error when parsing the provided XML text stream.
        /// </summary>
        public static string QB_ERROR_WHEN_PARSING { get; } = "0x80040400";

        /// <summary>
        /// 0x80040401 - Could not access QuickBooks.
        /// </summary>
        public static string QB_COULDNT_ACCESS_QB { get; } = "0x80040401";

        /// <summary>
        /// 0x80040402 - Unexpected error. Check the qbsdklog.txt file for possible, additional information.
        /// </summary>
        public static string QB_UNEXPECTED_ERROR { get; } = "0x80040402";

        // Add more as you need...

        #endregion QuickBook Error Code

        #region Quickbook Value Defaults

        /// <summary>
        /// "Cost of Goods Sold", this value is inserted in QuickBook and used as default for exporting data from ZNode to QuickBook
        /// </summary>
        public static string IncomeAccountType { get; } = "Cost of Goods Sold";

        /// <summary>
        /// "Cost of Goods Sold", this value is inserted in QuickBook and used as default for exporting data from ZNode to QuickBook
        /// </summary>
        public static string COGSAccountType { get; } = "Cost of Goods Sold";

        /// <summary>
        /// "Inventory Asset", this value is inserted in QuickBook and used as default for exporting data from ZNode to QuickBook
        /// </summary>
        public static string AssetAccountType { get; } = "Inventory Asset";

        /// <summary>
        /// "Out of State", this value is inserted in QuickBook and used as default for exporting data from ZNode to QuickBook
        /// </summary>
        public static string ItemSalesTaxType { get; } = "Out of State";

        #endregion Quickbook Value Defaults

        #region Default Constant Data

        /// <summary>
        /// Default QB date format is "yyyy-MM-dd"
        /// </summary>
        public static string DateFormat { get; } = "yyyy-MM-dd";

        public static string DefaultCustomerName { get; } = "Guest";

        public static string AscSortOrder { get; } = "ASC";

        /// <summary>
        /// Gives string "N/A", It an not be empty as in some cases empty string was not accepted by QB XML import.
        /// </summary>
        public static string DefaultNAText { get; } = "N/A";

        /// <summary>
        /// Page number 1 as default.
        /// </summary>
        public static string DefaultPaginationStartPage { get; } = "1";

        /// <summary>
        /// 10 records per page.
        /// </summary>
        public static string DefaultPaginationPageSize { get; } = "10";

        /// <summary>
        /// "W:We recommend that you upgrade your QBWebConnector", Message for up-gradation of QBWeb Connector recommendation
        /// </summary>
        public static string QBWebConnectorUpgradeRecommendationMessage { get; } = "W:We recommend that you upgrade your QBWebConnector";

        /// <summary>
        /// "E:You need to upgrade your QBWebConnector", Message for up-gradation of QBWeb Connector requirement
        /// </summary>
        public static string QBWebConnectorUpgradeRequiredMessage { get; } = "E:You need to upgrade your QBWebConnector";

        /// <summary>
        /// Default message for logging. It'll get message as "No data received from QuickBook".
        /// </summary>
        public static string DefaultNALogMessage { get; set; } = "No data received from QuickBook";

        #endregion Default Constant Data
    }
}