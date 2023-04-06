using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.MediaStorage;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class MediaManagerAgent : BaseAgent, IMediaManagerAgent
    {
        #region private data member 
        private readonly IMediaManagerClient _mediaManagerClient;
        private readonly IMediaConfigurationClient _mediaConfigurationClient;
        #endregion

        #region Public Constructor
        public MediaManagerAgent(IMediaManagerClient mediaManagerClient, IMediaConfigurationClient mediaConfigurationClient)
        {
            _mediaManagerClient = GetClient<IMediaManagerClient>(mediaManagerClient);
            _mediaConfigurationClient = GetClient<IMediaConfigurationClient>(mediaConfigurationClient);
        }
        #endregion

        #region Public methods

        //Get application header
        public AjaxHeadersModel GetAppHeader()
        {
            var _header = GetClient<MediaManagerClient>();
            return new AjaxHeadersModel { Authorization = _header.GetAuthorizationHeader(_header.DomainName, _header.DomainKey), DomainName = _header.DomainName, Token = _header.Token };
        }

        //Get medias.
        public virtual MediaManagerListViewModel GetMedias(FilterCollectionDataModel model, int? folderId = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(model.SortCollection))
            {
                model.SortCollection = new SortCollection();
                model.SortCollection.Add(ZnodeMediaEnum.MediaId.ToString(), DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return GetMedias(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, folderId);
        }

        //Get all media list.
        public virtual MediaManagerListViewModel GetMedias(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int? folderId = null)
        {
          ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });
          if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            if (folderId > 0 || folderId == -1)
            {
                //Checking whether MediaPathId already exist in filter.
                if (filters.Exists(x => x.Item1 == FilterKeys.MediaPathId))
                {   //If MediaPathId is already present in filters remove it.
                    filters.RemoveAll(x => x.Item1 == FilterKeys.MediaPathId);
                    //Add new MediaPathId into filters.
                    filters.Add(new FilterTuple(FilterKeys.MediaPathId, FilterOperators.Equals, folderId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.MediaPathId, FilterOperators.Equals, folderId.ToString()));
            }
            return MediaManagerModelMap.ToListVieWModel(_mediaManagerClient.GetMedias(filters, sortCollection, pageIndex, recordPerPage));
        }

        //Set media manager list with required properties.
        public virtual MediaManagerViewModel MediaManagerList(FilterCollectionDataModel model, PopupViewModel popupViewModel, MediaManagerViewModel viewModel, MediaManagerListViewModel mediaManager)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            viewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, mediaManager.MediaList, GridListType.View_GetMediaPathDetail.ToString(), string.Empty, null, true, true, viewModel?.GridModel?.FilterColumn?.ToolMenuList);
            viewModel.GridModel.TotalRecordCount = mediaManager.TotalResults;
            viewModel.GridModel.FolderId = viewModel.FolderId;
            //Gets the list of view modes.
            viewModel.GridModel.ViewModeType = GetViewModes(model.ViewMode);
            viewModel.PopupViewModel = popupViewModel;

            //Set Media Manager view model properties.
            SetMediaManagerViewProperties(viewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            return viewModel;
        }

        //Get tree structure for folders.
        public virtual string GetTree()
            => $"[{JsonConvert.SerializeObject(MediaManagerModelMap.ToTreeViewModel(_mediaManagerClient.GetTree()))}]";

        //Checks if file name already exist.
        public virtual string IsFileNamePresent(string fileName, int folderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { fileName = fileName, folderId = folderId });

              if (!string.IsNullOrEmpty(fileName))
               {
                FilterCollection filter = new FilterCollection() { new FilterTuple(FilterKeys.FileName, FilterOperators.Is, fileName), new FilterTuple(FilterKeys.MediaPathId, FilterOperators.Equals, folderId.ToString()) };

                MediaManagerListModel mediaList = _mediaManagerClient.GetMedias(filter);
              ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
               return (mediaList?.MediaList?.Count > 0) ? mediaList.MediaList.First().Path : string.Empty;
            }
      ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

      return string.Empty;
        }

        //Add new folder.
        public virtual MediaManagerFolderViewModel AddFolder(int parentId, string folderName, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                MediaManagerFolderModel mediaManagerFolderModel = _mediaManagerClient.AddFolder(MediaManagerModelMap.ToFolderModel(parentId, folderName));
                return (mediaManagerFolderModel.Id > 0) ? new MediaManagerFolderViewModel { Id = mediaManagerFolderModel.Id, HasError = true } : new MediaManagerFolderViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = MediaManager_Resources.FailedAddFolder;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                message = MediaManager_Resources.FailedAddFolder;
            }

         return new MediaManagerFolderViewModel();
        }

        //Rename existing folder.
        public virtual bool RenameFolder(int folderId, string folderName, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _mediaManagerClient.RenameFolder(MediaManagerModelMap.ToFolderModel(folderId, folderName));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = (ex.ErrorCode.HasValue) ? MediaManager_Resources.ErrorPermissionRenameFolder : MediaManager_Resources.FailedRenameFolder;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Gets media attribute values.
        public virtual MediaAttributeValuesListViewModel GetMediaAttributeValues(int mediaId)
        {
          ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
          ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeMediaEnum.ZnodeMediaCategories.ToString());

            //Get media attribute values.
            MediaAttributeValuesListModel mediaAttributeValuesListModel = _mediaManagerClient.GetMediaAttributeValues(mediaId, expands);

      ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
      //Map media values.
      return MapMediaValues(mediaId, mediaAttributeValuesListModel);
        }

        //Get media by media id.
        public virtual MediaManagerViewModel GetMedia(int mediaId)
        {
            if (mediaId > 0)
                return GetMedias(new FilterCollection() { new FilterTuple(FilterKeys.MediaId, FilterOperators.Equals, mediaId.ToString()) }, null, null, null, null)?.MediaList?.FirstOrDefault();

            return new MediaManagerViewModel();
        }

        //Update media attribute value list.
        public virtual bool UpdateMediaAttributeValueList(BindDataModel bindDataModel, MediaManagerViewModel metaInfo, out string errorMessage)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                errorMessage = string.Empty;
                //Remove unwanted values.
                RemoveNonAttributeKeys(bindDataModel);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return _mediaManagerClient.UpdateMediaAttributeValue(MediaAttributeValuesViewModelMap.ToListModel(bindDataModel, metaInfo));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                errorMessage = MediaManager_Resources.ErrorUpdateMediaAttributeValue;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                errorMessage = MediaManager_Resources.ErrorUpdateMediaAttributeValue;
                return false;
            }
        }

        //Move folder.
        public virtual bool MoveFolder(int folderId, int addtoFolderId, out string message)
        {
          ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
          message = string.Empty;
           try
            {
                return _mediaManagerClient.MoveFolder(MediaManagerModelMap.ToMoveFolderModel(folderId, addtoFolderId));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = (ex.ErrorCode.HasValue) ? MediaManager_Resources.ErrorPermissionMoveFolder : MediaManager_Resources.MoveFolderError;
                return false;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                message = MediaManager_Resources.MoveFolderError;
                return false;
            }
        }

        //Method To Delete existing MediaFile by MediaId
        public virtual bool DeleteMedia(string mediaIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            message = MediaManager_Resources.ErrorMediaId;
            try
            {
                if (!string.IsNullOrEmpty(mediaIds))
                {
                    string deletedMediaPaths = _mediaManagerClient.DeleteMedia(new DeleteMediaModel() { MediaIds = mediaIds });
                    //Delete Media From Server.
                    DeleteFromServer(deletedMediaPaths);
                    int deletedMedias = 0;

                    if (!string.IsNullOrEmpty(deletedMediaPaths))
                        //count of medias which are deleted.
                        deletedMedias = deletedMediaPaths.Split(',').Length;

                    if (mediaIds.Split(',').Length > deletedMedias)
                    {
                        message = MediaManager_Resources.ErrorMediaDelete;
                        return false;
                    }
                    else
                        return true;
                }
                return false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = (ex.ErrorCode.HasValue) ? MediaManager_Resources.ErrorPermissionDeleteMedia : MediaManager_Resources.DeleteMediaFailed;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                message = MediaManager_Resources.DeleteMediaFailed;
                return false;
            }
        }

        //Method To Delete existing MediaFile by MediaId
        public virtual bool DeleteFolder(int folderId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorFailedToDeleteFolder;
            try
            {
                DeleteFromServer(_mediaManagerClient.DeleteMedia(new DeleteMediaModel() { FolderId = folderId }));
                return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = (ex.ErrorCode.HasValue) ? MediaManager_Resources.ErrorPermissionDeleteFolder : Admin_Resources.ErrorFailedToDeleteFolder;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDeleteFolder;
                return false;
            }
        }

        //Move media file to folder.
        public virtual bool MoveMediaToFolder(int folderId, string mediaIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _mediaManagerClient.MoveMediaToFolder(AddMediaToFolderModelMap.ToModel(folderId, mediaIds));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = MediaManager_Resources.MoveMediaToFolderError;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                message = MediaManager_Resources.MoveMediaToFolderError;
                return false;
            }
        }

        //Share media folder.
        public virtual bool ShareMediaFolder(int folderId, string accountIds, out string message)
         {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            message = MediaManager_Resources.ErrorMediaShare;
            try
            {
                return _mediaManagerClient.ShareMediaFolder(MediaManagerModelMap.ToShareMediaFolderListModel(folderId, accountIds));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        message = MediaManager_Resources.MediaAlreadyShared;
                        break;
                    case ErrorCodes.NotPermitted:
                        message = MediaManager_Resources.NotPermittedToShareMedia;
                        break;
                    default:
                        message = MediaManager_Resources.ErrorMediaShare;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                message = MediaManager_Resources.ErrorMediaShare;
            }
            return false;
        }

        //Setting MediaManger Models properties for view.
        public virtual void SetMediaManagerViewProperties(MediaManagerViewModel viewModel)
        {
            //Creating instance for json of tree view.      
            viewModel.TreeView = new TreebuttonViewModel();
            viewModel.TreeView.Tree = GetTree();
            viewModel.TreeView.IsButtonVisible = false;
        }

        //Get view mode types.
        public virtual List<SelectListItem> GetViewModes(string selectedViewMode)
        {
            List<SelectListItem> viewModes = new List<SelectListItem>();
            viewModes.Add(new SelectListItem() { Text = (ViewModeTypes.Tile).ToString(), Value = (ViewModeTypes.Tile).ToString(), Selected = selectedViewMode.Equals((ViewModeTypes.Tile).ToString()) });
            viewModes.Add(new SelectListItem() { Text = (ViewModeTypes.List).ToString(), Value = (ViewModeTypes.List).ToString(), Selected = selectedViewMode.Equals((ViewModeTypes.List).ToString()) });
            return viewModes;
        }

        //Create data source.
        public virtual List<dynamic> CreateDataSource(List<dynamic> dataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            List<dynamic> XMLConfigurableList = GetFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);

            List<dynamic> _sortedXmlList = XMLConfigurableList?.FindAll(x => x.isvisible.Equals(DynamicGridConstants.Yes));
            List<dynamic> _resultList = new List<dynamic>();

            dataModel.ForEach(row =>
            {
                var columnObject = (IDictionary<string, object>)new ExpandoObject();
                _sortedXmlList.ForEach(col =>
                {
                    if (HelperUtility.IsNotNull(row.GetType().GetProperty(col.name)))
                        columnObject.Add(col.headertext, row.GetType().GetProperty(col.name).GetValue(row, null));
                });
                _resultList.Add(columnObject);
            });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            return _resultList;
        }

        //Get allowed extensions.
        public virtual FamilyExtensionListModel GetAllowedExtensions()
             => _mediaManagerClient.GetAllowedExtensions();

        //Get view model for media list.
        public virtual MediaManagerViewModel GetMediaManagerViewModel(FilterCollectionDataModel model, PopupViewModel popupViewModel, string displayMode, int folderId)
        {
      ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

      MediaManagerViewModel viewModel = new MediaManagerViewModel();

            viewModel.FolderId = folderId;
            if (!string.IsNullOrEmpty(displayMode))
                model.ViewMode = displayMode;

            //To get Checkbox in tilview.
            model.IsMultiSelectList = popupViewModel.IsMultiSelect;

            //Set tool menu for this grid view.
            SetMediaManagerListToolMenu(viewModel);
      ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

      return viewModel;
        }

        #region Static Methods
        /// <summary>
        ///get ImageFormat from string extentions
        /// </summary>
        /// <param name="extension">string extentions</param>
        /// <returns>ImageFormat</returns>
        public static ImageFormat GetImageFormat(string extension)
        {
            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    return ImageFormat.Png;
            }
        }
        #endregion

        #region Generate image.

        //Generate specific image on edit.
        public virtual bool GenerateImageOnEdit(string mediaPath)
            => _mediaManagerClient.GenerateImageOnEdit(mediaPath);
        #endregion

        #endregion

        #region Private Methods
        //Get editable controls.
        private List<MediaAttributeValuesViewModel> GetEditableControls(MediaAttributeValuesListViewModel mediaAttributeValues)
        {
            MediaAttributeValuesListViewModel mediaAttributeValuesListViewModel = new MediaAttributeValuesListViewModel();
            mediaAttributeValuesListViewModel.MediaAttributeValues = mediaAttributeValues.MediaAttributeValues.GroupBy(x => x.AttributeCode).Select(g => g.First()).ToList();
            var distinctAttributeTypeName = mediaAttributeValues.MediaAttributeValues.Select(e => e.AttributeCode).Distinct().ToList();
            int i = 0;
            foreach (var item in distinctAttributeTypeName)
            {
                List<MediaAttributeValuesViewModel> data = mediaAttributeValues.MediaAttributeValues.Where(x => x?.AttributeCode == item?.ToString()).ToList();

                //Appended keys with property name {AttributeCode}[0]_{mediaAttributeValueId}[1]_{DefaultAttributeValueId}[2]_{AttributeId}[3]_{MediaCategoryId}[4].
                string controlName = $"{data[0].AttributeCode}_{data[0].MediaAttributeValueId.GetValueOrDefault()}_{data[0].DefaultAttributeValueId.GetValueOrDefault()}_{data[0].AttributeId.GetValueOrDefault()}_{data[0].MediaCategoryId}";

                mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.Id = controlName;
                mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.ControlType = data[0].AttributeTypeName;
                mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.Name = $"{controlName}_attr";
                mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.ControlLabel = data[0].AttributeName;
                mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.Value = string.IsNullOrEmpty(data[0].MediaAttributeValue) ? data[0].DefaultAttributeValue : data[0].MediaAttributeValue;
                mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.HelpText = data[0].HelpDescription;
                if (Equals(Regex.Replace(data[0].AttributeTypeName, @"\s", ""), ControlTypes.MultiSelect.ToString()) || Equals(Regex.Replace(data[0].AttributeTypeName, @"\s", ""), ControlTypes.SimpleSelect.ToString()))
                {
                    mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.SelectOptions = new List<SelectListItem>();
                    var SelectOptionsList = data.Select(x => new { x.DefaultAttributeValue, x.DefaultAttributeValueId }).ToList();
                    foreach (var SelectOptions in SelectOptionsList)
                        mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.SelectOptions.Add(new SelectListItem() { Text = SelectOptions.DefaultAttributeValue, Value = SelectOptions.DefaultAttributeValueId.ToString() });
                }
                if (data[0].IsRequired.GetValueOrDefault())
                    mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.htmlAttributes.Add(AdminConstants.IsRequired, data[0].IsRequired);

                foreach (var dataItem in data)
                {
                    if (!string.IsNullOrEmpty(dataItem.ValidationName) && !Equals(dataItem.ValidationName, AdminConstants.Extensions)
                        && !(mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.htmlAttributes.ContainsKey(dataItem.ValidationName)))
                    {
                        if (Equals(dataItem.ControlName, AdminConstants.Select) || Equals(dataItem.ControlName, AdminConstants.MultiSelect))
                            mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.SubValidationName);
                        else
                            mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.ValidationValue);
                    }
                    else if ((data.Select(x => x.ValidationName == AdminConstants.Extensions).ToList()).Any(m => m))
                    {
                        if (mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.htmlAttributes.ContainsKey(AdminConstants.Extensions) == false)
                        {
                            string result = string.Join(",", data.Where(x => x.ValidationName == AdminConstants.Extensions).Select(k => k.SubValidationName).ToArray());
                            mediaAttributeValuesListViewModel.MediaAttributeValues[i].ControlProperty.htmlAttributes.Add(AdminConstants.Extensions, result);
                        }
                    }
                }
                i++;
            }
            return mediaAttributeValuesListViewModel.MediaAttributeValues;
        }

        //Delete media from server.
        private bool DeleteFromServer(string mediaPaths)
        {
      ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { mediaPaths = mediaPaths });

      if (!string.IsNullOrEmpty(mediaPaths))
            {
                string className = string.Empty;
                ServerConnector connectorObject = GetServerConnection(out className);

                //Delete the original file
                object deletedObject = connectorObject.CallConnector(className, MediaStorageAction.Delete, mediaPaths, string.Empty);

                //delete the thumbnail file
                connectorObject.CallConnector(className, MediaStorageAction.Delete, mediaPaths, connectorObject.UploadPolicyModel.ThumbnailFolderName);

                return HelperUtility.IsNotNull(deletedObject);
            }
            return false;
        }

        //Get the server connection.
        private ServerConnector GetServerConnection(out string className)
        {
            ServerConnector connectorObject = null;
            //Gets the default server configuration.
            MediaConfigurationModel defaultConfiguration = _mediaConfigurationClient.GetDefaultMediaConfiguration();

            if (HelperUtility.IsNotNull(defaultConfiguration))
            {
                //Sets the server connection.
                connectorObject = new ServerConnector(new FileUploadPolicyModel(defaultConfiguration.AccessKey, defaultConfiguration.SecretKey, defaultConfiguration.BucketName, defaultConfiguration.ThumbnailFolderName, defaultConfiguration.URL, defaultConfiguration.NetworkUrl));
                className = defaultConfiguration.MediaServer?.ClassName;
            }
            else
            {
                //Local
                connectorObject = new ServerConnector(new FileUploadPolicyModel(string.Empty, string.Empty, AdminConstants.DefaultMediaFolder, AdminConstants.ThumbnailFolderName, string.Empty, string.Empty));
                className = AdminConstants.DefaultMediaClassName;
            }
            return connectorObject;
        }

        //Remove the filter on folder click. 
        private void RemoveFilters(FilterCollectionDataModel model, int? folderId)
        {
            if (model?.Filters?.Count > 1)
            {
                if (!Equals(folderId, -1) && !Equals(GetFromSession<int>(DynamicGridConstants.FolderId), Convert.ToInt32(folderId)))
                    model.Filters.RemoveAll(x => x.FilterName != FilterKeys.MediaPathId);
            }
        }

        //Set tool menu for media manager list grid view.
        private void SetMediaManagerListToolMenu(MediaManagerViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = MediaManager_Resources.ButtonMoveMedia, DataToggleModel = "modal", JSFunctionName = "MediaManagerTools.prototype.SeIsMoveFolderValue('TreeViewModelPopup')", ControllerName = "MediaManager", ActionName = "MoveMediaToFolder" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('MediaDeletePopup')", ControllerName = "MediaManager", ActionName = "DeleteMedia" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDownload, JSFunctionName = "MediaManagerTools.prototype.Download('Download')", ControllerName = "MediaManager", ActionName = "List" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = MediaManager_Resources.ButtonMediaSettings, Url = "/MediaManager/MediaConfiguration/MediaConfiguration", ControllerName = "MediaConfiguration", ActionName = "MediaConfiguration" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.CSV,
                    ControllerName = "Export",
                    ActionName = "Export",
                    Value = "2",
                    Type = "Media",
                    JSFunctionName = "Products.prototype.Export(event)"
                });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.Excel,
                    ControllerName = "Export",
                    ActionName = "Export",
                    Value = "1",
                    Type = "Media",
                    JSFunctionName = "Products.prototype.Export(event)"
                });
            }
        }

        //Map media values.
        private MediaAttributeValuesListViewModel MapMediaValues(int mediaId, MediaAttributeValuesListModel mediaAttributeValuesListModel)
        {
            //Maps MediaAttributeValuesListModel to MediaAttributeValuesListViewModel.
            MediaAttributeValuesListViewModel mediaAttributeValuesListViewModel = MediaAttributeValuesViewModelMap.ToListViewModel(mediaAttributeValuesListModel);

            //Get editable controls for media.
            mediaAttributeValuesListViewModel.MediaAttributeValues = GetEditableControls(mediaAttributeValuesListViewModel);
            ZnodeLogging.LogMessage("MediaAttributeValues list count.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new  { MediaAttributeValuesCount = mediaAttributeValuesListViewModel?.MediaAttributeValues?.Count });
            mediaAttributeValuesListViewModel.MediaId = mediaId;

            if (HelperUtility.IsNotNull(mediaAttributeValuesListModel.Media))
            {
                //Map the values of media.
                mediaAttributeValuesListViewModel.MediaPath = string.IsNullOrEmpty(mediaAttributeValuesListModel.Media.MediaServerPath) ? string.Empty : mediaAttributeValuesListModel.Media.MediaServerPath;
                mediaAttributeValuesListViewModel.MediaVirtualPath = mediaAttributeValuesListViewModel.MediaVirtualPath;
                mediaAttributeValuesListViewModel.FamilyCode = mediaAttributeValuesListViewModel?.FamilyCode;
                mediaAttributeValuesListViewModel.Path = mediaAttributeValuesListViewModel.Path;
            }

            return mediaAttributeValuesListViewModel;
        }
        #endregion
    }
}
