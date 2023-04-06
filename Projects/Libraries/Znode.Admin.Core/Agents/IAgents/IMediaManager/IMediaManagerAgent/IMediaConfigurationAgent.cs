using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface IMediaConfigurationAgent
    {
        /// <summary>
        /// Get list of media server.
        /// </summary>
        /// <returns>Returns list of media server.</returns>
        MediaServerListModel GetMediaServer();

        /// <summary>
        /// Get media configuration.
        /// </summary>
        /// <param name="serverId">Server Id.</param>
        /// <returns>Returns the media configuration.</returns>
        MediaConfigurationViewModel GetMediaConfiguration(int serverId);

        /// <summary>
        /// Update media configuration.
        /// </summary>
        /// <param name="model">Media configuration model.</param>
        /// <returns>Returns updated model.</returns>
        MediaConfigurationViewModel UpdateMediaConfiguration(MediaConfigurationViewModel model);

        /// <summary>
        /// Create media configuration.
        /// </summary>
        /// <param name="model">Model to create.</param>
        /// <returns>Returns created model.</returns>
        MediaConfigurationViewModel CreateMediaConfiguration(MediaConfigurationViewModel model);

        /// <summary>
        /// Get default media configuration.
        /// </summary>
        /// <returns>Returns default media configuration.</returns>
        MediaConfigurationViewModel GetDefaultMediaConfiguration();

        /// <summary>
        /// Get option list of media server.
        /// </summary>
        /// <param name="selectedServerId">Selected server id.</param>
        /// <returns>Returns options for server.</returns>
        string GetOptionsForMediaServer(int selectedServerId);

        /// <summary>
        /// Set local settings for media server.
        /// </summary>
        /// <param name="model">Model to set local configuration.</param>
        void SetLocalConfiguration(MediaConfigurationViewModel model);

        /// <summary>
        /// For Sync media 
        /// </summary>
        bool SyncMedia(string folderName);

        /// <summary>
        /// get Local Serer Url
        /// </summary>
        /// <returns>url string</returns>
        string GetLocalServerURL();

        /// <summary>
        /// get Network Drive Url
        /// </summary>
        /// <returns>url string</returns>
        string GetNetworkDriveURL();

        /// <summary>
        /// Generate all images.
        /// </summary>
        /// <returns>True/false according to status.</returns>
        bool GenerateImages();
    }
}
