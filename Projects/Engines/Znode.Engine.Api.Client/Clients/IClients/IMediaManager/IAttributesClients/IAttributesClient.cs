using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IAttributesClient : IBaseClient
    {
        /// <summary>
        ///  Get Attribute Types
        /// </summary>
        /// <returns></returns>
        AttributesTypeListModel GetAttributeTypes();

        /// <summary>
        /// Get Input Validation List
        /// </summary>
        /// <param name="typeId">Attribute type Id</param>
        /// <returns></returns>
        AttributesInputValidationListModel GetInputValidations(int typeId, int attributeId = 0);

        /// <summary>
        /// Gets the list of attributes.
        /// </summary>
        /// <param name="expands">Expands for attribute list.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for attribute list.</param>
        /// <returns>Gets the list of attributes.</returns>
        AttributesListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets paged list of attributes.
        /// </summary>
        /// <param name="expands">Expands for attribute list.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for attribute list.</param>
        /// <param name="pageIndex">Page Index of attribute list.</param>
        /// <param name="pageSize">Page size of attribute list.</param>
        /// <returns>Paged list of attributes</returns>
        AttributesListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the attribute by ID.
        /// </summary>
        /// <param name="id">ID of the attribute.</param>
        /// <param name="expands">Expand for the attribute.</param>
        /// <returns>Attribute having the specified ID.</returns>
        AttributesDataModel GetAttribute(int id, ExpandCollection expands);

        /// <summary>
        /// Creates an attribute.
        /// </summary>
        /// <param name="attributeModel">Attribute Model to be created.</param>
        /// <returns>Created attribute Model.</returns>
        AttributesDataModel CreateAttributeModel(AttributesDataModel attributeModel);

        /// <summary>
        /// Updates an attribute.
        /// </summary>
        /// <param name="attributeModel">Updates an attribute model.</param>
        /// <returns>Updated attribute model.</returns>
        AttributesDataModel UpdateAttributeModel(AttributesDataModel attributeModel);

        /// <summary>
        /// Deletes an attribute.
        /// </summary>
        /// <param name="pimAttributeIds">Ids of the attributes to be deleted.</param>
        /// <returns>True or false.</returns>
        bool DeleteAttributeModel(ParameterModel pimAttributeIds);

        /// <summary>
        /// Save Locale values
        /// </summary>
        /// <param name="model"> Locale value model</param>
        /// <returns></returns>
        AttributesLocaleListModel SaveLocales(AttributesLocaleListModel model);

        /// <summary>
        /// Save Locale default Values
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AttributesDefaultValueModel SaveDefaultValues(AttributesDefaultValueModel model);

        /// <summary>
        /// Get Attribute Locales By attribute id
        /// </summary>
        /// <param name="attributeId">Pim attribute Id</param>
        /// <returns></returns>
        AttributesLocaleListModel GetAttributeLocale(int attributeId);

        /// <summary>
        /// Get attribute Default Values
        /// </summary>
        /// <param name="attributeId">attribute</param>
        /// <returns></returns>
        AttributesDefaultValueListModel GetDefaultValues(int attributeId);

        /// <summary>
        /// Delete Attribute Default Values
        /// </summary>
        /// <param name="defaultvalueId">PimAttributeDefaultValueId</param>
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
