using System;
using System.Text.RegularExpressions;

namespace Znode.Engine.ERPConnector
{
    public class QuickBooksWebServiceHelper : IDisposable
    {
        #region Private Variables

        private bool isDisposed;

        #endregion Private Variables

        #region Constructor

        ~QuickBooksWebServiceHelper()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Implementation of IDisposable
        /// </summary>
        public void Dispose() => isDisposed = true;

        // You could also return "none" to indicate there is no work to do
        // or a company filename in the format C:\full\path\to\company.qbw
        // based on your program logic and requirements.
        /// <summary>
        /// Gets array of string required in QuickBooks authentication response
        /// </summary>
        /// <param name="userName">Username in QuickBooks web connector</param>
        /// <param name="password">Password in QuickBooks web connector</param>
        /// <returns></returns>
        public string[] GetQBAuthenticationResponseStringArray(string userName, string password)
        {
            string[] authReturn = new string[2];
            if (IsValidUser(userName, password, QuickBooksConstants.Password))
            {
                ZnodeLoggerUtility.LogInfoMessage($"Authentication requested for username: { userName }");
                authReturn = GetValidQBAuthenticationResponseStringArray(userName, password);
            }
            else
            {
                authReturn = GetInvalidQBAuthenticationResponseStringArray();
            }
            return authReturn;
        }

        /// <summary>
        /// comparison of hresult error code and error code defined in constants. It is used while QuickBooks connection error web method call
        /// </summary>
        /// <param name="hresult">Error code passed to QuickBooks connection error web method call</param>
        /// <returns>Boolean value of comparison against available error code</returns>
        public bool StoppingWBConnectorCondition(string hresult)
        => (hresult.Trim().Equals(QuickBooksConstants.QB_ERROR_WHEN_PARSING))
        || (hresult.Trim().Equals(QuickBooksConstants.QB_COULDNT_ACCESS_QB))
        || (hresult.Trim().Equals(QuickBooksConstants.QB_UNEXPECTED_ERROR));

        /// <summary>
        /// Request QuickBooks web connector client version return message
        /// </summary>
        /// <param name="quickBooksWebConnectorVersion"></param>
        /// <returns>
        /// - NULL or <emptyString> = QuickBooks web connector will let the web service update
        /// - "E:<any text>" = popup ERROR dialog with <any text>
        /// 				- abort update and force download of new QuickBooks web connector.
        /// - "W:<any text>" = popup WARNING dialog with <any text>
        /// 				- choice to user, continue update or not.
        /// </returns>
        public string RequestClientVersionReturnMessage(string quickBooksWebConnectorVersion)
        {
            double suppliedVersion = Convert.ToDouble(this.parseForVersion(quickBooksWebConnectorVersion ?? "0.0"));
            string responseMessage = (suppliedVersion < QuickBooksConstants.SupportedMinVersion)
                                                      ? (QuickBooksConstants.QBWebConnectorUpgradeRequiredMessage)
                                                      : ((suppliedVersion < QuickBooksConstants.RecommendedVersion)
                                                                          ? QuickBooksConstants.QBWebConnectorUpgradeRecommendationMessage
                                                                          : string.Empty);

            ZnodeLoggerUtility.LogInfoMessage($"Requested client version response message. Responded message is { responseMessage }");

            return responseMessage;
        }

        /// <summary>
        /// Saves last inserted sales order id in text file
        /// </summary>
        /// <param name="response">QB XMl response string received from QuickBooks</param>
        public void LogSalesOrderRefNumber(string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                string insertedRefNumber = GetLastSalesOrderReferenceNumber(response);
                if (!string.IsNullOrEmpty(insertedRefNumber))
                    (new TextFileUtility()).WriteRefNumberInTextFile(insertedRefNumber);
            }
        }

