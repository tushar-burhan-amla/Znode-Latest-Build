using System;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.ERPConnector
{
    public class ZnodeLoggerUtility : IDisposable
    {
        #region Private Variables

        private bool isDisposed;

        #endregion Private Variables

        #region Constructor

        ~ZnodeLoggerUtility()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Implementation of IDisposable interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        /// <summary>
        /// Log message against QuickBooks component.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void LogInfoMessage(string message)
        => ZnodeLogging.LogMessage(message, QuickBooksConstants.QuickBooksLoggingComponent);

        /// <summary>
        /// Logs status message for insertion of given XML node in QuickBooks
        /// </summary>
        /// <param name="responseXMLString">CML string received as response from QB web connector</param>
        /// <param name="requestXML">Requested QuickBooks xml string</param>
        public static void LogQBResponseActivityMessage(string responseXMLString, string requestXML)
        => ZnodeLogging.LogMessage(GetResponseStatusMessage(responseXMLString, requestXML), QuickBooksConstants.QuickBooksLoggingComponent);

        /// <summary>
        /// Logs given object with help of Znode logger
        /// </summary>
        /// <param name="type">Type of object to be logged</param>
        /// <param name="objectInstance">Instance of logging object</param>
        public static void LogInfoObject(Type type, object objectInstance)
        => ZnodeLogging.LogObject(type, objectInstance, QuickBooksConstants.QuickBooksLoggingComponent);

        /// <summary>
        /// Log Error message against QuickBooks component.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void LogErrorMessage(string message)
        => ZnodeLogging.LogMessage(message, QuickBooksConstants.QuickBooksErrorLoggingComponent);
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns status message associated within given xml string in QuickBooks
        /// </summary>
        /// <param name="response">Responded QuickBooks xml string</param>
        /// <param name="requestXML">Requested QuickBooks xml string</param>
        /// <returns>
        /// Status OK: When everything worked fine !
        /// "Some error message" : If something went wrong
        /// </returns>
        private static string GetResponseStatusMessage(string response, string requestXML)
        {
            string statusMessage = QuickBooksConstants.DefaultNALogMessage;
            //Customer
            if (HelperUtility.ContainGivenNode(response, QuickBooksConstants.CustomerAddXMLResponseAttribute))
                statusMessage = AddNewCustomerLogMessage(response, requestXML);

            //Item Inventory
            if (HelperUtility.ContainGivenNode(response, QuickBooksConstants.ItemInventoryAddXMLResponseAttribute))
                statusMessage = AddNewItemInventoryLogMessage(response, requestXML);

            //Sales Order
            if (HelperUtility.ContainGivenNode(response, QuickBooksConstants.SalesOrderAddXMLResponseAttribute))
                statusMessage = AddNewSalesOrderLogMessage(response, requestXML);

            return statusMessage;
        }

        /// <summary>
        /// Generates and returns log message for addition of new sales order in QuickBooks
        /// </summary>
        /// <param name="response">Response XML string received from QuickBooks</param>
        /// <param name="requestXML">Requested QuickBooks xml string</param>
        /// <returns>log message for addition of new sales order</returns>
        private static string AddNewSalesOrderLogMessage(string response, string requestXML)
        {
            string statusMessage = QuickBooksConstants.DefaultNALogMessage;

            statusMessage = HelperUtility.GetStatusMessageOfGivenNode(response, QuickBooksConstants.SalesOrderAddXMLResponseAttribute);
            string statusCode = HelperUtility.GetStatusCodeOfGivenNode(response, QuickBooksConstants.SalesOrderAddXMLResponseAttribute);

            if (statusMessage == QuickBooksConstants.XMLQuickBooksOKStatus)
                statusMessage = $"Sales Order {  HelperUtility.GetGivenNodeText(response, QuickBooksConstants.SOReferenceNumberXMLAttribute) } is successfully added in QuickBooks";

            if (statusCode == QuickBooksConstants.QuickBooksStatusCode_Error)
            {
                string refNumber = (new QuickBooksWebServiceHelper()).GetSalesOrderReferenceNumber(requestXML);
                LogErrorMessage($"Error : Sales Order { HelperUtility.GetGivenNodeText(response, QuickBooksConstants.SOReferenceNumberXMLAttribute) } is not successfully added in QuickBooks . {statusMessage} Reference Number : {refNumber}. Request XML is : {  requestXML }");
            }
            return statusMessage;
        }

        /// <summary>
        /// Generates and returns log message for addition of new inventory item in QuickBooks
        /// </summary>
        /// <param name="response">Response XML string received from QuickBooks</param>
        /// <param name="requestXML">Requested QuickBooks xml string</param>
        /// <returns>log message for addition of new inventory item</returns>
        private static string AddNewItemInventoryLogMessage(string response, string requestXML)
        {
            string statusMessage = QuickBooksConstants.DefaultNALogMessage;
            statusMessage = HelperUtility.GetStatusMessageOfGivenNode(response, QuickBooksConstants.ItemInventoryAddXMLResponseAttribute);
            string statusCode = HelperUtility.GetStatusCodeOfGivenNode(response, QuickBooksConstants.ItemInventoryAddXMLResponseAttribute);

            if (statusMessage == QuickBooksConstants.XMLQuickBooksOKStatus)
                statusMessage = $"Item inventory { HelperUtility.GetGivenNodeText(response, QuickBooksConstants.NameXMLAttribute) } is successfully added in QuickBooks";

            if (statusCode == QuickBooksConstants.QuickBooksStatusCode_Error)
                LogErrorMessage($"Error : Item inventory { HelperUtility.GetGivenNodeText(response, QuickBooksConstants.NameXMLAttribute) } is not successfully added in QuickBooks . {statusMessage}  Request XML is : {  requestXML }");

            return statusMessage;
        }

        /// <summary>
        /// Generates and returns log message for addition of new customer in QuickBooks
        /// </summary>
        /// <param name="response">Response XML string received from QuickBooks</param>
        /// <param name="requestXML">Requested QuickBooks xml string</param>
        /// <returns>log message for addition of new customer in QuickBooks</returns>
        private static string AddNewCustomerLogMessage(string response, string requestXML)
        {
            string statusMessage = QuickBooksConstants.DefaultNALogMessage;
            statusMessage = HelperUtility.GetStatusMessageOfGivenNode(response, QuickBooksConstants.CustomerAddXMLResponseAttribute);
            string statusCode = HelperUtility.GetStatusCodeOfGivenNode(response, QuickBooksConstants.CustomerAddXMLResponseAttribute);

            if (statusMessage == QuickBooksConstants.XMLQuickBooksOKStatus)
                statusMessage = $"Customer { HelperUtility.GetGivenNodeText(response, QuickBooksConstants.NameXMLAttribute) } is successfully added in QuickBooks";

            if (statusCode == QuickBooksConstants.QuickBooksStatusCode_Error)
                LogErrorMessage($"Error : Customer { HelperUtility.GetGivenNodeText(requestXML, QuickBooksConstants.NameXMLAttribute) } is not successfully added in QuickBooks . {statusMessage} Request XML is : {  requestXML }");

            return statusMessage;
        }

        #endregion Private Methods
    }
}