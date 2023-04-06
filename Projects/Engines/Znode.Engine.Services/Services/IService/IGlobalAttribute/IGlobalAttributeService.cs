using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IGlobalAttributeService
    {
        /// <summary>
        /// Creates an attribute.
        /// </summary>
        /// <param name="model">Attribute Model to be created.</param>
        /// <returns>Created attribute model.</returns>
        GlobalAttributeDataModel CreateAttribute(GlobalAttributeDataModel model);

        /// <summary>
        /// Updates an attribute Model.
        /// </summary>
        /// <param name="model">Attribute model to be updated.</param>
        /// <returns>Updated attribute Model.</returns>
        bool UpdateAttribute(GlobalAttributeDataModel model);

        /// <summary>
        /// Deletes an attribute.
        /// </summary>
        /// <param name="attributeIds">ID of the attribute to be deleted.</param>
        /// <returns>True or false.</returns>
        bool DeleteAttribute(ParameterModel attributeIds);

        /// <summary>
        /// Gets Attribute by ID.
        /// </summary>
        /// <param name="attributeId">ID of the Attribute.</param>
        /// <param name="expands">Expands for the attribute.</param>
        /// <returns>Attribute for the provided ID.</returns>
        GlobalAttributeModel GetAttributeById(int attributeId, NameValueCollection expands);

        /// <summary>
        /// Gets the list of Attribute List.
        /// </summary>
        /// <param name="expands">Expands for Attribute List.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for Attribute List.</param>
        /// <param name="page">Paging for attribute list.</param>
        /// <returns>List of attributes.</returns>
        GlobalAttributeListModel GetAttributeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Save Attribute localevalues
        /// </summary>
        /// <param name="model">Locale List Model</param>
        /// <returns></returns>
        GlobalAttributeLocaleListModel SaveLocales(GlobalAttributeLocaleListModel model);

        /// <summary>
        /// Save Attribute Default Values
        /// </summary>
        /// <param name="model">Attribute Default Values Model</param>
        /// <returns></returns>
        GlobalAttributeDefaultValueModel SaveDefaultValues(GlobalAttributeDefaultValueModel model);

        /// <summary>
        /// Check attribute Code already exist or not
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <returns>returns true if attribute code exist</returns>
        bool IsAttributeCodeExist(string attributeCode);

        /// <summary>
        /// Get InputValidation List By Id
        /// </summary>
        /// <param name="typeId">Attribute Id</param>
        /// <returns></returns>
        GlobalAttributeInputValidationListModel GetInputValidations(int typeId, int attributeId);

        /// <summary>
        /// Get attribute locale values by attribute Id.
        /// </summary>
        /// <param name="attributeId">Attribute Id</param>
        /// <returns>Returns model with data.</returns>
        GlobalAttributeLocaleListModel GetAttributeLocale(int attributeId);

        /// <summary>
        /// Get attribute default values by attribute Id.
        /// </summary>
        /// <param name="attributeId">attribute Id</param>
        /// <returns>Returns model with data.</returns>
        GlobalAttributeDefaultValueListModel GetDefaultValues(int attributeId);

        /// <summary>
        /// Delete attribute's deafult Values By id.
        /// </summary>
        /// <param name="defaultValueId">GlobalAttributeDefaultValueId</param>
        /// <returns></returns>
        bool DeleteDefaultValues(int defaultValueId);

        /// <summary>
        /// Get is attribute value unique or not.
        /// </summary>
        /// <param name="attributeCodeValues">Attribute Code and their values.</param>
        /// <returns>If values already exists then return name of attributes else empty string.</returns>
        string IsAttributeValueUnique(GlobalAttributeValueParameterModel attributeCodeValues);
    }
}
