using System;
using System.Drawing;
using System.IO;
using System.Web;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.MediaStorage;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using System.Linq;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class MediaConfigurationAgent : BaseAgent, IMediaConfigurationAgent
    {
        #region Private Variables
        private readonly IMediaConfigurationClient _mediaConfigurationClient;
        private readonly string LocaleServer = "Local";
        #endregion

        #region Constructor
        public MediaConfigurationAgent(IMediaConfigurationClient mediaConfigurationClient)
        {
            _mediaConfigurationClient = GetClient<IMediaConfigurationClient>(mediaConfigurationClient);

        }
        #endregion

        #region public Methods
        //Get List Of Servers Available.
        public virtual MediaServerListModel GetMediaServer()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeMediaServerMasterEnum.ZnodeMediaConfigurations.ToString());
            return _mediaConfigurationClient.GetMediaServerList(expands);
        }

        //Get Current Configuration setting Present.
        public virtual MediaConfigurationViewModel GetMediaConfiguration(int serverId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.MediaServerMasterId, FilterOperators.Equals, serverId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.Caller, FilterOperators.Equals, FilterKeys.MediaSettings));
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return _mediaConfigurationClient.GetMediaConfiguration(filters).ToViewModel<MediaConfigurationViewModel>();
        }

        //Get Default Configuration setting for Media.
        public virtual MediaConfigurationViewModel GetDefaultMediaConfiguration()
            => _mediaConfigurationClient.GetDefaultMediaConfiguration().ToViewModel<MediaConfigurationViewModel>();

        //Update Existing Media Configuration Setting.
        public virtual MediaConfigurationViewModel UpdateMediaConfiguration(MediaConfigurationViewModel model)
        {
           try
          {
               ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
              //Check if server credential is valid.
              if (model.Server != LocaleServer && model.Server != AdminConstants.NetworkDrive)
                    CheckServerCredential(model);

                MediaConfigurationViewModel updatedModel = _mediaConfigurationClient.UpdateMediaConfiguration(model.ToModel<MediaConfigurationModel>()).ToViewModel<MediaConfigurationViewModel>();
                SaveInSession<int>(AdminConstants.MediaConfigurationIdSessionKey, updatedModel.MediaConfigurationId);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return updatedModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                return (MediaConfigurationViewModel)GetViewModelWithErrorMessage(model, MediaManager_Resources.ErrorValidServerCredentials);
            }
        }

        //Create New Media Configration Setting.
        public virtual MediaConfigurationViewModel CreateMediaConfiguration(MediaConfigurationViewModel model)
            => _mediaConfigurationClient.CreateMediaConfiguration(model.ToModel<MediaConfigurationModel>()).ToViewModel<MediaConfigurationViewModel>();

        //Get the option for media server and select the current server.
        public virtual string GetOptionsForMediaServer(int selectedServerId)
        {
          ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
          ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { selectedServerId = selectedServerId });
          MediaServerListModel mediaServerList = new MediaServerListModel();

            //Get media server list.
            mediaServerList = MediaServerList();
            if (mediaServerList?.MediaServers?.Count > 0)
            {
                var localServerID = mediaServerList.MediaServers.Find(x => x.IsOtherServer == false).MediaServerMasterId;
                string options = $"<option  value='{localServerID}' data-partialviewname=' '>Select Server</option>";
                foreach (var item in mediaServerList.MediaServers.FindAll(x => x.IsOtherServer == true))
                {
                    string selected = item.MediaServerMasterId == selectedServerId ? "selected='selected'" : string.Empty;
                    options += $"<option {selected} value='{ item.MediaServerMasterId }' data-partialviewname='{item.PartialViewName}' >{item.ServerName}</option>";
                }
              ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

              return options;
            }
      ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

      return string.Empty;
        }

        //Set Configuration for Local Server.
        public virtual void SetLocalConfiguration(MediaConfigurationViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            MediaServerListModel mediaServerList = MediaServerList();

            //Map Media Configuration Properties.
            MapMediaConfigurationProperties(model, mediaServerList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
        }
        //For sync media functionality
        public virtual bool SyncMedia(string folderName)
        {
            try
            {
              ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
              _mediaConfigurationClient.SyncMedia(folderName);
              ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

             return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                return false;
            }
        }
        //Get local server url.
        public virtual string GetLocalServerURL()
          => GetMediaServer().MediaServers?.FirstOrDefault(x => x.ServerName == AdminConstants.DefaultMediaServerName)?.URL;

        public virtual string GetNetworkDriveURL()
          => GetMediaServer().MediaServers?.FirstOrDefault(x => x.ServerName == AdminConstants.NetworkDrive)?.URL;

        // Generate all images.
        public virtual bool GenerateImages()
            => _mediaConfigurationClient.GenerateImages();
        #endregion

        #region Private Methods
        //Map Media Configuration Properties.
        private static void MapMediaConfigurationProperties(MediaConfigurationViewModel model, MediaServerListModel mediaServerList)
        {
            model.AccessKey = string.Empty;
            model.Server = model.IsNetworkDrive ? AdminConstants.NetworkDrive : AdminConstants.DefaultMediaServerName;
            model.SecretKey = string.Empty;
            model.URL = model.IsNetworkDrive ? model.URL : string.Empty;
            model.CDNUrl = null;
            model.BucketName = AdminConstants.DefaultMediaFolder;
            model.MediaServerMasterId = mediaServerList.MediaServers.Find(x => x.ServerName.Equals(model.Server, StringComparison.OrdinalIgnoreCase)).MediaServerMasterId;
        }

        //Validate Server Access key by uploading default image.
        private bool CheckServerCredential(MediaConfigurationViewModel model)
        {
             ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
             try
            {
                //Sets the server connection.
                ServerConnector _connectorobj = new ServerConnector(new FileUploadPolicyModel(model.AccessKey, model.SecretKey, model.BucketName, model.ThumbnailFolderName, model.URL, model.NetworkUrl));
                string defaultImagePath = HttpContext.Current.Server.MapPath(ZnodeAdminSettings.DefaultImagePath);
                ZnodeLogging.LogMessage("defaultImagePath ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { defaultImagePath = defaultImagePath });
                using (Image image = Image.FromFile(defaultImagePath))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        image.Save(stream, MediaManagerAgent.GetImageFormat(Path.GetExtension(defaultImagePath)));
                        model.MediaServerMasterId = Convert.ToInt32(model.Server);
                        var _mediaServers = MediaServerList()?.MediaServers.FirstOrDefault(x => x.MediaServerMasterId == Convert.ToInt32(model.Server));
                        string className = _mediaServers?.ClassName;
                        model.Server = _mediaServers?.ServerName;
                        _connectorobj.CallConnector(className, MediaStorageAction.Upload, stream, Path.GetFileName(defaultImagePath), _connectorobj.UploadPolicyModel.ThumbnailFolderName);
                    }
                }
                  ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

                  return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        //Get media server list if present in session or make a call.
        private MediaServerListModel MediaServerList()
        {
            MediaServerListModel mediaServerList = GetFromSession<MediaServerListModel>(AdminConstants.MediaServerListSessionKey);

            //Get media server list.
            if (HelperUtility.IsNull(mediaServerList))
            {
                mediaServerList = GetMediaServer();
                SaveInSession<MediaServerListModel>(AdminConstants.MediaServerListSessionKey, mediaServerList);
            }
            
            return mediaServerList;
        }
        #endregion
    }
}