using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IManageMessageCache 
    {
        #region Manage Message
        /// <summary>
        /// Get ManageMessage list. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetManageMessages(string routeUri, string routeTemplate);

        /// <summary>
        ///  Get ManageMessage details.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <param name="manageMessageMapperModel">ManageMessageMapperModel.</param>
        /// <returns>String Data.</returns>
        string GetManageMessage(ManageMessageMapperModel manageMessageMapperModel, string routeUri, string routeTemplate);
        #endregion
    }
}
