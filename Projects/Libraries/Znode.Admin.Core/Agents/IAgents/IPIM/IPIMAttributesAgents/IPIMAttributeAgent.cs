using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface IPIMAttributeAgent
    {

        /// <summary>
        ///Get Attribute List 
        /// </summary>
        /// <param name="model">filter collection data model</param>
        /// <param name="gridListName">grid name</param>
        /// <param name="IsCategory">flag for category or product.</param>
        /// <returns></returns>
        PIMAttributeListViewModel AttributeList(FilterCollectionDataModel model, string gridListName, string IsCategory);

        /// <summary>
        /// Create Method For Attribute
        /// </summary>
        PIMAttributeViewModel Create(bool IsCategory);

        /// <summary>
        /// Get Attribute
        /// </summary>
        /// <param name="attributeId">Pim attribute id</param>
        /// <param name="isCategory">isCategory for flag. </param>
        PIMAttributeViewModel Attribute(int attributeId, bool isCategory);

        /// <summary>
        /// Get Input Validation List
        /// </summary>
        /// <param name="typeId">attribute Type Id</param>
        /// <param name="attributeId">Pim attribute id</param>
        /// <returns></returns>
        List<AttributeInputValidationModel> AttributeInputValidations(int typeId, int attributeId = 0);

        /// <summary>
        /// Save Attribute Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns>attributeview model</returns>
        PIMAttributeDataViewModel Save(BindDataModel model);


        /// <summary>
        /// Get Frontend Properties
        /// </summary>
        /// <param name="pimAttributeId"> Pim attribute id</param>
        /// <returns> FrontProperties View Model</returns>
        PIMFrontPropertiesViewModel FrontEndProperties(int pimAttributeId);

        /// <summary>
        ///Get Attribute By Attribute Id 
        /// </summary>
        /// <param name="attributeId">pim attribute id</param>
        /// <param name="isCategory">isCategory for flag. </param>
        /// <returns></returns>
        PIMAttributeViewModel GetAttribute(int attributeId, bool isCategory);

        /// <summary>
        /// Get list of all attributes.
        /// </summary>
        /// <returns>Returns Attribute list </returns>
        PIMAttributeListViewModel AttributeList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Update existing Attribute Values
        /// </summary>
        /// <param name="model">Bind Data model having key and value from view</param>
        /// <returns></returns>
        PIMAttributeDataViewModel Update(BindDataModel model);

        /// <summary>
        /// Delete attributes by attribute Id
        /// </summary>
        /// <param name="pimAttributeIds">Id of the attributes to delete.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteAttribute(string pimAttributeIds, out string errorMessage);

        /// <summary>
        /// Get Locale List
        /// </summary>
        /// <returns></returns>
        List<LocaleDataModel> GetLocales(int attributeId);

        /// <summary>
        /// Get list of default value 
        /// </summary>
        /// <returns></returns>
        List<DefaultValueListModel> DefaultValues(int attributeId);

        /// <summary>
        /// Create Default Values
        /// </summary>
        /// <param name="model">Attribute Default values</param>
        /// <param name="attributeId">Pim Attribute Id</param>
        /// <param name="displayOrder">Pim default value display order.</param>
        /// <param name="defaultvaluecode">PIM attribute defaultvalueCode</param>
        /// <param name="isDefault">PIM attribute isDefault</param>
        /// <param name="isSwatch">PIM attribute isswatch</param>
        /// <param name="swatchText">PIM attribute swatchtext/MediaID</param>
        /// <returns>True/False</returns>
        int SaveDefaultValues(string model, int attributeId, string defaultvaluecode, bool isDefault, bool isSwatch, string swatchText, int displayOrder, int defaultvalueId);

        /// <summary>
        /// Create Default Values
        /// </summary>
        /// <param name="model">Attribute Default values</param>
        /// <param name="attributeId">Pim Attribute Id</param>
        /// <returns>True/False</returns>
        bool DeleteDefaultValues(int defaultvalueId, out string errorMessage);

        /// <summary>
        /// Get Attribute Group list
        /// </summary>
        /// <param name="attributeGroupId">AttributeGroup Id</param>
        /// <param name="isCategory">get group list for category if true</param>
        /// <returns></returns>
        List<SelectListItem> AttributeGroupList(int attributeGroupId, string isCategory);

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
        /// <param name="id">Id of product</param>
        /// <param name="isCategory">Flag  to check it is category or product</param>
        /// <returns>Comma seperated attribute names.</returns>
        string IsAttributeValueUnique(string attributeCodeValues, int id, bool isCategory);

        /// <summary>
        /// Check attributedefault value code is exist or not.
        /// </summary>
        /// <param name="attributeId">id of attribute</param>
        /// <param name="defaultValueCode">default value code</param>
        /// <param name="defaultValueId">default value Id</param>
        /// <returns>return true and false</returns>
        bool CheckAttributeDefaultValueCodeExist(int attributeId, string defaultValueCode, int defaultValueId);
    }
}
