using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPIMAttributeService
    {   /// <summary>
        /// Get Attribute Type List
        /// </summary>
        /// <returns></returns>
        PIMAttributeTypeListModel GetAttributeTypes(bool isCategory);

        /// <summary>
        /// Get InputValidation List By Id
        /// </summary>
        /// <param name="typeId">Attribute Id</param>
        /// <returns></returns>
        PIMAttributeInputValidationListModel GetInputValidations(int typeId, int attributeId);

        /// <summary>
        /// Gets the list of Attribute List.
        /// </summary>
        /// <param name="expands">Expands for Attribute List.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for Attribute List.</param>
        /// <param name="page">Paging for attribute list.</param>
        /// <returns>List of attributes.</returns>
        PIMAttributeListModel GetAttributeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets Attribute by ID.
        /// </summary>
        /// <param name="attributeId">ID of the Attribute.</param>
        /// <param name="expands">Expands for the attribute.</param>
        /// <returns>Attribute for the provided ID.</returns>
        PIMAttributeModel GetAttributeById(int attributeId, NameValueCollection expands);

        /// <summary>
        /// Updates an attribute Model.
        /// </summary>
        /// <param name="pimAttributemodel">Attribute model to be updated.</param>
        /// <returns>Updated attribute Model.</returns>
        bool UpdateAttribute(PIMAttributeDataModel pimAttributemodel);

        /// <summary>
        /// Creates an attribute.
        /// </summary>
        /// <param name="pimAttributemodel">Attribute Model to be created.</param>
        /// <returns>Created attribute model.</returns>
        PIMAttributeDataModel CreateAttribute(PIMAttributeDataModel pimAttributemodel);

        /// <summary>
        /// Deletes an attribute.
        /// </summary>
        /// <param name="pimAttributeIds">ID of the attribute to be deleted.</param>
        /// <returns>True or false.</returns>
        bool DeleteAttribute(ParameterModel pimAttributeIds);

        /// <summary>
        /// Get Front End Properties
        /// </summary>
        /// <param name="pimAttributeId"></param>
        /// <returns></returns>
        PIMFrontPropertiesModel FrontEndProperties(int pimAttributeId);

        /// <summary>
        /// Save Attribute localevalues
        /// </summary>
        /// <param name="model">Locale List Model</param>
        /// <returns></returns>
        PIMAttributeLocaleListModel SaveLocales(PIMAttributeLocaleListModel model);

        /// <summary>
        /// Save Attribute Default Values
        /// </summary>
        /// <param name="model">Attribute Default Values Model</param>
        /// <returns></returns>
        PIMAttributeDefaultValueModel SaveDefaultValues(PIMAttributeDefaultValueModel model);

        /// <summary>
        /// Get attribute Locale Values By attribute Id
        /// </summary>
        /// <param name="attributeId">attribute Id</param>
        /// <returns></returns>
        PIMAttributeLocaleListModel GetAttributeLocale(int attributeId);

        /// <summary>
        /// Get attribute Locale Values By attribute Code
        /// </summary>
        /// <param name="attributeCode">attribute Code</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>Attribute Locale list.</returns>
        string GetAttributeLocale(string attributeCode, int localeId);

        /// <summary>
        /// Get attribute Default Values By attribute Id
        /// </summary>
        /// <param name="attributeId">attribute Id</param>
        /// <returns></returns>
        PIMAttributeDefaultValueListModel GetDefaultValues(int attributeId);

        /// <summary>
        /// Delete Attribute Deafult Values By id
        /// </summary>
        /// <param name="defaultvalueId">PimAttributeDefaultValueId</param>
        /// <returns></returns>
        bool DeleteDefaultValues(int defaultvalueId);

        /// <summary>
        /// Check attribute Code already exist or not
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <param name="isCategory">true for category attribute else false</param>
        /// <returns>returns true if attribute code exist</returns>
        bool IsAttributeCodeExist(string attributeCode, bool isCategory);

        /// <summary>
        /// Check if attribute values are already exists or not. 
        /// </summary>
        /// <param name="PimAttributeValueParameterModel">Attribute code and their values.</param>
        /// <returns>If values already exists then return name of attributeNames else null.</returns>
        string IsAttributeValueUnique(PimAttributeValueParameterModel attributeCodeValues);

        /// <summary>
        /// Get attribute validation by attribute code.
        /// </summary>
        /// <param name="attributeCodes">parameter model with codes.</param>
        /// <returns>PIM family details model</returns>
        PIMFamilyDetailsModel GetAttributeValidationByCodes(ParameterProductModel attributeCodes);
    }
}
