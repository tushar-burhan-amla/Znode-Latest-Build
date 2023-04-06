using System.Collections.Generic;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IFormBuilderAgent
    {
        /// <summary>
        /// Create Form.
        /// </summary>
        /// <param name="formBuilderViewModel">Uses model with data.</param>
        /// <returns>Returns form builder model with information.</returns>
        FormBuilderViewModel CreateForm(FormBuilderViewModel formBuilderViewModel);

        /// <summary>
        /// Gets the list of Form Builder.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Form Builder list.</param>
        /// <param name="filters">Filters to be applied on Form Builder list.</param>
        /// <param name="sorts">Sorting to be applied on Form Builder list.</param>
        /// <param name="pageIndex">Start page index of Form Builder list.</param>
        /// <param name="pageSize">Page size of Form Builder list.</param>
        /// <returns>Returns Form Builder list.</returns>
        FormBuilderListViewModel GetFormBuilderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        ///Get form data by form Id. 
        /// </summary>
        /// <param name="formBuilderId">Form id.</param>
        /// <returns>Form builder model with data.</returns>
        FormBuilderViewModel GetForm(int formBuilderId);

        /// <summary>
        /// Delete form builder.
        /// </summary>
        /// <param name="formBuilderId">Form Builder Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteFormBuilder(string formBuilderId);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="formBuilderId">Form builder formBuilderId .</param>
        /// <returns>Returns unassigned attributes.</returns>
        List<BaseDropDownList> GetUnAssignedAttributes(int formBuilderId);

        /// <summary>
        /// Check form code already exist or not.
        /// </summary>
        /// <param name="formCode">formCode</param>
        /// <returns>Returns true if form code exist.</returns>
        bool IsFormCodeExist(string formCode);

        /// <summary>
        /// Method to create tab structure.
        /// </summary>
        /// <param name="formBuilderId">Form Builder Id.</param>
        /// <returns>Returns TabViewListModel.</returns>
        TabViewListModel CreateTabStructure(int formBuilderId);

        /// <summary>
        /// Get unassigned attribute groups.
        /// </summary>
        /// <param name="formBuilderId">form builder id.</param>
        /// <returns>Returns list of unassigned groups.</returns>
        List<BaseDropDownList> GetUnAssignedGroups(int formBuilderId);

        /// <summary>
        /// Get formbuilder attribute and groups.
        /// </summary>
        /// <param name="formBuilderId">Int LocaleId</param>
        /// <returns>returns FormBuilderAttributeGroupViewModel</returns>
        FormBuilderAttributeGroupViewModel GetFormBuilderAttributeDetails(int formBuilderId);

        /// <summary>
        /// Assign group to form.
        /// </summary>
        /// <param name="attributeGroupIds">Group ids.</param>
        /// <param name="formBuilderId">Form Builder id.</param>
        /// <param name="message">Message regarding the status.</param>
        /// <returns>Returns true/false.</returns>
        bool AssignGroups(string attributeGroupIds, int formBuilderId, out string message);

        /// <summary>
        /// Assign attributes to form.
        /// </summary>
        /// <param name="attributeIds">Attribute ids.</param>
        /// <param name="formBuilderId">Form Builder id.</param>
        /// <param name="message">Message regarding the status.</param>
        /// <returns>Returns true/false.</returns>
        bool AssignAttributes(string attributeGroupIds, int formBuilderId, out string message);

        /// <summary>
        /// Update formbuilder.
        /// </summary>
        /// <param name="formBuilderViewModel">Formbuilder view model to update.</param>
        /// <returns>Returns updated frombuilder model.</returns>
        FormBuilderViewModel Update(FormBuilderViewModel formBuilderViewModel);

        /// <summary>
        /// Update Attribute DisplayOrder of form
        /// </summary>
        /// <param name="model">FormBuilderAttributeGroupDisplayOrderModel </param>
        /// <param name="message">Message regarding the status</param>
        /// <returns>Returns true/false.</returns>
        bool UpdateAttributeDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model, out string message);

        /// <summary>
        ///Update Group Display Order
        /// </summary>
        /// <param name="model">model to extract data from.</param>
        /// <returns>Returns updated model.</returns>
        bool UpdateGroupDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model,string message);

        /// <summary>
        /// Un assign group from form builder
        /// </summary>
        /// <param name="formBuilderId">form builder Id </param>
        /// <param name="groupId">Group Id</param>
        /// <param name="message">error message</param>
        /// <returns>returns true/false</returns>
        bool UnAssignFormBuilderGroups(int formBuilderId, int groupId, out string message);

        /// <summary>
        /// Un assign group from form builder
        /// </summary>
        /// <param name="formBuilderId">Entity Id </param>
        /// <param name="attributeId">Attribute Id</param>
        /// <param name="message">error message</param>
        /// <returns>returns true/false</returns>
        bool UnAssignFormBuilderAttributes(int formBuilderId, int attributeId, out string message);
    }
}
