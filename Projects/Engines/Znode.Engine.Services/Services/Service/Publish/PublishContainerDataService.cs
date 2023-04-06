using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class PublishContainerDataService : BaseService, IPublishContainerDataService
    {
        #region Public Constructor
        public PublishContainerDataService()
        {
        }
        #endregion


        //Publish the entire content container with variants
        public PublishedModel PublishContentContainer(string containerKey, string targetPublishState)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters containerKey, targetPublishState: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { containerKey, targetPublishState });


            if (string.IsNullOrEmpty(containerKey) || string.IsNullOrWhiteSpace(containerKey))
                throw new ZnodeException(ErrorCodes.InvalidEntityPassedDuringPublish, Api_Resources.InvalidEntityMessageDuringPublish);

            ZnodeLogging.LogMessage("Publish content container with the parameters :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { containerKey, targetPublishState });

            //Call an SP to publish the content container
            bool isDataPublished = PublishContainer(containerKey, targetPublishState);

            if (isDataPublished)
            {

                ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
                {
                    Comment = $"From container publishing a message.",
                    RouteTemplateKeys = new[] { string.Concat(CachedKeys.ContainerKey_, containerKey) }
                });

                ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
                {
                    Comment = $"From container publishing a message.",
                    Key = string.Concat(CachedKeys.ContainerKey_, containerKey) 
                });

            }

            return new PublishedModel { IsPublished = isDataPublished, ErrorMessage = isDataPublished ? String.Empty : Api_Resources.GenericExceptionMessageDuringPublish };
        }

        //Publish the content container individual variants
        public PublishedModel PublishContainerVariant(string containerKey, int containerProfileVariantId, string targetPublishState)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters containerKey, containerProfileVariantId, targetPublishState: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { containerKey, targetPublishState });

            ValidateVariantBeforePublish(containerKey, containerProfileVariantId);
           
            ZnodeLogging.LogMessage("Publish content container individual Variant with the parameters :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { containerKey, containerProfileVariantId, targetPublishState });

            //Call an SP to publish the content container individual variant, 
            bool isDataPublished = PublishContainerVariantData(containerKey, containerProfileVariantId, targetPublishState);

            if (isDataPublished)
            {

                ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
                {
                    Comment = $"From container variant publishing a message.",
                    RouteTemplateKeys = new[] { CachedKeys.ContainerVariantKey_ }
                });

                ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
                {
                    Comment = $"From container variant publishing a message.",
                    Key = CachedKeys.ContainerVariantKey_
                });

            }

            return new PublishedModel { IsPublished = isDataPublished, ErrorMessage = isDataPublished ? String.Empty : Api_Resources.GenericExceptionMessageDuringPublish };
        }

        //Publish Content Container
        protected virtual bool PublishContainer(string containerKey, string targetPublishState)
        {
            ZnodeLogging.LogMessage("Single Container Publish Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (targetPublishState.Equals(ZnodeConstant.None, StringComparison.InvariantCultureIgnoreCase))
            {
                targetPublishState = ZnodePublishStatesEnum.PRODUCTION.ToString();
            }
            try
            {
                int status = 0;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

                objStoredProc.SetParameter("@PreviewVersionId", 0, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@IsPreviewEnable", 0, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@ProductionVersionId", 0, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@ContainerKey", containerKey, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
                IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_SetPublishContainerEntity @PreviewVersionId,@IsPreviewEnable,@ProductionVersionId,@RevisionState,@ContainerKey,@UserId,@Status OUT", 6, out status);

                return isDataPublished.FirstOrDefault().Status.Value;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                ZnodeLogging.LogMessage("Content Container Publish SP execution has failed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        //Publish Content Container individual Variant
        protected virtual bool PublishContainerVariantData(string containerKey, int containerProfileVariantId, string targetPublishState)
        {
            ZnodeLogging.LogMessage("Individual Container Variant Publish Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (targetPublishState.Equals(ZnodeConstant.None, StringComparison.InvariantCultureIgnoreCase))
            {
                targetPublishState = ZnodePublishStatesEnum.PRODUCTION.ToString();
            }
            try
            {
                int status = 0;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

                objStoredProc.SetParameter("@ContainerKey", containerKey, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RevisionState", targetPublishState, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@OldPreviewId", 0, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@OldProductionId", 0, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@ContainerProfileVariantId", containerProfileVariantId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
                IList<View_ReturnBoolean> isDataPublished = objStoredProc.ExecuteStoredProcedureList("Znode_PublishContentContainerVariantEntity @ContainerKey,@RevisionState,@UserId,@OldPreviewId,@OldProductionId,@ContainerProfileVariantId,@Status OUT", 6, out status);

                return isDataPublished.FirstOrDefault().Status.Value;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                ZnodeLogging.LogMessage("Individual Content Container Variant Publish SP execution has failed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        protected virtual void ValidateVariantBeforePublish(string containerKey, int containerProfileVariantId)
        {

            if (string.IsNullOrEmpty(containerKey) || string.IsNullOrWhiteSpace(containerKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorContentContainerKey);

            if (containerProfileVariantId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorContentContainerVariantId);

            IZnodeRepository<ZnodePublishContentContainerEntity> _publishContentContainerEntity = new ZnodeRepository<ZnodePublishContentContainerEntity>();
            int containerPublishId = Convert.ToInt32(_publishContentContainerEntity.Table.FirstOrDefault(x => x.ContainerKey == containerKey)?.PublishContentContainerEntityId);
            if (containerPublishId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ContainerPublishCheckForVariantPublish);
        }
    }
}
