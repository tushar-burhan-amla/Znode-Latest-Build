using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.Admin
{
    public interface IZnodeProductFeedHelper
    {
        /// <summary>
        ///  Gets the Product list.
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <param name="rootTagValue">rootTagValue</param>
        /// <param name="xmlFileName">xmlFileName</param>
        /// <param name="googleFeedTitle">googleFeedTitle</param>
        /// <param name="googleFeedLink">googleFeedLink</param>
        /// <param name="googleFeedDesc">googleFeedDesc</param>
        /// <param name="feedType">feedType</param>
        /// <param name="localeId">localeId</param>
        /// <returns> This method is used to get Product List</returns>
        List<string> GetProductList(int portalId, string rootTagValue, string xmlFileName, string googleFeedTitle, string googleFeedLink, string googleFeedDesc, string feedType, int localeId);

        /// <summary>
        /// Gets the Category List.
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <param name="productFeedModel">productFeedModel</param>
        /// <param name="rootTag">rootTag</param>
        /// <param name="rootTagValue">rootTagValue</param>
        /// <returns> This method is used to get Category List</returns>
        List<string> GetCategoryList(int portalId, ProductFeedModel productFeedModel, string rootTag, string rootTagValue);

        /// <summary>
        /// Gets the GetContentPages List.
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <param name="rootTag">rootTag</param>
        /// <param name="rootTagValue">rootTagValue</param>
        /// <param name="model">model</param>
        /// <returns> This method is used to get Content Page List</returns>
        List<string> GetContentPagesList(int portalId, string rootTag, string rootTagValue, ProductFeedModel model);

        /// <summary>
        /// Gets the GetAll List.
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="portalId">portalId</param>
        /// <param name="rootTag">rootTag</param>
        /// <param name="rootTagValue">rootTagValue</param>
        /// <param name="feedType">feedType</param>
        /// <returns> This method is used to get All List</returns>
        List<string> GetAllList(ProductFeedModel model, int portalId, string rootTag, string rootTagValue, string feedType);

        /// <summary>
        /// This method will GenerateGoogleSiteMapIndex Files.
        /// </summary>
        /// <param name="fileNameCount">fileNameCount</param>
        /// <param name="txtXMLFileName">txtXMLFileName</param>
        /// <returns> This method is used to get Generated Google SiteMap Index Files</returns>
        string GenerateGoogleSiteMapIndexFiles(int fileNameCount, string txtXMLFileName);

        /// <summary>
        ///  Gets the GetProductXML List.
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <param name="rootTagValue">rootTagValue</param>
        /// <param name="feedType">feedType</param>
        /// <param name="rootTag">rootTag</param>
        /// <param name="productFeedModel">productFeedModel</param>
        /// <returns> This method is used to get Product XML List</returns>
        List<string> GetProductXMLList(int portalId, string rootTagValue, string feedType, string rootTag, ProductFeedModel productFeedModel);

    }
}
