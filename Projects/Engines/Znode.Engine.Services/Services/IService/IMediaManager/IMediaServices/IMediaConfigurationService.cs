using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IMediaConfigurationService
    {
        /// <summary>
        /// Gets the media server list.
        /// </summary>
        /// <param name="expands">expands to get the associated class data.</param>
        /// <param name="filters">filter data with help of filters</param>
        /// <param name="sortCollection">sort data with help of filters sortCollection</param>
        /// <param name="page">page number to see on page pageIndex</param>
        /// <returns>Returns list of media server.</returns>
        MediaServerListModel GetMediaServers(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get media configuration.
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="expands">Expands</param>
        /// <returns>Returns media configuration.</returns>
        MediaConfigurationModel GetMediaConfiguration(FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Create media configuration.
        /// </summary>
        /// <param name="model">Media configuration model.</param>
        /// <returns>Returns the created model.</returns>
        MediaConfigurationModel Create(MediaConfigurationModel model);

        /// <summary>
        /// Update media configuration.
        /// </summary>
        /// <param name="model">Media configuration model.</param>
        /// <returns>Returns the updated model.</returns>
        MediaConfigurationModel Update(MediaConfigurationModel model);

        /// <summary>
        /// Get the default media configuration.
        /// </summary>
        /// <returns>Returns default configuration.</returns>
        MediaConfigurationModel GetDefaultMediaConfiguration();

        /// <summary>
        /// For inserting media list 
        /// </summary>
        /// <param name="listMedia">get data in key value pair</param>
        /// <param name="mediaConfigurationId">mediaConfigurationId</param>
        void InsertSyncMedia(Dictionary<string, long> listMedia, int mediaConfigurationId);

        /// <summary>
        /// Generate all images.
        /// </summary>
        /// <returns>True/false according to status.</returns>
        bool GenerateImages();

        /// <summary>
        /// Get global media display setting.
        /// </summary>
        /// <param name="configurationModel">MediaConfigurationModel</param>
        /// <returns>Returns global media display setting.</returns>
        GlobalMediaDisplaySettingModel GetGlobalMediaDisplaySetting(MediaConfigurationModel configurationModel);

        /// <summary>
        /// Get Media Count
        /// </summary>
        /// <returns>Total media count.</returns>
        int GetMediaCount();

        /// <summary>
        /// Get the media data list.
        /// </summary>
        /// <param name="expands">expands to get the associated class data.</param>
        /// <param name="filters">filter data with help of filters</param>
        /// <param name="sortCollection">sort data with help of filters sortCollection</param>
        /// <param name="page">page number to see on page pageIndex</param>
        /// <returns>Returns list of media.</returns>
        MediaManagerListModel GetMediaListData(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

    }
}
