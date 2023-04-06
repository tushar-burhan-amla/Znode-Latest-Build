using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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

namespace Znode.Engine.Admin.Agents
{
    public class BlogNewsAgent : BaseAgent, IBlogNewsAgent
    {
        #region Private Variables
        private readonly IContentPageClient _contentPageClient;
        private readonly ILocaleClient _localeClient;
        private readonly IBlogNewsClient _blogNewsClient;
        #endregion

        #region Constructor
        public BlogNewsAgent(IContentPageClient contentPageClient, ILocaleClient localeClient, IBlogNewsClient blogNewsClient)
        {
            _contentPageClient = GetClient<IContentPageClient>(contentPageClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
            _blogNewsClient = GetClient<IBlogNewsClient>(blogNewsClient);
        }
        #endregion

        #region Public Methods

        #region Blog/News
        //Create the blog/news.
        public virtual BlogNewsViewModel CreateBlogNews(BlogNewsViewModel blogNewsViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                blogNewsViewModel.BlogNewsType = blogNewsViewModel.BlogNewsTypeValue.ToString();

                BlogNewsModel blogNews = _blogNewsClient.CreateBlogNews(blogNewsViewModel?.ToModel<BlogNewsModel>());
                return IsNotNull(blogNews) ? blogNews.ToViewModel<BlogNewsViewModel>() : new BlogNewsViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        return new BlogNewsViewModel { HasError = true, ErrorMessage = Admin_Resources.SeoFriendlyUrlAlreadyExist };
                    default:
                        return (BlogNewsViewModel)GetViewModelWithErrorMessage(blogNewsViewModel, string.IsNullOrEmpty(ex.ErrorMessage) ? Admin_Resources.UpdateError : ex.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (BlogNewsViewModel)GetViewModelWithErrorMessage(blogNewsViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get blog/news data on the basis of blog/news id and locale id.
        public virtual BlogNewsViewModel GetBlogNews(int blogNewsId, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set locale id from cookie or default locale if locale id is 0.
            if (localeId == 0)
                localeId = GetLocaleValue();

            BlogNewsModel model = (_blogNewsClient.GetBlogNews(blogNewsId, localeId, SetExpand()));

            //Maps blog/news model to blog/news view model
            BlogNewsViewModel viewModel = IsNotNull(model) ? model.ToViewModel<BlogNewsViewModel>() : new BlogNewsViewModel();

            //Bind Locale List.
            viewModel.Locale = GetLocalesList(viewModel.LocaleId);

            viewModel.BlogNewsTypeValue = (BlogNewsType)Enum.Parse(typeof(BlogNewsType), viewModel.BlogNewsType);

            //Get selected tab
            viewModel.SelectedTab = GetSelectedtabValue();
            ZnodeLogging.LogMessage("Input parameters SelectedTab:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { SelectedTab= viewModel.SelectedTab });
            CookieHelper.RemoveCookie("_selectedtab");
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //Update an existing blog/news.
        public virtual BlogNewsViewModel UpdateBlogNews(BlogNewsViewModel blogNewsViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                if (!string.IsNullOrEmpty(blogNewsViewModel.SelectedTab))
                    CookieHelper.SetCookie("_selectedtab", blogNewsViewModel.SelectedTab.ToString());

                BlogNewsModel model = _blogNewsClient.UpdateBlogNews(blogNewsViewModel.ToModel<BlogNewsModel>());
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return IsNotNull(model) ? model.ToViewModel<BlogNewsViewModel>() : new BlogNewsViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        return new BlogNewsViewModel { HasError = true, ErrorMessage = Admin_Resources.UpdateError };
                    default:
                        return (BlogNewsViewModel)GetViewModelWithErrorMessage(blogNewsViewModel, ex.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (BlogNewsViewModel)GetViewModelWithErrorMessage(blogNewsViewModel, Admin_Resources.UpdateError);
            }
        }
        //Get list of all blog/news.
        public virtual BlogNewsListViewModel GetBlogNewsList(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set sort collection for blog/news.
            if (IsNull(sortCollection))
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeBlogNewEnum.BlogNewsId.ToString(), DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, expands = expands, sorts = sortCollection });

            BlogNewsListModel blogNewsListModel = _blogNewsClient.GetBlogNewsList(filters, null, sortCollection, pageIndex, recordPerPage);

            //Maps blog/news list model to blog/news list view model.
            BlogNewsListViewModel blogNewsListViewModel = new BlogNewsListViewModel { BlogNewsList = blogNewsListModel?.BlogNewsList?.ToViewModel<BlogNewsViewModel>().ToList() };
            SetListPagingData(blogNewsListViewModel, blogNewsListModel);

            //Set tool menu for blog/news list on grid view.
            SetBlogNewsListToolMenu(blogNewsListViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return blogNewsListViewModel?.BlogNewsList?.Count > 0 ? blogNewsListViewModel : new BlogNewsListViewModel() { BlogNewsList = new List<BlogNewsViewModel>() };
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
                return localeList.Locales.Select(x => new SelectListItem { Text = x.Name, Value = x.LocaleId.ToString(), Selected = Equals(x.LocaleId, localeId) }).ToList();
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new List<SelectListItem>();
        }

        //Delete blogNews by id.
        public virtual bool DeleteBlogNews(string blogNewsIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(blogNewsIds))
            {
                try
                {
                    return _blogNewsClient.DeleteBlogNews(new ParameterModel { Ids = blogNewsIds });
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Activate/Deactivate blog(s)/news or allow/deny guest comments.
        public virtual bool ActivateDeactivateBlogNews(string blogNewsIds, bool isTrueOrFalse, string activity)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(blogNewsIds))
            {
                try
                {
                    return _blogNewsClient.ActivateDeactivateBlogNews(new BlogNewsParameterModel() { BlogNewsId = blogNewsIds, IsTrueOrFalse = isTrueOrFalse, Activity = activity });
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Get Content Page list.
        public virtual ContentPageListViewModel GetContentPageList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int? portalId, int? localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Sorting list in descending order of modified date.
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            //Set filters for content pages list.
            SetFiltersForContentPageList(filters, portalId, localeId);

            //Get Content Page List.
            ContentPageListModel contentPageListModel = _contentPageClient.GetContentPageList(filters, null, sorts, pageIndex, pageSize);
            ContentPageListViewModel contentPageListViewModel = new ContentPageListViewModel { ContentPageList = contentPageListModel?.ContentPageList?.ToViewModel<ContentPageViewModel>().ToList() };
            SetListPagingData(contentPageListViewModel, contentPageListModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return contentPageListViewModel?.ContentPageList?.Count > 0 ? contentPageListViewModel : new ContentPageListViewModel() { ContentPageList = new List<ContentPageViewModel>() };
        }
        #endregion

        #region Blog/News Comment
        //Get blog/news comment list.
        public virtual BlogNewsCommentListViewModel GetBlogNewsCommentList(int blogNewsId, FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNotNull(filters) && blogNewsId > 0)
                filters.Add(new FilterTuple(ZnodeBlogNewEnum.BlogNewsId.ToString(), FilterOperators.Equals, blogNewsId.ToString()));

            //Set sort collection for blog/news comment.
            if (IsNull(sortCollection))
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeBlogNewsCommentEnum.BlogNewsCommentId.ToString(), DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, expands = expands, sorts = sortCollection });
            BlogNewsCommentListModel blogNewsCommentListModel = _blogNewsClient.GetBlogNewsCommentList(filters, null, sortCollection, pageIndex, recordPerPage);

            //Maps blog/news comment list model to blog/news comment list view model.
            BlogNewsCommentListViewModel blogNewsCommentListViewModel = new BlogNewsCommentListViewModel { BlogNewsCommentList = blogNewsCommentListModel?.BlogNewsCommentList?.ToViewModel<BlogNewsCommentViewModel>().ToList() };

            //Binds dropdown values to list.
            List<SelectListItem> isApprovedList = GetIsApprovedDropdownList();
            ZnodeLogging.LogMessage("isApprovedList list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, isApprovedList?.Count());
            blogNewsCommentListViewModel.BlogNewsCommentList?.ForEach(item => { item.IsApprovedList = isApprovedList; item.IsApprovedValue = item.IsApproved.ToString(); });

            SetListPagingData(blogNewsCommentListViewModel, blogNewsCommentListModel);

            //Set tool menu for blog/news comment list on grid view.
            SetBlogNewsCommentListToolMenu(blogNewsCommentListViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return blogNewsCommentListViewModel?.BlogNewsCommentList?.Count > 0 ? blogNewsCommentListViewModel
                : new BlogNewsCommentListViewModel() { BlogNewsCommentList = new List<BlogNewsCommentViewModel>() };
        }

        //Update blog/news comment data.
        public virtual BlogNewsCommentViewModel UpdateBlogNewsComment(string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            BlogNewsCommentViewModel blogNewsCommentViewModel = new BlogNewsCommentViewModel()
            {
                BlogNewsCommentId = JsonConvert.DeserializeObject<BlogNewsCommentViewModel[]>(data)[0].BlogNewsCommentId,
                IsApproved = Convert.ToBoolean(JsonConvert.DeserializeObject<BlogNewsCommentViewModel[]>(data)[0].IsApproved)
            };
            try
            {
                return _blogNewsClient.UpdateBlogNewsComment(blogNewsCommentViewModel.ToModel<BlogNewsCommentModel>())?.ToViewModel<BlogNewsCommentViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (BlogNewsCommentViewModel)GetViewModelWithErrorMessage(blogNewsCommentViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete blogNews comment(s) by id.
        public virtual bool DeleteBlogNewsComment(string blogNewsCommentIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(blogNewsCommentIds))
            {
                try
                {
                    return _blogNewsClient.DeleteBlogNewsComment(new ParameterModel { Ids = blogNewsCommentIds });
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Approve/Disapprove blog/news comment(s).
        public virtual bool ApproveDisapproveBlogNewsComment(string blogNewsCommentIds, bool isApproved)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(blogNewsCommentIds))
            {
                try
                {
                    return _blogNewsClient.ApproveDisapproveBlogNewsComment(new BlogNewsParameterModel() { BlogNewsId = blogNewsCommentIds, IsTrueOrFalse = isApproved });
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region Blog/News Publish
        //Publish Blog/News.
        public virtual bool PublishBlogNews(string blogNewsId, out string errorMessage, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                //Publish the Blog/News.         
                PublishedModel result = _blogNewsClient.PublishBlogNews(new BlogNewsParameterModel { BlogNewsId = blogNewsId, LocaleId = localeId, TargetPublishState = targetPublishState, TakeFromDraftFirst = takeFromDraftFirst, IsCMSPreviewEnable = ZnodeAdminSettings.EnableCMSPreview });
                if (IsNull(result) || result?.IsPublished == false)
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

        #endregion

        #endregion

        #region Private Methods
        #region Blog/News
        //Set expand for blog/news to get data from different table. 
        private static ExpandCollection SetExpand()
        {
            //Expands to get data from another table.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeBlogNewEnum.ZnodeBlogNewsLocales.ToString());
            expands.Add(ZnodeBlogNewEnum.ZnodeBlogNewsContents.ToString());
            return expands;
        }

        //Set tool option menus for blog/news grid.
        private void SetBlogNewsListToolMenu(BlogNewsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('BlogNewsDeletePopUp')", ControllerName = "BlogNews", ActionName = "DeleteBlogNews" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "BlogNews.prototype.ActivateDeactivateBlogNews('True')", ControllerName = "BlogNews", ActionName = "ActivateDeactivateBlogNews" }); 
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "BlogNews.prototype.ActivateDeactivateBlogNews('False')", ControllerName = "BlogNews", ActionName = "ActivateDeactivateBlogNews" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextAllowGuestComment, JSFunctionName = "BlogNews.prototype.ActiveDeactiveGuestCommentsBlogNews('True')", ControllerName = "BlogNews", ActionName = "ActivateDeactivateBlogNews" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextDoesNotAllowGuestComment, JSFunctionName = "BlogNews.prototype.ActiveDeactiveGuestCommentsBlogNews('False')", ControllerName = "BlogNews", ActionName = "ActivateDeactivateBlogNews" });
            }
        }

        //Get Locale Id.
        public virtual int GetLocaleValue()
        {
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_blogNewsCulture"))
                localeId = CookieHelper.GetCookieValue<Int32>("_blogNewsCulture");
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_blogNewsCulture", Convert.ToString(localeId));
            }
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            return localeId;
        }

        //Get selected tab
        public virtual string GetSelectedtabValue()
        {
            string tabvalue = string.Empty;
            if (CookieHelper.IsCookieExists("_selectedtab"))
                tabvalue = CookieHelper.GetCookieValue<string>("_selectedtab");

            return tabvalue;
        }

        //Set filters for content pages list.
        private static void SetFiltersForContentPageList(FilterCollection filters, int? portalId, int? localeId)
        {
            filters.Add(new FilterTuple(ZnodeCMSContentPageEnum.IsActive.ToString(), FilterOperators.Contains, "1"));

            //Checking For localeId already Exists in Filters Or Not. 
            if (filters.Exists(x => x.Item1 == ZnodeCMSContentPageGroupLocaleEnum.LocaleId.ToString()) && localeId != 0)
                //If localeId already present in filters then remove it.
                filters.RemoveAll(x => x.Item1 == ZnodeCMSContentPageGroupLocaleEnum.LocaleId.ToString());
            //Add New localeId Into filters.
            if (localeId > 0)
                filters.Add(new FilterTuple(ZnodeCMSContentPageGroupLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));

            if (IsNotNull(filters) && portalId > 0)
            {
                filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
                filters.Add(new FilterTuple(ZnodeBlogNewEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            }
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { filters = filters });
        }
        #endregion

        #region Blog/News Comment
        //Get IsApproved drop down list.
        public virtual List<SelectListItem> GetIsApprovedDropdownList()
        {
            List<SelectListItem> isApprovedList = new List<SelectListItem>();
            isApprovedList.Add(new SelectListItem { Text = ZnodeConstant.True, Value = ZnodeConstant.True });
            isApprovedList.Add(new SelectListItem { Text = ZnodeConstant.False, Value = ZnodeConstant.False });
            ZnodeLogging.LogMessage("isApprovedList list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, isApprovedList?.Count());
            return isApprovedList;
        }

        //Set tool option menus for blog/news comment grid.
        private void SetBlogNewsCommentListToolMenu(BlogNewsCommentListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('BlogNewsCommentDeletePopUp')", ControllerName = "BlogNews", ActionName = "DeleteBlogNewsComment" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "BlogNews.prototype.ApproveDisapproveBlogNewsComment('True')" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "BlogNews.prototype.ApproveDisapproveBlogNewsComment('False')" });
            }
        }
        #endregion
        #endregion
    }
}
