using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Admin.Core.Maps;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Admin.Agents
{
    public class ContentAgent : BaseAgent, IContentAgent
    {
        #region Private Variables
        private readonly IPortalClient _portalClient;
        private readonly IContentPageClient _contentPageClient;
        private readonly ICMSWidgetConfigurationClient _widgetConfigurationClient;
        private readonly ITemplateClient _templateClient;
        private readonly IPortalProfileClient _portalProfileClient;
        private readonly ISEOSettingClient _seoSettingClient;
        private readonly ILocaleClient _localeClient;
        private readonly ILocaleAgent _localeAgent;
        #endregion

        #region Constructor
        public ContentAgent(IPortalClient portalClient, IContentPageClient contentPageClient, ICMSWidgetConfigurationClient widgetConfigurationClient, IPortalProfileClient portalProfileClient, ITemplateClient templateClient, ISEOSettingClient seoSettingClient, ILocaleClient localeClient)
        {
            _portalClient = GetClient<IPortalClient>(portalClient);
            _contentPageClient = GetClient<IContentPageClient>(contentPageClient);
            _widgetConfigurationClient = GetClient<ICMSWidgetConfigurationClient>(widgetConfigurationClient);
            _portalProfileClient = GetClient<IPortalProfileClient>(portalProfileClient);
            _templateClient = GetClient<ITemplateClient>(templateClient);
            _seoSettingClient = GetClient<ISEOSettingClient>(seoSettingClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
            _localeAgent = new LocaleAgent(GetClient<ILocaleClient>(_localeClient));
        }
        #endregion

        #region Content Page  
        //Get the required details for the Content pages.
        [Obsolete]
        public virtual ContentPageViewModel GetContentPageInformation(ContentPageViewModel contentPageViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Get the List of Available Templates.
            contentPageViewModel.TemplateList = ContentPageViewModelMap.ToTemplateListItems(_templateClient.GetTemplates(null, null, null, null, null)?.Templates);

            //Get the list of available Portals.
            contentPageViewModel.PortalList = ContentPageViewModelMap.ToListItems(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return contentPageViewModel;
        }

        //Get the required details for the Content pages with Image Template.
        public virtual ContentPageViewModel GetContentPageDetail(ContentPageViewModel contentPageViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Get the List of Available Templates.           
            contentPageViewModel.TemplateImageList = ContentPageViewModelMap.ToTemplateListItemsWithImage(_templateClient.GetTemplates(null, null, null, null, null)?.Templates);

            //Get the list of available Portals.
            contentPageViewModel.PortalList = ContentPageViewModelMap.ToListItems(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set the IsActive Flag True.
            contentPageViewModel.IsActive = true;
            return contentPageViewModel;
        }

        //Get Content Page list.
        public virtual ContentPageListViewModel GetContentPageList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            ZnodeLogging.LogMessage("localeId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());

            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));

            //Sorting list in descending order of modified date.
            if (HelperUtility.IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters of method GetContentPageList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            //Get Content Page List.
            ContentPageListModel contentPageListModel = _contentPageClient.GetContentPageList(filters, null, sorts, pageIndex, pageSize);
            ContentPageListViewModel contentPageListViewModel = new ContentPageListViewModel { ContentPageList = contentPageListModel?.ContentPageList?.ToViewModel<ContentPageViewModel>().ToList() };
            SetListPagingData(contentPageListViewModel, contentPageListModel);
            contentPageListViewModel.Locale = GetLocalList();
            //Set tool menu for content page grid list view.
            SetContentPageListToolMenu(contentPageListViewModel);
            contentPageListViewModel.Tree = GetTree();
            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(contentPageListViewModel.Locale, localeId.ToString());
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return contentPageListViewModel?.ContentPageList?.Count > 0 ? contentPageListViewModel : new ContentPageListViewModel() { ContentPageList = new List<ContentPageViewModel>(), Locale = contentPageListViewModel.Locale };
        }

        //Get the Associated Profiles based on the Portal Id.
        public virtual List<SelectListItem> GetProfileList(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("Input parameters of method GetPortalProfiles: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            return ContentPageViewModelMap.ToProfileListItems(_portalProfileClient.GetPortalProfiles(new ExpandCollection { ExpandKeys.Profiles }, filters, null, null, null)?.PortalProfiles);
        }

        //Check whether the Content Page Name already exists.
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
       " This method mark as obsolute due to portalId check was not present while checking page name exist" +
       " Please use overload of this method having contentPageName & portalId as a parameters")]
        public virtual bool CheckContentPageNameExist(string contentPageName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSContentPageEnum.PageName.ToString(), FilterOperators.Is, contentPageName.Trim()));
            ZnodeLogging.LogMessage("Input parameters of method GetContentPageList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the Content Page List based on the content page name filter.
            ContentPageListModel contentPageList = _contentPageClient.GetContentPageList(filters, null, null, null, null);
            if (IsNotNull(contentPageList) && IsNotNull(contentPageList.ContentPageList))
            {
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return contentPageList.ContentPageList.FindIndex(x => x.PageName == contentPageName) != -1;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return false;
        }

        //Check whether the Content Page Name already exists for the particular portal.
        public virtual bool CheckContentPageNameExist(string contentPageName, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSContentPageEnum.PageName.ToString(), FilterOperators.Is, contentPageName.Trim()));
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("Input parameters of method GetContentPageList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the Content Page List based on the content page name filter.
            ContentPageListModel contentPageList = _contentPageClient.GetContentPageList(filters, null, null, null, null);
            if (IsNotNull(contentPageList) && IsNotNull(contentPageList.ContentPageList))
            {
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return contentPageList.ContentPageList.FindIndex(x => x.PageName == contentPageName) != -1;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return false;
        }

        //Create content page.
        public virtual ContentPageViewModel CreateContentPage(ContentPageViewModel contentPageViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Set the default locale value.
                contentPageViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                contentPageViewModel.RobotTagValue = IsNotNull(contentPageViewModel.RobotTag) ? contentPageViewModel.RobotTag.ToString() : RobotTag.None.ToString();
                //Create the Content Page.
                ContentPageModel contentPage = _contentPageClient.CreateContentPage(contentPageViewModel?.ToModel<ContentPageModel>());
                contentPage.LocaleId = contentPageViewModel.LocaleId;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(contentPage) ? contentPage.ToViewModel<ContentPageViewModel>() : new ContentPageViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    //Catch exception when page name already exists or for an error.
                    case ErrorCodes.AlreadyExist:
                        return (ContentPageViewModel)GetViewModelWithErrorMessage(contentPageViewModel, Admin_Resources.ContentPageNameAlreadyExistErrorMessage);
                    case ErrorCodes.SEOUrlAlreadyExists:
                        return (ContentPageViewModel)GetViewModelWithErrorMessage(contentPageViewModel, Admin_Resources.ErrorSeoNameAlreadyExists);
                    default:
                        return (ContentPageViewModel)GetViewModelWithErrorMessage(contentPageViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (ContentPageViewModel)GetViewModelWithErrorMessage(contentPageViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get content page details on the basis of content page id and localeId.
        public virtual ContentPageViewModel GetContentPage(int contentPageId, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            IPublishPopupHelper publishStateList = GetService<IPublishPopupHelper>();
            //Set Filter for Locale Id.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId == 0 ? DefaultSettingHelper.DefaultLocale : localeId.ToString()));
            filters.Add(new FilterTuple(ZnodeCMSContentPageEnum.CMSContentPagesId.ToString(), FilterOperators.Equals, contentPageId.ToString()));
            ZnodeLogging.LogMessage("Input parameters of method GetContentPage: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });

            //Get Content Page Details by Id.
            ContentPageViewModel contentPageModel = _contentPageClient.GetContentPage(filters)?.ToViewModel<ContentPageViewModel>();
            if (HelperUtility.IsNotNull(contentPageModel))
            {
                //Get the path of template.
                string templatePath = HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath + $"/{contentPageModel.PageTemplateFileName}.cshtml");
                ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { templatePath = templatePath });

                contentPageModel.TemplatePath = templatePath;

                //Bind Profile List.
                contentPageModel.ProfileList = GetProfileList(contentPageModel.PortalId);
                contentPageModel.LocaleId = (contentPageModel.LocaleId > 0) ? contentPageModel.LocaleId : Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

                //Bind Locale List.
                contentPageModel.Locale = _localeAgent.GetLocalesList(contentPageModel.LocaleId);

                if (!string.IsNullOrEmpty(contentPageModel.PageTemplateName))
                {
                    //Get the available Widgets for the Content Pages.
                    contentPageModel.Widgets = WidgetHelper.GetAvailableWidgets(templatePath, null, false);
                    contentPageModel.Widgets.TypeOFMapping = ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString();
                    contentPageModel.Widgets.DisplayName = contentPageModel.PageName;
                }

                contentPageModel.IsPreviewGloballyEnabled = Convert.ToBoolean(publishStateList.GetAvailablePublishStatesforPreview().FirstOrDefault(x => x.ApplicationType == ApplicationTypesEnum.WebstorePreview.ToString())?.IsEnabled);
                contentPageModel.PreviewUrl = GetPreviewURL(contentPageModel.PortalId, contentPageModel.IsPreviewGloballyEnabled);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return HelperUtility.IsNotNull(contentPageModel) ? contentPageModel : new ContentPageViewModel();
        }

        //Update the content page template for both Website content Page and Content Page.
        public virtual ContentPageViewModel UpdateContentPage(ContentPageViewModel contentPageViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Update content page template and map the model to view model.
                contentPageViewModel.RobotTagValue = IsNotNull(contentPageViewModel.RobotTag) ? contentPageViewModel.RobotTag.ToString() : RobotTag.None.ToString();
                contentPageViewModel = _contentPageClient.UpdateContentPage(contentPageViewModel?.ToModel<ContentPageModel>())?.ToViewModel<ContentPageViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return contentPageViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (ContentPageViewModel)GetViewModelWithErrorMessage(contentPageViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        // Get preview url depending on the condition
        public virtual string GetPreviewURL(int portalId, bool isPreviewGloballyEnabled, bool isStorePublish = true)
        {
            IStoreAgent _storeAgent = GetService<IStoreAgent>();

            if (ZnodeAdminSettings.EnableCMSPreview && isPreviewGloballyEnabled && isStorePublish)
            {
                return _storeAgent.GetDomains(portalId, null, null, null, null)?.Domains?.FirstOrDefault(x => x.ApplicationType == ApplicationTypesEnum.WebstorePreview.ToString() && x.Status == true)?.DomainName;
            }
            return string.Empty;
        }

        //Delete content page template.
        public virtual bool DeleteContentPage(string contentPageId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                //Delete content page template.
                ZnodeLogging.LogMessage("Deleting ContentPage with", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info, new { Ids = contentPageId });

                return _contentPageClient.DeleteContentPage(new ParameterModel { Ids = contentPageId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Publish content page.
        public virtual bool PublishContentPage(string contentPageId, out string errorMessage, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                //Publish the Content Page.
                var result = _contentPageClient.PublishContentPage(new ContentPageParameterModel { Ids = contentPageId, localeId = localeId, TargetPublishState = targetPublishState, TakeFromDraftFirst = takeFromDraftFirst });
                if (IsNull(result?.IsPublished) || result?.IsPublished == false)
                    errorMessage = string.IsNullOrEmpty(result?.ErrorMessage) ? Admin_Resources.ErrorPublished : result?.ErrorMessage;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return Convert.ToBoolean(result?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = ex.Message;
                        return false;
                    case ErrorCodes.StoreNotPublishedForAssociatedEntity:
                        errorMessage = ex.Message;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorPublished;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        //Publish content page.
        public virtual bool PublishContentPage(string contentPageId, int localeId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                localeId = localeId == 0 ? GetLocaleValue() : localeId;
                //Publish the Content Page.
                var result = _contentPageClient.PublishContentPage(new ContentPageParameterModel { Ids = contentPageId, localeId = localeId });
                errorMessage = result?.ErrorMessage;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return Convert.ToBoolean(result?.IsPublished);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get locale list.
        public virtual List<SelectListItem> GetLocalList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            LocaleListModel locales = _localeClient.GetLocaleList(null, new FilterCollection { new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, "true") }, null);

            return (from item in locales.Locales
                    select new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.LocaleId.ToString(),
                        Selected = item.IsDefault
                    }).ToList();
        }

        //Get LocaleId
        public virtual int GetLocaleValue()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_contentCulture"))
            {
                localeId = string.IsNullOrEmpty(CookieHelper.GetCookieValue<string>("_contentCulture")) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : CookieHelper.GetCookieValue<Int32>("_contentCulture");
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_contentCulture", Convert.ToString(localeId));
            }
            ZnodeLogging.LogMessage("localeId returned: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            return localeId;
        }

        //Get portal Associated Widgets.
        public virtual CMSWidgetsListViewModel GetTemplateWidgets(int cmsMappingId, string typeOfMapping, string templatePath, string displayName, string fileName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            CMSWidgetsListViewModel cmsWidgetsListViewModel = new CMSWidgetsListViewModel();
            if (!string.IsNullOrEmpty(templatePath))
                cmsWidgetsListViewModel = WidgetHelper.GetAvailableWidgets(templatePath, null, false);

            //Bind the Request details.
            cmsWidgetsListViewModel.DisplayName = HttpUtility.UrlDecode(displayName);
            cmsWidgetsListViewModel.CMSMappingId = cmsMappingId;
            cmsWidgetsListViewModel.TypeOFMapping = typeOfMapping;
            cmsWidgetsListViewModel.FileName = fileName;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return HelperUtility.IsNotNull(cmsWidgetsListViewModel) ? cmsWidgetsListViewModel : new CMSWidgetsListViewModel();
        }

        //Check whether the Seo page Name already exists.
        public virtual bool CheckSeoNameExist(string seoName, int cmsContentPagesId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { seoName = seoName, cmsContentPagesId = cmsContentPagesId, PortalId = portalId });

            FilterCollection filters = new FilterCollection
            {
                new FilterTuple(ZnodeCMSSEODetailEnum.SEOUrl.ToString(), FilterOperators.Is, seoName.Trim()),
                new FilterTuple(ZnodeCMSSEODetailEnum.PortalId.ToString(), FilterOperators.Is, Convert.ToString(portalId))
            };
            ZnodeLogging.LogMessage("Input parameters of method GetContentPageList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the Content Page List based on the SEO Url filter.
            ContentPageListModel contentPageListModel = _contentPageClient.GetContentPageList(filters, null, null, null, null);

            if (HelperUtility.IsNotNull(contentPageListModel?.ContentPageList))
            {
                if (cmsContentPagesId > 0)
                {
                    //Set the status in case the Content page is open in edit mode.
                    ContentPageModel contentPage = contentPageListModel.ContentPageList.Find(x => x.CMSContentPagesId == cmsContentPagesId);
                    if (HelperUtility.IsNotNull(contentPage))
                        return !Equals(contentPage.SEOUrl, seoName.Trim());
                }
                return contentPageListModel.ContentPageList.FindIndex(x => x.SEOUrl == seoName.Trim() && x.PortalId == portalId) != -1;
            }
            return false;
        }

        //Publish the Saved TextWidgetConfiguration to Preview.
        public virtual bool PublishTextWidgetConfigurationToPreview(bool IsGlobalPreviewEnabled, string CMSMappingId, int localeId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            bool status = false;
            message = string.Empty;
            try
            {
                //Publish to Preview after content saving
                if (IsGlobalPreviewEnabled && ZnodeAdminSettings.EnableCMSPreview)
                {
                    //publish content page details.
                    status = PublishContentPage(CMSMappingId, out message, localeId, ZnodePublishStatesEnum.PREVIEW.ToString(), true);
                    //setting error message based on returned status
                    message = status ? (message == string.Empty ? Admin_Resources.TextPublishedSuccessfully : Admin_Resources.ErrorPublished) : message;
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                message = Admin_Resources.ErrorPublish;
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Content Page Tree

        //get tree structure for folders
        public virtual string GetTree()
            => $"[{JsonConvert.SerializeObject(GetTreeViewModel(_contentPageClient.GetTree()))}]";

        //Add new Folder
        public virtual int AddFolder(int parentId, string folderName)
            => parentId > 0 ? Convert.ToInt32(_contentPageClient.AddFolder(new ContentPageFolderModel { ParentCMSContentPageGroupId = parentId, Code = folderName })?.CMSContentPageGroupId) : 0;

        //Rename The  Existing folder
        public virtual bool RenameFolder(int folderId, string folderName)
            => folderId > 0
                ? _contentPageClient.RenameFolder(new ContentPageFolderModel { CMSContentPageGroupId = folderId, Code = folderName })
                : false;

        //Delete The  Existing folder
        public virtual bool DeleteFolder(string folderId)
            => !string.IsNullOrEmpty(folderId)
                ? _contentPageClient.DeleteFolder(new ParameterModel { Ids = folderId })
                : false;

        public virtual ContentPageListViewModel GetContentPageListViewModel(FilterCollectionDataModel model, int folderId, bool isRootFolder)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            SetFilters(model.Filters, folderId, isRootFolder);
            return GetContentPageList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
        }


        //Get the select list of locales.
        public virtual List<SelectListItem> GetLocalesList(int localeId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            LocaleListModel localeList = _localeClient.GetLocaleList(null, new FilterCollection { new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, "true") }, null, null, null);

            if (localeList?.Locales?.Count > 0)
            {
                if (localeId == 0)
                    localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                ZnodeLogging.LogMessage("localeId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId });

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return localeList.Locales.Select(x => new SelectListItem { Text = x.Name, Value = x.LocaleId.ToString(), Selected = Equals(x.LocaleId, localeId) }).ToList();
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new List<SelectListItem>();
        }

        //Move folder.
        public virtual bool MoveContentPagesFolder(int folderId, int addtoFolderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _contentPageClient.MoveContentPagesFolder(new ContentPageFolderModel() { CMSContentPageGroupId = folderId, ParentCMSContentPageGroupId = addtoFolderId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Move pages from one folder to another.
        public virtual bool MovePageToFolder(int folderId, string pageIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _contentPageClient.MovePageToFolder(AddPageToFolderModelMap.ToModel(folderId, pageIds));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                message = MediaManager_Resources.MoveMediaToFolderError;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                message = MediaManager_Resources.MoveMediaToFolderError;
                return false;
            }
        }

        #endregion

        #region Private Method       

        //Set tool option menus for content page grid.
        private void SetContentPageListToolMenu(ContentPageListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ContentPageDeletePopup')", ControllerName = "Content", ActionName = "DeleteContentPage" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.Move, DataToggleModel = "modal", JSFunctionName = "Content.prototype.SetIsMoveFolderValue('TreeViewModelPopup')", ControllerName = "Content", ActionName = "MovePageToFolder" });
            }
        }

        //Bind the Tree View Model.
        private ContentPageTreeViewModel GetTreeViewModel(ContentPageTreeModel treeModel)
        {
            if (HelperUtility.IsNull(treeModel))
                return new ContentPageTreeViewModel();

            return new ContentPageTreeViewModel
            {
                icon = treeModel.Icon,
                id = treeModel.Id,
                text = treeModel.Text,
                children = treeModel.Children.Select(x => GetTreeViewModel(x)).ToList()
            };
        }

        // Sets the filter for CMSContentPageGroupId property.
        private void SetFilters(FilterCollection filters, int folderId, bool isRootFolder)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking For CMSContentPageGroupId already Exists in Filters Or Not                         
                if (filters.Exists(x => x.Item1 == ZnodeCMSContentPageGroupEnum.CMSContentPageGroupId.ToString()))
                    //If CMSContentPageGroupId Already present in filters Remove It     
                    filters.RemoveAll(x => x.Item1 == ZnodeCMSContentPageGroupEnum.CMSContentPageGroupId.ToString());

                if (!isRootFolder)
                    //Add New CMSContentPageGroupId Into filters
                    filters.Add(new FilterTuple(ZnodeCMSContentPageGroupEnum.CMSContentPageGroupId.ToString(), FilterOperators.Equals, folderId.ToString()));
            }
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
        }
        #endregion
    }
}