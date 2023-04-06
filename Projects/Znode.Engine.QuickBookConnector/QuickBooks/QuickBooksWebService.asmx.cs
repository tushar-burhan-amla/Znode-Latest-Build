using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Services;
using Znode.Engine.ERPConnector;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.QuickBookConnector.QuickBooks
{
    /// <summary>
    /// Summary description for DemoService
    /// </summary>
    [WebService(Namespace = "http://developer.intuit.com/",
         Name = "WCWebService",
         Description = "WebService for QuickBooks WebConnector")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    // [System.Web.Script.Services.ScriptService]
    public class QuickBooksWebService : WebService
    {
        #region Private Variables

        private readonly QuickBooksWebServiceHelper _quickBooksWebServiceHelper;

        #endregion Private Variables

        #region Constructor

        public QuickBooksWebService()
        {
            _quickBooksWebServiceHelper = new QuickBooksWebServiceHelper();
        }

        #endregion Constructor

        public int count = 0;

        /// <summary>
        /// WebMethod - serverVersion()
        /// To enable web service with its version number returned back to QBWC
        /// Signature: public string serverVersion()
        ///
        /// OUT:
        /// string
        /// Possible values:
        /// Version string representing server version
        /// </summary>
        [WebMethod]
        public string serverVersion()
        => RequestServerVersion();

        /// <summary>
        /// WebMethod - clientVersion()
        /// To enable web service with QBWC version control
        /// Signature: public string clientVersion(string strVersion)
        ///
        /// IN:
        /// string strVersion
        ///
        /// OUT:
        /// string errorOrWarning
        /// Possible values:
        /// string retVal
        /// - NULL or <emptyString> = QBWC will let the web service update
        /// - "E:<any text>" = popup ERROR dialog with <any text>
        ///					- abort update and force download of new QBWC.
        /// - "W:<any text>" = popup WARNING dialog with <any text>
        ///					- choice to user, continue update or not.
        /// </summary>
        [WebMethod]
        public string clientVersion(string strVersion)
        => _quickBooksWebServiceHelper.RequestClientVersionReturnMessage(strVersion);

        /// <summary>
        /// WebMethod - authenticate()
        /// To verify username and password for the web connector that is trying to connect
        /// Signature: public string[] authenticate(string strUserName, string strPassword)
        ///
        /// IN:
        /// string strUserName
        /// string strPassword
        ///
        /// OUT:
        /// string[] authReturn
        /// Possible values:
        /// string[0] = ticket
        /// string[1]
        /// - empty string = use current company file
        /// - "none" = no further request/no further action required
        /// - "nvu" = not valid user
        /// - any other string value = use this company file
        /// </summary>
        [WebMethod]
        public string[] authenticate(string strUserName, string strPassword)
        => _quickBooksWebServiceHelper.GetQBAuthenticationResponseStringArray(strUserName, strPassword);

        /// <summary>
        /// WebMethod - connectionError()
        /// To facilitate capturing of QuickBooks error and notifying it to web services
        /// Signature: public string connectionError (string ticket, string hresult, string message)
        ///
        /// IN:
        /// string ticket = A GUID based ticket string to maintain identity of QBWebConnector
        /// string hresult = An HRESULT value thrown by QuickBooks when trying to make connection
        /// string message = An error message corresponding to the HRESULT
        ///
        /// OUT:
        /// string retVal
        /// Possible values:
        /// - “done” = no further action required from QBWebConnector
        /// - any other string value = use this name for company file
        /// </summary>
        [WebMethod(Description = "This web method facilitates web service to handle connection error between QuickBooks and QBWebConnector",
                   EnableSession = true)]
        public string connectionError(string ticket, string hresult, string message)
        => GetQuickBooksErrorMessage(hresult, message);

        /// <summary>
        /// WebMethod - sendRequestXML()
        /// Signature: public string sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName,
        /// string Country, int qbXMLMajorVers, int qbXMLMinorVers)
        ///
        /// IN:
        /// int qbXMLMajorVers
        /// int qbXMLMinorVers
        /// string ticket
        /// string strHCPResponse
        /// string strCompanyFileName
        /// string Country
        /// int qbXMLMajorVers
        /// int qbXMLMinorVers
        ///
        /// OUT:
        /// string request
        /// Possible values:
        /// - “any_string” = Request XML for QBWebConnector to process
        /// - "" = No more request XML
        /// </summary>
        [WebMethod(Description = "This web method facilitates web service to send request XML to QuickBooks via QBWebConnector",
                   EnableSession = true)]
        public string sendRequestXML(
            string ticket,
            string strHCPResponse,
            string strCompanyFileName,
            string qbXMLCountry,
            int qbXMLMajorVers,
            int qbXMLMinorVers)
        => GetQBCompatibleXMLRequestString(ticket);

        /// <summary>
        /// WebMethod - receiveResponseXML()
        /// Signature: public int receiveResponseXML(string ticket, string response, string hresult, string message)
        ///
        /// IN:
        /// string ticket
        /// string response
        /// string hresult
        /// string message
        ///
        /// OUT:
        /// int retVal
        /// Greater than zero  = There are more request to send
        /// 100 = Done. no more request to send
        /// Less than zero  = Custom Error codes
        /// </summary>
        [WebMethod(Description = "This web method facilitates web service to receive response XML from QuickBooks via QBWebConnector",
                   EnableSession = true)]
        public int receiveResponseXML(string ticket, string response, string hresult, string message)
        => LogMessageAndGetRemainingPercentage(ticket, response, hresult, message);

        /// <summary>
        /// WebMethod - getLastError()
        /// Signature: public string getLastError(string ticket)
        ///
        /// IN:
        /// string ticket
        ///
        /// OUT:
        /// string retVal
        /// Possible Values:
        /// Error message describing last web service error
        /// </summary>
        [WebMethod]
        public string getLastError(string ticket)
        => null;

        /// <summary>
        /// WebMethod - closeConnection()
        /// At the end of a successful update session, QBWebConnector will call this web method.
        /// Signature: public string closeConnection(string ticket)
        ///
        /// IN:
        /// string ticket
        ///
        /// OUT:
        /// string closeConnection result
        /// </summary>
        [WebMethod]
        public string closeConnection(string ticket)
        => GetCloseConncetionCode();

        #region private Methods

        /// <summary>
        /// Logs activity message in Znode logger and calculates remaining percentage for QuickBook
        /// </summary>
        /// <param name="ticket">Token passed in request header</param>
        /// <param name="response">XML string received from QuickBook web connector</param>
        /// <param name="hresult">Error code received from QuickBook web connector</param>
        /// <param name="message">Response message received from QuickBook web connector</param>
        /// <returns></returns>
        private int LogMessageAndGetRemainingPercentage(string ticket, string response, string hresult, string message)
        {
            //Log response message
            if (!string.IsNullOrEmpty(message))
            {
                ZnodeLoggerUtility.LogInfoMessage("There is a message(" + message + ") against xml: " + GetCurrentRequestXMLString(ticket));
            }
            else
                ZnodeLoggerUtility.LogQBResponseActivityMessage(response, GetCurrentRequestXMLString(ticket));

            return GetQBResponseRemaingPercentage(ticket, response, hresult);
        }

        /// <summary>
        /// Gives requested xml string to the QuickBook
        /// </summary>
        /// <param name="ticket">Token available in request header</param>
        /// <returns>Requested xml string</returns>
        private string GetCurrentRequestXMLString(string ticket)
        {
            List<String> req = buildRequestQueue(ticket);
            count = Convert.ToInt32(Session["counter"]) - 1;

            return (count < req.Count) ? req[count].ToString() : string.Empty;
        }

        /// <summary>
        /// Receives remaining percentage from received response
        /// </summary>
        /// <param name="ticket">Token available in request header</param>
        /// <param name="response">Responded XML string received from QuickBook</param>
        /// <param name="hresult">Error code</param>
        /// <returns>Remaining percentage to be requested in the queue</returns>
        private int GetQBResponseRemaingPercentage(string ticket, string response, string hresult)
        {
            _quickBooksWebServiceHelper.LogSalesOrderRefNumber(response);

            return AuthenticationHelper.ValidateTicket(ticket) ? GenerateRemainingPercentage(ticket, hresult) : -101;
        }

        // Depending on various hresults return different value
        // retVal = "" means : Try again with this company file
        /// <summary>
        /// Generates quick book error message
        /// </summary>
        /// <param name="hresult">Error code received</param>
        /// <param name="message">Error message received</param>
        /// <returns>Quick book error message</returns>
        private string GetQuickBooksErrorMessage(string hresult, string message)
        {
            if (Session["ce_counter"] == null)
            {
                Session["ce_counter"] = 0;
            }

            string returnValue = (_quickBooksWebServiceHelper.StoppingWBConnectorCondition(hresult)) ?
                                 ("DONE") :
                                 (((int)Session["ce_counter"] == 0) ? (string.Empty) : ("DONE"));

            ZnodeLoggerUtility.LogInfoMessage("QuickBooks connection error occurred, message is \"" + message + "\", responded code is \"" + returnValue + "\"");

            return returnValue;
        }

        /// <summary>
        /// Gets QuickBooks compatible xml request string from the given queue
        /// </summary>
        /// <param name="ticket">Token available in request header</param>
        /// <returns>QuickBooks compatible xml request string</returns>
        private string GetQBCompatibleXMLRequestString(string ticket)
        {
            string request = string.Empty;
            if (AuthenticationHelper.ValidateTicket(ticket))
            {
                if (Session["counter"] == null)
                    Session["counter"] = 0;

                List<String> requestQueue = buildRequestQueue(ticket);
                request = GenerateSingleRequestQBXMLString(requestQueue);
            }
            return request;
        }

        /// <summary>
        /// Generates single serving request from request queue
        /// </summary>
        /// <param name="requestQueue">Request queue</param>
        /// <returns>Single serving request xml string</returns>
        private string GenerateSingleRequestQBXMLString(List<string> requestQueue)
        {
            string request = "";
            int total = requestQueue.Count;
            count = Convert.ToInt32(Session["counter"]);

            if (count < total)
            {
                request = requestQueue[count].ToString();
                Session["counter"] = ((int)Session["counter"]) + 1;

                ZnodeLoggerUtility.LogInfoMessage("Request number " + count + " out of total " + total + " requests is sent to QuickBook.");
            }
            else
            {
                clearRequestQueue();
                request = "";
            }

            return request;
        }

        // retVal = -101; means : if there is an error with response received, web service could also return a -ve int
        /// <summary>
        ///
        /// Calculate remaining percentage or return -ve percentage for null hresult
        /// </summary>
        /// <param name="ticket">Token received in request header</param>
        /// <param name="hresult">Result code received from QuickBooks</param>
        /// <returns>Remaining percentage </returns>
        private int GenerateRemainingPercentage(string ticket, string hresult)
        {
            if (!string.IsNullOrEmpty(hresult))
            {
                List<String> req = buildRequestQueue(ticket);
                int count = Convert.ToInt32(Session["counter"]);
                string reqXML = req[count];
                if (!string.IsNullOrEmpty(reqXML))
                {
                    QuickBooksWebServiceHelper quickBooksWebServiceHelper = new QuickBooksWebServiceHelper();
                    string id = quickBooksWebServiceHelper.GetSalesOrderReferenceNumber(reqXML);
                    ZnodeLoggerUtility.LogInfoMessage("Error : Generated queue of " + req.Count + " requests and id is " + id + " for not inserting them into QuickBooks. Request XML is : " + reqXML);
                }
            }
            return CalculateRemainingPercentage(ticket);
        }
        /// <summary>
        /// Clears all session related data
        /// </summary>
        private void clearRequestQueue()
        {
            count = 0;
            Session["counter"] = 0;
            Session["requestQueue"] = null;
        }

        /// <summary>
        /// calculate remaining percentage in the request queue
        /// </summary>
        /// <param name="ticket">Token available in request header</param>
        /// <returns>Remaining Percentage</returns>
        private int CalculateRemainingPercentage(string ticket)
        {
            int retVal;
            List<String> req = buildRequestQueue(ticket);
            int total = req.Count;
            int count = Convert.ToInt32(Session["counter"]);

            int percentage = (count * 100) / total;
            if (percentage >= 100)
            {
                clearRequestQueue();
            }
            retVal = percentage;
            return retVal;
        }

        /// <summary>
        /// Generates new request queue or fetch available queue from session
        /// </summary>
        /// <param name="ticket">Token available in request header</param>
        /// <returns>QuickBooks XML request string</returns>
        private List<String> buildRequestQueue(string ticket)
        {
            if (Session["requestQueue"] == null)
            {
                Session["requestQueue"] = new List<string>();
            }

            List<String> req = (List<string>)Session["requestQueue"];

            if (req.Count == 0)
            {
                HttpContext.Current.Request.Headers.Add("Znode-UserId", (new AuthenticationHelper().GetQBZnodeAdminUserId()).ToString());
                req = (new SalesOrderMapper()).GetSalesOrderAddRqXML(ticket);
                Session["requestQueue"] = req;
                
                ZnodeLoggerUtility.LogInfoMessage("Generated queue of " + req.Count + " requests for inserting them into QuickBooks.");
            }
            return req;
        }

        /// <summary>
        /// Gets the close connection code
        /// </summary>
        /// <returns>Close connection code</returns>
        private string GetCloseConncetionCode()
        {
            ZnodeLoggerUtility.LogInfoMessage("Connection with QuickBooks web-connector is closed.");
            return QuickBooksConstants.QuickBooksOKResponseCode;
        }

        /// <summary>
        /// Request available server version
        /// </summary>
        /// <returns>Available server version</returns>
        private string RequestServerVersion()
        {
            ZnodeLoggerUtility.LogInfoMessage("Requested server version. available server version is " + QuickBooksConstants.ServerVersion);
            return QuickBooksConstants.ServerVersion;
        }

        #endregion private Methods
    }
}