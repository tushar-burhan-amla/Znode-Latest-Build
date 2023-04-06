using Znode.Engine.Api.Models;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.WebStore.Core.Agents
{
    public interface IFormBuilderAgent
    {
        /// <summary>
        /// Get Entity Associated Attribute Details
        /// </summary>
        /// <param name="entityId">id of the entity</param>
        /// <param name="entityType">type of the entity</param>
        /// <returns>return the Associated Attribute Details.</returns>
        FormBuilderAttributeGroupViewModel GetFormTemplate(int formBuilderId, int localeId, int mappingId = 0);

        /// <summary>
        /// Save Form template data 
        /// </summary>
        /// <param name="bindDataModel">Pass bindDataModel</param>
        /// <returns>Returns FormSubmitViewModel</returns>
        FormSubmitViewModel CreateFormTemplate(BindDataModel bindDataModel);

        /// <summary>
        /// Check value unique or not.
        /// </summary>
        /// <param name="model">GlobalAttributeValueParameterModel</param>
        /// <returns>Returns string.</returns>
        string IsFormAttributeValueUnique(GlobalAttributeValueParameterModel model);

        /// <summary>
        /// Get Entity Associated Attribute Details
        /// </summary>
        /// <param name="entityId">id of the entity</param>
        /// <param name="entityType">type of the entity</param>
        /// <returns>return the Associated Attribute Details.</returns>
        GlobalAttributeEntityDetailsViewModel GetEntityAttributeDetails(int entityId, string entityType);

        /// <summary>
        /// Save Entity Attributes value
        /// </summary>
        /// <param name="model">BindDataModel</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>returns Entity Attribute View Model</returns>
        EntityAttributeViewModel SaveEntityAttributeDetails(BindDataModel model, out string errorMessage);
    }
}
