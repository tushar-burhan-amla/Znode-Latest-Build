using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.MediaManager.Controllers
{
    public class MediaManagerController : BaseController
    {
        #region Private data member
        private IMediaManagerAgent _mediaManagerAgent;
        private INavigationAgent _navigationAgent;
        private IUserAgent _userAgent;
        private const string MediaDetailsView = "MediaEdit";
        private const string _MediaManagerBreadCrumbPartialView = "_MediaManagerBreadCrum";
        private const string ControllerName = "MediaManager/MediaManager";
        private const string GetMediaAttributeValuesAction = "GetMediaAttributeValues";
        private const string MediaIdKey = "MediaId";
        private const string MinDateKey = "mindate";
        private const string MaxDateKey = "maxdate";
        private const string MediaDetailsPartialView = "MediaDetails";
        private const string MediaStoreAdminUserListView = "_MediaUserAccountList";
        #endregion

        #region Public constructor
        public MediaManagerController(IMediaManagerAgent mediaManagerAgent, INavigationAgent navigationAgent, IUserAgent userAgent)
        {
            _mediaManagerAgent = mediaManagerAgent;
            _navigationAgent = navigationAgent;
            _userAgent = userAgent;
        }

        #endregion

        #region Public Methods

        //Get the index view for Media Manager.
        public virtual ActionResult Index() => View();

        //Gets list of media.
        [MvcSiteMapNode(Title = "$Resources:MediaManager_Resources.MediaManager_Resources, TitleMediaManager", Key = "MediaManager", Area = "MediaManager", ParentKey = "Home")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, PopupViewModel popupViewModel, string displayMode = "", int folderId = -1)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_GetMediaPathDetail.ToString(), model);

            if (!string.IsNullOrEmpty(popupViewModel.Type))
            {
                model.Filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.MediaType, StringComparison.InvariantCultureIgnoreCase));
                model.Filters.Add(new FilterTuple(popupViewModel.Type, FilterOperators.In, popupViewModel.Value));
            }

            //Get media manager view model with properties set to its value.
            MediaManagerViewModel viewModel = _mediaManagerAgent.GetMediaManagerViewModel(model, popupViewModel, displayMode, folderId);
            //Get all medias.
            MediaManagerListViewModel mediaManager = _mediaManagerAgent.GetMedias(model, viewModel.FolderId);
            //Bind media manager list to the grid.
            _mediaManagerAgent.MediaManagerList(model, popupViewModel, viewModel, mediaManager);
            //Return particular view based on request type.
            return (popupViewModel.IsPopup) ? View("_MediaUploadView", viewModel) : (Request.IsAjaxRequest() ? PartialView("_MediaGrid", viewModel.GridModel) : ActionView("MediaList", viewModel));
        }

        //Checks if file name already exists.
        public virtual string IsFileNamePresent(string fileName, int folderId)
            => _mediaManagerAgent.IsFileNamePresent(fileName, folderId);

        //Creates new folder.
        public virtual ActionResult AddFolder(int parentId, string folderName)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            MediaManagerFolderViewModel mediaManagerFolderViewModel = _mediaManagerAgent.AddFolder(parentId, folderName, out message);
            responsemessage.HasNoError = mediaManagerFolderViewModel.HasError;
            responsemessage.Id = mediaManagerFolderViewModel.Id;
            responsemessage.Message = responsemessage.HasNoError ? MediaManager_Resources.AddFolderTree : message;
            responsemessage.FolderJsonTree = _mediaManagerAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Rename existing folder.
        public virtual ActionResult RenameFolder(int folderId, string folderName)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _mediaManagerAgent.RenameFolder(folderId, folderName, out message);
            responsemessage.Message = responsemessage.HasNoError ? MediaManager_Resources.RenameFolder : message;
            responsemessage.FolderJsonTree = _mediaManagerAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Move folder to another folder.
        public virtual ActionResult MoveFolder(int addtoFolderId, int folderId)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _mediaManagerAgent.MoveFolder(folderId, addtoFolderId, out message);
            responsemessage.Message = responsemessage.HasNoError ? MediaManager_Resources.MoveFolderSuccess : message;
            responsemessage.FolderJsonTree = _mediaManagerAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Get media attribute values.
        [HttpGet]
        public virtual ActionResult GetMediaAttributeValues(int mediaId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            MediaAttributeValuesListViewModel mediaAttributeValues = _mediaManagerAgent.GetMediaAttributeValues(mediaId);
            if (Request.IsAjaxRequest())
            {
                mediaAttributeValues.navigationModel = _navigationAgent.GetNavigationDetails(mediaId.ToString(), ControllerName, ZnodeEntities.ZnodeMedia.ToString(), "mediaId", null, GetMediaAttributeValuesAction, string.Empty, GetMediaAttributeValuesAction);
                return PartialView(MediaDetailsPartialView, mediaAttributeValues);
            }
            return View(MediaDetailsView, mediaAttributeValues);
        }

        //Add/Move medias to folder according to flag.
        public virtual ActionResult MoveMediaToFolder(int folderId, string mediaIds)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _mediaManagerAgent.MoveMediaToFolder(folderId, mediaIds, out message);
            responsemessage.Message = responsemessage.HasNoError ? MediaManager_Resources.MoveMediaSuccess : message;
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Delete medias.
        public virtual ActionResult DeleteMedia(string mediaId)
        {
            string message = MediaManager_Resources.ErrorMediaId;
            if (!string.IsNullOrEmpty(mediaId))
            {
                bool status = _mediaManagerAgent.DeleteMedia(mediaId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Delete folder.
        public virtual ActionResult DeleteFolder(int folderId)
        {
            string message = Admin_Resources.ErrorFailedToDeleteFolder;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _mediaManagerAgent.DeleteFolder(folderId, out message);
            responsemessage.Message = responsemessage.HasNoError ? Admin_Resources.DeleteMessage : message;
            responsemessage.FolderJsonTree = _mediaManagerAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Update media attribute value list.
        [HttpPost]
        public virtual ActionResult UpdateMediaAttributeValueList([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model, MediaManagerViewModel metaInfo, int selectedfolder = 0)
        {
            string errorMessage = string.Empty;
            int mediaId = Convert.ToInt32(model.GetValue(MediaIdKey));
            int folderId = Convert.ToInt32(model.GetValue(ZnodeMediaCategoryEnum.MediaPathId.ToString()));
            metaInfo.MediaId = mediaId;
            if (_mediaManagerAgent.UpdateMediaAttributeValueList(model, metaInfo, out errorMessage))
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                //-1 as folder id to get data of root folder.
                return RedirectToAction<MediaManagerController>(x => x.GetMediaAttributeValues(mediaId));
            }
            SetNotificationMessage(GetErrorNotificationMessage(string.IsNullOrEmpty(errorMessage) ? MediaManager_Resources.ErrorUpdateMediaAttributeValue : errorMessage));
            return RedirectToAction<MediaManagerController>(x => x.GetMediaAttributeValues(mediaId));
        }

        //Get allowed extensions.
        public virtual JsonResult GetAllowedExtensions()
             => Json(_mediaManagerAgent.GetAllowedExtensions(), JsonRequestBehavior.AllowGet);

        //Get Ajax headers.
        [AllowAnonymous]
        public virtual JsonResult GetAjaxHeaders()
        => Json(_mediaManagerAgent.GetAppHeader(), JsonRequestBehavior.AllowGet);

        #region Export Media

        //Export media.
        public virtual ActionResult ExportMedia(string exportFileTypeId)
        {
            //Media Folder Id.
            int folderId = SessionHelper.GetDataFromSession<int>(DynamicGridConstants.FolderId);

            //Get the Export Details.
            FilterCollection filters = SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);

            //Get medias.
            MediaManagerListViewModel mediaManagerViewModel = _mediaManagerAgent.GetMedias(filters, null, null, null, folderId);
            MediaExportViewModel model = new MediaExportViewModel();

            if (HelperUtility.IsNotNull(mediaManagerViewModel) && HelperUtility.IsNotNull(mediaManagerViewModel.MediaList))
            {
                model.ExportFileType = exportFileTypeId;
                List<dynamic> mediaList = new List<dynamic>();
                mediaList.AddRange(mediaManagerViewModel.MediaList);

                DownloadHelper downloadHelper = new DownloadHelper();
                //Download the Exported Data in Specified File.
                downloadHelper.ExportDownload(_mediaManagerAgent.CreateDataSource(mediaList), model.ExportFileType, Response, null, Equals(exportFileTypeId, "1") ? $"FolderId_{folderId}.xls" : $"FolderId_{folderId}.csv", false);
            }
            else
                SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.NoRecordFoundText, NotificationType.info));

            return View(model);
        }

        #endregion

        #region Share Media

        //Get user account list to share media with.
        public virtual ActionResult GetUserAccountList(string folderId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUser.ToString(), model);

            UsersListViewModel usersList = _userAgent.GetUserAccountList(HttpContext.User.Identity.Name, AdminConstants.AdminRoleName, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, usersList.List, GridListType.ZnodeUser.ToString(), string.Empty, null, true);

            //Set the total record count
            gridModel.TotalRecordCount = usersList.TotalResults;
            ViewData["FolderID"] = folderId;
            return ActionView(MediaStoreAdminUserListView, gridModel);
        }

        //Share folder.
        public virtual JsonResult ShareFolder(int folderId, string accountIds)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _mediaManagerAgent.ShareMediaFolder(folderId, accountIds, out message);
            responsemessage.Message = responsemessage.HasNoError ? MediaManager_Resources.MediaSharedSuccess : message;
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Generate images on Edit.        
        public virtual ActionResult GenerateImageOnEdit(string mediaPath)
        {
            bool result = _mediaManagerAgent.GenerateImageOnEdit(mediaPath);
            return Json(new { status = result, message = Admin_Resources.ImageGeneratedSuccessfully }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}