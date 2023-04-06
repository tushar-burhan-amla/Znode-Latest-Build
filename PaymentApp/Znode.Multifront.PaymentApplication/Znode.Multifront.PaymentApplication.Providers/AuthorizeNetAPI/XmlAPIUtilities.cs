using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Znode.Multifront.PaymentApplication.Providers.AuthorizeNetAPI
{
    public class XmlApiUtilities
    {

        public static string MerchantName { get; set; }
        public static string TransactionKey { get; set; }

        public static void PopulateMerchantAuthentication(ANetApiRequest request, string merchantName, string transactionKey)
        {
            request.merchantAuthentication = new merchantAuthenticationType
            {
                name = merchantName,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = transactionKey
            };
        }
        public static bool ProcessXmlResponse(XmlDocument xmldoc, out object apiResponse)
        {
            bool bResult = true;

            apiResponse = null;

            try
            {
                // Use the root node to determine the type of response object to create
                XmlSerializer serializer;
                switch (xmldoc.DocumentElement.Name)
                {
                    case "ARBCreateSubscriptionResponse":
                        serializer = new XmlSerializer(typeof(ARBCreateSubscriptionResponse));
                        apiResponse = (ARBCreateSubscriptionResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "createCustomerPaymentProfileResponse":
                        serializer = new XmlSerializer(typeof(createCustomerPaymentProfileResponse));
                        apiResponse = (createCustomerPaymentProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "createCustomerProfileResponse":
                        serializer = new XmlSerializer(typeof(createCustomerProfileResponse));
                        apiResponse = (createCustomerProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "createCustomerProfileTransactionResponse":
                        serializer = new XmlSerializer(typeof(createCustomerProfileTransactionResponse));
                        apiResponse = (createCustomerProfileTransactionResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "createCustomerShippingAddressResponse":
                        serializer = new XmlSerializer(typeof(createCustomerShippingAddressResponse));
                        apiResponse = (createCustomerShippingAddressResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "deleteCustomerPaymentProfileResponse":
                        serializer = new XmlSerializer(typeof(deleteCustomerPaymentProfileResponse));
                        apiResponse = (deleteCustomerPaymentProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "deleteCustomerProfileResponse":
                        serializer = new XmlSerializer(typeof(deleteCustomerProfileResponse));
                        apiResponse = (deleteCustomerProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "deleteCustomerShippingAddressResponse":
                        serializer = new XmlSerializer(typeof(deleteCustomerShippingAddressResponse));
                        apiResponse = (deleteCustomerShippingAddressResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "getCustomerPaymentProfileResponse":
                        serializer = new XmlSerializer(typeof(getCustomerPaymentProfileResponse));
                        apiResponse = (getCustomerPaymentProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "getCustomerProfileIdsResponse":
                        serializer = new XmlSerializer(typeof(getCustomerProfileIdsResponse));
                        apiResponse = (getCustomerProfileIdsResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "getCustomerProfileResponse":
                        serializer = new XmlSerializer(typeof(getCustomerProfileResponse));
                        apiResponse = (getCustomerProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "getCustomerShippingAddressResponse":
                        serializer = new XmlSerializer(typeof(getCustomerShippingAddressResponse));
                        apiResponse = (getCustomerShippingAddressResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "isAliveResponse":
                        serializer = new XmlSerializer(typeof(isAliveResponse));
                        apiResponse = (isAliveResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "updateCustomerPaymentProfileResponse":
                        serializer = new XmlSerializer(typeof(updateCustomerPaymentProfileResponse));
                        apiResponse = (updateCustomerPaymentProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "updateCustomerProfileResponse":
                        serializer = new XmlSerializer(typeof(updateCustomerProfileResponse));
                        apiResponse = (updateCustomerProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "updateCustomerShippingAddressResponse":
                        serializer = new XmlSerializer(typeof(updateCustomerShippingAddressResponse));
                        apiResponse = (updateCustomerShippingAddressResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "validateCustomerPaymentProfileResponse":
                        serializer = new XmlSerializer(typeof(validateCustomerPaymentProfileResponse));
                        apiResponse = (validateCustomerPaymentProfileResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "getTransactionDetailsResponse":
                        serializer = new XmlSerializer(typeof(getTransactionDetailsResponse));
                        apiResponse = (getTransactionDetailsResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;
                    case "ErrorResponse":
                        serializer = new XmlSerializer(typeof(ANetApiResponse));
                        apiResponse = (ANetApiResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;

                    default:
                        Console.WriteLine($"Unexpected type of object: {xmldoc.DocumentElement.Name}");
                        bResult = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                bResult = false;
                apiResponse = null;
                Console.WriteLine($"{ex.GetType().ToString()}: {ex.Message}");
            }

            return bResult;
        }
        public static void ProcessResponse(object response)
        {
            // Every response is based on ANetApiResponse so you can always do this sort of type casting.
            var baseResponse = (ANetApiResponse)response;

            // Write the results to the console window
            Console.Write("Result: ");
            Console.WriteLine(baseResponse.messages.resultCode.ToString());

            // If the result code is "Ok" then the request was successfully processed.
            if (Equals(baseResponse.messages.resultCode, messageTypeEnum.Ok))
            {
                // CreateSubscription is the only method that returns additional data
                if (response.GetType() == typeof(createCustomerProfileResponse))
                {
                    var createResponse = (createCustomerProfileResponse)response;
                    // _subscriptionId = createResponse.subscriptionId;

                    // Console.WriteLine("Subscription ID: " + _subscriptionId);
                }
            }
            else
            {
                // Write error messages to console window
                for (int i = 0; i < baseResponse.messages.message.Length; i++)
                    Console.WriteLine($"[{baseResponse.messages.message[i].code}] {baseResponse.messages.message[i].text}");
            }
        }
        public static bool PostRequest(object ApiRequest, out XmlDocument xmldoc, bool gatewayTestMode)
        {
            bool bResult = false;
            XmlSerializer serializer;

            xmldoc = null;

            try
            {
                string API_URL = (gatewayTestMode) ? Convert.ToString(ConfigurationManager.AppSettings["AuthorizeNetAPIURL"]) : Convert.ToString(ConfigurationManager.AppSettings["AuthorizeNetAPILiveURL"]);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(API_URL);
                webRequest.Method = "POST";
                webRequest.ContentType = "text/xml";
                webRequest.KeepAlive = true;

                // Serialize the request
                serializer = new XmlSerializer(ApiRequest.GetType());
                XmlWriter writer = new XmlTextWriter(webRequest.GetRequestStream(), Encoding.UTF8);
                serializer.Serialize(writer, ApiRequest);
                writer.Close();

                // Get the response
                WebResponse webResponse = webRequest.GetResponse();

                // Load the response from the API server into an XmlDocument.
                xmldoc = new XmlDocument();
                xmldoc.Load(XmlReader.Create(webResponse.GetResponseStream()));

                bResult = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().ToString()}: {ex.Message}");
                bResult = false;
            }

            return bResult;
        }
    }
}
