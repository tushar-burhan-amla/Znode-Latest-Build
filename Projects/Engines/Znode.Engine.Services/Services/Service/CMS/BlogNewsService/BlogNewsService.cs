using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Helper;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Caching.Events;
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
    public partial class BlogNewsService : BaseService, IBlogNewsService
    {
        #region Private variables              
        private readonly IZnodeRepository<ZnodeBlogNew> _blogNewsRepository;  
        private readonly IZnodeRepository<ZnodeBlogNewsContent> _blogNewsContentRepository;
        private readonly IZnodeRepository<ZnodeBlogNewsLocale> _blogNewsLocaleRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _cmsSEODetailRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetailLocale> _cmsSEODetailLocaleRepository;
        private readonly IZnodeRepository<ZnodeCMSSEOType> _cmsSEOTypeRepository;
        private readonly IZnodeRepository<ZnodeCMSContentPage> _cmsContentPagesRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodeBlogNewsComment> _blogNewsCommentRepository;
        private readonly IZnodeRepository<ZnodeBlogNewsCommentLocale> _blogNewsCommentLocaleRepository;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;


        #endregion

        #region Constructor
        public BlogNewsService()
        {
            _blogNewsRepository = new ZnodeRepository<ZnodeBlogNew>();
            _blogNewsContentRepository = new ZnodeRepository<ZnodeBlogNewsContent>();
            _blogNewsLocaleRepository = new ZnodeRepository<ZnodeBlogNewsLocale>();
            _cmsSEODetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
            _cmsSEODetailLocaleRepository = new ZnodeRepository<ZnodeCMSSEODetailLocale>();
            _cmsSEOTypeRepository = new ZnodeRepository<ZnodeCMSSEOType>();
            _cmsContentPagesRepository = new ZnodeRepository<ZnodeCMSContentPage>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _blogNewsCommentRepository = new ZnodeRepository<ZnodeBlogNewsComment>();
            _blogNewsCommentLocaleRepository = new ZnodeRepository<ZnodeBlogNewsCommentLocale>();
            _userRepository = new ZnodeRepository<ZnodeUser>();

        }
        #endregion

        #region Public Methods
        #region Blog/News
        //Create new blog or news.
        public virtual BlogNewsModel CreateBlogNews(BlogNewsModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Check whether the model is null or not.
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorBlogsNewsModelNull);

            ZnodeLogging.LogMessage("Input parameter BlogNewsModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model);

            //Checks if the blog news code already exist for current portal id.
            if (IsBlogNewsCodeAvailable(model.BlogNewsCode, model.PortalId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.AlreadyExistCode);

            //Set localeId if localeId is 0.Also set CMSTypeId and content page id.
            SaveBlogNewsModelValues(model);

            //Here we check the Current date and future date not to add past date for Blog Activation date
            DateTime currentDate = DateTime.Now;
            if (IsNotNull(model.ActivationDate) && model.ActivationDate < currentDate.Date)
            {              
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.CurrentDateMessage);
            }

            string error = string.Empty;
            //Save data into blog/news table and maps updated values in model and checks whether the url is already present or not.
            if (!IsRepeatSeoUrl(model, out error))
                model.BlogNewsId = _blogNewsRepository.Insert(model.ToEntity<ZnodeBlogNew>()).ToModel<BlogNewsModel>().BlogNewsId;

            if (!string.IsNullOrEmpty(error))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.SeoFriendlyUrlAlreadyExist);
         
            if (model.BlogNewsId > 0)
            {
                ZnodeLogging.LogMessage("BlogNewsId generated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model?.BlogNewsId);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessBlogNewsCreate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                model.SEOCode = model.BlogNewsCode;
                model.SEOId = model.BlogNewsId;
                model.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;

                //Insert data into blog/news locale entity.
                SaveBlogNewsLocaleData(model);
                SaveSeoData(model);
                return model;
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorBlogNewsUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Check if blog news code already exists for current portal id.
        private bool IsBlogNewsCodeAvailable(string blognewscode, int portalId)
         => _blogNewsRepository.Table.Any(x => x.BlogNewsCode == blognewscode.Trim() && x.PortalId == portalId);

        //Get blog news list.
        public virtual BlogNewsListModel GetBlogNewsList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get blogNewsList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<BlogNewsModel> objStoredProc = new ZnodeViewRepository<BlogNewsModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<BlogNewsModel> blogNewsList = objStoredProc.ExecuteStoredProcedureList("Znode_GetBlogNewsList  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("blogNewsList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsList?.Count);

            BlogNewsListModel listModel = new BlogNewsListModel();

            listModel.BlogNewsList = blogNewsList.ToList();

            //Set for pagination.
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get blog or news by blog/news id.
        public virtual BlogNewsModel GetBlogNews(int blogNewsId, int localeId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters blogNewsId and localeId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { blogNewsId, localeId });

            //Get expands and add them to navigation properties.
            List<string> navigationProperties = GetExpands(expands);

            //Get where clause for blog/news id.
            EntityWhereClauseModel whereClauseModel = GetWhereClauseForBlogNewsId(blogNewsId);
            ZnodeLogging.LogMessage("WhereClause generated to get blog/news entity data: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause );

            if (blogNewsId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ErrorBlogNewsIdLessThan1);

            //Get blog/news entity data & maps it into blog/news model.
            BlogNewsModel blogNewsModel = _blogNewsRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties).ToModel<BlogNewsModel>();

            blogNewsModel.LocaleId = localeId;

            //Get where clause for blog/news id & locale id.
            whereClauseModel = GetWhereClauseForLocaleId(blogNewsId, localeId);
            ZnodeLogging.LogMessage("WhereClause generated to get blog/news locale and content data: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel);

            if (IsNull(blogNewsModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorBlogsNewsModelNull);

            //Get blogNews locale data & maps it into blog/news model.
            GetBlogNewsLocaleData(blogNewsModel, whereClauseModel);

            //Get blogNews content data.
            GetBlogNewsContentData(blogNewsModel, whereClauseModel);

            blogNewsModel.SEOCode = blogNewsModel.BlogNewsCode;

            //Get data from CMSSEODetail table & CMSSEODetailLocale table.
            GetCMSSEODetailData(blogNewsModel);

            //Get store name, cms page title and media path.
            GetBlogNewsData(blogNewsModel);
            ZnodeLogging.LogMessage("blogNewsModel to be returned: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return blogNewsModel;
        }

        //Update blog or news.
        public virtual bool UpdateBlogNews(BlogNewsModel blogNewsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(blogNewsModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorBlogsNewsModelNull);

            ZnodeLogging.LogMessage("Input parameter blogNewsModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsModel);

            if (blogNewsModel.BlogNewsId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorBlogNewsIdLessThan1);

            bool isUpdated = false;
            blogNewsModel.CMSSEOTypeId = _cmsSEOTypeRepository.Table.Where(x => x.Name == Admin_Resources.BlogNews).Select(x => x.CMSSEOTypeId).FirstOrDefault();
            string error = string.Empty;

            if (IsRepeatSeoUrlOnUpdate(blogNewsModel, out error))
            {
                blogNewsModel.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;
                _blogNewsRepository.Update(blogNewsModel.ToEntity<ZnodeBlogNew>());
                isUpdated = true;
            }
            else
            {
                if (string.IsNullOrEmpty(error))
                    throw new ZnodeException(ErrorCodes.InternalItemNotUpdated, Admin_Resources.ErrorSEOURLSameName);
                isUpdated = false;
            }

            //Returns true if data updated successfully in all tables.
            if (isUpdated)
            {
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessBlogNewsUpdate, blogNewsModel.BlogNewsCode), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                blogNewsModel.SEOCode = blogNewsModel.BlogNewsCode;

                blogNewsModel.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;

                //Update the blog/news locale table.
                UpdateBlogNewsLocale(blogNewsModel);

                if (blogNewsModel.CMSSEODetailId > 0)
                {
                    UpdateCMSSEODetail(blogNewsModel);
                    UpdateCMSSEODetailLocale(blogNewsModel);
                }
                else
                    SaveSeoData(blogNewsModel);

                //Insert or update in blog/news content table.
                UpdateBlogNewsContentData(blogNewsModel);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return isUpdated;
            }
            else
            {
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.ErrorBlogNewsUpdate, blogNewsModel.BlogNewsCode), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);

                throw string.IsNullOrEmpty(error) ? new ZnodeException(ErrorCodes.InternalItemNotUpdated, Admin_Resources.UpdateErrorMessage) :
                    new ZnodeException(ErrorCodes.InternalItemNotUpdated, Admin_Resources.ErrorSEOURLSameName);
            }
        }

        //Delete blog(s) or news.
        public virtual bool DeleteBlogNews(ParameterModel blogNewsIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Check blog/news ids.
            if (string.IsNullOrEmpty(blogNewsIds?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorBlogNewsIdNull);

            ZnodeLogging.LogMessage("Input parameter blogNewsIds to delete blog(s) or news: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsIds);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

            //SP paramters.
            objStoredProc.SetParameter(ZnodeBlogNewEnum.BlogNewsId.ToString(), blogNewsIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteBlogNews @BlogNewsId,  @Status OUT", 1, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessBlogNewsDelete, blogNewsIds.Ids), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, String.Format(Admin_Resources.ErrorBlogNewsDelete, blogNewsIds.Ids));
            }
        }

        //Activate/deactivate blog(s)/news or allow/deny guest comments.
        public bool ActivateDeactivateBlogNews(BlogNewsParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            bool isBlogNewsUpdated = false;

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ErrorBlogsNewsModelNull);

            ZnodeLogging.LogMessage("Input parameter BlogNewsParameterModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model);
            if (string.IsNullOrEmpty(model.BlogNewsId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorBlogNewsIdNull);

                FilterCollection filterList = new FilterCollection();
                filterList.Add(new FilterTuple(ZnodeBlogNewEnum.BlogNewsId.ToString(), ProcedureFilterOperators.In, model.BlogNewsId));

                //Gets the where clause with filter values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get blogNewsList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause );

            List<ZnodeBlogNew> blogNewsList = _blogNewsRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
            ZnodeLogging.LogMessage("blogNewsList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsList?.Count);

            if (blogNewsList.Count > 0)
                {
                    //Update muliple records.
                    isBlogNewsUpdated = UpdateActivateDeactivateBlogNewsData(model, isBlogNewsUpdated, blogNewsList);
                }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isBlogNewsUpdated;           
        }
        #endregion

        #region Blog/News Comments
        //Get Blog/News Comment list.
        public virtual BlogNewsCommentListModel GetBlogNewsCommentList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get blogNewsCommentList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<BlogNewsCommentModel> objStoredProc = new ZnodeViewRepository<BlogNewsCommentModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<BlogNewsCommentModel> blogNewsCommentList = objStoredProc.ExecuteStoredProcedureList("Znode_GetBlogNewsCommentList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("blogNewsCommentList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsCommentList?.Count);

            BlogNewsCommentListModel listModel = new BlogNewsCommentListModel();
            listModel.BlogNewsCommentList = blogNewsCommentList.ToList();

            //Set for pagination.
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Update blog/news comment by id.
        public virtual bool UpdateBlogNewsComment(BlogNewsCommentModel blogNewsCommentModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(blogNewsCommentModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorBlogNewsCommentModelNull);

            ZnodeLogging.LogMessage("Input parameter blogNewsCommentModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsCommentModel);

            if (blogNewsCommentModel.BlogNewsCommentId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.IdCanNotBeLessThanOne);

            //Get blog/news comment data and map it to blog/news comment model.
            ZnodeBlogNewsComment blogNewsComment = _blogNewsCommentRepository.GetById(blogNewsCommentModel.BlogNewsCommentId);
            BlogNewsMap.MapBlogNewsComment(blogNewsCommentModel, blogNewsComment);

            //Update blog/news comment entity.
            if (IsNotNull(blogNewsComment))
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _blogNewsCommentRepository.Update(blogNewsCommentModel.ToEntity<ZnodeBlogNewsComment>());
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return false;
        }

        //Delete blog/news comment(s).
        public virtual bool DeleteBlogNewsComment(ParameterModel blogNewsCommentIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Check blog/news ids.
            if (string.IsNullOrEmpty(blogNewsCommentIds?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.ErrorBlogNewsIdLessThan1);

            ZnodeLogging.LogMessage("Input parameter blogNewsIds to delete blog/news comment(s): ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsCommentIds);

            //Generates filter clause for multiple blogNewsCommentIds.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeBlogNewsCommentEnum.BlogNewsCommentId.ToString(), ProcedureFilterOperators.In, blogNewsCommentIds.Ids));

            //Returns true if blog/news comment locale deleted successfully else return false.
            bool IsDeleted = _blogNewsCommentLocaleRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? String.Format(Admin_Resources.SuccessBlogNewsCommentLocaleDelete,blogNewsCommentIds.Ids) :String.Format(Admin_Resources.ErrorBlogNewsCommentLocaleDelete,blogNewsCommentIds.Ids), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Returns true if blog/news comment deleted successfully else return false.
            IsDeleted = _blogNewsCommentRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? String.Format(Admin_Resources.SuccessBlogNewsCommentLocaleDelete, blogNewsCommentIds.Ids) : String.Format(Admin_Resources.ErrorBlogNewsCommentLocaleDelete, blogNewsCommentIds.Ids), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return IsDeleted;
        }

        //Approves or disapproves blog/news comments(s).
        public virtual bool ApproveDisapproveBlogNewsComment(BlogNewsParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            bool isBlogNewsCommentUpdated = false;

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorBlogsNewsModelNull);

            ZnodeLogging.LogMessage("Input parameter BlogNewsParameterModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model);

            if (string.IsNullOrEmpty(model?.BlogNewsId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorBlogNewsIdNull);

                FilterCollection filterList = new FilterCollection();
                filterList.Add(new FilterTuple(ZnodeBlogNewsCommentEnum.BlogNewsCommentId.ToString(), ProcedureFilterOperators.In, model.BlogNewsId));

                //Gets the where clause with filter values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get blogNewsCommentList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause);

            List<ZnodeBlogNewsComment> blogNewsCommentList = _blogNewsCommentRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
            ZnodeLogging.LogMessage("blogNewsCommentList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, blogNewsCommentList?.Count);

            if (blogNewsCommentList.Count > 0)
                {
                    foreach (var item in blogNewsCommentList)
                    {
                        if (item.IsApproved != model.IsTrueOrFalse)
                        {
                            item.IsApproved = model.IsTrueOrFalse;
                            isBlogNewsCommentUpdated = _blogNewsCommentRepository.Update(item);
                        }
                        else
                            isBlogNewsCommentUpdated = true;
                    }
                }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isBlogNewsCommentUpdated;           
        }
        #endregion

        #region Publish BlogNews
        public virtual PublishedModel PublishBlogNews(int blogNewsId, int portalId, bool IsCMSPreviewEnable, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Publish blogs news with the parameters :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { blogNewsId, portalId, portalId, targetPublishState });


            if (blogNewsId < 0)
                throw new ZnodeException(ErrorCodes.InvalidEntityPassedDuringPublish, Api_Resources.InvalidEntityMessageDuringPublish);

            if (string.IsNullOrEmpty(targetPublishState) || targetPublishState.Equals("NONE", StringComparison.OrdinalIgnoreCase) || (!IsCMSPreviewEnable))
            {
                targetPublishState = "NONE";
            }
            else if (targetPublishState == ZnodePublishStatesEnum.PREVIEW.ToString() && (!IsCMSPreviewEnable))
            {
                targetPublishState = ZnodePublishStatesEnum.PRODUCTION.ToString();
            }

            bool isDataPublished = false;
            BlogNewsModel blogNewsModel = _blogNewsRepository.Table.FirstOrDefault(o => o.BlogNewsId == blogNewsId).ToModel<BlogNewsModel>();
            if (IsNotNull(blogNewsModel))
            {
                portalId = blogNewsModel.PortalId;
                isDataPublished = GetService<IPublishPortalDataService>().PublishBlogNews(blogNewsModel.BlogNewsCode, blogNewsModel.BlogNewsType, blogNewsModel.PortalId, localeId, targetPublishState, takeFromDraftFirst);
            }

            if (isDataPublished)
            {
                GetService<ICMSPageSearchService>().CreateIndexForPortalCMSPages(portalId, Convert.ToString(targetPublishState));
                ClearCacheHelper.EnqueueEviction(new PortalPublishEvent()
                {
                    Comment = $"From blog news publishing.",
                    PortalIds = new int[] { portalId }
                });
                ZnodeLogging.LogMessage("If the Cloudflare is enabled then purge the store URL manually otherwise changes will not reflect on store.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new PublishedModel { IsPublished = isDataPublished, ErrorMessage = isDataPublished ? String.Empty : Api_Resources.GenericExceptionMessageDuringPublish };
        }
        #endregion

        #endregion

        #region Private Methods
        /// <summary>
        /// Check if SEO Url already exists.
        /// </summary>
        /// <param name="model">BlogNewsModel</param>
        /// <returns>true if unique SEO Url is provided</returns>
        private bool IsRepeatSeoUrl(BlogNewsModel model, out string message)
        {
            message = string.Empty;
            if (!string.IsNullOrEmpty(model.SEOUrl))
            {
                bool result = Convert.ToBoolean((from seoDetail in _cmsSEODetailRepository.Table
                                                 where seoDetail.CMSSEOTypeId == (int)SEODetailsEnum.BlogNews
                                                 && seoDetail.CMSSEODetailId > 0 ? seoDetail.CMSSEODetailId != model.CMSSEODetailId : true
                                                 select new { seoDetail.SEOUrl, seoDetail.PortalId }
                         )?.Any(a => a.SEOUrl == model.SEOUrl.Trim() && a.PortalId == model.PortalId));

                message = result ? Admin_Resources.ErrorSEOURLSameName : message;
                return result;
            }
            return false;
        }

        private bool IsRepeatSeoUrlOnUpdate(BlogNewsModel model, out string message)
        {
            message = string.Empty;
            List<ZnodeCMSSEODetail> entityList = _cmsSEODetailRepository.Table.Where(x => x.SEOUrl != null && x.SEOUrl == model.SEOUrl.Trim() && x.PortalId == model.PortalId)?.Select(x => x)?.ToList();
            bool result = false;
            if (entityList.Any(x => x.CMSSEODetailId == model.CMSSEODetailId)) return true;
            if (entityList.Any(x => x.SEOUrl == model.SEOUrl))
            {
                message = result ? Admin_Resources.ErrorSEOURLSameName : message;
                return result;
            }
            return true;
        }

        /// <summary>
        /// Save all the data related to SEO conditionally as per the availability of the SEO Url.
        /// </summary>
        /// <param name="model">BlogNewsModel</param>
        private void SaveSeoData(BlogNewsModel model)
        {
            if (model.CMSSEOTypeId < 1)
            {
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCMSSEOTypeLessThan1);
            }
            string error = string.Empty;
            if ((IsNotNull(model.SEOTitle) || IsNotNull(model.SEOKeywords) || IsNotNull(model.SEODescription) || IsNotNull(model.SEOUrl)) && !IsRepeatSeoUrl(model, out error))
            {                
                SaveCMSSEODetailData(model);
                SaveCMSSEODetailLocaleData(model);
            }
        }

        //Sets the properties of blog/news model.
        private void SetBlogNewsModel(ZnodeBlogNew blogNewsEntity, BlogNewsModel blogNewsModel, int localeId)
        {
            //get the slider nammer locale entity.
            ZnodeBlogNewsLocale blogNewsLocale = blogNewsEntity.ZnodeBlogNewsLocales.Where(x => x.BlogNewsId == blogNewsEntity.BlogNewsId && x.LocaleId == localeId)?.FirstOrDefault();

            if (IsNull(blogNewsLocale))
            {
                //get the slider nammer locale entity.
                blogNewsLocale = blogNewsEntity.ZnodeBlogNewsLocales.Where(x => x.BlogNewsId == blogNewsEntity.BlogNewsId && x.LocaleId == GetDefaultLocaleId())?.FirstOrDefault();
            }

            //Set the properties.
            blogNewsModel.LocaleId = (int)blogNewsLocale.LocaleId;
            blogNewsModel.BlogNewsTitle = blogNewsLocale.BlogNewsTitle;
            blogNewsModel.BodyOverview = blogNewsLocale.BodyOverview;
            blogNewsModel.Tags = blogNewsLocale.Tags;

        }

        // Set localeId if localeId is 0. Also set CMSSEOTypeId and content page id.
        private void SaveBlogNewsModelValues(BlogNewsModel model)
        {
            //If LocaleId is zero then get default locale.
            if (model.LocaleId == 0)
                model.LocaleId = GetDefaultLocaleId();

            model.CMSSEOTypeId = _cmsSEOTypeRepository.Table.Where(x => x.Name == Admin_Resources.BlogNews).Select(x => x.CMSSEOTypeId).FirstOrDefault();
            model.CMSContentPagesId = model.CMSContentPagesId > 0 ? model.CMSContentPagesId : null;
            model.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;
        }
        
        // Insert data into blog/news locale table & store the updated data in model.  
        private void SaveBlogNewsLocaleData(BlogNewsModel model)
        {
            //Get locale values for blog new model.
            ZnodeBlogNewsLocale blogNewsLocale = _blogNewsLocaleRepository.Insert(model?.ToEntity<ZnodeBlogNewsLocale>());

            //Maps blog/news locale data in blogNews model.
            BlogNewsMap.MapBlogNewsLocale(model, blogNewsLocale);

            ZnodeLogging.LogMessage(model?.BlogNewsLocaleId > 0 ? Admin_Resources.SuccessBlogNewsInsertLocale : Admin_Resources.ErrorBlogNewsInsertLocale, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        //Insert blog/news data into content table & store the updated data in model.
        private void SaveBlogNewsContentData(BlogNewsModel model)
        {
            //Get default locale data for blog/news content.
            GetDefaultLocaleBlogNewsContentData(model);

            //Entry in content table.
            ZnodeBlogNewsContent blogNewsContent = _blogNewsContentRepository.Insert(model?.ToEntity<ZnodeBlogNewsContent>());

            //mapping content details in blog news model.
            BlogNewsMap.MapBlogNewsContentDetail(model, blogNewsContent);
            ZnodeLogging.LogMessage(model?.BlogNewsContentId > 0 ? String.Format(Admin_Resources.SuccessBlogNewsInsertContentDetails,model.BlogNewsCode) : String.Format(Admin_Resources.ErrorBlogNewsInsertContentDetails,model.BlogNewsCode), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        //Get default locale data for blog/news content.
        private void GetDefaultLocaleBlogNewsContentData(BlogNewsModel model)
        {
            if (model.LocaleId != GetDefaultLocaleId())
            {
                EntityWhereClauseModel whereClauseModelForDefaultLocale = GetWhereClauseForLocaleId(model.BlogNewsId, GetDefaultLocaleId());
                ZnodeLogging.LogMessage("whereClauseModel for default locale: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModelForDefaultLocale);
                ZnodeBlogNewsContent blogNewsContentEntity = _blogNewsContentRepository.GetEntity(whereClauseModelForDefaultLocale.WhereClause);

                if (IsNull(blogNewsContentEntity))
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.DefaultLocaleFirst);
            }
        }


        // Insert data into CMSSEODetail table & store the updated data in model.   
        private void SaveCMSSEODetailData(BlogNewsModel model)
        {
            //Entry in cms SEO detail table.
            ZnodeCMSSEODetail cmsSEODetail = _cmsSEODetailRepository.Insert(model?.ToEntity<ZnodeCMSSEODetail>());

            //mapping seo details in blog news model.
            BlogNewsMap.MapCMSSEODetail(model, cmsSEODetail);

            ZnodeLogging.LogMessage(model?.CMSSEODetailId > 0 ? Admin_Resources.SuccessBlogNewsInsertSEOdetails : Admin_Resources.ErrorBlogNewsInsertSEOdetails, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        // This method will insert data into CMSSEODetail locale table & store the updated data in model 
        private void SaveCMSSEODetailLocaleData(BlogNewsModel model)
        {
            //Inserting cms seo locale detail table for blog news model.
            ZnodeCMSSEODetailLocale cmsSEODetailLocale = _cmsSEODetailLocaleRepository.Insert(model?.ToEntity<ZnodeCMSSEODetailLocale>());

            //mapping cms seo locale detail in blog news model.
            BlogNewsMap.MapCMSSEODetailLocale(model, cmsSEODetailLocale);

            ZnodeLogging.LogMessage(model?.CMSSEODetailLocaleId > 0 ? Admin_Resources.SuccessBlogNewsInsertSEOdetailsLocale : Admin_Resources.ErrorBlogNewsInsertSEOdetailsLocale, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        //Get where clause for blog/news id.
        private EntityWhereClauseModel GetWhereClauseForBlogNewsId(int blogNewsId)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodeBlogNewEnum.BlogNewsId.ToString(), FilterOperators.Equals, blogNewsId.ToString());

            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
        }

        //Get where clause for blog/news id & locale id.
        private EntityWhereClauseModel GetWhereClauseForLocaleId(int blogNewsId, int localeId)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodeBlogNewEnum.BlogNewsId.ToString(), FilterOperators.Equals, blogNewsId.ToString());
            filter.Add(ZnodeBlogNewsLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
        }

        //Get where clause for cmsSEODetailId and localeId for SEO detail locale table.
        private EntityWhereClauseModel GetWhereClauseForSEO(int cmsSEODetailId, int localeId)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodeCMSSEODetailEnum.CMSSEODetailId.ToString(), FilterOperators.Equals, cmsSEODetailId.ToString());
            filter.Add(ZnodeBlogNewsLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
        }

        //Get expands and add them to navigation properties.
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodeBlogNewEnum.ZnodeBlogNewsLocales.ToString().ToLower())) SetExpands(ZnodeBlogNewEnum.ZnodeBlogNewsLocales.ToString(), navigationProperties);
                    if (Equals(key, ZnodeBlogNewEnum.ZnodeBlogNewsContents.ToString().ToLower())) SetExpands(ZnodeBlogNewEnum.ZnodeBlogNewsContents.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Get blogNews Locale data.
        private void GetBlogNewsLocaleData(BlogNewsModel blogNewsModel, EntityWhereClauseModel whereClauseModel)
        {
            //Get blog/news data from locale table.
            ZnodeBlogNewsLocale blogNewsLocale = _blogNewsLocaleRepository.GetEntity(whereClauseModel.WhereClause);

            //Get default locale data if data not present for entered locale.
            if (IsNull(blogNewsLocale))
            {
                EntityWhereClauseModel whereClauseModelForDefaultLocale = GetWhereClauseForLocaleId(blogNewsModel.BlogNewsId, GetDefaultLocaleId());
                ZnodeLogging.LogMessage("WhereClauseModel for DefaultLocale: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModelForDefaultLocale);
                blogNewsLocale = _blogNewsLocaleRepository.GetEntity(whereClauseModelForDefaultLocale?.WhereClause);
            }

            if (IsNotNull(blogNewsLocale))
                //Maps blog/news locale entity data to blog/news model.
                BlogNewsMap.MapBlogNewsLocaleToGetData(blogNewsModel, blogNewsLocale);
        }

        //Get blogNews Content data.
        private void GetBlogNewsContentData(BlogNewsModel blogNewsModel, EntityWhereClauseModel whereClauseModel)
        {      
            //Get blog/news data from content table.
            ZnodeBlogNewsContent blogNewsContent = _blogNewsContentRepository.GetEntity(whereClauseModel.WhereClause);

            //Get default locale data if data not present for entered locale.
            if (IsNull(blogNewsContent))
            {
                EntityWhereClauseModel whereClauseModelForDefaultLocale = GetWhereClauseForLocaleId(blogNewsModel.BlogNewsId, GetDefaultLocaleId());
                ZnodeLogging.LogMessage("WhereClauseModel for default locale: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModelForDefaultLocale);
                blogNewsContent = _blogNewsContentRepository.GetEntity(whereClauseModelForDefaultLocale.WhereClause);
            }

            if (IsNotNull(blogNewsContent))
                //Maps blog/news content entity data to blog/news model.
                BlogNewsMap.MapBlogNewsContentDetail(blogNewsModel, blogNewsContent);
        }

        //Get store name, cms page title and media path.
        private void GetBlogNewsData(BlogNewsModel blogNewsModel)
        {
            blogNewsModel.PageName = _cmsContentPagesRepository.Table.Where(x => x.CMSContentPagesId == blogNewsModel.CMSContentPagesId).Select(x => x.PageName).FirstOrDefault();
            blogNewsModel.StoreName = _portalRepository.Table.Where(x => x.PortalId == blogNewsModel.PortalId).Select(x => x.StoreName).FirstOrDefault();

            //Set media path for blog/news.
            if (blogNewsModel?.MediaId > 0)
            {
                MediaManagerModel mediaDetails = GetMediaPath(blogNewsModel.MediaId.GetValueOrDefault());
                if (IsNotNull(mediaDetails))
                    blogNewsModel.MediaPath = string.IsNullOrEmpty(mediaDetails.MediaServerThumbnailPath) ? string.Empty : mediaDetails.MediaServerThumbnailPath;
            }
        }

        //Get data from CMSSEODetail table & CMSSEODetailLocale table.
        private void GetCMSSEODetailData(BlogNewsModel blogNewsModel)
        {
            SEODetailsModel seoDetailModel = MapsZnodeCMSSEODetailToSEODetailModel(blogNewsModel.BlogNewsCode);

            //Get default locale data if data not present for entered locale.
            if (IsNotNull(seoDetailModel))
            {
                //Maps SEODetail model detail into blog/news model.
                BlogNewsMap.MapSEODetailsModelToBlogNewsModel(seoDetailModel, blogNewsModel);

                GetCMSSEODetailLocaleData(blogNewsModel);
            }
        }

        //Get blogNews Locale data.
        private void GetCMSSEODetailLocaleData(BlogNewsModel blogNewsModel)
        {
            //Get where clause for cmsSEODetailId and localeId for SEO detail locale table.
            EntityWhereClauseModel whereClauseModelForSEO = GetWhereClauseForSEO(blogNewsModel.CMSSEODetailId, blogNewsModel.LocaleId);
            ZnodeLogging.LogMessage("WhereClauseModel for SEO: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModelForSEO.WhereClause);
            //Get blog/news data from SEO locale table.
            ZnodeCMSSEODetailLocale cmsSEODetailLocale = _cmsSEODetailLocaleRepository.GetEntity(whereClauseModelForSEO.WhereClause);

            if (IsNull(cmsSEODetailLocale))
            {
                EntityWhereClauseModel whereClauseModelForSEODefaultLocale = GetWhereClauseForSEO(blogNewsModel.CMSSEODetailId, GetDefaultLocaleId());
                ZnodeLogging.LogMessage("WhereClauseModel for SEO default locale: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModelForSEODefaultLocale);
                cmsSEODetailLocale = _cmsSEODetailLocaleRepository.GetEntity(whereClauseModelForSEODefaultLocale.WhereClause);
            }

            if (IsNotNull(cmsSEODetailLocale))
                //Maps blog/news locale entity data to blog/news model.
                BlogNewsMap.MapCMSSEODetailLocale(blogNewsModel, cmsSEODetailLocale);
        }

        //This method is used to get media path from media Id.
        private MediaManagerModel GetMediaPath(int mediaId)
        => (mediaId > 0) ? ZnodeDependencyResolver.GetService<IMediaManagerServices>().GetMediaByID(mediaId, null) : new MediaManagerModel();

        //This method will update the blog/news locale table. 
        private void UpdateBlogNewsLocale(BlogNewsModel blogNewsModel)
        {
            //Returns true if information updated successfully.
            bool isUpdated = UpdateBlogNewsLocaleByLocaleId(blogNewsModel);

            if (isUpdated)
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessBlogNewsUpdateLocaleDetail,blogNewsModel.BlogNewsCode), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            else
            {
                throw new ZnodeException(ErrorCodes.ProcessingFailed, String.Format(Admin_Resources.ErrorBlogNewsUpdateLocaleDetail, blogNewsModel.BlogNewsCode));
            }
        }

        //Checks whether data for selected locale is present in entity or not & then perform insert or update accordingly.
        private bool UpdateBlogNewsLocaleByLocaleId(BlogNewsModel blogNewsModel)
        {
            bool isUpdated = false;
            EntityWhereClauseModel whereClauseModel = GetWhereClauseForLocaleId(blogNewsModel.BlogNewsId, blogNewsModel.LocaleId);
            ZnodeLogging.LogMessage("WhereClause generated to get blogNewsLocale: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause);

            ZnodeBlogNewsLocale blogNewsLocale = _blogNewsLocaleRepository.GetEntity(whereClauseModel.WhereClause);

            //If data not present in entity table for that locale then insert data else update data.
            if (IsNull(blogNewsLocale))
            {
                //Insert data into entity blog/news locale table.
                SaveBlogNewsLocaleData(blogNewsModel);
                isUpdated = true;
            }
            else
                isUpdated = _blogNewsLocaleRepository.Update(blogNewsModel.ToEntity<ZnodeBlogNewsLocale>());

            return isUpdated;
        }

        //This method will update the content table for blog/news. 
        private void UpdateBlogNewsContent(BlogNewsModel blogNewsModel)
        {
            bool isUpdated = UpdateBlogNewsContentByLocaleId(blogNewsModel);

            if (isUpdated)
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessBlogNewsContentDetailUpdate,blogNewsModel.BlogNewsCode), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            else
            {
                throw new ZnodeException(ErrorCodes.ProcessingFailed, String.Format(Admin_Resources.ErrorBlogNewsContentDetailUpdate, blogNewsModel.BlogNewsCode));
            }
        }

        //Checks whether data for selected locale is present in entity or not & then perform insert or update accordingly.
        private bool UpdateBlogNewsContentByLocaleId(BlogNewsModel blogNewsModel)
        {
            bool isUpdated = false;
            EntityWhereClauseModel whereClauseModel = GetWhereClauseForLocaleId(blogNewsModel.BlogNewsId, blogNewsModel.LocaleId);
            ZnodeLogging.LogMessage("WhereClause generated to get blogNewsContent: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause);
            ZnodeBlogNewsContent blogNewsContent = _blogNewsContentRepository.GetEntity(whereClauseModel.WhereClause);

            if (IsNotNull(blogNewsModel.CMSContentPagesId) && !Equals(blogNewsModel.BlogNewsContent, blogNewsContent.BlogNewsContent) && !IsNotNull(blogNewsContent.BlogNewsContentId))
                throw new ZnodeException(ErrorCodes.ProcessingFailed, Admin_Resources.ClearExistingContentPage);

            //If data not present in entity table for that locale then insert data else update data.
            if (IsNull(blogNewsContent))
            {
                //Insert data into entity blog/news content table & maps it into model.
                SaveBlogNewsContentData(blogNewsModel);
                isUpdated = true;
            }
            else
                isUpdated = _blogNewsContentRepository.Update(blogNewsModel.ToEntity<ZnodeBlogNewsContent>());

            return isUpdated;
        }

        // Update the CMSSEODetail table for blog/news & maps updated values into model.
        private void UpdateCMSSEODetail(BlogNewsModel blogNewsModel)
        {
            bool isUpdated = _cmsSEODetailRepository.Update(blogNewsModel.ToEntity<ZnodeCMSSEODetail>());

            if (isUpdated)
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessBlogNewsUpdateSEODetail,blogNewsModel.BlogNewsCode), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            else
            {
                throw new ZnodeException(ErrorCodes.ProcessingFailed, String.Format(Admin_Resources.ErrorBlogNewsUpdateSEODetail, blogNewsModel.BlogNewsCode));
            }
        }

        //This method will update the CMSSEODetailLocale table for blog/news.  
        private void UpdateCMSSEODetailLocale(BlogNewsModel blogNewsModel)
        {
            bool isUpdated = UpdateCMSSEODetailLocaleByLocaleId(blogNewsModel);

            if (isUpdated)
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessBlogNewsUpdateSEOdetailsLocale, blogNewsModel.BlogNewsCode), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            else
            {
                throw new ZnodeException(ErrorCodes.ProcessingFailed, String.Format(Admin_Resources.ErrorBlogNewsUpdateSEOdetailsLocale, blogNewsModel.BlogNewsCode));
            }
        }

        //Checks whether data for selected locale is present in entity or not & then perform insert or update accordingly.
        private bool UpdateCMSSEODetailLocaleByLocaleId(BlogNewsModel blogNewsModel)
        {
            bool isUpdated = false;

            EntityWhereClauseModel whereClauseModel = GetWhereClauseForSEO(blogNewsModel.CMSSEODetailId, blogNewsModel.LocaleId);
            ZnodeLogging.LogMessage("WhereClause generated to get cmsSEODetailLocale: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause );

            ZnodeCMSSEODetailLocale cmsSEODetailLocale = _cmsSEODetailLocaleRepository.GetEntity(whereClauseModel.WhereClause);

            //If data not present in entity table for that locale then insert data else update data.
            if (IsNull(cmsSEODetailLocale))
            {
                //Insert data into entity blog/news content table & maps it into model.
                SaveCMSSEODetailLocaleData(blogNewsModel);
                isUpdated = true;
            }
            else
                isUpdated = _cmsSEODetailLocaleRepository.Update(blogNewsModel.ToEntity<ZnodeCMSSEODetailLocale>());

            return isUpdated;
        }

        //Maps entity data into SEO Detail model.
        private SEODetailsModel MapsZnodeCMSSEODetailToSEODetailModel(string blogNewsCode)
        {
            //Get SEO Details for blog/news from CMSSEODetailEntity.
            return (from cmsSEODetail in _cmsSEODetailRepository.Table
                    join cmsSEODetailType in _cmsSEOTypeRepository.Table on cmsSEODetail.CMSSEOTypeId equals cmsSEODetailType.CMSSEOTypeId
                    where cmsSEODetailType.Name == Admin_Resources.BlogNews && cmsSEODetail.SEOCode == blogNewsCode
                    select new SEODetailsModel
                    {
                        CMSSEODetailId = cmsSEODetail.CMSSEODetailId,
                        CMSSEOTypeId = cmsSEODetail.CMSSEOTypeId,
                        SEOCode = blogNewsCode,
                        SEOUrl = cmsSEODetail.SEOUrl,
                    }).FirstOrDefault();
        }

        //Check whether to insert data in blog/news content table or not.
        private void UpdateBlogNewsContentData(BlogNewsModel blogNewsModel)
        {
            //Check whether existing content page and new content is inserted simultaneously.
            if (IsNotNull(blogNewsModel.CMSContentPagesId) && IsNotNull(blogNewsModel.BlogNewsContent) && IsNull(blogNewsModel.BlogNewsContentId) && IsNull(blogNewsModel.PageName))
                throw new ZnodeException(ErrorCodes.ProcessingFailed, Admin_Resources.ClearExistingContentPage);

            //Insert or update in blog/news content table.
            CreateEditBlogNewsContent(blogNewsModel);
        }

        //Insert or update in blog/news content table.
        private void CreateEditBlogNewsContent(BlogNewsModel blogNewsModel)
        {
            if (IsNull(blogNewsModel.BlogNewsContentId))
            {
                if (IsNotNull(blogNewsModel.BlogNewsContent))
                    //Insert blog/news data into content table & store the updated data in model.
                    SaveBlogNewsContentData(blogNewsModel);
            }
            else
                //Update the BlogNewsContent table for blog/news & maps updated values into model.
                UpdateBlogNewsContent(blogNewsModel);
        }

        //Update multiple Activate/Deactivate blog/news or allow guest comment in blog/news.
        private bool UpdateActivateDeactivateBlogNewsData(BlogNewsParameterModel model, bool isBlogNewsUpdated, List<ZnodeBlogNew> blogNewsList)
        {
            foreach (var item in blogNewsList)
            {
                if (HelperUtility.IsNotNull(model))
                {
                    if (string.Equals(model.Activity, Admin_Resources.TextIsActive, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (item.IsBlogNewsActive != model.IsTrueOrFalse)
                        {
                            item.IsBlogNewsActive = model.IsTrueOrFalse;
                            item.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;
                            isBlogNewsUpdated = _blogNewsRepository.Update(item);
                        }
                        else
                            isBlogNewsUpdated = true;
                    }
                    else
                    {
                        if (item.IsAllowGuestComment != model.IsTrueOrFalse)
                        {
                            item.IsAllowGuestComment = model.IsTrueOrFalse;
                            item.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;
                            isBlogNewsUpdated = _blogNewsRepository.Update(item); 
                        }
                        else
                            isBlogNewsUpdated = true;
                    }
                }
            }
            return isBlogNewsUpdated;
        }
        #endregion
    }
}

