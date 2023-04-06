using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IContainerTemplateService
    {
        /// <summary>
        /// Get the List of Container Template
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>ContainerTemplateListModel model</returns>
        ContainerTemplateListModel GetContainerTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create Container Template
        /// </summary>
        /// <param name="containerTemplate">ContainerTemplateCreateModel model</param>
        /// <returns>ContainerTemplateModel model</returns>
        ContainerTemplateModel CreateContainerTemplate(ContainerTemplateCreateModel containerTemplate);

        /// <summary>
        /// Get Container Template
        /// </summary>
        /// <param name="templateCode">Container Template code</param>
        /// <returns>ContainerTemplateModel model</returns>
        ContainerTemplateModel GetContainerTemplate(string templateCode);

        /// <summary>
        /// Update Container Template
        /// </summary>
        /// <param name="containerTemplateModel">ContainerTemplateUpdateModel model</param>
        /// <returns>ContainerTemplate model</returns>
        ContainerTemplateModel UpdateContainerTemplate(ContainerTemplateUpdateModel containerTemplateModel);

        /// <summary>
        /// Delete Container Template
        /// </summary>
        /// <param name="ContainerTemplateIds">Container Template Ids</param>
        /// <returns>status</returns>
        bool DeleteContainerTemplateById(ParameterModel ContainerTemplateIds);

        /// <summary>
        /// Delete Template By Code
        /// </summary>
        /// <param name="templateCode">templateCode</param>
        /// <returns>Status</returns>
        bool DeleteContainerTemplateByCode(string templateCode);

        /// <summary>
        /// Validate if Container Template Exists
        /// </summary>
        /// <param name="code">Container Template code</param>
        /// <returns>status</returns>
        bool IsContainerTemplateExists(string templateCode);





    }
}
