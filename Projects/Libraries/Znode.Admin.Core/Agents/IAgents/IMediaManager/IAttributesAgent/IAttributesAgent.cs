using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface IAttributesAgent
    {

        /// <summary>
        /// Get AttributeList
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AttributesListViewModel AttributeList(FilterCollectionDataModel model);

        /// <summary>
        /// Create Method For Attribute
        /// </summary>
        AttributesViewModel Create();

        /// <summary>
        /// Get Attribute
        /// </summary>
        /// <param name="attributeId">media attribute id</param>
        AttributesViewModel GetAttribute(int attributeId);

        /// <summary>
        /// Get Input Validation List
        /// </summary>
        /// <param name="typeId">attribute Type Id</param>
        /// <param name="attributeId">media attribute id</param>
        /// <returns></returns>
        List<AttributeInputValidationModel> AttributeInputValidations(int typeId, int attributeId = 0);

        /// <summary>
        /// Save Attribute Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns>attributeview model</returns>
        AttributesViewModel Save(BindDataModel model);

        /// <summary>
        /// Get list of all attributes.
        /// </summary>
        /// <returns>Returns Attribute list </returns>
        AttributesListViewModel AttributeList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Update existing Attribute Values
        /// </summary>
        /// <param name="model">Bind Data model having key and value from view</param>
        /// <returns></returns>
        AttributesViewModel Update(BindDataModel model);

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
        /// <param name="attributeId">media Attribute Id</param>
        /// <param name="defaultvaluecode">media attribute defaultvalueCode</param>
        /// <returns>True/False</returns>
        int SaveDefaultValues(string model, int attributeId, string defaultvaluecode, int defaultvalueId);

        /// <summary>
        /// Create Default Values
        /// </summary>
        /// <param name="model">Attribute Default values</param>
        /// <param name="attributeId">media Attribute Id</param>
        /// <returns>True/False</returns>
        bool DeleteDefaultValues(int defaultvalueId, out string errorMessage);

        /// <summary>
        /// Get Attribute Group list
        /// </summary>
        /// <returns>attributegroup</returns>
        List<SelectListItem> AttributeGroupList(int attributeGroupId);

        /// <summary>
        /// Get regular expression value.
        /// </summary>
        /// <param name="AttributeTypeId">Attribute type Id</param>
        /// <param name="ruleName">Regular expression rule name</param>
        /// <returns></returns>
        string GetValidationRuleRegularExpression(int AttributeTypeId, string ruleName = null);

        /// <summary>
        /// Check attribute Code already exist or not
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <returns>returns true if attribute code exist</returns>
        bool IsAttributeCodeExist(string attributeCode);

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
