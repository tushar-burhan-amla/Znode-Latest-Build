using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public class PublishPortalDataService : BaseService, IPublishPortalDataService
    {

        #region Publish Portal
        //Publish Portal
        public virtual bool PublishPortal(int portalId, Guid jobId, string targetPublishState = null, string publishContent = null, bool takeFromDraftFirst = false)
        {
            bool publishCMSContent = publishContent.Contains(ZnodePublishContentTypeEnum.CmsContent.ToString());

            int portalPublishSPTimeOut = ZnodeApiSettings.PublishPortalConnectionTime;

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Boolean);
            objStoredProc.SetParameter("@IsContentType", publishCMSContent, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@NewGUID", jobId.ToString(), ParameterDirection.Input, DbType.String);

            IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_PublishPortalEntity @PortalId,@LocaleId,@RevisionState,@UserId,@Status OUT,@IsContentType,@NewGUID", portalPublishSPTimeOut, 4, out status);
            return isDataPublished.FirstOrDefault().Status.Value;

        }
        #endregion

        #region Publish Content Page
        //Publish Content Page
        public virtual bool PublishContentPage(int contentPageId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@CMSContentPagesId", contentPageId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_PublishContentPageEntity @PortalId,@LocaleId,@RevisionState,@CMSContentPagesId,@UserId,@Status OUT", 5, out status);

            return isDataPublished.FirstOrDefault().Status.Value;

        }
        #endregion

        #region Publish Slider
        //Publish Slider
        public virtual bool PublishSlider(string cmsPortalSliderId, int portalId, int localeId, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PreviewVersionId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsPreviewEnable", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProductionVersionId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@CMSContentPagesId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@CMSSliderId", cmsPortalSliderId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_SetPublishWidgetSliderBannerEntity @PortalId,@LocaleId,@PreviewVersionId,@IsPreviewEnable,@ProductionVersionId,@RevisionState,@CMSContentPagesId,@CMSSliderId,@UserId,@Status OUT", 9, out status);

            return isDataPublished.FirstOrDefault().Status.Value;
        }
        #endregion

        #region Publish Manage Message
        //Publish Content Block
        public virtual bool PublishManageMessage(string cmsMessageKeyId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PreviewVersionId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsPreviewEnable", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProductionVersionId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@CMSMessageKeyId", cmsMessageKeyId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_SetPublishMessageEntity @PortalId,@LocaleId,@PreviewVersionId,@IsPreviewEnable,@ProductionVersionId,@RevisionState,@CMSMessageKeyId,@UserId,@Status OUT", 8, out status);

            return isDataPublished.FirstOrDefault().Status.Value;

        }
        #endregion

        #region PublishSEO
        //Publish SEO
        public virtual bool PublishSEO(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@CMSSEOTypeId", seoTypeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@CMSSEOCode", seoCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Boolean);

            IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_SetPublishSEOCodeEntity @PortalId,@LocaleId,@RevisionState,@CMSSEOTypeId,@CMSSEOCode,@UserId,@Status OUT", 6, out status);

            return isDataPublished.FirstOrDefault().Status.Value;

        }

        #endregion

        #region Publish Blog/News
        //Publish Blog/News
        public virtual bool PublishBlogNews(string blogNewsCode, string blogNewsType, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@BlogNewsCode", blogNewsCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@BlogNewsType", blogNewsType, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);

            IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_PublishBlogNewsEntity @PortalId,@LocaleId,@RevisionState,@BlogNewsCode,@BlogNewsType,@UserId,@Status OUT", 6, out status);

            return isDataPublished.FirstOrDefault().Status.Value;
        }
        #endregion
    }
}
