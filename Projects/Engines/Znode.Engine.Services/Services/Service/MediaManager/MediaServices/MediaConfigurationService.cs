using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Hangfire;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.MediaStorage;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class MediaConfigurationService : BaseService, IMediaConfigurationService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeMediaConfiguration> _mediaConfigurationRepository;
        private readonly IZnodeRepository<ZnodeMediaServerMaster> _serverMasterRepository;
        private readonly IZnodeRepository<ZnodeMedia> _mediaRepository;
        private readonly IZnodeRepository<ZnodeMediaPath> _mediaPathRepository;
        private readonly IZnodeRepository<ZnodeMediaPathLocale> _mediaPathLocaleRepository;
        private readonly IZnodeRepository<ZnodeMediaCategory> _mediaCategoryRepository;
        private readonly IZnodeRepository<ZnodeLocale> _localeRepository;
        protected readonly IZnodeRepository<ZnodeGlobalMediaDisplaySetting> _globalMediaDisplaySettingRepository;
        private readonly IERPJobs _eRPJob;
        #endregion

        #region Constructor
        public MediaConfigurationService()
        {
            _mediaConfigurationRepository = new ZnodeRepository<ZnodeMediaConfiguration>();
            _serverMasterRepository = new ZnodeRepository<ZnodeMediaServerMaster>();
            _mediaRepository = new ZnodeRepository<ZnodeMedia>();

            _mediaPathRepository = new ZnodeRepository<ZnodeMediaPath>();
            _mediaPathLocaleRepository = new ZnodeRepository<ZnodeMediaPathLocale>();
            _mediaCategoryRepository = new ZnodeRepository<ZnodeMediaCategory>();
            _localeRepository = new ZnodeRepository<ZnodeLocale>();
            _globalMediaDisplaySettingRepository = new ZnodeRepository<ZnodeGlobalMediaDisplaySetting>();
            _eRPJob = GetService<IERPJobs>();
        }
        #endregion

        #region Public Methods
        //Get media server list.
        public virtual MediaServerListModel GetMediaServers(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate ZnodeMediaServerMaster list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeMediaServerMaster> list = _serverMasterRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("ZnodeMediaServerMaster list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, list?.Count());
            MediaServerListModel mediaServerListModel = new MediaServerListModel() { MediaServers = list.ToModel<MediaServerModel>()?.ToList() };

            //Set for pagination
            mediaServerListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return mediaServerListModel;
        }

        //Get media configurations.
        public virtual MediaConfigurationModel GetMediaConfiguration(FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            bool removeSecretKey = false;
            if (filters.Exists(x => x.FilterName == Znode.Libraries.ECommerce.Utilities.FilterKeys.Caller))
            {
                removeSecretKey = true;
                filters.Remove(filters.FirstOrDefault(x => x.FilterName == Znode.Libraries.ECommerce.Utilities.FilterKeys.Caller));
            }
            //Gets the where clause with filter Values.  
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to get mediaConfiguration ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseModel);
            var mediaConfiguration = _mediaConfigurationRepository.GetEntity(whereClauseModel.WhereClause, GetExpands(expands), whereClauseModel.FilterValues).ToModel<MediaConfigurationModel>();
            ZnodeLogging.LogMessage("mediaConfiguration:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaConfiguration);
            // We cannot show secret key to the user hence hiding it, however user can update it from the page.
            if (HelperUtility.IsNotNull(mediaConfiguration) && removeSecretKey) mediaConfiguration.SecretKey = "";
            if (IsNotNull(mediaConfiguration))
            {
                mediaConfiguration.GlobalMediaDisplaySetting = GetGlobalMediaDisplaySetting(mediaConfiguration);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return mediaConfiguration;
        }

        //Create media configuration.
        public virtual MediaConfigurationModel Create(MediaConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            //Get Url from the server.
            model.URL = model.Server == ZnodeConstant.NetworkDrive ? model.URL : GetUrlFromServer(model);
            //Set the is active field to false and update it in database too.
            SetIsActiveFalse();
            ZnodeMediaConfiguration createdEntity = _mediaConfigurationRepository.Insert(model.ToEntity<ZnodeMediaConfiguration>());

            if (createdEntity?.MediaConfigurationId > 0)
            {
                UpdateGlobalMediaDisplaySetting(model?.GlobalMediaDisplaySetting);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessCreateMediaConfiguration, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return createdEntity.ToModel<MediaConfigurationModel>();
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Update media configurations.
        public virtual MediaConfigurationModel Update(MediaConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter MediaConfigurationModel having MediaConfigurationId and MediaServerMasterId", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { model.MediaConfigurationId, model.MediaServerMasterId });
            
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaConfigurationModelNull);

            if (model.MediaServerMasterId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, "Media server master id can not less than zero.");

            if (model.MediaConfigurationId <= 0 && string.IsNullOrEmpty(model.Server))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMediaConfigurationIdGreaterThanZero);

            ZnodeMediaConfiguration mediaConfiguration = _mediaConfigurationRepository.Table.FirstOrDefault(x => x.MediaServerMasterId == model.MediaServerMasterId);

            if (mediaConfiguration?.MediaConfigurationId > 0)
            {
                model.MediaConfigurationId = mediaConfiguration.MediaConfigurationId;
                model.IsActive = true;
                model.URL = model.Server == ZnodeConstant.NetworkDrive ? model.URL : GetUrlFromServer(model);
                SetIsActiveFalse();
                _mediaConfigurationRepository.Update(model.ToEntity<ZnodeMediaConfiguration>());
                UpdateGlobalMediaDisplaySetting(model?.GlobalMediaDisplaySetting);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessUpdateMediaConfiguration, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            }
            else
            {
                model.MediaConfigurationId = 0;
                model.IsActive = true;
                return Create(model);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }


        //Get default media configuration.
        public virtual MediaConfigurationModel GetDefaultMediaConfiguration()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            MediaConfigurationModel data = Equals(HttpRuntime.Cache["DefaultMediaConfigurationCache"], null)
              ? InsertAndGetDefaultMediaConfiguration()
              : (MediaConfigurationModel)HttpRuntime.Cache.Get("DefaultMediaConfigurationCache");
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return data;
        }

        //For inserting media list into database
        public virtual void InsertSyncMedia(Dictionary<string, long> listMedia, int mediaConfigurationId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter mediaConfigurationId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { mediaConfigurationId });
            int MediaPathId = InsertMediaPath(MediaManager_Resources.RootFolder);
            if (!HelperUtility.IsNull(MediaPathId))
            {
                int? MediaPathLocaleId = InsertMediaPathLocal(MediaPathId, MediaManager_Resources.FolderName);
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { MediaPathId = MediaPathId, MediaPathLocaleId = MediaPathLocaleId });
                if (!HelperUtility.IsNull(MediaPathLocaleId))
                {
                    var mediaListModel = InsertMediaList(listMedia, mediaConfigurationId);
                    if (!HelperUtility.IsNull(mediaListModel))
                    {
                        InsertMediaCategoryList(MediaPathLocaleId, mediaListModel);
                    }
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
        }

        #region Generate Images
        //Generate all images.
        public virtual bool GenerateImages()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            try
            {
                //call the image utility scheduler 
                int chunkSize = ZnodeConstant.ImageChunkSize;
                int localeId = 1;
                int loginUserId = GetLoginUserId();
                ZnodeLogging.LogMessage("loginUserId", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, new object[] { loginUserId });

                string apiURL = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}";
                string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers["Token"]) ? "0" : HttpContext.Current.Request.Headers["Token"];

                var eRPTaskSchedulerModel = new ERPTaskSchedulerModel
                {
                    SchedulerName = ZnodeConstant.ImageHelper,
                    SchedulerCallFor = ZnodeConstant.ImageHelper,
                    IsInstantJob = true
                };
                eRPTaskSchedulerModel.ExeParameters = $"{ZnodeConstant.ImageHelper},{chunkSize},{loginUserId},{localeId},{apiURL},{0},{HttpContext.Current.Request.Headers["Authorization"]?.Replace("Basic ", "")},{tokenValue},{ZnodeApiSettings.RequestTimeout}";
                ZnodeLogging.LogMessage($"Arguments Passed : {eRPTaskSchedulerModel.ExeParameters}", ZnodeLogging.Components.ImageScheduler.ToString(), TraceLevel.Info);
                bool result = _eRPJob.ConfigureJobs(eRPTaskSchedulerModel, out string hangfireJobId);
                ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ImageScheduler.ToString(), TraceLevel.Info);
                return result;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"{Admin_Resources.ErrorProcessingImage}", ZnodeLogging.Components.ImageScheduler.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        // Get global media display setting.
        public virtual GlobalMediaDisplaySettingModel GetGlobalMediaDisplaySetting(MediaConfigurationModel configurationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            GlobalMediaDisplaySettingModel globalMediaDisplaySetting = _globalMediaDisplaySettingRepository.Table.FirstOrDefault().ToModel<GlobalMediaDisplaySettingModel>();

            if (IsNotNull(globalMediaDisplaySetting))
            {
                if (globalMediaDisplaySetting?.MediaId > 0)
                {
                    string path = _mediaRepository.Table.Where(x => x.MediaId == globalMediaDisplaySetting.MediaId).Select(x => x.Path)?.FirstOrDefault();

                    string mediaServerThumbnailPath = $"{GetMediaServerUrl(configurationModel)}{configurationModel.ThumbnailFolderName}/{path}";

                    globalMediaDisplaySetting.DefaultImageName = string.IsNullOrEmpty(path) ? string.Empty : path;
                    globalMediaDisplaySetting.MediaPath = string.IsNullOrEmpty(mediaServerThumbnailPath) ? string.Empty : mediaServerThumbnailPath;
                }
            }
            else
            {
                globalMediaDisplaySetting = GlobalMediaDisplaySettingModel.GetGlobalMediaDisplaySetting();
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            return globalMediaDisplaySetting;
        }

        //Get media count.
        public virtual int GetMediaCount()
        {
            return _mediaRepository.Table.Count();
        }

        //Get media list data for generate images.
        public virtual MediaManagerListModel GetMediaListData(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            //Set sorts to media id asc if sorts is null.
            if (sorts.Count <= 0)
                sorts.Add("asc", "mediaid");

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            IZnodeViewRepository<MediaManagerModel> objStoredProc = new ZnodeViewRepository<MediaManagerModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);

            IList<MediaManagerModel> mediaManagerList = objStoredProc.ExecuteStoredProcedureList("Znode_GetAllMedia @WhereClause,@Order_By,@Rows,@PageNo,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("mediaManagerList list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaManagerList?.Count());
            //Bind media list.
            MediaManagerListModel mediaManagerListModel = new MediaManagerListModel();
            mediaManagerListModel.MediaList = mediaManagerList?.Count > 0 ? mediaManagerList?.ToList() : null;            
            mediaManagerListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return mediaManagerListModel;
        }
        #endregion

        #endregion

        #region Private Methods

        #region Sync Media 
        //Insert Create new media path 
        protected virtual int InsertMediaPath(string rootFolder)
        {
            ZnodeLogging.LogMessage("Input Parameter rootFolder:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { rootFolder });
            ZnodeMediaPath mediaPath = new ZnodeMediaPath();
            mediaPath.ParentMediaPathId = _mediaPathLocaleRepository.Table.Where(x => x.PathName == rootFolder).Select(x => x.MediaPathId).SingleOrDefault();
            mediaPath.PathCode = MediaManager_Resources.MediaPathCode;
            ZnodeLogging.LogMessage("ParentMediaPathId and PathCode:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { mediaPath.ParentMediaPathId, mediaPath.PathCode });
            mediaPath = _mediaPathRepository.Insert(mediaPath);
            return mediaPath.MediaPathId;
        }

        //To save media path local 
        protected virtual int? InsertMediaPathLocal(int MediaPathId, string folderName)
        {
            ZnodeLogging.LogMessage("Input parameter MediaPathId and folderName:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { MediaPathId, folderName });
            string folder = HelperUtility.IsNull(folderName) ? MediaManager_Resources.FolderName : folderName;
            ZnodeLogging.LogMessage("Folder:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { folder });
            bool isPresent = _mediaPathLocaleRepository.Table.Where(x => x.PathName == folder).Any();
            ZnodeMediaPathLocale mediaPathLocale = isPresent ? _mediaPathLocaleRepository.Table.FirstOrDefault(x => x.PathName == folder) : new ZnodeMediaPathLocale();
            mediaPathLocale.PathName = folder;
            mediaPathLocale.MediaPathId = !isPresent ? MediaPathId : _mediaPathLocaleRepository.Table.Where(x => x.PathName == folder)?.Select(x => x.MediaPathId).FirstOrDefault();
            mediaPathLocale.LocaleId = _localeRepository.Table.Where(x => x.IsDefault).Select(x => x.LocaleId).FirstOrDefault();

            if (!isPresent)
                mediaPathLocale = _mediaPathLocaleRepository.Insert(mediaPathLocale);
            return mediaPathLocale.MediaPathId;
        }

        //To Insert znode media category images
        protected virtual void InsertMediaCategoryList(int? MediaPathId, IEnumerable<ZnodeMedia> mediaListModel)
        {
            ZnodeLogging.LogMessage("Input parameter MediaPathId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, MediaPathId);
            List<ZnodeMediaCategory> mediaListCategory = new List<ZnodeMediaCategory>();
            foreach (ZnodeMedia list in mediaListModel)
            {
                ZnodeMediaCategory mediaCategory = new ZnodeMediaCategory();
                mediaCategory.MediaPathId = MediaPathId;
                mediaCategory.MediaId = list.MediaId;
                mediaListCategory.Add(mediaCategory);
            }
            ZnodeLogging.LogMessage("mediaListCategory list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaListCategory?.Count());
            _mediaCategoryRepository.Insert(mediaListCategory);
        }

        //For insert media into znodeMedia 
        protected virtual IEnumerable<ZnodeMedia> InsertMediaList(Dictionary<string, long> listMedia, int mediaConfigurationId)
        {
            ZnodeLogging.LogMessage("Input Parameter mediaConfigurationId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { mediaConfigurationId });
            List<ZnodeMedia> mediaList = new List<ZnodeMedia>();
            foreach (var list in listMedia)
            {
                string imageName = list.Key;
                if (list.Key?.Split('/').Length > 1)
                {
                    int length = list.Key.Split('/').Length - 1;
                    imageName = list.Key?.Split('/')[length];
                }
                if (!_mediaRepository.Table.Where(x => x.FileName == imageName || x.Path == imageName).Any())
                {
                    ZnodeMedia mediaModel = new ZnodeMedia()
                    {
                        FileName = imageName,
                        Path = imageName,
                        Size = list.Value.ToString(),
                        Type = imageName?.Split('.').Length >= 1 ? string.Empty : imageName?.Split('.')?[1],
                        MediaConfigurationId = mediaConfigurationId,
                        Length = imageName?.Split('.').Length >= 1 ? string.Empty : imageName?.Split('.')?[1],
                    };
                    mediaList.Add(mediaModel);
                }
            }
            ZnodeLogging.LogMessage("mediaList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, mediaList?.Count());
            return _mediaRepository.Insert(mediaList);
        }

        #endregion
        //Gets the list of expands.
        protected virtual List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (HelperUtility.IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    var value = expands.Get(key);
                    if (Equals(value, ExpandKeys.MediaServer)) { SetExpands(ZnodeMediaConfigurationEnum.ZnodeMediaServerMaster.ToString(), navigationProperties); }
                    if (string.Equals(value, ZnodeMediaServerMasterEnum.ZnodeMediaConfigurations.ToString(), StringComparison.OrdinalIgnoreCase)) { SetExpands(ZnodeMediaServerMasterEnum.ZnodeMediaConfigurations.ToString(), navigationProperties); }
                }
            }
            return navigationProperties;
        }

        //Set the is active field to false and update it in database too.
        protected virtual void SetIsActiveFalse()
        {
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(IsActiveFilter().ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause to generate ZnodeMediaConfiguration list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, whereClause);
            List<ZnodeMediaConfiguration> mediaConfigurationList = _mediaConfigurationRepository.GetEntityList(whereClause.WhereClause, whereClause.FilterValues).ToList();
            ZnodeLogging.LogMessage("mediaConfigurationList list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaConfigurationList?.Count());
            mediaConfigurationList.ForEach(x => x.IsActive = false);
            mediaConfigurationList.ForEach(x => _mediaConfigurationRepository.Update(x));
        }

        //Get the base url from server 
        protected virtual string GetUrlFromServer(MediaConfigurationModel model)
        {
            IConnector connector;
            if (string.Equals(model.Server, ZnodeConstant.Azure))
                connector = new AzureAgent(new FileUploadPolicyModel(model.AccessKey, model.SecretKey, model.BucketName, model.ThumbnailFolderName, model.URL, model.NetworkUrl));
            else if (string.Equals(model.Server, ZnodeConstant.AmazonS3))
                connector = new AWSS3Agent(new FileUploadPolicyModel(model.AccessKey, model.SecretKey, model.BucketName, model.ThumbnailFolderName, model.URL, model.NetworkUrl));
            else if (string.Equals(model.Server, ZnodeConstant.NetworkDrive))
                connector = new NetworkDriveAgent(new FileUploadPolicyModel(model.AccessKey, model.SecretKey, model.BucketName, model.ThumbnailFolderName, model.URL, model.NetworkUrl));
            else
                connector = new LocalAgent(new FileUploadPolicyModel(model.AccessKey, model.SecretKey, model.BucketName, model.ThumbnailFolderName, model.URL, model.NetworkUrl));

            string serverUrl = connector.GetServerUrl();
            ZnodeLogging.LogMessage("serverUrl: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { serverUrl });
            return (!string.IsNullOrEmpty(serverUrl) && !serverUrl.EndsWith("/")) ? $"{serverUrl}/" : serverUrl;
        }

        //Create filters for IsActive.
        private static FilterCollection IsActiveFilter()
        {
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(new FilterTuple(ZnodeMediaConfigurationEnum.IsActive.ToString(), ProcedureFilterOperators.Equals, ZnodeConstant.TrueValue));
            return filtersList;
        }

        //Get expands for media server.
        private static NameValueCollection ExpandsMediaServer()
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ExpandKeys.MediaServer, ExpandKeys.MediaServer);
            return expands;
        }

        //Get Data from cache if cache null then insert into Cache.
        protected virtual MediaConfigurationModel InsertAndGetDefaultMediaConfiguration()
        {
            MediaConfigurationModel activeMediaConfiguration = _mediaConfigurationRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(IsActiveFilter().ToFilterDataCollection()).WhereClause, GetExpands(ExpandsMediaServer()))?.ToModel<MediaConfigurationModel>();
            if (HelperUtility.IsNotNull(activeMediaConfiguration))
            {
                activeMediaConfiguration.GlobalMediaDisplaySetting = GetGlobalMediaDisplaySetting(activeMediaConfiguration);
                ZnodeCacheDependencyManager.Insert("DefaultMediaConfigurationCache", activeMediaConfiguration, "ZnodeMediaConfiguration");
            }
            else
            {
                activeMediaConfiguration = new MediaConfigurationModel { MediaServer = new MediaServerModel(), GlobalMediaDisplaySetting = GetGlobalMediaDisplaySetting(activeMediaConfiguration) };
                ZnodeLogging.LogMessage("No active media configuration available.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                ZnodeCacheDependencyManager.Insert("DefaultMediaConfigurationCache", activeMediaConfiguration, "ZnodeMediaConfiguration");
            }

            return activeMediaConfiguration;
        }        

        //Method to update global media display setting.
        protected virtual bool UpdateGlobalMediaDisplaySetting(GlobalMediaDisplaySettingModel displaySettingModel)
        {
            bool isUpdateDisplaySetting = false;
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            if (IsNotNull(displaySettingModel))
            {
                displaySettingModel.MediaId = displaySettingModel.MediaId > 0 ? displaySettingModel.MediaId : null;
                isUpdateDisplaySetting = _globalMediaDisplaySettingRepository.Update(displaySettingModel.ToEntity<ZnodeGlobalMediaDisplaySetting>());

                ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            }
            return isUpdateDisplaySetting;
        }
        #endregion
    }
}