        /// <summary>
        /// This method is created just to parse the first two version components
        /// out of the standard four component version number: <Major>.<Minor>.<Release>.<Build>
        /// As long as you get the version in right format, you could use any algorithm here.
        /// </summary>
        /// <param name="versionComponent"></param>
        /// <returns>
        /// - NULL or <emptyString> = QuickBooks web connector will let the web service update
        /// - "E:<any text>" = popup ERROR dialog with <any text>
        /// 				- abort update and force download of new QuickBooks web connector.
        /// - "W:<any text>" = popup WARNING dialog with <any text>
        /// 				- choice to user, continue update or not.
        /// </returns>
        public string parseForVersion(string versionComponent)
        {
            string returnValue = string.Empty;
            string major = string.Empty;
            string minor = string.Empty;
            Regex version = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)(\.\w+){0,2}$", RegexOptions.Compiled);
            Match versionMatch = version.Match(versionComponent);
            if (versionMatch.Success)
            {
                major = versionMatch.Result("${major}");
                minor = versionMatch.Result("${minor}");
                returnValue = major + "." + minor;
            }
            else
                returnValue = versionComponent;
            return returnValue;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets sales order reference number from responded QuickBooks xml string
        /// </summary>
        /// <param name="response">responded xml string from QuickBooks</param>
        /// <param name="requestXml">requested xml string from QuickBooks</param>
        /// <returns>Reference number from responded QuickBooks xml string</returns>
        public string GetLastSalesOrderReferenceNumber(string response)
        => (HelperUtility.ContainGivenNode(response, QuickBooksConstants.SalesOrderAddXMLResponseAttribute))
                ? HelperUtility.GetGivenNodeText(response, QuickBooksConstants.SOReferenceNumberXMLAttribute)
                : string.Empty;

        /// <summary>
        /// Gets sales order reference number from responded QuickBooks xml string
        /// </summary>
        /// <param name="response">responded xml string from QuickBooks</param>
        /// <param name="requestXml">requested xml string from QuickBooks</param>
        /// <returns>Reference number from responded QuickBooks xml string</returns>
        public string GetSalesOrderReferenceNumber(string requestXml)
        => (HelperUtility.ContainGivenNode(requestXml, QuickBooksConstants.SOReferenceNumberXMLAttribute))
                 ? HelperUtility.GetGivenNodeText(requestXml, QuickBooksConstants.SOReferenceNumberXMLAttribute)
                 : string.Empty;

        // Code below uses a random GUID to use as session ticket
        // An example of a GUID is {85B41BEE-5CD9-427a-A61B-83964F1EB426}
        /// <summary>
        /// String array for response message of invalid authentication request
        /// </summary>
        /// <returns>String array with default authentication data</returns>
        private string[] GetInvalidQBAuthenticationResponseStringArray()
        {
            string[] authReturn = new string[2];
            authReturn[0] = Guid.NewGuid().ToString();
            authReturn[1] = "nvu";//Not Valid User
            return authReturn;
        }

        // An empty string for authReturn[1] means asking QBWebConnector
        // to connect to the company file that is currently opened in QB
        /// <summary>
        /// String array for response message of valid authentication request
        /// </summary>
        /// <param name="quickBooksUserName">Username passed from QuickBooks web connector</param>
        /// <param name="quickBooksPassword">Password passed from QuickBooks web connector</param>
        /// <returns>String array required for valid authentication request</returns>
        private string[] GetValidQBAuthenticationResponseStringArray(string quickBooksUserName, string quickBooksPassword)
        {
            string[] authReturn = new string[2];
            authReturn[0] = AuthenticationHelper.Base64Encode(quickBooksUserName + quickBooksPassword);
            authReturn[1] = QuickBooksConstants.QBAuthenticationPath;
            return authReturn;
        }

        // For simplicity of sample, a hardcoded username/password is used.
        // In real world, you should handle authentication in using a standard way.
        // For example, you could validate the username/password against an LDAP
        // or a directory server
        /// <summary>
        /// Checks if given credentials is valid or not against system credentials
        /// </summary>
        /// <param name="quickBooksUserName">QuickBooks username</param>
        /// <param name="quickBooksPassword">QuickBooks password</param>
        /// <param name="systemPassword">Password set in the system</param>
        /// <returns>Comparison of QuickBooks password and Password set in the system</returns>
        private bool IsValidUser(string quickBooksUserName, string quickBooksPassword, string systemPassword)
        => quickBooksUserName.Trim().Equals(QuickBooksConstants.Username)
        && quickBooksPassword.Trim().Equals(systemPassword);

        #endregion Private Methods
    }
}