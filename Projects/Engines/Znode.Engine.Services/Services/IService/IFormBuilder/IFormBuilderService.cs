using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IFormBuilderService
    {
        /// <summary>
        /// Get the list of form builder.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with form builder list.</param>
        /// <param name="filters">Filters to be applied on form builder list.</param>
        /// <param name="sorts">Sorting to be applied on form builder list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns form builder list.</returns>
        FormBuilderListModel GetFormBuilderList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create a form.
        /// </summary>
        /// <param name="formBuilderModel">Model with data.</param>
        /// <returns>Created form details.</returns>
        FormBuilderModel CreateForm(FormBuilderModel formBuilderModel);

        /// <summary>
        /// Get FormBuilder details By Id
        /// </summary>
        /// <param name="formBuilderId">FormBuilder Id</param>
        /// <param name="expands">Expands for the attribute.</param>
        /// <returns>FormBuilderModel </returns>
        FormBuilderModel GetFormBuilderById(int formBuilderId, NameValueCollection expands);

        /// <summary>
        /// Assign groups to Form pass formbuilderId & GroupIds
        /// </summary>
        /// <param name="model">GlobalAttributeGroupEntityModel Model</param>
        /// <returns>boolean value true/false</returns>
        bool AssignGroupsToForm(GlobalAttributeGroupEntityModel model);

        /// <summary>
        /// Assign groups to Form pass formbuilderId & attributeIds
        /// </summary>
        /// <param name="model">GlobalAttributeGroupEntityModel Model</param>
        /// <returns>boolean value true/false</returns>
        bool AssignAttributesToForm(GlobalAttributeGroupEntityModel model);

        /// <summary>
        /// Un Assign Group from Formbuilder
        /// </summary>
        /// <param name="formbuilderId">Formbuilder Id</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>returns true or false</returns>
        bool UnAssignFormBuilderGroup(int formBuilderId, int groupId);

        /// <summary>
        /// Un Assign Attribute from Formbuilder
        /// </summary>
        /// <param name="formbuilderId">Formbuilder Id</param>
        /// <param name="attributeId">Group Id</param>
        /// <returns>returns true or false</returns>
        bool UnAssignFormBuilderAttribute(int formBuilderId, int attributeId);


        /// <summary>
        /// Update form builder Model.
        /// </summary>
        /// <param name="model">form builder model to be updated.</param>
        /// <returns>True or false.</returns>
        bool Update(FormBuilderModel model);

        /// <summary>
        /// Update Attribute Display Order
        /// </summary>
        /// <param name="model">FormBuilderAttributeGroupDisplayOrderModel</param>
        /// <returns>returns true/false</returns>
        bool UpdateAttributeDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model);

        /// <summary>
        /// Update Group Display Order
        /// </summary>
        /// <param name="model">FormBuilderAttributeGroupDisplayOrderModel</param>
        /// <returns>returns true/false</returns>
        bool UpdateGroupDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model);

        /// <summary>
        /// Get Un Assigned Attributes list
        /// </summary>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="page">Page Collection</param>
        /// <returns>Attributes list</returns>
        GlobalAttributeListModel GetUnAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Un Assigned Groups list
        /// </summary>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="page">Page Collection</param>
        /// <returns>Group List</returns>
        GlobalAttributeGroupListModel GetUnAssignedGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete FormBuilder by FormBuilderId.
        /// </summary>
        /// <param name="formBuilderId">Id of FormBuilder</param>
        /// <returns>return status</returns>
        bool DeleteFormBuilder(ParameterModel formBuilderId);

        /// <summary>
        /// Get FormBuilder Attribute Group.
        /// </summary>
        /// <param name="formBuilderId">int formbuilderId</param>
        /// <param name="localeId">int localeId</param>
        /// <returns>returns FormBuilderAttributeGroupModel</returns>
        FormBuilderAttributeGroupModel GetFormBuilderAttributeGroup(int formBuilderId, int localeId, int mappingId);

        /// <summary>
        /// Check form Code already exist or not.
        /// </summary>
        /// <param name="formCode">formCode</param>
        /// <returns>Returns true if form code already exist.</returns>
        bool IsFormCodeExist(string formCode);
        
        /// <summary>
        /// Save form template 
        /// </summary>
        /// <param name="model">FormSubmitModel</param>
        /// <returns> Return FormSubmitModel.</returns>
        FormSubmitModel CreateFormTemplate(FormSubmitModel model);

        /// <summary>
        /// To check form attribute value is unique.
        /// </summary>
        /// <param name="attributeCodeValues">attributeCodeValues model</param>
        /// <returns>Returns string</returns>
        string FormAttributeValueUnique(GlobalAttributeValueParameterModel attributeCodeValues);
    }
}
