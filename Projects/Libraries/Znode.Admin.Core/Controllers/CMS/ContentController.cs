using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ContentController: BaseController
    {
        #region Private Read only members
        private readonly IContentAgent _contentAgent;
        private readonly IManageMessageAgent _manageMessageAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly ISEOSettingAgent _seoSettingAgent;
        #endregion

        #region Public Constructor
        public ContentController(IContentAgent contentAgent, IManageMessageAgent manageMessageAgent, IStoreAgent storeAgent, ISEOSettingAgent seoSettingAgent)
        {
            _contentAgent = contentAgent;
            _manageMessageAgent = manageMessageAgent;
            _storeAgent = storeAgent;
            _seoSettingAgent = seoSettingAgent;
        }
        #endregion

        #region Manage Message
        //Method return a list view to display list of Content pages.
        public virtual ActionResult ManageMessageList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.View_GetManageMessageList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_GetManageMessageList.ToString(), model);
            //Get manage message list.
            ManageMessageListViewModel manageMessageList = _manageMessageAgent.GetManageMessages(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            manageMessageList.GridModel = FilterHelpers.GetDynamicGridModel(model, manageMessageList.ManageMessages, GridListType.View_GetManageMessageList.ToString(), string.Empty, null, true, true, manageMessageList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            manageMessageList.GridModel.TotalRecordCount = manageMessageList.TotalResults;
            return ActionView(manageMessageList);
        }

        //Get:Create message for portal.
        [HttpGet]
        public virtual ActionResult CreateManageMessage()
        {
            ActionResult action = GotoBackURL();
            if(action != null)
                return action;

            ManageMessageViewModel viewModel = new ManageMessageViewModel();
            viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

            //Get view model for mange message view.
            _manageMessageAgent.GetManageMessageViewModel(viewModel);
            return View(viewModel);
        }

        //Post:Create message for portal.
        [HttpPost]
        public virtual ActionResult CreateManageMessage(ManageMessageViewModel viewModel)
        {
            //Create manage message.
            ManageMessageViewModel data = _manageMessageAgent.CreateManageMessage(viewModel);
            if(data.HasError || data.CMSMessageId < 0)
            {     //Get view model for mange message view
                _manageMessageAgent.GetManageMessageViewModel(viewModel);
                SetNotificationMessage(GetErrorNotificationMessage(data.ErrorMessage));
                return View(viewModel);
            } else
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<ContentController>(x => x.UpdateManageMessage(data.CMSMessageKeyId, data.PortalId, data.LocaleId));
            }
        }

        //Get: Edit Manage Message.
        [HttpGet]
        public virtual ActionResult UpdateManageMessage(int cmsMessageKeyId, int portalId = 0, int localeId = 0)
        {
            ActionResult action = GotoBackURL();
            if(action != null)
                return action;
            //Get manage message details by its id.
            return Request.IsAjaxRequest() ? PartialView("_ManageMessageLocale", _manageMessageAgent.GetManageMessage(cmsMessageKeyId, portalId, localeId)) : ActionView("CreateManageMessage", _manageMessageAgent.GetManageMessage(cmsMessageKeyId, portalId, localeId));
        }

        //Post: Edit Manage Message.
        [HttpPost]
        public virtual ActionResult UpdateManageMessage(ManageMessageViewModel manageMessageViewModel)
        {
            string message = Admin_Resources.UpdateErrorMessage;
            ManageMessageViewModel manageMessage = _manageMessageAgent.UpdateManageMessage(manageMessageViewModel);
            if(manageMessage.HasError || manageMessage.CMSMessageId < 0)
                SetNotificationMessage(GetErrorNotificationMessage(manageMessage.ErrorMessage));
            else
                //Updates Manage message details and redirect to Update Manage Message with success message else returns an error message.
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            return RedirectToAction<ContentController>(x => x.UpdateManageMessage(manageMessageViewModel.CMSMessageKeyId, manageMessageViewModel.PortalId, manageMessageViewModel.LocaleId));
        }

        //Delete Manage Messages.
        public virtual JsonResult DeleteManageMessage(string cmsPortalMessageId)
        {
            string message = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsPortalMessageId))
            {
                //Delete manage message.
                status = _manageMessageAgent.DeleteManageMessage(cmsPortalMessageId);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new {
                status = status, message = message
            }, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("To be discontinued in upcoming versions.")]
        //Publish Message
        public virtual JsonResult PublishManageMessage(int cmsMessageId, int portalId)
        {
            if(cmsMessageId > 0)
            {
                string errorMessage;
                bool status = _manageMessageAgent.PublishManageMessage(Convert.ToString(cmsMessageId), portalId, out errorMessage);
                return Json(new {
                    status = status, message = status ? PIM_Resources.TextPublishedSuccessfully : errorMessage
                }, JsonRequestBehavior.AllowGet);
            } else
                return Json(new {
                    status = false, message = Admin_Resources.Error
                }, JsonRequestBehavior.AllowGet);
        }

        //Publish Message
        public virtual JsonResult PublishManageMessageWithPreview(int cmsMessageKeyId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            if(cmsMessageKeyId > 0)
            {
                string errorMessage;
                bool status = _manageMessageAgent.PublishManageMessage(Convert.ToString(cmsMessageKeyId), portalId, localeId, out errorMessage, targetPublishState, takeFromDraftFirst);
                return Json(new {
                    status = status, message = status ? PIM_Resources.TextPublishedSuccessfully : errorMessage
                }, JsonRequestBehavior.AllowGet);
            } else
                return Json(new {
                    status = false, message = Admin_Resources.Error
                }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult UpdateAndPublishManageMessage(ManageMessageViewModel model)
        {
            string errorMessage = string.Empty;
            var updatePublish = _manageMessageAgent.UpdateManageMessage(model);

            var updatedModel = _manageMessageAgent.GetManageMessage(updatePublish.CMSMessageKeyId, updatePublish.PortalId, updatePublish.LocaleId);
            int localeId = 0;
            if(updatedModel.CMSMessageId > 0)
            {
                bool status = _manageMessageAgent.PublishManageMessage(Convert.ToString(updatedModel.CMSMessageKeyId), model.PortalId, localeId, out errorMessage, model.TargetPublishState, model.TakeFromDraftFirst);
                SetNotificationMessage(status ? GetSuccessNotificationMessage(PIM_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(errorMessage));
                return RedirectToAction<ContentController>(x => x.ManageMessageList(null));
            } else
                SetNotificationMessage(GetErrorNotificationMessage(updatedModel.ErrorMessage));
            return RedirectToAction<ContentController>(x => x.ManageMessageList(null));
        }

        #endregion

        #region Content Page

        //Method return a list view to display list of Content pages.
        public virtual ActionResult ContentPageList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int folderId = -1, bool isRootFolder = true)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.StaticPageList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.StaticPageList.ToString(), model);
            //Get content page list.
            ContentPageListViewModel contentPageList = _contentAgent.GetContentPageListViewModel(model, folderId, isRootFolder);

            //Get the grid model.
            contentPageList.GridModel = FilterHelpers.GetDynamicGridModel(model, contentPageList.ContentPageList, GridListType.StaticPageList.ToString(), string.Empty, null, true, true, contentPageList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            contentPageList.GridModel.TotalRecordCount = contentPageList.TotalResults;
            contentPageList.FolderId = folderId;
            contentPageList.IsRootFolder = isRootFolder;
            contentPageList.Tree = _contentAgent.GetTree();

            if(Request.IsAjaxRequest())
                return PartialView("_ContentPageList", contentPageList);

            return ActionView(contentPageList);
        }

        //Get: Add Content.
        [HttpGet]
        public virtual ActionResult AddContentPage(int folderId = -1)
            => ActionView(AdminConstants.CreateEdit, _contentAgent.GetContentPageDetail(new ContentPageViewModel { Tree = _contentAgent.GetTree(), Locale = _contentAgent.GetLocalesList(), LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale), FolderId = folderId }));

        //Method to add a new Content templates, having parameter ContentPageViewModel.
        [HttpPost]
        public virtual ActionResult AddContentPage(ContentPageViewModel contentPageViewModel)
        {
            if(ModelState.IsValid)
            {
                //Create content page.
                contentPageViewModel = _contentAgent.CreateContentPage(contentPageViewModel);

                //Based on the HasError property sets success or failure message.
                if(contentPageViewModel?.CMSContentPagesId > 0 && !contentPageViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    var contentViewModel = _contentAgent.GetContentPage(contentPageViewModel.CMSContentPagesId, contentPageViewModel.LocaleId);
                    var publishStatus = (HelperUtility.IsNotNull(contentViewModel) && contentViewModel.IsPreviewGloballyEnabled) ? "Draft" : string.Empty;
                    return RedirectToAction<ContentController>(x => x.EditContentPage(contentPageViewModel.CMSContentPagesId, string.Empty, contentPageViewModel.LocaleId, false, publishStatus));
                } else
                {
                    contentPageViewModel = _contentAgent.GetContentPageDetail(contentPageViewModel);
                    contentPageViewModel.Tree = _contentAgent.GetTree();
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(contentPageViewModel.ErrorMessage));
            return ActionView(AdminConstants.CreateEdit, contentPageViewModel);
        }

        //Get:Portal list.
        public virtual ActionResult GetProfileList(int portalId)
         => PartialView("_ProfileList", new ContentPageViewModel { ProfileList = _contentAgent.GetProfileList(portalId) });

        //Check content page name already exists or not.
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method mark as obsolute due to portalId check was not present while checking page name exist" +
        " Please use overload of this method having contentPageName & portalId as a parameters")]
        public virtual JsonResult IsContentPageNameExist(string name)
        {
            return Json(_contentAgent.CheckContentPageNameExist(name), JsonRequestBehavior.AllowGet);
        }
        //Check content page name already exists or not for the particular portal.
        public virtual JsonResult IsContentPageNameExistForPortal(string name, int portalId)
        {
            return Json(_contentAgent.CheckContentPageNameExist(name, portalId), JsonRequestBehavior.AllowGet);
        }

        //Get: Edit Content page.
        [HttpGet]
        public virtual ActionResult EditContentPage(int cmsContentPagesId, string fileName = "", int localeId = 0, bool isFromWebSiteController = false, string publishStatus = "")
        {
            ActionResult action = GotoBackURL();
            if(action != null)
                return action;

            //Get the Content Page Details by id and localeId.
            ContentPageViewModel model = _contentAgent.GetContentPage(cmsContentPagesId, localeId);
            model.FileName = fileName;
            model.OldSEOURL = model.SEOUrl;
            model.LocaleId = localeId;
            model.StorePublished = (_storeAgent.GetPortalPublishStatus(model.PortalId, null, null, null, null).PublishPortalLog.Count > 0);            
            return ActionView(Request.IsAjaxRequest() ? "_ContentPageForLocale" : AdminConstants.CreateEdit, model);
        }
        //Post: Edit Content Page.
        [HttpPost]
        public virtual ActionResult EditContentPage(ContentPageViewModel contentPageViewModel)
        {
            //update content page details.
            contentPageViewModel = _contentAgent.UpdateContentPage(contentPageViewModel);

            if(!contentPageViewModel.HasError)
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<ContentController>(x => x.EditContentPage(contentPageViewModel.CMSContentPagesId, string.Empty, contentPageViewModel.LocaleId, false, string.Empty));
        }

        //Delete: Delete Content pages.
        public virtual JsonResult DeleteContentPage(string cmsContentPagesId)
        {
            string errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsContentPagesId))
            {
                //Delete Content pages.
                status = _contentAgent.DeleteContentPage(cmsContentPagesId, out errorMessage);

                if(status)
                    errorMessage = Admin_Resources.DeleteMessage;
            }
            return Json(new {
                status = status, message = errorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Obsolete("To be discontinued in one of the upcoming versions.")]
        //Publish Content Page, Based on Locale.
        public virtual ActionResult PublishContentPage(string cmsContentPagesId, int localeId)
        {
            if(!string.IsNullOrEmpty(cmsContentPagesId))
            {
                string errorMessage = string.Empty;
                bool status = _contentAgent.PublishContentPage(cmsContentPagesId, localeId, out errorMessage);
                return Json(new {
                    status = status, message = status ? (errorMessage == String.Empty ? Admin_Resources.TextPublishedSuccessfully : errorMessage) : errorMessage
                }, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json(new {
                    status = false, message = Admin_Resources.ErrorPublished
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        //Publish Content Page, Based on Locale.
        public virtual ActionResult PublishContentPageWithPreview(string cmsContentPagesId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            if(!string.IsNullOrEmpty(cmsContentPagesId))
            {
                string errorMessage = string.Empty;
                bool status = _contentAgent.PublishContentPage(cmsContentPagesId, out errorMessage, localeId, targetPublishState, takeFromDraftFirst);
                return Json(new {
                    status = status, message = status ? (errorMessage == String.Empty ? Admin_Resources.TextPublishedSuccessfully : errorMessage) : errorMessage
                }, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json(new {
                    status = false, message = Admin_Resources.ErrorPublished
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //Post: Update and Publish Content Page.
        [HttpPost]
        public virtual ActionResult UpdateAndPublishContentPage(ContentPageViewModel contentPageViewModel)
        {
            string errorMessage = string.Empty;
            bool status = false;
            string targetPublishState = contentPageViewModel.TargetPublishState;
            bool takeFromDraftFirst = contentPageViewModel.TakeFromDraftFirst;
            string FileName = contentPageViewModel.FileName;
            //update content page details.
            contentPageViewModel = _contentAgent.UpdateContentPage(contentPageViewModel);

            if(!contentPageViewModel.HasError)
            {
                //publish content page details.
                status = _contentAgent.PublishContentPage(contentPageViewModel.CMSContentPagesId.ToString(), out errorMessage, contentPageViewModel.LocaleId, targetPublishState, takeFromDraftFirst);
                //setting error message based on returned status
                errorMessage = status ? (errorMessage == String.Empty ? Admin_Resources.TextPublishedSuccessfully : errorMessage) : errorMessage;
                //setting notification message.
                SetNotificationMessage(status ? GetSuccessNotificationMessage(errorMessage) : GetErrorNotificationMessage(errorMessage));
            } else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorPublished));
            }
            return RedirectToAction<ContentController>(x => x.EditContentPage(contentPageViewModel.CMSContentPagesId, FileName, 0, false, string.Empty));
        }

        //Get the details of widget.
        public virtual ActionResult GetTemplateWidgets(int cmsMappingId, string typeOfMapping, string templatePath, string displayName, string fileName, bool isStorePublish = false)
        {
            var localeId = 0; // TODO - hookup
            ContentPageViewModel pageModel = _contentAgent.GetContentPage(cmsMappingId, localeId);
            CMSWidgetsListViewModel widgetsListModel = _contentAgent.GetTemplateWidgets(cmsMappingId, typeOfMapping, templatePath, displayName, fileName);
            widgetsListModel.ParentContentPageSEOUrl = !string.IsNullOrEmpty(pageModel.SEOUrl) ? pageModel.SEOUrl : $"contentpage/{cmsMappingId}";
            widgetsListModel.PreviewUrl = _contentAgent.GetPreviewURL(pageModel.PortalId, pageModel.IsPreviewGloballyEnabled, isStorePublish);
            return PartialView(AdminConstants.WidgetPageList, widgetsListModel);
        }

        //Get the details of widget.
        public virtual ActionResult RedirectToConfigureWidget(int cmsMappingId,string widgetKey, int localeId = 0)
        {
            ContentPageViewModel pageModel = _contentAgent.GetContentPage(cmsMappingId, localeId);
            CMSWidgetsListViewModel widgetsListModel = _contentAgent.GetTemplateWidgets(cmsMappingId, 
                                                                                        pageModel.Widgets.TypeOFMapping,
                                                                                        pageModel.TemplatePath,
                                                                                        HttpUtility.UrlEncode(pageModel.Widgets.DisplayName),
                                                                                        HttpUtility.UrlEncode("Widgets")
                                                                                        );
            WidgetHelper.SetWidgetActions(widgetsListModel);
            var widget = widgetsListModel.CMSWidgetsList.Where(w => w.MappingKey == widgetKey).Single();
            if (!widget.WidgetActionUrl.Equals("#"))
                return Redirect(widget.WidgetActionUrl);
            else
                return Redirect("/WebSite/ErrorOnWidget");
        }

        //Get the details of widget.
        public virtual ActionResult GetPreviewContentPage(int cmsMappingId, int localeId = 0)
        {
            ContentPageViewModel pageModel = _contentAgent.GetContentPage(cmsMappingId, localeId);
            List<DomainViewModel> domainViewModel = pageModel.IsPreviewGloballyEnabled ? _storeAgent.GetDomains(pageModel.PortalId, null, null, null, null)?.Domains.Where(x => x.Status == true).ToList() : new List<DomainViewModel>();
            return PartialView(AdminConstants.PreviewContentPage, new CMSWidgetsListViewModel() {
                ParentContentPageSEOUrl = !string.IsNullOrEmpty(pageModel.SEOUrl) ? pageModel.SEOUrl : $"contentpage/{cmsMappingId}",
                PreviewUrl = pageModel.IsPreviewGloballyEnabled ? domainViewModel?.FirstOrDefault(x => x.ApplicationType == ApplicationTypesEnum.WebstorePreview.ToString() && x.IsDefault == true)?.DomainName : string.Empty,
                ProductionUrl = domainViewModel?.FirstOrDefault(x => x.ApplicationType == ApplicationTypesEnum.WebStore.ToString())?.DomainName
            });
        }


        //Check seo name is exist or not.
        [HttpPost]
        public virtual JsonResult IsSeoNameExist(string SEOUrl, int CMSContentPagesId, int PortalId)
            => Json(!_contentAgent.CheckSeoNameExist(SEOUrl, CMSContentPagesId, PortalId), JsonRequestBehavior.AllowGet);

        //Move content page to folder according to flag.
        public virtual ActionResult MovePageToFolder(int folderId, string pageIds)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _contentAgent.MovePageToFolder(folderId, pageIds, out message);
            responsemessage.Message = responsemessage.HasNoError ? Admin_Resources.MoveContentPageSuccess : message;
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);
            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true, true, new List<ToolMenuModel>());

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView("_asideStoreListPanelPoup", storeList);
        }

        public virtual ActionResult GetDefaultCMSSEODetails(int seoTypeId, int itemId = 0, int localeId = 0, int portalId = 0, string seoCode = null)
        {
            ContentPageViewModel model = _seoSettingAgent.GetDefaultCMSSEODetails(seoTypeId, localeId, portalId, seoCode, itemId);
            return ActionView(AdminConstants.DefaultCMSSEOPage, model);

        }

        #region Tree

        //Creates new folder.
        public virtual ActionResult AddFolder(int parentId, string folderName)
        {
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.Id = _contentAgent.AddFolder(parentId, folderName);
            responsemessage.HasNoError = responsemessage.Id > 0;
            responsemessage.Message = responsemessage.HasNoError ? Admin_Resources.AddFolder : Admin_Resources.FailedAddFolder;
            responsemessage.FolderJsonTree = _contentAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Rename existing folder.
        public virtual ActionResult RenameFolder(int folderId, string folderName)
        {
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _contentAgent.RenameFolder(folderId, folderName);
            responsemessage.Message = responsemessage.HasNoError ? Admin_Resources.RenameFolder : Admin_Resources.FailedRenameFolder;
            responsemessage.FolderJsonTree = _contentAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Delete existing folder.
        public virtual ActionResult DeleteFolder(string folderId)
        {
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _contentAgent.DeleteFolder(folderId);
            responsemessage.Message = responsemessage.HasNoError ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDeleteFolder;
            responsemessage.FolderJsonTree = _contentAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Move folder to another folder.
        public virtual ActionResult MoveContentPagesFolder(int addtoFolderId, int folderId)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _contentAgent.MoveContentPagesFolder(folderId, addtoFolderId);
            responsemessage.Message = responsemessage.HasNoError ? MediaManager_Resources.MoveFolderSuccess : message;
            responsemessage.FolderJsonTree = _contentAgent.GetTree();
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
    }
}