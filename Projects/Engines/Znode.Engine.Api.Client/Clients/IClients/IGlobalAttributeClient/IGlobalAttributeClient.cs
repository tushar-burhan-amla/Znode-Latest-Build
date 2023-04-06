using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IGlobalAttributeClient : IBaseClient
    {
        /// <summary>
        /// Creates a global attribute.
        /// </summary>
        /// <param name="attributeModel">Attribute Model to be created.</param>
        /// <returns>Created attribute Model.</returns>
        GlobalAttributeDataModel CreateAttributeModel(GlobalAttributeDataModel attributeModel);

        /// <summary>
        /// Save locale values.
        /// </summary>
        /// <param name="model">Uses locale value model.</param>
        /// <returns>Returns model with data.</returns>
        GlobalAttributeLocaleListModel SaveLocales(GlobalAttributeLocaleListModel model);

        /// <summary>
        /// Save default values.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        GlobalAttributeDefaultValueModel SaveDefaultValues(GlobalAttributeDefaultValueModel model);

        /// <summary>
        /// Get Input Validation List
        /// </summary>
        /// <param name="typeId">Attribute type Id</param>
        /// <returns></returns>
        GlobalAttributeInputValidationListModel GetInputValidations(int typeId, int attributeId = 0);

        /// <summary>
        /// Get paged list of attributes.
        /// </summary>
        /// <param name="expands">Expands for attribute list.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for attribute list.</param>
        /// <param name="pageIndex">Page Index of attribute list.</param>
        /// <param name="pageSize">Page size of attribute list.</param>
        /// <returns>Paged list of attributes</returns>
        GlobalAttributeListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the global attribute by ID.
        /// </summary>
        /// <param name="id">ID of the attribute.</param>
        /// <param name="expands">Expand for the attribute.</param>
        /// <returns>Attribute having the specified ID.</returns>
        GlobalAttributeModel GetAttribute(int id, ExpandCollection expands);

        /// <summary>
        /// Updates an attribute.
        /// </summary>
        /// <param name="attributeModel">Updates an attribute model.</param>
        /// <returns>Updated attribute model.</returns>
        GlobalAttributeDataModel UpdateAttributeModel(GlobalAttributeDataModel attributeModel);

        /// <summary>
        /// Deletes an attribute.
        /// </summary>
        /// <param name="globalAttributeIds">IDs of the attributes to be deleted.</param>
        /// <returns>True or false.</returns>
        bool DeleteAttributeModel(ParameterModel globalAttributeIds);

        /// <summary>
        /// Get attribute locales by attribute id.
        /// </summary>
        /// <param name="attributeId">Global attribute Id</param>
        /// <returns></returns>
        GlobalAttributeLocaleListModel GetAttributeLocale(int attributeId);

        /// <summary>
        /// Get attribute Default Values
        /// </summary>
        /// <param name="attributeId">global attribute id.</param>
        /// <returns></returns>
        GlobalAttributeDefaultValueListModel GetDefaultValues(int attributeId);

        /// <summary>
        /// Check attribute code already exist or not.
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <returns>Returns true if attribute code exist</returns>
        bool IsAttributeCodeExist(string attributeCode);

        /// <summary>
        /// Delete attribute's default values.
        /// </summary>
        /// <param name="defaultValueId">defaultValueId</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteDefaultValues(int defaultValueId);

        /// <summary>
        /// Check if attribute values are already exists or not. 
        /// </summary>
        /// <param name="attributeCodeValues">Attribute code and their values.</param>
        /// <returns>If values already exists then return name of attributes else null.</returns>
        ParameterModel IsAttributeValueUnique(GlobalAttributeValueParameterModel attributeCodeValues);
    }
}
