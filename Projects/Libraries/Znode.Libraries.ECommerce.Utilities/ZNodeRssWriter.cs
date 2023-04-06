using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class ZnodeRssWriter : ZnodeBusinessBase, IZnodeRssWriter
    {
        #region Private Variables
        private readonly int recCount = Convert.ToInt32(ZnodeApiSettings.ProductFeedRecordCount);
        private int fileCount = 0;
        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the XMLSite Map
        /// </summary>
        /// <param name="datasetValues">data set values</param>
        /// <param name="rootTag">Root Tag for the XML</param>
        /// <param name="rootTagValue">Root Tag value</param>
        /// <param name="xmlFileName">FileName of the XML</param>
        /// <returns>Returns the list of XML files.</returns>
        public virtual List<string> CreateGoogleSiteMap(DataSet datasetValues, string rootTagValue, string xmlFileName, string googleFeedTitle, string googleFeedLink, string googleFeedDesc)
        {
            List<string> xmlFiles = new List<string>();
            try
            {
                int recordsCount = datasetValues.Tables[0].Rows.Count;
                int loopCnt = 0;
                
                for (; loopCnt < recordsCount;)
                {
                    string[] rootTagValues = rootTagValue.Split(',');

                    // Construct the XML for the Site Map creation
                    XmlDocument requestXMLDoc = new XmlDocument();

                    XmlNode rss = requestXMLDoc.CreateElement("rss");
                    XmlAttribute version = requestXMLDoc.CreateAttribute("version");
                    version.Value = "2.0";
                    rss.Attributes.Append(version);

                    XmlAttribute nameSpace = requestXMLDoc.CreateAttribute("xmlns:g");
                    nameSpace.Value = rootTagValues[0];
                    rss.Attributes.Append(nameSpace);

                    XmlNode channel = requestXMLDoc.CreateElement("channel");

                    XmlNode Title = requestXMLDoc.CreateElement("title");
                    Title.InnerText = googleFeedTitle;
                    channel.AppendChild(Title);

                    XmlNode link = requestXMLDoc.CreateElement("link");
                    link.InnerText = googleFeedLink;
                    channel.AppendChild(link);

                    XmlNode description = requestXMLDoc.CreateElement("description");
                    description.InnerText = googleFeedDesc;
                    channel.AppendChild(description);

                    // Loop thro the dataset values.
                    do
                    {
                        XmlElement itemElement = requestXMLDoc.CreateElement("item");
                        DataRow dr = datasetValues.Tables[0].Rows[loopCnt];

                        foreach (DataColumn dc in datasetValues.Tables[0].Columns)
                        {
                            if (!string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                            {
                                itemElement.AppendChild(this.MakeElement(requestXMLDoc, dc.ColumnName, dr[dc.ColumnName].ToString()?.Trim()));
                            }
                        }

                        channel.AppendChild(itemElement);

                        loopCnt++;
                    }
                    while (loopCnt < recordsCount && (loopCnt + 1) % recCount != 0);

                    rss.AppendChild(channel);
                    requestXMLDoc.AppendChild(rss);

                    xmlFiles.Add(requestXMLDoc.OuterXml);

                    // Increment the file count if the file has to be splitted.
                    this.fileCount++;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return xmlFiles;
            }

            return xmlFiles;
        }

        // Creates the XMLSite Map which Returns the list of XML files
        public virtual List<string> CreateXMLSiteMap(DataSet datasetValues, string rootTag, string rootTagValue, string xmlFileName)
        {
            XmlDocument requestXMLDoc = null;
            List<string> xmlFiles = new List<string>();
            try
            {
                if (datasetValues != null)
                {
                    int recordsCount = datasetValues.Tables[0].Rows.Count;
                    int loopCnt = 0;

                    for (; loopCnt < recordsCount;)
                    {
                        string[] rootTagValues = rootTagValue.Split(',');
                        // Construct the XML for the Site Map creation
                        requestXMLDoc = new XmlDocument();
                        requestXMLDoc.AppendChild(requestXMLDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
                        XmlElement urlsetElement = null;
                        urlsetElement = requestXMLDoc.CreateElement(rootTag);
                        if (rootTagValues?.Count() > 0)
                        {
                            string[] values = rootTagValues[0].Split('=');
                            urlsetElement.SetAttribute(values[0], values[1]);
                        }

                        requestXMLDoc.AppendChild(urlsetElement);

                        // Loop thro the dataset values.
                        do
                        {
                            DataRow dr = datasetValues.Tables[0].Rows[loopCnt];
                            XmlElement urlElement = requestXMLDoc.CreateElement(datasetValues.Tables[0].TableName);                           
                            urlsetElement.AppendChild(urlElement);
                            foreach (DataColumn dc in datasetValues.Tables[0].Columns)
                            {
                                if (!string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                                    urlElement.AppendChild(this.MakeElement(requestXMLDoc, dc.ColumnName, dr[dc.ColumnName].ToString()?.Trim()));
                            }

                            loopCnt++;
                        }
                        while (loopCnt < recordsCount && (loopCnt + 1) % recCount != 0);
                        xmlFiles.Add(requestXMLDoc.OuterXml);
                        // Increment the file count if the file has to be splitted.
                        this.fileCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return xmlFiles;
            }
            return xmlFiles;
        }

        // Creates the XMLSite Map which Returns the XML files.
        public virtual List<string> CreateXMLSiteMapForAll(DataSet datasetValues, string rootTag, string rootTagValue, string xmlFileName)
        {
            List<string> xmlFiles = new List<string>();
            XmlDocument requestXMLDoc = null;
            try
            {
                int tableCount = datasetValues.Tables.Count;

                for (int i = 1; i < datasetValues.Tables.Count ; i++)
                    datasetValues.Tables[0].Merge(datasetValues.Tables[i]);

                int recordsCount = datasetValues.Tables[0].Rows.Count;
                int loopCnt = 0;

                for (; loopCnt < recordsCount;)
                {
                    string[] rootTagValues = rootTagValue.Split(',');
                    // Construct the XML for the Site Map creation
                    requestXMLDoc = new XmlDocument();
                    requestXMLDoc.AppendChild(requestXMLDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
                    XmlElement urlsetElement = null;
                    urlsetElement = requestXMLDoc.CreateElement(rootTag);
                    if (rootTagValues?.Count() > 0)
                    {
                        string[] values = rootTagValues[0].Split('=');
                        urlsetElement.SetAttribute(values[0], values[1]);
                    }
                    requestXMLDoc.AppendChild(urlsetElement);
                    // Loop thro the dataset values.
                    do
                    {
                        DataRow dr = datasetValues.Tables[0].Rows[loopCnt];
                        XmlElement urlElement = requestXMLDoc.CreateElement("url");
                        urlsetElement.AppendChild(urlElement);
                        foreach (DataColumn dc in datasetValues.Tables[0].Columns)
                        {
                            if (!string.IsNullOrEmpty(dr[dc.ColumnName].ToString()) && Equals(dc.ColumnName, "loc"))
                                urlElement.AppendChild(this.MakeElement(requestXMLDoc, dc.ColumnName, dr[dc.ColumnName].ToString()));
                        }
                        loopCnt++;
                    }
                    while (loopCnt < recordsCount && (loopCnt + 1) % recCount != 0);
                    xmlFiles.Add(requestXMLDoc.OuterXml);
                }
                
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return xmlFiles;
            }
            return xmlFiles;
        }

        // Creates and Returns an Element with the specified tagName with the value 
        protected virtual XmlElement MakeElement(XmlDocument doc, string tagName, string tagValue)
        {
            XmlElement elem = tagName.Contains("g:") ? doc.CreateElement("g", tagName.Split(':')[1], "http://base.google.com/ns/1.0") : doc.CreateElement(tagName);
            elem.InnerText = tagValue;
            return elem;
        }

        public virtual string GenerateGoogleSiteMapIndexFiles(int fileNameCount, string txtXMLFileName)
        {
            string rootTag = "sitemapindex";
            string roottagxmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            // Construct the XML for the Site Map creation
            XmlDocument requestXMLDoc = new XmlDocument();
            requestXMLDoc.AppendChild(requestXMLDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement urlsetElement = null;
            urlsetElement = requestXMLDoc.CreateElement(rootTag);
            urlsetElement.SetAttribute("xmlns", roottagxmlns);
            requestXMLDoc.AppendChild(urlsetElement);

            for (int i = 0; i < fileNameCount; i++)
            {
                XmlElement urlElement = requestXMLDoc.CreateElement("sitemap");
                string fileName = ZnodeStorageManager.HttpPath($"{ZnodeConfigManager.EnvironmentConfig.ContentPath}{txtXMLFileName.Trim()}_{i}{".xml"}");
                if (fileName.StartsWith("~/")) fileName = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, string.Empty) + fileName.Substring(1);
                urlElement.AppendChild(this.MakeElement(requestXMLDoc, "loc", fileName));
                urlElement.AppendChild(this.MakeElement(requestXMLDoc, "lastmod", DateTime.Now.ToString("yyyy-MM-ddhh-mm-ss")));
                urlsetElement.AppendChild(urlElement);
            }
            string strSiteMapIndexFile = $"{ZnodeConfigManager.EnvironmentConfig.ContentPath}{txtXMLFileName.Trim()}_{DateTime.Now.ToString("yyyy-MM-ddhh-mm-ss")}{".xml"}";
            ZnodeStorageManager.WriteTextStorage(requestXMLDoc.OuterXml, strSiteMapIndexFile);

            return strSiteMapIndexFile;
        }

        public virtual List<string> CreateProductSiteMap(DataSet datasetValues, string rootTag, string rootTagValue, string xmlFileName)
        {
            XmlDocument requestXMLDoc = null;
            List<string> xmlFiles = new List<string>();
            try
            {
                if (datasetValues != null)
                {
                    int recordsCount = datasetValues.Tables[0].Rows.Count;
                    int loopCnt = 0;

                    for (; loopCnt < recordsCount;)
                    {
                        string[] rootTagValues = rootTagValue.Split(',');
                        // Construct the XML for the Site Map creation
                        requestXMLDoc = new XmlDocument();
                        requestXMLDoc.AppendChild(requestXMLDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
                        XmlElement urlsetElement = null;
                        urlsetElement = requestXMLDoc.CreateElement(rootTag);
                        if (rootTagValues?.Count() > 0)
                        {
                            string[] values = rootTagValues[0].Split('=');
                            urlsetElement.SetAttribute(values[0], values[1]);
                        }
                        requestXMLDoc.AppendChild(urlsetElement);

                        // Loop thro the dataset values.
                        do
                        {
                            DataRow dr = datasetValues.Tables[0].Rows[loopCnt];
                            XmlElement urlElement = requestXMLDoc.CreateElement("url");
                            urlsetElement.AppendChild(urlElement);
                            foreach (DataColumn dc in datasetValues.Tables[0].Columns)
                            {
                                if (!string.IsNullOrEmpty(dr[dc.ColumnName].ToString()) && Equals(dc.ColumnName, "loc"))
                                    urlElement.AppendChild(this.MakeElement(requestXMLDoc, dc.ColumnName, dr[dc.ColumnName].ToString()?.Trim()));
                            }
                            loopCnt++;
                        }
                        while (loopCnt < recordsCount && (loopCnt + 1) % recCount != 0);
                        xmlFiles.Add(requestXMLDoc.OuterXml);
                        // Increment the file count if the file has to be splitted.
                        this.fileCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return xmlFiles;
            }
            return xmlFiles;
        }
        #endregion
    }
}
