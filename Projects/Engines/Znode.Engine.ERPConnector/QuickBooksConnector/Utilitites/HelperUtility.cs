using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.ERPConnector
{
    internal static class HelperUtility
    {
        #region Public Methods

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

                StringWriter stringwriter = new StringWriter();
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
                ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Error, ex);
                return string.Empty;
            }
        }

        /// <summary>
        ///map Xml to Model
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="xmlString">xmlString</param>
        /// <returns>Model</returns>
        public static T ConvertXMLStringToModel<T>(string xmlString) where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(xmlString)) return null;

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader rdr = new StringReader(xmlString);
                T model = (T)serializer.Deserialize(rdr);
                return model;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Error, ex);
                return null;
            }
        }

        /// <summary>
        /// Generate Quick Book xml using Type
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="t">Data object</param>
        /// <param name="typeName">name of node(xml node)</param>
        /// <returns>QuickBooks Xml String</returns>
        /// <example>GetQuickBooksXML<ItemQueryRq>(itemQueryRq, "ItemQueryRq")</example>
        public static string GetQuickBooksXML<T>(T t, string typeName) where T : class
        {
            try
            {
                XmlDocument inputXMLDoc = null;

                // CustomerQuery
                inputXMLDoc = GetQBHeaderXML();
                XmlElement qbXMLMsgsRq = GetQBXMLElements(inputXMLDoc);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ToXML<T>(t));

                XmlElement xeHeadere = doc.DocumentElement;
                XmlNode importNewsItem = inputXMLDoc.ImportNode(doc.SelectSingleNode(typeName), true);
                qbXMLMsgsRq.AppendChild(importNewsItem);

                return inputXMLDoc.OuterXml;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Error, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns true if the passed value is not null, else return false.
        /// </summary>
        /// <param name="value">Value that needs to be checked against empty value</param>
        /// <returns>Boolean value of comparison against null data</returns>
        public static bool IsNotNull(object value)
        => !Equals(value, null);

        public static string GetCustomerNameFromOrder(OrderModel orderModel)
        => (string.IsNullOrEmpty(orderModel?.UserName?.Trim()))
        ? (QuickBooksConstants.DefaultCustomerName)
        : (orderModel?.UserName);

        /// <summary>
        /// Checks for the existence of xmlNodeName node in responded XML string from QB
        /// </summary>
        /// <param name="response">Responded XML string from QB</param>
        /// <param name="xmlNodeName">XML node name whose existence need to be checked within given responded string</param>
        /// <returns>Boolean value for existence of xmlNodeName node in responded XML string</returns>
        public static bool ContainGivenNode(string response, string xmlNodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(xmlNodeName);
            return xmlNodeList != null && xmlNodeList.Count > 0;
        }

        /// <summary>
        /// Gives states message available in attribute "statusMessage" of given give node
        /// </summary>
        /// <param name="response">Responded XML string from QB</param>
        /// <param name="xmlNodeName">XML node name whose existence need to be checked within given responded string</param>
        /// <returns></returns>
        public static string GetStatusMessageOfGivenNode(string response, string xmlNodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(xmlNodeName);

            return (xmlNodeList != null && xmlNodeList.Count > 0)
                 ? (xmlNodeList[0].Attributes["statusMessage"]?.Value ?? "")
                 : ($"Did not received an xml node of type { xmlNodeName }");
        }

        /// <summary>
        /// Gives states Code available in attribute "statusCode" of given give node
        /// </summary>
        /// <param name="response">Responded XML string from QB</param>
        /// <param name="xmlNodeName">XML node name whose existence need to be checked within given responded string</param>
        /// <returns></returns>
        public static string GetStatusCodeOfGivenNode(string response, string xmlNodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(xmlNodeName);

            return (xmlNodeList != null && xmlNodeList.Count > 0)
                 ? (xmlNodeList[0].Attributes["statusCode"]?.Value ?? "")
                 : ($"Did not received an xml node of type { xmlNodeName }");
        }
        /// <summary>
        /// Gets inner text of given xml node if its available in provided xml response
        /// </summary>
        /// <param name="response">Responded XML string from QB</param>
        /// <param name="xmlNodeName">XML node name whose existence need to be checked within given responded string<</param>
        /// <returns>string: Inner text of given xml node</returns>
        public static string GetGivenNodeText(string response, string xmlNodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(xmlNodeName);

            return (xmlNodeList != null && xmlNodeList.Count > 0)
                 ? (xmlNodeList[0].InnerText)
                 : (string.Empty);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Decorated XML document with required QuickBooks XML format
        /// </summary>
        /// <param name="inputXMLDoc">XMLDocument that needs to be decorated</param>
        /// <returns>Decorated Xml Element</returns>
        private static XmlElement GetQBXMLElements(XmlDocument inputXMLDoc)
        {
            XmlElement qbXML = inputXMLDoc.CreateElement("QBXML");
            inputXMLDoc.AppendChild(qbXML);

            XmlElement qbXMLMsgsRq = inputXMLDoc.CreateElement("QBXMLMsgsRq");
            qbXML.AppendChild(qbXMLMsgsRq);

            qbXMLMsgsRq.SetAttribute("onError", "stopOnError");

            return qbXMLMsgsRq;
        }

        /// <summary>
        /// Gives XML document with required quick book header element
        /// </summary>
        /// <returns>XmlDocument with require QuickBooks header element</returns>
        private static XmlDocument GetQBHeaderXML()
        {
            XmlDocument inputXMLDoc = new XmlDocument();
            inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0", null, null));
            inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"4.0\""));
            return inputXMLDoc;
        }
        //Returns true if the passed value is null else false.
        public static bool IsNull(object value)
            => Equals(value, null);
        #endregion Private Methods
    }
}