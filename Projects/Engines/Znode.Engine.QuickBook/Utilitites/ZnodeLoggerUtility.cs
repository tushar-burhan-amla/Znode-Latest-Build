using System;
using ZNode.Libraries.Framework.Business;

namespace Znode.Engine.QuickBook
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
        /// Implementation of IDisposibale interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        /// <summary>
        /// Log message against QuickBook component.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void LogInfoMessage(string message)
        => ZNodeLogging.
           LogMessage(message, QuickBookConstants.QuickBookLoggingComponent);

        /// <summary>
        /// Logs status message for insertion of given XML node in QuickBook
        /// </summary>
        /// <param name="responseXMLString">CML string received as response from QB web connector</param>
        public static void LogQBResponseActivityMessage(string responseXMLString)
        => ZNodeLogging.
           LogMessage(GetResponseStatusMessage(responseXMLString), QuickBookConstants.QuickBookLoggingComponent);

        /// <summary>
        /// Logs given object with help of Znode logger
        /// </summary>
        /// <param name="type">Type of object to be logged</param>
        /// <param name="objectInstance">Instance of logging object</param>
        public static void LogInfoObjct(Type type, object objectInstance)
        => ZNodeLogging.
           LogObject(type, objectInstance, QuickBookConstants.QuickBookLoggingComponent);

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns status message associated within given xml string in QuickBook
        /// </summary>
        /// <param name="response">Responded QuickBook xml string</param>
        /// <returns>
        /// Status OK: When everything worked fine !
        /// "Some error message" : If something went wrong
        /// </returns>
        private static string GetResponseStatusMessage(string response)
        {
            string statusMessage = QuickBookConstants.DefaultNALogMessage;
            //Customer
            if (HelperUtility.ContainGivenNode(response, QuickBookConstants.CustomerAddXMLResponseAttribute))
            {
                statusMessage = AddNewCustomerLogMessage(response);
            }

            //Item Inventory
            if (HelperUtility.ContainGivenNode(response, QuickBookConstants.ItemInventoryAddXMLResponseAttribute))
            {
                statusMessage = AddNewItemInventoryLogMessage(response);
            }

            //Sales Order
            if (HelperUtility.ContainGivenNode(response, QuickBookConstants.SalesOrderAddXMLResponseAttribute))
            {
                statusMessage = AddNewSalesOrderLogMessage(response);
            }

            return statusMessage;
        }

        /// <summary>
        /// Generates and returns log message for addition of new sales order in QuickBook
        /// </summary>
        /// <param name="response">Response XML string received from QuickBook</param>
        /// <returns>log message for addition of new sales order</returns>
        private static string AddNewSalesOrderLogMessage(string response)
        {
            string statusMessage = QuickBookConstants.DefaultNALogMessage;

            statusMessage = HelperUtility.GetStatusMessageOfGivenNode(response, QuickBookConstants.SalesOrderAddXMLResponseAttribute);
            if (statusMessage == QuickBookConstants.XMLQuickBookOKStatus)
            {
                statusMessage = $"Sales Order {  HelperUtility.GetGivenNodeText(response, QuickBookConstants.SOReferanceNumberXMLAttribute) } is successfully added in QuickBook";
            }

            return statusMessage;
        }

        /// <summary>
        /// Generates and returns log message for addition of new inventory item in QuickBook
        /// </summary>
        /// <param name="response">Response XML string received from QuickBook</param>
        /// <returns>log message for addition of new inventory item</returns>
        private static string AddNewItemInventoryLogMessage(string response)
        {
            string statusMessage = QuickBookConstants.DefaultNALogMessage;
            statusMessage = HelperUtility.GetStatusMessageOfGivenNode(response, QuickBookConstants.ItemInventoryAddXMLResponseAttribute);
            if (statusMessage == QuickBookConstants.XMLQuickBookOKStatus)
            {
                statusMessage = $"Item inventory { HelperUtility.GetGivenNodeText(response, QuickBookConstants.NameXMLAttribute) } is successfully added in QuickBook";
            }

            return statusMessage;
        }

        /// <summary>
        /// Generates and returns log message for addition of new customer in QuickBook
        /// </summary>
        /// <param name="response">Response XML string received from QuickBook</param>
        /// <returns>log message for addition of new customer in QuickBook</returns>
        private static string AddNewCustomerLogMessage(string response)
        {
            string statusMessage = QuickBookConstants.DefaultNALogMessage;
            statusMessage = HelperUtility.GetStatusMessageOfGivenNode(response, QuickBookConstants.CustomerAddXMLResponseAttribute);
            if (statusMessage == QuickBookConstants.XMLQuickBookOKStatus)
            {
                statusMessage = $"Customer { HelperUtility.GetGivenNodeText(response, QuickBookConstants.NameXMLAttribute) } is successfully added in QuickBook";
            }

            return statusMessage;
        }

        #endregion Private Methods
    }
}