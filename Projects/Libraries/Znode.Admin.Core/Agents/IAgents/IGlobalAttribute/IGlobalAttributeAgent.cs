using System.Collections.Generic;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Agents
{
    public interface IGlobalAttributeAgent
    {
        /// <summary>
        ///Get Attribute List. 
        /// </summary>
        /// <param name="model">filter collection data model</param>
        /// <param name="gridListName">grid name</param>
        /// <returns></returns>
        GlobalAttributeListViewModel AttributeList(FilterCollectionDataModel model, string gridListName, int entityId = 0, string entityType = null);

        /// <summary>
        /// Create global attribute.
        /// </summary>
        /// <returns>Returns model with data.</returns>
        GlobalAttributeViewModel Create();

        /// <summary>
        /// Save global attribute data.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>global attribute view model with data.</returns>
        GlobalAttributeViewModel Save(BindDataModel model);

        /// <summary>
        ///Get attribute data by global attribute Id. 
        /// </summary>
        /// <param name="attributeId">Global attribute id</param>
        /// <returns>Global Attribute model with data.</returns>
        GlobalAttributeViewModel GetAttributeData(int attributeId);

        /// <summary>
        /// Update existing Attribute Values
        /// </summary>
        /// <param name="model">Bind Data model having key and value from view</param>
        /// <returns></returns>
        GlobalAttributeViewModel Update(BindDataModel model);

        /// <summary>
        /// Delete attributes by attribute Id.
        /// </summary>
        /// <param name="globalAttributeIds">Id of the attributes to delete.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteAttribute(string globalAttributeIds, out string errorMessage);

        /// <summary>
        /// Get Input Validation List
        /// </summary>
        /// <param name="typeId">attribute Type Id</param>
        /// <param name="attributeId">Global attribute id</param>
        /// <returns></returns>
        List<AttributeInputValidationModel> AttributeInputValidations(int typeId, int attributeId = 0);

        /// <summary>
        /// Get locale list for global attribute.
        /// </summary>
        /// <returns></returns>
        List<LocaleDataModel> GetLocales(int attributeId);

        /// <summary>
        /// Get list of default value for global attribute.
        /// </summary>
        /// <returns></returns>
        List<DefaultValueListModel> GetDefaultValues(int attributeId);

        /// <summary>
        /// Check attribute Code already exist or not
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <returns>returns true if attribute code exist</returns>
        bool IsAttributeCodeExist(string attributeCode);

        /// <summary>
        /// Create Default Values.
        /// </summary>
        /// <param name="model">Attribute Default values</param>
        /// <param name="attributeId">Global Attribute Id</param>
        /// <param name="displayOrder">Global default value display order.</param>
        /// <param name="defaultvaluecode">Global attribute defaultvalueCode</param>
        /// <param name="isDefault">Global attribute isDefault</param>
        /// <param name="isSwatch">Global attribute isswatch</param>
        /// <param name="swatchText">Global attribute swatchtext/MediaID</param>
        /// <returns>True/False</returns>
        int SaveDefaultValues(string model, int attributeId, string defaultvaluecode, bool isDefault, bool isSwatch, string swatchText, int displayOrder, int defaultvalueId);

        /// <summary>
        /// Delete default value
        /// </summary>
        /// <param name="defaultvalueId">Uses default value id.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Returns true or false.</returns>
        bool DeleteDefaultValues(int defaultvalueId, out string errorMessage);

        /// <summary>
        /// Check attributedefault value code is exist or not.
        /// </summary>
        /// <param name="attributeId">id of attribute</param>
        /// <param name="defaultValueCode">default value code</param>
        /// <param name="defaultValueId">default value Id</param>
        /// <returns>return true and false</returns>
        bool CheckAttributeDefaultValueCodeExist(int attributeId, string defaultValueCode, int defaultValueId);

        /// <summary>
        /// Check if attribute values are already exists or not. 
        /// </summary>
        /// <param name="model">Model with entity related data.</param>
        /// <returns>Comma seperated attribute names.</returns>
        string IsAttributeValueUnique(GlobalAttributeValueParameterModel model);
    }
}