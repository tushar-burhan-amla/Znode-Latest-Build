using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface INavigationAgent
    {
        /// <summary>
        /// Get the details required for generic navigation control.
        /// </summary>
        /// <param name="Id">Parameters required to get details of generic navigation control</param>
        /// <param name="controllerName">Name of controller</param>
        /// <param name="entity">Name of entity</param>
        /// <param name="queryParameter">Query parameters</param>
        /// <param name="areaName">Name of area</param>
        /// <param name="editAction">Name of action shows edit record</param>
        /// <param name="deleteAction">Name of action to delete record</param>
        /// <param name="detailAction">Name of action shows details page</param>
        /// <param name="entity">Name of entity to get record details</param>
        /// <param name="queryParameter">Name of queryParameter to get record details</param>
        /// <returns>Navigation ViewModel</returns>
        NavigationViewModel GetNavigationDetails(string Id, string controllerName, string entity, string queryParameter, string areaName = null, string editAction = null, string deleteAction = null, string detailAction = null);
    }
}
