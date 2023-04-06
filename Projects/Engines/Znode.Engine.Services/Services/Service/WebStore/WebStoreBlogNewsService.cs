using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public partial class BlogNewsService
    {
        #region Public Region
        //Get list of blogs or news to display them on webstore.
        public virtual WebStoreBlogNewsListModel GetBlogNewsListForWebstore(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int portalId, localeId;

            string blogNewsType = filters.Find(x => x.FilterName == FilterKeys.BlogNewsType)?.Item3;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);

            ZnodeLogging.LogMessage("localeId, blogNewsType and portalId generated from filters: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { localeId, blogNewsType, portalId });

            WebStoreBlogNewsListModel listModel = new WebStoreBlogNewsListModel();
            listModel.BlogNewsList = GetBlogNewsData(filters, blogNewsType);

            MapBlogNewsWithSEO(listModel.BlogNewsList, localeId, portalId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get published blog/news for displaying on webstore along with comments against it.
        public virtual WebStoreBlogNewsModel GetBlogNewsForWebstore(int blogNewsId, int localeId, int portalId, NameValueCollection expands, string activationDate = null)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters blogNewsId, localeId, portalId and activationDate: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { blogNewsId, localeId, portalId, activationDate });

            IPublishedPortalDataService publishedDataService = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>();

            ZnodePublishBlogNewsEntity blogNews = publishedDataService.GetBlogNewsData(portalId, localeId, blogNewsId, activationDate);
            WebStoreBlogNewsModel model = blogNews?.ToModel<WebStoreBlogNewsModel>();
            if (HelperUtility.IsNotNull(model))
            {
                //If content page id exist, then get the content text . Else get the new blog/news content added.
                if (model?.CMSContentPagesId > 0)
                {
                    FilterCollection filter = new FilterCollection();
                    filter.Add(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString());
                    filter.Add(FilterKeys.MappingId, FilterOperators.Equals, model.CMSContentPagesId.ToString());


                    model.BlogNewsContent = publishedDataService.GetTextWidget(filter)?.Text;

                    model.BlogNewsContent = !string.IsNullOrEmpty(model.BlogNewsContent) ? Regex.Replace(model.BlogNewsContent, "<.*?>", string.Empty) : string.Empty;
                }

                //Get the list of user comments against a blog/news.
                FilterCollection filters = new FilterCollection();
                filters.Add(ZnodeBlogNewsCommentEnum.BlogNewsId.ToString(), FilterOperators.Equals, blogNewsId.ToString());
                model.Comments = GetUserCommentList(filters, null, null, null);
                model.ActivationDate = blogNews.ActivationDate.ToString();

                MapSEODetails(localeId, portalId, model, blogNews);
                ZnodeLogging.LogMessage("WebStoreBlogNewsModel to be returned having id: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, model.BlogNewsId);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return model;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return new WebStoreBlogNewsModel();
        }

        //Create new blog or news comments.
        public virtual WebStoreBlogNewsCommentModel SaveComments(WebStoreBlogNewsCommentModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorBlogNewsCommentModelNull);

            //Get userid of logged in user.
            int? userId = GetLoginUserId();
            ZnodeLogging.LogMessage("userId returned from method GetLoginUserId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, userId);

            model.UserId = userId > 0 ? userId : null;

            model.IsApproved = true;
            //Save data into blog/news comment table.
            model.BlogNewsCommentId = _blogNewsCommentRepository.Insert(model.ToEntity<ZnodeBlogNewsComment>()).ToModel<WebStoreBlogNewsCommentModel>().BlogNewsCommentId;

            //Save data into blog/news comment locale table.
            if (model?.BlogNewsCommentId > 0)
            {
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessBlogNewsCreate,model.BlogNewsCommentId), ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

                //Insert data into blog/news comment locale entity.
                SaveBlogNewsCommentLocale(model);
                ZnodeLogging.LogMessage("WebStoreBlogNewsModel to be returned having id: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, model.BlogNewsId);
                return model;
            }

            ZnodeLogging.LogMessage(String.Format(Admin_Resources.ErrorBlogNewsCreate,model.BlogNewsCommentId), ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("WebStoreBlogNewsModel to be returned: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, model);
            return model;
        }

        //Get the list of user comments against a blog/news.
        public virtual List<WebStoreBlogNewsCommentModel> GetUserCommentList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page)
        {
            List<WebStoreBlogNewsCommentModel> listModel = BlogNewCommentData(filters);
            if (listModel?.Count > 0)
            {
                List<int?> userIds = listModel.Where(x => x.UserId != null).Select(y => y.UserId).ToList();
                IList<ZnodeUser> users = null;

                if (userIds.Count > 0)
                {
                    FilterCollection usersFilter = new FilterCollection();
                    usersFilter.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.In, string.Join(",", userIds));
                    users = _userRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(usersFilter.ToFilterDataCollection()).WhereClause);
                }

                foreach (WebStoreBlogNewsCommentModel item in listModel)
                {
                    ZnodeUser userData = users?.FirstOrDefault(x => x.UserId == item.UserId);
                    if (HelperUtility.IsNull(userData))
                        item.UserName = "Guest";
                    else
                        item.UserName = $"{userData.FirstName} {userData.LastName}";
                }
                ZnodeLogging.LogMessage("WebStoreBlogNewsCommentModel list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, listModel?.Count);
                return listModel;
            }
            return new List<WebStoreBlogNewsCommentModel>();
        }
        #endregion

        #region Private Region
        //Get blog news data .
        protected virtual List<WebStoreBlogNewsModel> GetBlogNewsData(FilterCollection filters, string type)
        {
            type = (type.ToLower() == "blog") ? "Blog" : "News";

            //Get Portal and locale id from filter.
            int portalId, localeId;
            string activationDate;

            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
            activationDate = filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.ActivationDate, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;

            List<ZnodePublishBlogNewsEntity> blogNewsData = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>().GetBlogNewsDataList(portalId, localeId, type, activationDate);
            return blogNewsData?.ToModel<WebStoreBlogNewsModel>().ToList();
        }

        //Map seo data with its blog or news.
        protected virtual void MapBlogNewsWithSEO(List<WebStoreBlogNewsModel> blogNewsList, int localeId, int portalId)
        {
            List<ZnodePublishSeoEntity> seoSettings = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>().GetSEOSettings(GetFiltersForSEO(portalId, localeId, ZnodeConstant.BlogNews));
            foreach (var blogNews in blogNewsList)
            {
                //Get blog news seo.
                ZnodePublishSeoEntity seo = seoSettings?
                            .FirstOrDefault(seoDetail => seoDetail.SEOCode == blogNews.BlogNewsCode);

                if (HelperUtility.IsNotNull(seo))
                {
                    blogNews.SEOUrl = seo.SEOUrl;
                    blogNews.SEODescription = seo.SEODescription;
                    blogNews.SEOTitle = seo.SEOTitle;
                    blogNews.SEOKeywords = seo.SEOKeywords;

                }
            }
        }



        //Insert data into blog/news comment locale table.
        protected virtual void SaveBlogNewsCommentLocale(WebStoreBlogNewsCommentModel model)
        {
            ZnodeBlogNewsCommentLocale blogNewsCommentLocale = _blogNewsCommentLocaleRepository.Insert(model?.ToEntity<ZnodeBlogNewsCommentLocale>());

            ZnodeLogging.LogMessage(blogNewsCommentLocale?.BlogNewsCommentLocaleId > 0
             ? Admin_Resources.SuccessBlogNewsCommentLocaleInsert : Admin_Resources.ErrorBlogNewsCommentLocaleInsert, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
        }

        //Get blog news comment data.
        protected virtual List<WebStoreBlogNewsCommentModel> BlogNewCommentData(FilterCollection filters)
        {
            //Get Portal Locale data.
            return (from asl in _blogNewsCommentRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause)
                    join locale in _blogNewsCommentLocaleRepository.GetEntityList(string.Empty) on asl.BlogNewsCommentId equals locale.BlogNewsCommentId
                    where asl.IsApproved == true
                    select new WebStoreBlogNewsCommentModel
                    {
                        BlogNewsCommentId = locale.BlogNewsCommentId.Value,
                        BlogNewsComment = locale.BlogComment,
                        UserId = asl.UserId,
                        IsApproved = asl.IsApproved.GetValueOrDefault(),
                        CreatedDate = asl.CreatedDate,
                    }).ToList();


        }

        //Map SEO details for a particular blog/news.
        protected virtual void MapSEODetails(int localeId, int portalId, WebStoreBlogNewsModel model, ZnodePublishBlogNewsEntity blogNews)
        {
            //Get blog news seo.

            List<ZnodePublishSeoEntity> seoSettings = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>().GetSEOSettings(GetFiltersForSEO(portalId, localeId, ZnodeConstant.BlogNews));

            ZnodePublishSeoEntity seo = seoSettings?
                        .FirstOrDefault(seoDetail => seoDetail.SEOCode == blogNews.BlogNewsCode);

            if (HelperUtility.IsNotNull(seo))
            {
                model.SEOUrl = seo.SEOUrl;
                model.SEODescription = seo.SEODescription;
                model.SEOTitle = seo.SEOTitle;
                model.SEOKeywords = seo.SEOKeywords;

            }
        }

        protected virtual FilterCollection GetFiltersForSEO(int portalId, int localeId, string seoTypeName)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple("SEOTypeName", FilterOperators.Is, seoTypeName));
            filters.Add(new FilterTuple("LocaleId", FilterOperators.Equals, localeId.ToString()));
            filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, WebstoreVersionId.ToString()));
            if (HelperUtility.IsNotNull(portalId))
                filters.Add(new FilterTuple("PortalId", FilterOperators.Equals, portalId.ToString()));
            return filters;
        }
        #endregion
    }
}
