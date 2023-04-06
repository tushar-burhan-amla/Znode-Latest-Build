using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IContainerTemplateClient : IBaseClient
    {
        /// <summary>
        /// Get the List of Container Template
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ContainerTemplateListModel model</returns>
        ContainerTemplateListModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create Container Template
        /// </summary>
        /// <param name="model">ContainerTemplateCreateModel model</param>
        /// <returns>ContainerTemplateModel model</returns>
        ContainerTemplateModel Create(ContainerTemplateCreateModel model);

        /// <summary>
        /// Get Container Template
        /// </summary>
        /// <param name="templateCode">template code</param>
        /// <returns>ContainerTemplateModel model</returns>
        ContainerTemplateModel GetContainerTemplate(string templateCode);

        /// <summary>
        /// Update Container Template
        /// </summary>
        /// <param name="model">ContainerTemplateUpdateModel model</param>
        /// <returns>ContainerTemplateModel model</returns>
        ContainerTemplateModel Update(ContainerTemplateUpdateModel model);

        /// <summary>
        /// Delete Container Template
        /// </summary>
        /// <param name="containerTemplateIds">Container Template Ids</param>
        /// <returns>status</returns>
        bool Delete(ParameterModel containerTemplateIds);

        /// <summary>
        /// Validate if the Container Template exists
        /// </summary>
        /// <param name="templateCode">Template code</param>
        /// <returns>status</returns>
        bool IsContainerTemplateExist(string templateCode);


    }
}
