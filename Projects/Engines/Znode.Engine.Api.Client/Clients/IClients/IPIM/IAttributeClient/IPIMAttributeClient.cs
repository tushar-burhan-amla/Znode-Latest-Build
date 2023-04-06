using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IPIMAttributeClient : IBaseClient
    {
        /// <summary>
        ///  Get Attribute Types
        /// </summary>
        /// <returns></returns>
        PIMAttributeTypeListModel GetAttributeTypes(bool isCategory);

        /// <summary>
        /// Get Input Validation List
        /// </summary>
        /// <param name="typeId">Attribute type Id</param>
        /// <returns></returns>
        PIMAttributeInputValidationListModel GetInputValidations(int typeId, int attributeId = 0);

        /// <summary>
        /// Gets the list of attributes.
        /// </summary>
        /// <param name="expands">Expands for attribute list.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for attribute list.</param>
        /// <returns>Gets the list of attributes.</returns>
        PIMAttributeListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets paged list of attributes.
        /// </summary>
        /// <param name="expands">Expands for attribute list.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for attribute list.</param>
        /// <param name="pageIndex">Page Index of attribute list.</param>
        /// <param name="pageSize">Page size of attribute list.</param>
        /// <returns>Paged list of attributes</returns>
        PIMAttributeListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the attribute by ID.
        /// </summary>
        /// <param name="id">ID of the attribute.</param>
        /// <param name="expands">Expand for the attribute.</param>
        /// <returns>Attribute having the specified ID.</returns>
        PIMAttributeModel GetAttribute(int id, ExpandCollection expands);

        /// <summary>
        /// Creates an attribute.
        /// </summary>
        /// <param name="attributeModel">Attribute Model to be created.</param>
        /// <returns>Created attribute Model.</returns>
        PIMAttributeDataModel CreateAttributeModel(PIMAttributeDataModel attributeModel);

        /// <summary>
        /// Updates an attribute.
        /// </summary>
        /// <param name="attributeModel">Updates an attribute model.</param>
        /// <returns>Updated attribute model.</returns>
        PIMAttributeDataModel UpdateAttributeModel(PIMAttributeDataModel attributeModel);

        /// <summary>
        /// Deletes an attribute.
        /// </summary>
        /// <param name="pimAttributeIds">IDs of the attributes to be deleted.</param>
        /// <returns>True or false.</returns>
        bool DeleteAttributeModel(ParameterModel pimAttributeIds);

        /// <summary>
        /// GetFrontEndProperties
        /// </summary>
        /// <returns></returns>
        PIMFrontPropertiesModel FrontEndProperties(int pimAttributeId);

        /// <summary>
        /// Save Locale values
        /// </summary>
        /// <param name="model"> Locale value model</param>
        /// <returns></returns>
        PIMAttributeLocaleListModel SaveLocales(PIMAttributeLocaleListModel model);

        /// <summary>
        /// Save Locale default Values
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        PIMAttributeDefaultValueModel SaveDefaultValues(PIMAttributeDefaultValueModel model);

        /// <summary>
        /// Get Attribute Locales By attribute id
        /// </summary>
        /// <param name="attributeId">Pim attribute Id</param>
        /// <returns></returns>
        PIMAttributeLocaleListModel GetAttributeLocale(int attributeId);

        /// <summary>
        /// Get attribute Default Values
        /// </summary>
        /// <param name="attributeId">attribute</param>
        /// <returns></returns>
        PIMAttributeDefaultValueListModel GetDefaultValues(int attributeId);

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
        /// <param name="isCategory">true for category attribute else false</param>
        /// <returns>returns true if attribute code exist</returns>
        bool IsAttributeCodeExist(string attributeCode, bool isCategory);

        /// <summary>
        /// Check if attribute values are already exists or not. 
        /// </summary>
        /// <param name="attributeCodeValues">Attribute code and their values.</param>
        /// <returns>If values already exists then return name of attributes else null.</returns>
        ParameterModel IsAttributeValueUnique(PimAttributeValueParameterModel attributeCodeValues);

        /// <summary>
        /// Get attribute validation by attribute code.
        /// </summary>
        /// <param name="model">Parmeter model with codes and locale id.</param>
        /// <returns>PIM Family Details Model</returns>
        PIMFamilyDetailsModel GetAttributeValidationByCodes(ParameterProductModel model);
    }
}
