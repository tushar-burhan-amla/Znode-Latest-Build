using System.Collections.Generic;
using System.Data;

namespace Znode.Libraries.ECommerce.Utilities
{
    public interface IZnodeRssWriter
    {
        #region Methods

        /// <summary>
        /// Creates the XMLSite Map
        /// </summary>
        /// <param name="datasetValues">data set values</param>
        /// <param name="rootTag">Root Tag for the XML</param>
        /// <param name="rootTagValue">Root Tag value</param>
        /// <param name="xmlFileName">FileName of the XML</param>
        /// <returns>Returns the boolean value if it created or not.</returns>
        List<string> CreateGoogleSiteMap(DataSet datasetValues, string rootTagValue, string xmlFileName, string googleFeedTitle, string googleFeedLink, string googleFeedDesc);
        /// <summary>
        /// Creates the XMLSite Map
        /// </summary>
        /// <param name="datasetValues">data set values</param>
        /// <param name="rootTag">Root Tag for the XML</param>
        /// <param name="rootTagValue">Root Tag value</param>
        /// <param name="xmlFileName">FileName of the XML</param>
        /// <returns>Returns the boolean value if it created or not.</returns>
        List<string> CreateXMLSiteMap(DataSet datasetValues, string rootTag, string rootTagValue, string xmlFileName);
        /// <summary>
        /// Creates the XMLSite Map which Returns the count of files generated.
        /// </summary>
        /// <param name="datasetValues">data set values</param>
        /// <param name="rootTag">Root Tag for the XML</param>
        /// <param name="rootTagValue">Root Tag value</param>
        /// <param name="xmlFileName">FileName of the XML</param>
        /// /// <param name="priority">priority</param>
        /// <returns>Returns the count of files generated.</returns>
        List<string> CreateXMLSiteMapForAll(DataSet datasetValues, string rootTag, string rootTagValue, string xmlFileName);

        /// <summary>
        /// Generate the XML which contains the product feed XML file location
        /// </summary>
        /// <param name="fileNameCount">file name count</param>
        /// <param name="txtXMLFileName">txt xml file name</param>
        /// <param name="priority">priority</param>
        /// <returns>Returns File Name</returns>
        string GenerateGoogleSiteMapIndexFiles(int fileNameCount, string txtXMLFileName);

        /// <summary>
        /// Creates the XMLSite Map which Returns the count of files generated.
        /// </summary>
        /// <param name="datasetValues">data set values</param>
        /// <param name="rootTag">Root Tag for the XML</param>
        /// <param name="rootTagValue">Root Tag value</param>
        /// <param name="xmlFileName">FileName of the XML</param>
        /// /// <param name="priority">priority</param>
        /// <returns>Returns the count of files generated.</returns>
        List<string> CreateProductSiteMap(DataSet datasetValues, string rootTag, string rootTagValue, string xmlFileName);

        #endregion
    }
}
