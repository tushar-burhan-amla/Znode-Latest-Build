using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class BlogNewsMap
    {
        #region Blog/News
        //This will map ZnodeBlogNewsLocale entity data into blog/news model.
        public static void MapBlogNewsLocale(BlogNewsModel model, ZnodeBlogNewsLocale blogNewsLocaleEntity)
        {
            if (IsNotNull(blogNewsLocaleEntity))
            {
                model.BlogNewsLocaleId = blogNewsLocaleEntity.BlogNewsLocaleId;
                model.BlogNewsTitle = blogNewsLocaleEntity.BlogNewsTitle;
                model.BodyOverview = blogNewsLocaleEntity.BodyOverview;
                model.LocaleId = blogNewsLocaleEntity.LocaleId.Value;
                model.Tags = blogNewsLocaleEntity.Tags;
            }
        }

        //This will map ZnodeCMSSEODetail entity data into blog/news model.
        public static void MapCMSSEODetail(BlogNewsModel model, ZnodeCMSSEODetail cmsSEODetailEntity)
        {
            if (IsNotNull(cmsSEODetailEntity))
            {
                model.CMSSEODetailId = cmsSEODetailEntity.CMSSEODetailId;
                model.CMSSEOTypeId = cmsSEODetailEntity.CMSSEOTypeId;
                model.SEOId = cmsSEODetailEntity.SEOId;
                model.SEOUrl = cmsSEODetailEntity.SEOUrl;
            }
        }

        //This will map ZnodeCMSSEODetailLocale entity data into blog/news model.
        public static void MapCMSSEODetailLocale(BlogNewsModel model, ZnodeCMSSEODetailLocale cmsSEODetailLocaleEntity)
        {
            if (IsNotNull(cmsSEODetailLocaleEntity))
            {
                model.CMSSEODetailLocaleId = cmsSEODetailLocaleEntity.CMSSEODetailLocaleId;
                model.SEODescription = cmsSEODetailLocaleEntity.SEODescription;
                model.SEOKeywords = cmsSEODetailLocaleEntity.SEOKeywords;
                model.SEOTitle = cmsSEODetailLocaleEntity.SEOTitle;
            }
        }

        //This method will map SEODetail model detail into blog/news model.
        public static void MapSEODetailsModelToBlogNewsModel(SEODetailsModel seoDetailModel, BlogNewsModel blogNewsModel)
        {
            blogNewsModel.CMSSEODetailId = seoDetailModel.CMSSEODetailId;
            blogNewsModel.CMSSEOTypeId = seoDetailModel.CMSSEOTypeId;
            blogNewsModel.SEOCode = seoDetailModel.SEOCode;
            blogNewsModel.SEOUrl = seoDetailModel.SEOUrl;
        }

        //This will map ZnodeBlogNewsLocale entity data into blog/news model.
        public static void MapBlogNewsLocaleToGetData(BlogNewsModel model, ZnodeBlogNewsLocale blogNewsLocaleEntity)
        {
            if (IsNotNull(blogNewsLocaleEntity))
            {
                model.BlogNewsLocaleId = blogNewsLocaleEntity.BlogNewsLocaleId;
                model.BlogNewsTitle = blogNewsLocaleEntity.BlogNewsTitle;
                model.BodyOverview = blogNewsLocaleEntity.BodyOverview;
                model.Tags = blogNewsLocaleEntity.Tags;
            }
        }

        //This method will map blogNewsContent data into blognews model.
        public static void MapBlogNewsContentDetail(BlogNewsModel model, ZnodeBlogNewsContent blogNewsContentEntity)
        {
            if (IsNotNull(blogNewsContentEntity))
            {
                model.BlogNewsContentId = blogNewsContentEntity.BlogNewsContentId;
                model.BlogNewsContent = blogNewsContentEntity.BlogNewsContent;
            }
        }
        #endregion

        #region Blog/News Comments
        //This method will map blogNewsComment data into blog/news comment model.
        public static void MapBlogNewsComment(BlogNewsCommentModel model, ZnodeBlogNewsComment blogNewsCommentEntity)
        {
            if (IsNotNull(blogNewsCommentEntity))
            {
                model.BlogNewsCommentId = blogNewsCommentEntity.BlogNewsCommentId;
                model.BlogNewsId = blogNewsCommentEntity.BlogNewsId.GetValueOrDefault();
                model.UserId = blogNewsCommentEntity.UserId;
                model.CreatedDate = blogNewsCommentEntity.CreatedDate;
                model.ModifiedDate = blogNewsCommentEntity.ModifiedDate;
            }
        }
        #endregion
    }
}
