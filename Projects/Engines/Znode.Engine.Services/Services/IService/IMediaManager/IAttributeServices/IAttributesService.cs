using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAttributesService
    { 
        /// <summary>
        /// Get attribute list from database.
        /// </summary>
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns AttributeListModel</returns>
        AttributesListDataModel GetAttributeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Save attribute details in database.
        /// </summary>
        /// <param name="model">AttributesModel</param>
        /// <returns>Returns AttributesListModel</returns>
        AttributesDataModel CreateAttribute(AttributesDataModel model);

        /// <summary>
        /// Update attribute details in database.
        /// </summary>
        /// <param name="model">AttributesModel</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateAttribute(AttributesDataModel model);

        /// <summary>
        /// Gets attribute on the basis of attribute id.
        /// </summary>
        /// <param name="roleId">Attribute id.</param>
        /// <returns>Returns AttributesModel.</returns>
        AttributesDataModel GetAttribute(int attributeId);

        /// <summary>
        /// Gets attribute type list
        /// </summary>
        /// <returns></returns>
        AttributesListDataModel GetAttributeTypeList();

        /// <summary>
        /// Gets attribute inputvalidation list
        /// </summary>
        /// <returns></returns>
        AttributesInputValidationListModel GetAttributesInputValidations(int attributeTypeId, int attributeId);

        /// <summary>
        /// Delete attribute.
        /// </summary>
        /// <param name="mediaAttributeIds">Parameter model for Attribute Ids.</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteAttribute(ParameterModel mediaAttributeIds);

       /// <summary>
       /// Get Default Values By Attribute Id
       /// </summary>
       /// <param name="attributeId"></param>
       /// <returns></returns>  
        AttributesDefaultValueListModel GetDefaultValues(int attributeId);

        /// <summary>
        /// Gets attributelocal on the basis of attribute id.
        /// </summary>
        /// <param name="roleId">Attributelocal id.</param>
        /// <returns>Returns AttributeLocalDataListModel.</returns>
        AttributeLocalDataListModel GetAttributeLocalByAttributeId(int atributeId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AttributesLocaleListModel SaveLocales(AttributesLocaleListModel model);

        /// <summary>
        /// SaveDefaultValues
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AttributesDefaultValueModel SaveDefaultValues(AttributesDefaultValueModel model);

        /// <summary>
        /// DeleteDefaultValues by default value Id
        /// </summary>
        /// <param name="defaultvalueId"></param>
        /// <returns></returns>
        bool DeleteDefaultValues(int defaultvalueId);

        /// <summary>
        /// Check attribute Code already exist or not
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <returns>returns true if attribute code exist</returns>
        bool IsAttributeCodeExist(string attributeCode);
    }
}
