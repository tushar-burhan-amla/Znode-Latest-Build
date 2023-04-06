using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class BlogNewsController : BaseController
    {
        #region Private Read only members
        private readonly IBlogNewsAgent _blogNewsAgent;
        private readonly IStoreAgent _storeAgent;
        #endregion

        #region Public Constructor
        public BlogNewsController(IBlogNewsAgent blogNewsAgent, IStoreAgent storeAgent)
        {
            _blogNewsAgent = blogNewsAgent;
            _storeAgent = storeAgent;
        }
        #endregion

        #region Public Methods
        #region Blog/News
        //Create a new blog/news.
        [HttpGet]
        public virtual ActionResult AddBlogNews()
         => ActionView(AdminConstants.CreateEdit, new BlogNewsViewModel { Locale = _blogNewsAgent.GetLocalesList(), LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale) });

        //Method to add a new blog/news, having parameter BlogNewsViewModel.
        [HttpPost]
        public virtual ActionResult AddBlogNews(BlogNewsViewModel blogNewsViewModel)
        {
            string storeName = blogNewsViewModel?.StoreName;
            if (ModelState.IsValid)
            {
                //Create blog/news.
                blogNewsViewModel = _blogNewsAgent.CreateBlogNews(blogNewsViewModel);

                //Based on the HasError property sets success or failure message.
                if (!blogNewsViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SaveMessage));
                    return RedirectToAction<BlogNewsController>(x => x.EditBlogNews(blogNewsViewModel.BlogNewsId, blogNewsViewModel.LocaleId));
                }
            }

            SetNotificationMessage(GetErrorNotificationMessage(blogNewsViewModel.ErrorMessage));
            blogNewsViewModel.StoreName = storeName;
            blogNewsViewModel.Locale = _blogNewsAgent.GetLocalesList();
            return ActionView(AdminConstants.CreateEdit, blogNewsViewModel);
        }

        //Method to edit blog or news.
        public virtual ActionResult EditBlogNews(int blogNewsId, int localeId = 0)
        {
            Response.Cookies["_selectedtab"].Expires = DateTime.Now.AddDays(-1);
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(Request.IsAjaxRequest() ? "_BlogNewsForLocale" : AdminConstants.CreateEdit, _blogNewsAgent.GetBlogNews(blogNewsId, localeId));
        }

        //Post:update an existing blog/news.
        [HttpPost]
        public virtual ActionResult EditBlogNews(BlogNewsViewModel blogNewsViewModel)
        {
            if (IsNotNull(blogNewsViewModel) && ModelState.IsValid)
            {
                blogNewsViewModel = _blogNewsAgent.UpdateBlogNews(blogNewsViewModel);
                SetNotificationMessage(!(blogNewsViewModel.HasError)
                ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                 : GetErrorNotificationMessage(blogNewsViewModel.ErrorMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateError));
            return RedirectToAction<BlogNewsController>(x => x.EditBlogNews(blogNewsViewModel.BlogNewsId, blogNewsViewModel.LocaleId));
        }

        //Method to get blog/news list.
        public virtual ActionResult BlogNewsList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeBlogNewsList.ToString(), model);
            //Get blog/news list.
            BlogNewsListViewModel blogNewsList = _blogNewsAgent.GetBlogNewsList(model.Filters, model.SortCollection, model.Expands, model.Page, model.RecordPerPage);

            //Get the grid model.
            blogNewsList.GridModel = FilterHelpers.GetDynamicGridModel(model, blogNewsList?.BlogNewsList, GridListType.ZnodeBlogNewsList.ToString(), string.Empty, null, true, true, blogNewsList?.GridModel?.FilterColumn?.ToolMenuList);
            blogNewsList.GridModel.TotalRecordCount = blogNewsList.TotalResults;

            //Returns the blog/news list.
            return ActionView(blogNewsList);
        }

        //Delete blog/news by id.
        public virtual JsonResult DeleteBlogNews(string blogNewsId)
        {
            if (!string.IsNullOrEmpty(blogNewsId))
            {
                bool status = _blogNewsAgent.DeleteBlogNews(blogNewsId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Activate/Deactivate blog(s)/news or allow/deny guest comment(s).
        public virtual ActionResult ActivateDeactivateBlogNews(string blogNewsIds, bool isTrueOrFalse, string activity)
        {
            if (!string.IsNullOrEmpty(blogNewsIds))
            {
                bool status = _blogNewsAgent.ActivateDeactivateBlogNews(blogNewsIds, isTrueOrFalse, activity);
                return Json(new { status = status, message = status ? (status && isTrueOrFalse) ? Admin_Resources.SuccessMessageStatusActive : Admin_Resources.SuccessMessageStatusInactive : Admin_Resources.ErrorMessageFailedStatus }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false, message = Admin_Resources.ErrorMessageFailedStatus }, JsonRequestBehavior.AllowGet);
        }

        //Get Portal List.
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);
            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true, true, new List<ToolMenuModel>());

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView("_asideStoreListPanelPopup", storeList);
        }

        //Get Content Page List
        public virtual ActionResult GetContentPageList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0, int localeId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSContentPageList.ToString(), model);
            ContentPageListViewModel contentPageList = _blogNewsAgent.GetContentPageList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, localeId);

            //Get the grid model.
            contentPageList.GridModel = FilterHelpers.GetDynamicGridModel(model, contentPageList.ContentPageList, GridListType.ZnodeCMSContentPageList.ToString(), string.Empty, null, true, true, new List<ToolMenuModel>());

            //Set the total record count
            contentPageList.GridModel.TotalRecordCount = contentPageList.TotalResults;

            return ActionView("_asideContentPageListPanelPopup", contentPageList);
        }
        #endregion

        #region Blog/News Comments
        //Method to get blog/news comment list.
        public virtual ActionResult BlogNewsCommentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int blogNewsId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeBlogNewsCommentList.ToString(), model);
            //Get blog/news comment list.
            BlogNewsCommentListViewModel blogNewsCommentList = _blogNewsAgent.GetBlogNewsCommentList(blogNewsId, model.Filters, model.SortCollection, model.Expands, model.Page, model.RecordPerPage);
            blogNewsCommentList.BlogNewsId = blogNewsId;

            //Get the grid model.
            blogNewsCommentList.GridModel = FilterHelpers.GetDynamicGridModel(model, blogNewsCommentList?.BlogNewsCommentList, GridListType.ZnodeBlogNewsCommentList.ToString(), string.Empty, null, true, true, blogNewsCommentList?.GridModel?.FilterColumn?.ToolMenuList);
            blogNewsCommentList.GridModel.TotalRecordCount = blogNewsCommentList.TotalResults;

            //Returns the blog/news comment list.
            return ActionView(blogNewsCommentList);
        }

        //Method to edit blog/news comment.
        public virtual ActionResult EditBlogNewsComment(string data)
        {
            string message = string.Empty;
            if (ModelState.IsValid && IsNotNull(data))
            {
                BlogNewsCommentViewModel blogNewsCommentViewModel = _blogNewsAgent.UpdateBlogNewsComment(data);
                if (!blogNewsCommentViewModel.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = blogNewsCommentViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete blog/news comment(s) by id.
        public virtual JsonResult DeleteBlogNewsComment(string blogNewsCommentId)
        {
            if (!string.IsNullOrEmpty(blogNewsCommentId))
            {
                bool status = _blogNewsAgent.DeleteBlogNewsComment(blogNewsCommentId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Approve/Disapprove blog(s)/news comment(s).
        public virtual ActionResult ApproveDisapproveBlogNewsComment(string blogNewsCommentIds, bool isApproved)
        {
            if (!string.IsNullOrEmpty(blogNewsCommentIds))
            {
                bool status = _blogNewsAgent.ApproveDisapproveBlogNewsComment(blogNewsCommentIds, isApproved);
                return Json(new { status = status, message = status ? (status && isApproved) ? Admin_Resources.SuccessMessageStatusActive : Admin_Resources.SuccessMessageStatusInactive : Admin_Resources.ErrorMessageFailedStatus }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false, message = Admin_Resources.ErrorMessageFailedStatus }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Blog/News Publish
        [HttpPost]
        //Publish BlogNews, Based on Locale.
        public virtual ActionResult PublishBlogNewsPage(string blogNewsId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            if (!string.IsNullOrEmpty(blogNewsId))
            {
                string errorMessage = string.Empty;
                bool status = _blogNewsAgent.PublishBlogNews(blogNewsId, out errorMessage, localeId, targetPublishState, takeFromDraftFirst);
                return Json(new { status = status, message = status ? (errorMessage == String.Empty ? Admin_Resources.TextPublishedSuccessfully : errorMessage) : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, message = Admin_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
            }
        }
        //Post: Update and Publish BlogNews.
        [HttpPost]
        public virtual ActionResult UpdateAndPublishBlogNews(BlogNewsViewModel blogNewsViewModel)
        {
            string errorMessage = string.Empty;
            bool status = false;
            string targetPublishState = blogNewsViewModel.TargetPublishState;
            bool takeFromDraftFirst = blogNewsViewModel.TakeFromDraftFirst;
         
            //update blog news page details.
            blogNewsViewModel = _blogNewsAgent.UpdateBlogNews(blogNewsViewModel);

            if (!blogNewsViewModel.HasError)
            {
                //publish blog news page details.
                status = _blogNewsAgent.PublishBlogNews(blogNewsViewModel.BlogNewsId.ToString(), out errorMessage, blogNewsViewModel.LocaleId, targetPublishState, takeFromDraftFirst);
                //setting error message based on returned status
                errorMessage = status ? (errorMessage == String.Empty ? Admin_Resources.TextPublishedSuccessfully : errorMessage) : errorMessage;
                //setting notification message.
                SetNotificationMessage(status ? GetSuccessNotificationMessage(errorMessage) : GetErrorNotificationMessage(errorMessage));
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorPublished));
            }
            return RedirectToAction<BlogNewsController>(x => x.EditBlogNews(blogNewsViewModel.BlogNewsId, blogNewsViewModel.LocaleId));
        }
        #endregion

        #endregion
    }
}
