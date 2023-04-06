using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IContainerTemplateAgent
    {
        /// <summary>
        /// List of Container Template
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ContainerTemplateListViewModel model</returns>
        ContainerTemplateListViewModel List(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Create Container Template
        /// </summary>
        /// <param name="containerTemplateViewModel">containerTemplateViewModel model</param>
        /// <returns>ContainerTemplateViewModel</returns>
        ContainerTemplateViewModel Create(ContainerTemplateViewModel containerTemplateViewModel);

        /// <summary>
        /// Get Container Template
        /// </summary>
        /// <param name="containerTemplateId">container Template Id</param>
        /// <returns>containerTemplateViewModel model</returns>
        ContainerTemplateViewModel GetContainerTemplate(string templateCode);

        /// <summary>
        /// Update Container Template
        /// </summary>
        /// <param name="model">ContainerTemplateViewModel model</param>
        /// <returns>ContainerTemplateViewModel model</returns>
        ContainerTemplateViewModel Update(ContainerTemplateViewModel model);

        /// <summary>
        /// Delete Container Template
        /// </summary>
        /// <param name="containerTemplateIds">Container Template ids</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>status</returns>
        bool DeleteContainerTemplate(string containerTemplateIds, string fileName, out string errorMessage);

        /// <summary>
        /// Copy Container Template
        /// </summary>
        /// <param name="model">ContainerTemplateViewModel model</param>
        /// <returns>ContainerTemplateViewModel model</returns>
        ContainerTemplateViewModel CopyContainerTemplate(ContainerTemplateViewModel model);

        /// <summary>
        /// Download Container Template
        /// </summary>
        /// <param name="containerTemplateId">Container Template Id</param>
        /// <param name="fileName">filename</param>
        /// <returns>status</returns>
        string DownloadContainerTemplate(int containerTemplateId, string fileName);

        /// <summary>
        /// validate if the Container Template Exists
        /// </summary>
        /// <param name="name">Container Template name</param>
        /// <returns>status</returns>
        bool IsContainerTemplateExist(string templateCode);

    }
}
