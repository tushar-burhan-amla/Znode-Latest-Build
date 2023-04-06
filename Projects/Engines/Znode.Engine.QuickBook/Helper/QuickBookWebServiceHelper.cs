using System;
using System.Text.RegularExpressions;

namespace Znode.Engine.QuickBook
{
    public class QuickBookWebServiceHelper : IDisposable
    {
        #region Private Variables

        private bool isDisposed;

        #endregion Private Variables

        #region Constructor

        ~QuickBookWebServiceHelper()
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
        /// Gets array of string required in QuickBook authentication response
        /// </summary>
        /// <param name="userName">Username in QuickBook web connector</param>
        /// <param name="password">Password in QuickBook web connector</param>
        /// <returns></returns>
        public string[] GetQBAuthenticationResponseStringArray(string userName, string password)
        {
            string[] authReturn = new string[2];
            if (IsValidUser(userName, password, QuickBookConstants.Password))
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
        /// comparison of hresult error code and error code defined in constants. It is used while QuickBook connection error web method call
        /// </summary>
        /// <param name="hresult">Error code passed to QuickBook connection error web method call</param>
        /// <returns>Boolean value of comparison against available error code</returns>
        public bool StoppingWBConnectorCondition(string hresult)
        => (hresult.Trim().Equals(QuickBookConstants.QB_ERROR_WHEN_PARSING))
        || (hresult.Trim().Equals(QuickBookConstants.QB_COULDNT_ACCESS_QB))
        || (hresult.Trim().Equals(QuickBookConstants.QB_UNEXPECTED_ERROR));

        /// <summary>
        /// Request QuickBook web connector client version return message
        /// </summary>
        /// <param name="quickBookWebConnectorVersion"></param>
        /// <returns>
        /// - NULL or <emptyString> = QBWC will let the web service update
        /// - "E:<any text>" = popup ERROR dialog with <any text>
        /// 				- abort update and force download of new QBWC.
        /// - "W:<any text>" = popup WARNING dialog with <any text>
        /// 				- choice to user, continue update or not.
        /// </returns>
        public string RequestClientVerionReturnMessage(string quickBookWebConnectorVersion)
        {
            double suppliedVersion = Convert.ToDouble(this.parseForVersion(quickBookWebConnectorVersion ?? "0.0"));
            string responseMessage = (suppliedVersion < QuickBookConstants.SupportedMinVersion)
                                                      ? (QuickBookConstants.QBWebConnectorUpgradeRequiredMessage)
                                                      : ((suppliedVersion < QuickBookConstants.RecommendedVersion)
                                                                          ? QuickBookConstants.QBWebConnectorUpgradeRecommendationMessage
                                                                          : string.Empty);

            ZnodeLoggerUtility.LogInfoMessage($"Requested client version response message. Responded message is { responseMessage }");

            return responseMessage;
        }

        /// <summary>
        /// Saves last inserted sales order id in text file
        /// </summary>
        /// <param name="response">QB XMl response string received from QuickBook</param>
        public void LogSalesOrderRefNumber(string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                string insertedRefNumber = GetLastSalesOrderReferanceNumber(response);
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
        /// - NULL or <emptyString> = QBWC will let the web service update
        /// - "E:<any text>" = popup ERROR dialog with <any text>
        /// 				- abort update and force download of new QBWC.
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
        /// Gets sales order reference number from responded QuickBook xml string
        /// </summary>
        /// <param name="response">responded xml string from QuickBook</param>
        /// <returns>Reference number from responded QuickBook xml string</returns>
        private string GetLastSalesOrderReferanceNumber(string response)
        => (HelperUtility.ContainGivenNode(response, QuickBookConstants.SalesOrderAddXMLResponseAttribute))
          ? HelperUtility.GetGivenNodeText(response, QuickBookConstants.SOReferanceNumberXMLAttribute)
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
        /// <param name="quickBookUserName">Username passed from QuickBook web connector</param>
        /// <param name="quickBookPassword">Password passed from QuickBook web connector</param>
        /// <returns>String array required for valid authentication request</returns>
        private string[] GetValidQBAuthenticationResponseStringArray(string quickBookUserName, string quickBookPassword)
        {
            string[] authReturn = new string[2];
            authReturn[0] = AuthenticationHelper.Base64Encode(quickBookUserName + quickBookPassword);
            authReturn[1] = QuickBookConstants.QBAuthenticationPath;
            return authReturn;
        }

        // For simplicity of sample, a hardcoded username/password is used.
        // In real world, you should handle authentication in using a standard way.
        // For example, you could validate the username/password against an LDAP
        // or a directory server
        /// <summary>
        /// Checks if given credentials is valid or not against system credentials
        /// </summary>
        /// <param name="quickBookUserName">QuickBook username</param>
        /// <param name="quickBookPassword">QuickBook password</param>
        /// <param name="systemPassword">Password set in the system</param>
        /// <returns>Comparison of QuickBook password and Password set in the system</returns>
        private bool IsValidUser(string quickBookUserName, string quickBookPassword, string systemPassword)
        => quickBookUserName.Trim().Equals(QuickBookConstants.Username)
        && quickBookPassword.Trim().Equals(systemPassword);

        #endregion Private Methods
    }
}