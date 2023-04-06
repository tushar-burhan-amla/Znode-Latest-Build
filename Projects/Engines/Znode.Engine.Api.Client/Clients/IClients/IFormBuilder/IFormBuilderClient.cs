using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IFormBuilderClient : IBaseClient
    {
        /// <summary>
        /// Creates form.
        /// </summary>
        /// <param name="formBuilderModel">Model with form builder data.</param>
        /// <returns>Created form.</returns>
        FormBuilderModel CreateForm(FormBuilderModel formBuilderModel);

        /// <summary>
        /// Get the list of Form Builder.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Form Builder list.</param>
        /// <param name="filters">Filters to be applied on Form Builder list.</param>
        /// <param name="sorts">Sorting to be applied on Form Builder list.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Returns Form Builder list.</returns>
        FormBuilderListModel GetFormBuilderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get form details on the basis of form builder id.
        /// </summary>
        /// <param name="formBuilderId">form builder id.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Form builder model with data.</returns>
        FormBuilderModel GetForm(int formBuilderId, ExpandCollection expands);

        /// <summary>
        /// Delete Form Builder.
        /// </summary>
        /// <param name="formBuilderId">FormBuilder Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteFormBuilder(ParameterModel formBuilderId);

        /// <summary>
        /// Check form code already exist or not.
        /// </summary>
        /// <param name="formCode">formCode</param>
        /// <returns>Returns true if form code exist.</returns>
        bool IsFormCodeExist(string formCode);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="expands">Expands for Attribute.</param>
        /// <param name="filters">Filters for Attribute.</param>
        /// <param name="sorts">Sorts for Attribute.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">page size.</param>
        /// <returns>Returns list of unassigned attributes.</returns>
        GlobalAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get unassigned attribute groups.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filters Collection</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns list of unassigned groups.</returns>
        GlobalAttributeGroupListModel GetUnAssignedGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get From builder Attribute and Group
        /// </summary>
        /// <param name="formBuilderId">int formbuilderId</param>
        /// <param name="fromcode">string fromcode</param>
        /// <returns>returns FormBuilderAttributeGroupModel</returns>
        FormBuilderAttributeGroupModel GetFormAttributeGroup(int formBuilderId, int localeId, int mappingId = 0);

        /// <summary>
        /// Associate group to form.
        /// </summary>
        /// <param name="globalAttributeGroupEntityModel">Model with group data.</param>
        /// <returns>Returns true/false</returns>
        bool AssociateGroups(GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel);

        /// <summary>
        /// Associate attribute to form.
        /// </summary>
        /// <param name="globalAttributeGroupEntityModel">Model with attributes data.</param>
        /// <returns>Returns true/false</returns>
        bool AssociateAttributes(GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel);

        /// <summary>
        /// Update form builder data.
        /// </summary>
        /// <param name="formBuilderModel">FormBuilder model to update.</param>
        /// <returns>Returns updated formBuilder model.</returns>
        FormBuilderModel UpdateFormBuilder(FormBuilderModel formBuilderModel);

        /// <summary>
        /// Update Attribute DisplayOrder of form
        /// </summary>
        /// <param name="model">FormBuilderAttributeGroupDisplayOrderModel</param>
        /// <returns>Returns true/false</returns>
        bool UpdateAttributeDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model);

        /// <summary>
        /// Update group DisplayOrder of form
        /// </summary>
        /// <param name="model">FormBuilderAttributeGroupDisplayOrderModel</param>
        /// <returns>Returns true/false</returns>
        bool UpdateGroupDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model);

        /// <summary>
        /// Un Associate groups from form builder.
        /// </summary>
        /// <param name="formBuilderId">Form builder Id</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>true/false</returns>
        bool UnAssociateFormBuilderGroups(int formBuilderId, int groupId);

        /// <summary>
        /// Un Associate groups from form builder.
        /// </summary>
        /// <param name="formBuilderId">Form builder Id</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>true/false</returns>
        bool UnAssociateFormBuilderAttributes(int formBuilderId, int attributeId);

        /// <summary>
        /// Create form template.
        /// </summary>
        /// <param name="model">Model FormSubmitModel</param>
        /// <returns>Returns FormSubmitModel</returns>
        FormSubmitModel CreateFormTemplate(FormSubmitModel model);

        /// <summary>
        /// To check Form Attribute Value Unique
        /// </summary>
        /// <param name="attributeCodeValues">attributeCodeValues</param>
        /// <returns>Return ParameterModel</returns>
        ParameterModel FormAttributeValueUnique(GlobalAttributeValueParameterModel attributeCodeValues);
    }
}
