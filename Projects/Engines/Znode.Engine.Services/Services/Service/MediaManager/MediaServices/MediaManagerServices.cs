using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class MediaManagerServices : BaseService, IMediaManagerServices
    {
        #region Private Data Members
        private readonly IZnodeRepository<ZnodeMedia> _mediaRepository;
        private readonly IZnodeRepository<ZnodeMediaCategory> _mediaCategoryRepository;
        private readonly IZnodeRepository<ZnodeMediaPath> _mediaPathRepository;
        private readonly IZnodeRepository<ZnodeMediaPathLocale> _mediaPathLocaleRepository;
        private readonly IZnodeViewRepository<View_GetAttributeFamilyByName> _mediaAttributeFamilyByName;
        private readonly IZnodeRepository<ZnodeMediaAttributeValue> _mediaAttributeValue;
        private readonly IZnodeRepository<ZnodeMediaFolderUser> _mediaFolderUserRepository;
        #endregion

        #region Public Constructor.
        public MediaManagerServices()
        {
            _mediaRepository = new ZnodeRepository<ZnodeMedia>();
            _mediaPathRepository = new ZnodeRepository<ZnodeMediaPath>();
            _mediaPathLocaleRepository = new ZnodeRepository<ZnodeMediaPathLocale>();
            _mediaCategoryRepository = new ZnodeRepository<ZnodeMediaCategory>();
            _mediaAttributeValue = new ZnodeRepository<ZnodeMediaAttributeValue>();
            _mediaAttributeFamilyByName = new ZnodeViewRepository<View_GetAttributeFamilyByName>();
            _mediaFolderUserRepository = new ZnodeRepository<ZnodeMediaFolderUser>();
        }
        #endregion

        #region Public Methods

        //Save media.
        public virtual MediaManagerModel SaveMedia(MediaManagerModel mediaManagerModel)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter MediaManagerModel having FileName:",ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { mediaManagerModel.FileName });
            if (HelperUtility.IsNull(mediaManagerModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaManagerModelNull);

            //Insert media.  
            ZnodeMedia media = _mediaRepository.Insert(MediaManagerMap.ToEntity(mediaManagerModel));
            ZnodeLogging.LogMessage((media?.MediaId > 0) ? string.Format(Admin_Resources.SuccessSaveMedia, mediaManagerModel.FileName) : string.Format(Admin_Resources.ErrorSaveMedia, mediaManagerModel.FileName), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(media))
                return media.ToModel<MediaManagerModel>();
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return mediaManagerModel;
        }

        //Get tree node.
        public virtual MediaManagerTreeModel GetTreeNode()
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            IZnodeViewRepository<View_MediaFolderUserShare> objStoredProc = new ZnodeViewRepository<View_MediaFolderUserShare>();
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), -1, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), 1, ParameterDirection.Input, DbType.Int32);
            //Get all path from database.
            var list = objStoredProc.ExecuteStoredProcedureList("Znode_MediaFolderUserShare @LocaleId,@UserId")?.Select(x => new MediaManagerTreeModel
            {
                Text = x.PathName,
                Id = x.MediaPathId,
                ParentId = x.ParentMediaPathId.GetValueOrDefault()
            }).ToList();
            ZnodeLogging.LogMessage("GetTreeNode list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
            //Convert path to parent child pattern.
            return GetAllNode(ref list)?.FirstOrDefault();
        }

        //Get all medias.
        public virtual MediaManagerListModel GetMedias(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            //Set sorts to media id asc if sorts is null.
            if (sorts.Count <= 0)
                sorts.Add("asc", "mediaid");

          GetFiltersForMediaWidget(filters);
           
            int mediaPathId = GetMediaPathId(filters);
            ZnodeLogging.LogMessage("mediaPathId: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, mediaPathId);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate mediaManagerList list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<MediaManagerModel> objStoredProc = new ZnodeViewRepository<MediaManagerModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@MediaPathId", mediaPathId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);

            IList<MediaManagerModel> mediaManagerList = objStoredProc.ExecuteStoredProcedureList("Znode_GetMediaFolderDetails @WhereClause,@MediaPathId,@Rows,@PageNo,@Order_By,@RowCount OUT", 5, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("mediaManagerList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, mediaManagerList?.Count());
            //Bind media list.
            MediaManagerListModel mediaManagerListModel = new MediaManagerListModel();
            mediaManagerListModel.MediaList = mediaManagerList?.Count > 0 ? mediaManagerList?.ToList() : null;

            //Add version number in query string with image path to specify that it has been updated.
            mediaManagerListModel.MediaList?.Select(s => { s.MediaServerPath = AppendMediaVersion(s); return s; })?.ToList();

            mediaManagerListModel.BindPageListModel(pageListModel);
            return mediaManagerListModel;
        }

        protected virtual string AppendMediaVersion(MediaManagerModel s)
        {
            return $"{s.MediaServerPath}?v={s.Version}";
        }

        //Add media.
        public virtual bool AddMedia(MediaManagerFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaManagerFolderModelNull);

            //Insert Media.
            ZnodeMediaPath entity = _mediaPathRepository.Insert(MediaManagerMap.ToModel(model));
            ZnodeLogging.LogMessage((entity?.MediaPathId > 0) ? string.Format(Admin_Resources.SuccessAddMedia, model.FolderName) : string.Format(Admin_Resources.ErrorAddMedia, model.FolderName), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (entity?.MediaPathId > 0)
            {
                ZnodeLogging.LogMessage("MediaPathId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, entity?.MediaPathId);
                model.Id = entity.MediaPathId;
                return true;
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return false;
        }

        //Update media.
        public virtual MediaManagerModel Update(MediaManagerModel mediaManagerModel)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(mediaManagerModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaModelNull);

            //Update media logic.
            UpdateMedia(mediaManagerModel);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return mediaManagerModel;
        }

        //Rename Media.
        public virtual bool RenameMedia(MediaManagerFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Input Parameter MediaManagerFolderModel having FolderName:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { model.FolderName});
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaManagerFolderModelNull);

            //Get media path locale.
            ZnodeMediaPathLocale pathLocale = _mediaPathLocaleRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(MediaPathIdFilter(model).ToFilterDataCollection()).WhereClause);
          
            bool isMediaRenamed;

            if (Equals(pathLocale?.PathName, model.FolderName))
                isMediaRenamed = true;
            else
            {
                if(HelperUtility.IsNotNull(pathLocale))
                    pathLocale.PathName = model.FolderName;
                isMediaRenamed = _mediaPathLocaleRepository.Update(pathLocale);
            }

            ZnodeLogging.LogMessage((isMediaRenamed) ? string.Format(Admin_Resources.SuccessRenameFolder, model.FolderName) : string.Format(Admin_Resources.ErrorRenameFolder, model.FolderName), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return isMediaRenamed;
        }

        //Get media by media id.
        public virtual MediaManagerModel GetMediaByID(int mediaId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter mediaId: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, mediaId);
            if (mediaId <= 0)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.InvalidMedia);

            ZnodeMedia media = _mediaRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForMediaIds(Convert.ToString(mediaId))).WhereClause, GetExpands(expands));

            //Map ZnodeAccount Entity to Account Model.
            MediaManagerModel model = media?.ToModel<MediaManagerModel>();

            if (HelperUtility.IsNotNull(model))
            {
                MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
                string serverPath = GetMediaServerUrl(configurationModel);
                model.MediaServerPath = $"{serverPath}{model.Path}?v={model.Version}";
                model.MediaServerThumbnailPath = $"{serverPath}{configurationModel.ThumbnailFolderName}/{model.Path}?v={model.Version}";
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Move Folder.
        public virtual bool MoveFolder(MediaManagerMoveFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaManagerMoveFolderModelNull);

            ZnodeMediaPath znodeMediaPath = _mediaPathRepository.GetById(model.FolderId);

            if (znodeMediaPath?.CreatedBy != GetLoginUserId())
                throw new ZnodeException(ErrorCodes.NotPermitted, string.Format(Admin_Resources.ErrorPermissionToMoveFolder,model.FolderId));

            //Assign value to parent media path id.
            if(HelperUtility.IsNotNull(znodeMediaPath))
                znodeMediaPath.ParentMediaPathId = model.ParentId;
            bool result = _mediaPathRepository.Update(znodeMediaPath);
            ZnodeLogging.LogMessage(result ? string.Format(Admin_Resources.SuccessMoveFolder, model.FolderId) : string.Format(Admin_Resources.ErrorMoveFolder, model.FolderId), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return result;
        }

        //Move media to folder.
        public virtual bool MoveMediaToFolder(AddMediaToFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorAddMediaToFolderModelNull);

            string whereClauseForMedia = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForMediaIds(model.MediaIds)).WhereClause;
            ZnodeLogging.LogMessage("whereClauseForMedia to generate currency list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseForMedia);
            //Media to move.
            List<ZnodeMediaCategory> medialist = _mediaCategoryRepository.GetEntityList(whereClauseForMedia, new List<string>() { "ZnodeMedia" }).ToList();
            ZnodeLogging.LogMessage("medialist list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, medialist?.Count());
            if (HelperUtility.IsNull(medialist) || medialist.FindAll(m => m.MediaPathId == model.FolderId)?.Count > 0)
                return false;

            bool result = true;
            string whereClauseForMediaPathId = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForMediaPathId(model.FolderId)).WhereClause;
            ZnodeLogging.LogMessage("whereClauseForMediaPathId generated for mediaListInFolder:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, whereClauseForMediaPathId);
            //All media within folder.
            IList<ZnodeMediaCategory> mediaListInFolder = _mediaCategoryRepository.GetEntityList(whereClauseForMediaPathId, new List<string>() { "ZnodeMedia" });
            ZnodeLogging.LogMessage("mediaListInFolder list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaListInFolder?.Count());
            List<string> mediaIdsToRemove = new List<string>();
            foreach (ZnodeMediaCategory media in medialist)
            {
                if (HelperUtility.IsNotNull(mediaListInFolder.FirstOrDefault(x => x.ZnodeMedia.FileName == media.ZnodeMedia.FileName)))
                    mediaIdsToRemove.Add(Convert.ToString(media.MediaId));
            }
            ZnodeLogging.LogMessage("mediaIdsToRemove list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaIdsToRemove?.Count());
            if (mediaIdsToRemove.Count > 0)
            {
                result = false;
                List<string> allMediaIds = model.MediaIds.Split(',').ToList<string>();
                if (allMediaIds.Count == 1 && mediaIdsToRemove.Count == 1)
                    return false;
                else
                    whereClauseForMedia = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForMediaIds(string.Join(",", allMediaIds.Except(mediaIdsToRemove).ToList()))).WhereClause;
                medialist = _mediaCategoryRepository.GetEntityList(whereClauseForMedia).ToList();
            }
            string whereClauseForCategory = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForMediaCategoryIds(string.Join(",", medialist.Select(x => x.MediaCategoryId).ToList()))).WhereClause;
            ZnodeLogging.LogMessage("whereClauseForCategory generated:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, whereClauseForCategory);

            if (!string.IsNullOrEmpty(whereClauseForCategory))
                _mediaAttributeValue.Delete(whereClauseForCategory);

            medialist.ForEach(x => x.MediaPathId = model.FolderId);
            medialist.ForEach(x => _mediaCategoryRepository.Update(x));
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessMoveMediaToFolder, model.MediaIds, model.FolderId), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return result;
        }

        //Get media  attribute values.
        public virtual MediaAttributeValuesListModel GetMediaAttributeValues(int mediaId, NameValueCollection expands)
        {
            IZnodeViewRepository<View_MediaAttributeValues> _mediaAttributeValuesStoredProc= new ZnodeViewRepository<View_MediaAttributeValues>();
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("input parameter mediaId: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, mediaId);
            if (mediaId <= 0)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.InvalidMedia);
            
            //Execute SP to get media attribute values.
            _mediaAttributeValuesStoredProc.SetParameter("@MediaID", mediaId, ParameterDirection.Input, DbType.Int32);

            var mediaAttributeValues = _mediaAttributeValuesStoredProc.ExecuteStoredProcedureList("ZnodeGetMediaAttributeValues @MediaID");
            ZnodeLogging.LogMessage("mediaAttributeValues: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, mediaAttributeValues);
            MediaAttributeValuesListModel model = MediaAttributeValuesMap.ToListModel(mediaAttributeValues);
            //Returns model with media attribute values.
            model.Media = GetMediaByID(mediaId, expands);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Updates Attribute value for a media in media attributevalue repository.
        public virtual bool UpdateMediaAttributeValue(MediaAttributeValuesListModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaAttributeValuesListModelNull);

            if (model.Media?.MediaId > 0)
            {
                ZnodeMedia _media = _mediaRepository.GetById(model.Media.MediaId);
                _media.FileName = model.Media.FileName;
                _mediaRepository.Update(_media);
            }
            if (model.MediateAttributeValues?.Count > 0)
            {
                foreach (MediaAttributeValuesModel mediaAttributeValue in model.MediateAttributeValues)
                {
                    if (HelperUtility.IsNull(mediaAttributeValue))
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMediaAttributeValueModelNull);

                    if (HelperUtility.IsNotNull(mediaAttributeValue.AttributeCode))
                    {
                        //Check server side validation.
                        IServerValidationService objServerValidation = ZnodeDependencyResolver.GetService<IServerValidationService>();
                        bool isServerValid = objServerValidation.CheckServerValidation(mediaAttributeValue.AttributeCode.ToString(), mediaAttributeValue.MediaAttributeValue);

                        if (isServerValid)
                        {
                            if (mediaAttributeValue.MediaAttributeValueId > 1)
                            {
                                //Updates media attribute value.
                                var mediaAttributeValues = _mediaAttributeValue.GetById(mediaAttributeValue.MediaAttributeValueId.Value);
                                if (HelperUtility.IsNotNull(mediaAttributeValues))
                                {
                                    mediaAttributeValues.AttributeValue = mediaAttributeValue.MediaAttributeValue;
                                    if (_mediaAttributeValue.Update(mediaAttributeValues))
                                        ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessUpdateMediaAttributeValue, mediaAttributeValue.MediaAttributeValueId), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                                }
                                else
                                {
                                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.DataNotFoundForMediaAttributeValue, mediaAttributeValue.MediaAttributeValueId), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                                    throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.DataNotFound);
                                }
                            }
                            else
                            {
                                //Adds new entry for media attribute value.
                                ZnodeMediaAttributeValue mediaAttributeValues = _mediaAttributeValue.Insert(mediaAttributeValue.ToEntity<ZnodeMediaAttributeValue>());
                                if (mediaAttributeValues.MediaAttributeValueId < 1)
                                    return false;
                            }
                        }
                        else
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        //Creates a media attribute value.
        public virtual MediaAttributeValuesModel CreateMediaAttribute(MediaAttributeValuesModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorMediaAttributeValueModelNull);

            //Insert media attribute value.
            ZnodeMediaAttributeValue znodeMediaAttributeValue = _mediaAttributeValue.Insert(model.ToEntity<ZnodeMediaAttributeValue>());
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessCreateMediaAttribute, model.AttributeName), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return znodeMediaAttributeValue.ToModel<MediaAttributeValuesModel>();
        }

        //Delete Exiting Media File.
        public virtual string DeleteMedia(DeleteMediaModel deleteMediaModel)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter DeleteMediaModel having FolderId and MediaIds :", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { deleteMediaModel.FolderId, deleteMediaModel.MediaIds });
            if (HelperUtility.IsNull(deleteMediaModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorDeleteMediaModelNull);

            //It will permanently delete specified folder.
            if (deleteMediaModel.FolderId > 0)
                return DeleteFolder(deleteMediaModel.FolderId);

            if (string.IsNullOrEmpty(deleteMediaModel.MediaIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMediaIdsEmpty);

            IZnodeViewRepository<View_ReturnBooleanWithMessage> objStoredProc = new ZnodeViewRepository<View_ReturnBooleanWithMessage>();

            objStoredProc.SetParameter(ZnodeMediaEnum.MediaId.ToString(), deleteMediaModel.MediaIds, ParameterDirection.Input, DbType.String);

            List<View_ReturnBooleanWithMessage> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteMedia @MediaId").ToList();
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessDeleteMedia, deleteMediaModel.MediaIds), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            //return paths of media which is deleted.
            return string.Join(",", deleteResult.Select(x => x.MessageDetails));
        }

        //Get Attribute Family Id by Name.
        public virtual MediaAttributeFamily GetAttributeFamilyIdByName(string extension)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("extension: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, extension);
            _mediaAttributeFamilyByName.SetParameter("@AttributeValidationValue", extension, ParameterDirection.Input, DbType.String);
            return _mediaAttributeFamilyByName.ExecuteStoredProcedureList("Znode_GetAttributeFamilyByName @AttributeValidationValue")?.FirstOrDefault()?.ToModel<MediaAttributeFamily>();
        }

        //Share folder.
        public virtual bool ShareFolder(List<ShareMediaFolderModel> shareMediaFolderModel)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(shareMediaFolderModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (shareMediaFolderModel?.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorShareMediaFolderModelCountLessThanOne);

            //condition to check if media folder is going to shared with current logged in user.
            if (shareMediaFolderModel.Any(x => x.AccountId == GetLoginUserId()))
                throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ModelNotNull);

            FilterCollection filterList = new FilterCollection();

            FilterTuple filterTuple = new FilterTuple(ZnodeMediaCategoryEnum.MediaPathId.ToString(), ProcedureFilterOperators.Equals, shareMediaFolderModel.FirstOrDefault().ShareMediaFolderId.ToString());
            FilterTuple accountFilter = new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.In, string.Join(",", shareMediaFolderModel.Select(x => x.AccountId).ToList()));
            filterList.Add(filterTuple);
            filterList.Add(accountFilter);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());

            //condition to check if media folder is already shared with users.
            if (_mediaFolderUserRepository.GetEntityList(whereClauseModel.WhereClause).Count > 0)
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorMediaFolderShared);

            var sharedFolders = _mediaFolderUserRepository.Insert(MediaManagerMap.ToShareMediaFolderListEntity(shareMediaFolderModel));
            ZnodeLogging.LogMessage("sharedFolders: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, sharedFolders);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return sharedFolders?.Count() > 0;
        }

        public virtual FamilyExtensionListModel GetAllowedExtensions()
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            FamilyExtensionListModel model = new FamilyExtensionListModel() { FamilyExtensions = new List<FamilyExtensionModel>() };
            IZnodeViewRepository<View_FamilyExtensions> objStoredProc = new ZnodeViewRepository<View_FamilyExtensions>();
            IList<View_FamilyExtensions> extensions = objStoredProc.ExecuteStoredProcedureList("Znode_GetFamilyExtensions");
            ZnodeLogging.LogMessage("extensions list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, extensions?.Count());
            if (extensions?.Count > 0)
                model.FamilyExtensions = extensions.Select(t => new FamilyExtensionModel() { Extension = t.Extension, MaxFileSize = t.MaxFileSize, ValidationName = t.ValidationName.ToLower(), MediaAttributeFamilyId = t.MediaAttributeFamilyId, FamilyCode = t.FamilyCode }).ToList();
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Find if file already present or not.
        public virtual int FileAlreadyExist(string fileName, int folderId) =>
                (from media in _mediaRepository.GetEntityList(string.Empty).ToList()
                 join mediaCategory in _mediaCategoryRepository.GetEntityList(string.Empty).ToList() on media.MediaId equals mediaCategory.MediaId
                 where string.Equals(media.FileName, fileName, StringComparison.OrdinalIgnoreCase) && mediaCategory.MediaPathId == folderId
                 select media.MediaId).FirstOrDefault();

        //Get list of store associated to mediapath
        public List<int> GetAssociatedStoreOfMedia(string mediaPaths)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("mediaPath:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaPaths);
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@MediaPath", mediaPaths, ParameterDirection.Input, SqlDbType.VarChar);
            List<int> storeList = null;
            var dataSet = executeSpHelper.GetSPResultInDataSet("Znode_GetMediaAssociatedStoreList");
            if (dataSet?.Tables?.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];
                storeList = (from DataRow dr in dataTable?.Rows
                             select Convert.ToInt32(dr["PortalId"]))?.ToList();
            }
            ZnodeLogging.LogMessage("sharedFolders: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, storeList?.Count);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return storeList;
        }

        //Generate specific image on edit.
        public virtual bool GenerateImageOnEdit(string mediaPath)
        {           
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
       
            MediaConfigurationModel mediaConfiguration = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            IImageMediaHelper imageHelper = GetService<IImageMediaHelper>(new ZnodeNamedParameter("mediaConfiguration", mediaConfiguration));

            imageHelper.GenerateImageOnEdit(mediaPath);

            ZnodeLogging.LogMessage("Execution end: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return true;
        }

        //Get Media Details by Id to download the image via extension.
        public virtual MediaDetailModel GetMediaDetailsById(int mediaId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter mediaId: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, mediaId);
            MediaDetailModel model = null;
            if (mediaId > 0)
                model = _mediaRepository.GetById(mediaId).ToModel<MediaDetailModel>();

            SetMediaDetails(model);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Get Media Details by Guid  to download the image via extension.
        public virtual MediaDetailModel GetMediaDetailsByGuid(string mediaGuid)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter mediaId: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, mediaGuid);
            MediaDetailModel model = null;
            if (IsNotNull(mediaGuid))
            {
                mediaGuid = DecodeBase64(mediaGuid);
                model = _mediaRepository.Table.FirstOrDefault(x => x.Path.ToLower() == mediaGuid.ToLower()).ToModel<MediaDetailModel>();
            }
            SetMediaDetails(model);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //using this to SetMediaDetails 
        protected virtual void SetMediaDetails(MediaDetailModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
                string serverPath = GetMediaServerUrl(configurationModel);
                model.MediaServerPath = $"{serverPath}{model.Path}?v={model.Version}";
            }
        }
        #endregion

        #region Private Methods
        //Get Media Path Id.
        private int GetMediaPathId(FilterCollection filters)
        {
            FilterTuple parentMediaPathIDTuple = filters.Find(x => x.Item1.ToLower() == ZnodeMediaPathEnum.MediaPathId.ToString().ToLower());
            if (HelperUtility.IsNotNull(parentMediaPathIDTuple))
            {
                filters.Remove(parentMediaPathIDTuple);
                return Convert.ToInt32(parentMediaPathIDTuple.Item3);
            }
            return -1;
        }

        //Get Filter for Media
        private void GetFiltersForMediaWidget(FilterCollection filters)
        {
            if (filters.Any(x => x.FilterName.Equals(FilterKeys.MediaType, StringComparison.InvariantCultureIgnoreCase)))
            {
                string mediaTypeValue = filters.Find(x => x.FilterName.Equals(FilterKeys.MediaType, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;
                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.MediaType, StringComparison.InvariantCultureIgnoreCase));
                mediaTypeValue = mediaTypeValue?.Replace("^", ",");
                filters.Add(new FilterTuple(FilterKeys.MediaType, FilterOperators.In, mediaTypeValue));
            }
        }

        //Get all nodes of a tree.
        private List<MediaManagerTreeModel> GetAllNode(ref List<MediaManagerTreeModel> mediaPath)
        {
            if (HelperUtility.IsNotNull(mediaPath))
            {
                foreach (var item in mediaPath)
                {
                    //find all chid folder and add to list
                    List<MediaManagerTreeModel> child = mediaPath.Where(x => x.ParentId == item.Id).ToList();
                    item.Children = new List<MediaManagerTreeModel>();
                    item.Children.AddRange(GetAllNode(ref child));
                }
                return mediaPath;
            }
            return new List<MediaManagerTreeModel>();
        }

        //Get expands.
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (HelperUtility.IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                    if (Equals(key, ZnodeMediaEnum.ZnodeMediaCategories.ToString().ToLower())) SetExpands(ZnodeMediaEnum.ZnodeMediaCategories.ToString(), navigationProperties);
            }
            return navigationProperties;
        }

        //Get WhereClause For MediaIds.
        private FilterDataCollection GetWhereClauseForMediaIds(string mediaIds)
        {
            ZnodeLogging.LogMessage("mediaIds:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaIds);
            FilterCollection filterList = new FilterCollection();
            FilterTuple filterTuple = new FilterTuple(ZnodeMediaEnum.MediaId.ToString(), ProcedureFilterOperators.In, mediaIds);
            filterList.Add(filterTuple);
            return filterList.ToFilterDataCollection();
        }

        //Get WhereClause For Media Path Id.
        private FilterDataCollection GetWhereClauseForMediaPathId(int folderId)
        {
            ZnodeLogging.LogMessage("folderId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, folderId);
            FilterCollection filterList = new FilterCollection();
            FilterTuple filterTuple = new FilterTuple(ZnodeMediaCategoryEnum.MediaPathId.ToString(), ProcedureFilterOperators.Equals, folderId.ToString());
            filterList.Add(filterTuple);
            return filterList.ToFilterDataCollection();
        }

        //Get WhereClause For MediaCategoryIds.
        private FilterDataCollection GetWhereClauseForMediaCategoryIds(string mediaCategoryIds)
        {
            ZnodeLogging.LogMessage("mediaCategoryIds:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaCategoryIds);
            FilterCollection filterList = new FilterCollection();

            FilterTuple filterCategoryTuple = new FilterTuple(ZnodeMediaCategoryEnum.MediaCategoryId.ToString(), ProcedureFilterOperators.In, mediaCategoryIds);
            filterList.Add(filterCategoryTuple);
            return filterList.ToFilterDataCollection();
        }

        //Delete folder.
        private string DeleteFolder(int folderId)
        {
            ZnodeLogging.LogMessage("folderId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, folderId);
            if (folderId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.InvalidFolder);


            IZnodeViewRepository<View_ZnodeDeleteMediaFolder> objStoredProc = new ZnodeViewRepository<View_ZnodeDeleteMediaFolder>();
            objStoredProc.SetParameter("MediaPathId", folderId, ParameterDirection.Input, DbType.Int32);
            return string.Join(",", objStoredProc.ExecuteStoredProcedureList("Znode_DeleteMediaFolder @MediaPathId")?.Select(x => x.Path).ToList());
        }

        //Filter for Media Path Id.
        private static FilterCollection MediaPathIdFilter(MediaManagerFolderModel model)
        {
            FilterCollection filterList = new FilterCollection();
            FilterTuple filterTuple = new FilterTuple(ZnodeMediaCategoryEnum.MediaPathId.ToString(), ProcedureFilterOperators.Equals, model.Id.ToString());
            filterList.Add(filterTuple);
            return filterList;
        }

        //Update media.
        private void UpdateMedia(MediaManagerModel mediaManagerModel)
        {
            FilterCollection filterList = new FilterCollection();
            FilterTuple fileNameTuple = new FilterTuple(ZnodeMediaEnum.FileName.ToString(), ProcedureFilterOperators.Contains, mediaManagerModel.FileName);
            filterList.Add(fileNameTuple);

            //Gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to generate mediaEntityList list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, whereClauseModel);
            IList<ZnodeMedia> mediaEntityList = _mediaRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
            ZnodeLogging.LogMessage("mediaEntityList list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaEntityList?.Count());
            if (mediaEntityList?.Count > 0)
            {
                filterList = new FilterCollection();
                FilterTuple folderIdTuple = new FilterTuple(ZnodeMediaCategoryEnum.MediaPathId.ToString(), ProcedureFilterOperators.Equals, mediaManagerModel.MediaPathId.ToString());
                filterList.Add(folderIdTuple);
                FilterTuple mediaIdsTuple = new FilterTuple(ZnodeMediaEnum.MediaId.ToString(), ProcedureFilterOperators.In, string.Join(",", mediaEntityList.Select(x => x.MediaId)));
                filterList.Add(mediaIdsTuple);
                whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());

                //Gets the media id to update from media category table.
                int mediaIdToUpdate = Convert.ToInt32(_mediaCategoryRepository.GetEntity(whereClauseModel.WhereClause)?.MediaId);
                ZnodeLogging.LogMessage("mediaIdToUpdate:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaIdToUpdate);
                if (mediaIdToUpdate <= 0)
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMediaIdExists);

                ZnodeMedia mediaEntityData = mediaEntityList.First(x => x.MediaId == mediaIdToUpdate);

                mediaManagerModel.MediaId = mediaIdToUpdate;
                mediaManagerModel.OldMediaPath = mediaEntityData?.Path;

                //Increment version number of recently updated image.
                mediaManagerModel.Version = Convert.ToInt32(mediaEntityData?.Version) + 1;
                _mediaRepository.Update(MediaManagerMap.SetMediaEntity(mediaManagerModel, mediaEntityData));

                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessUpdateMedia, mediaManagerModel.MediaId), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            }
        }
        #endregion
    }
}
