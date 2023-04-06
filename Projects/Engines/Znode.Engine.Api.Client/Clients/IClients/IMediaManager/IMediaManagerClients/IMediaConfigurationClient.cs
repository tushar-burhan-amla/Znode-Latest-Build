using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IMediaConfigurationClient : IBaseClient
    {
        /// <summary>
        /// Gets the media server list.
        /// </summary>
        /// <param name="expands">Expands to get the associated class data.</param>
        /// <param name="filters">Filter data with help of filters.</param>
        /// <param name="sortCollection">Sort data with help of filters sortCollection.</param>
        /// <param name="pageIndex">Page number to see on page pageIndex.</param>
        /// <param name="recordPerPage">RecordPerPage.</param>
        /// <returns>Returns list of media server.</returns>
        MediaServerListModel GetMediaServerList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get the media configuration by filters.
        /// </summary>
        /// <param name="filters">To filter the result.</param>
        /// <returns>Returns media configuration model.</returns>
        MediaConfigurationModel GetMediaConfiguration(FilterCollection filters);

        /// <summary>
        /// Get the media configuration by filters and expands.
        /// </summary>
        /// <param name="filters">To filter the result.</param>
        /// <param name="expands">Expand.</param>
        /// <returns>Returns media configuration model.</returns>
        MediaConfigurationModel GetMediaConfiguration(FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Create media configuration.
        /// </summary>
        /// <param name="model">Media configuration model.</param>
        /// <returns>Returns the created model.</returns>
        MediaConfigurationModel CreateMediaConfiguration(MediaConfigurationModel model);

        /// <summary>
        /// Update media configuration.
        /// </summary>
        /// <param name="model">Media configuration model.</param>
        /// <returns>Returns the updated model.</returns>
        MediaConfigurationModel UpdateMediaConfiguration(MediaConfigurationModel model);

        /// <summary>
        /// Get the default media configuration set.
        /// </summary>
        /// <returns>Returns media configuration.</returns>
        MediaConfigurationModel GetDefaultMediaConfiguration();

        /// <summary>
        /// For Sync media 
        /// </summary>
        void SyncMedia(string folderName);

        /// <summary>
        /// Generate all images.
        /// </summary>
        /// <returns>True/false according to status.</returns>
        bool GenerateImages();
    }
}
