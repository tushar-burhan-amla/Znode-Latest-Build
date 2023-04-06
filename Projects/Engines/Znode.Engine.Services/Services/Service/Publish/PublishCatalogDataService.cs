using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class PublishCatalogDataService : BaseService, IPublishCatalogDataService, IPublishProcessValidationService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishCatalogLog> _publishCatalogLogRepository;
        private readonly IZnodeRepository<ZnodeExportProcessLog> _ExportProcessLogRepository;

        #endregion Private Variables

        #region Publish Catalog Constructor

        public PublishCatalogDataService()
        {
            _publishCatalogLogRepository = new ZnodeRepository<ZnodePublishCatalogLog>();
            _ExportProcessLogRepository = new ZnodeRepository<ZnodeExportProcessLog>();
        }

        #endregion Publish Catalog Constructor

        #region Public Methods

        //Check whether any other catalog is in publish state or not
        public virtual bool IsCatalogPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isCatalogPublishInProgress = _publishCatalogLogRepository.Table.Any(x => x.IsCatalogPublished == null || x.PublishStateId == publishStateId);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isCatalogPublishInProgress;
        }

        //Perform catalog publish operation by calling master store procedure
        public virtual bool ProcessCatalogPublish(int pimCatalogId, string revisionType, string jobId, bool isDraftProductsOnly, out int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                //The custom time is used for the catalog publish master sp rather than default time out.
                int catalogPublishSPTimeOut = ZnodeApiSettings.PublishCatalogConnectionTime;

                /*Call master sp and wait to finish sp operation to perform catalog publish operation such as 
                adding data into publish table depending on revision type and pimCatalogId. Sp timeout is set different.
                NOTE : if Revisiontype : Null - only production, Preview - only preview, Production - preview & production */
                IList<View_ReturnBoolean> result;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

                objStoredProc.SetParameter("@PimCatalogId", pimCatalogId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@RevisionType", revisionType, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@NewGUID", jobId, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@IsDraftProductsOnly", isDraftProductsOnly, ParameterDirection.Input, DbType.Boolean);

                result = objStoredProc.ExecuteStoredProcedureList("Znode_PublishCatalogEntity @PimCatalogId,@RevisionType,@UserId,@NewGUID,@IsDraftProductsOnly", catalogPublishSPTimeOut);

                bool status = result.FirstOrDefault().Status.GetValueOrDefault();

                if (status)
                    publishCatalogId = result.FirstOrDefault().Id;
                else
                    publishCatalogId = 0;

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                return status;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        //Perform catalog publish operation by calling master store procedure which catalog with associated product which has draft status.
        public virtual bool ProcessCatalogPublish(int pimCatalogId, string revisionType, string jobId, out int publishCatalogId)
        {
            return ProcessCatalogPublish(pimCatalogId, revisionType, jobId, true, out publishCatalogId);
        }

        //Clean up all previous version data of current publish catalog
        public virtual void PurgePreviouslyPublishedCatalogDetails(int publishCatalogId, string jobId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                //The custom time is used for the deletion of previously published catalog data than default time out.
                int purgeCatalogDetailsSPTimeOut = ZnodeApiSettings.PurgePublishCatalogConnectionTime;
                
                //SP will perform all clean up operation for the previous versions catalog data.
                IList<View_ReturnBoolean> result;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

                objStoredProc.SetParameter("@PublishCatalogId", publishCatalogId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);

                //IsRevertPublish flag is false mean it will delete old version data
                objStoredProc.SetParameter("@IsRevertPublish", false, ParameterDirection.Input, DbType.Boolean);
                objStoredProc.SetParameter("@NewGUID", jobId, ParameterDirection.Input, DbType.String);

                result = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePublishCatalogEntity @PublishCatalogId,@UserId,@IsRevertPublish,@NewGUID", purgeCatalogDetailsSPTimeOut);

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        //Revert all inserted data of processing catalog in case of any failure
        public virtual void RevertInProgressCatalogData(int publishCatalogId, string jobId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                //The custom time is used for the deletion of current in-progress catalog data than default time out.
                int revertCatalogDetailsSPTimeOut = ZnodeApiSettings.PurgePublishCatalogConnectionTime;

                IList<View_ReturnBoolean> result;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

                /* #IsRevertPublish flag is true, mean it will revert current processing catalog version data.
                 as error occurred during some code operation.
                   #PimCatalogId is 0, mean some issue occurred in master sp execution or timeout issue and revert 
                 all current processing catalog data*/
                objStoredProc.SetParameter("@PublishCatalogId", publishCatalogId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@IsRevertPublish", true, ParameterDirection.Input, DbType.Boolean);
                objStoredProc.SetParameter("@NewGUID", jobId, ParameterDirection.Input, DbType.String);

                //SP will perform all deletion operation for the current processing catalog data.
                result = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePublishCatalogEntity @PublishCatalogId,@UserId,@IsRevertPublish,@NewGUID", revertCatalogDetailsSPTimeOut);

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        //Call update store procedure to update associated & linked products data
        public virtual void UpdatePublishedProductAssociatedData()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_PublishAssociatedProduct");
                if (!deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage("Failed to update associated products of publish products", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }
                ZnodeLogging.LogMessage("Successfully update associated products of publish products", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.GenericExceptionDuringPublish, ex.Message);
            }
        }

        //Fetch appropriate revision type(s) for elastic search based on given revision type
        public virtual List<string> GetRevisionTypesForElasticIndex(string revisionType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if(revisionType == ZnodePublishStatesEnum.PRODUCTION.ToString())
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PREVIEW.ToString(), ZnodePublishStatesEnum.PRODUCTION.ToString() };
            }
            else if(revisionType == ZnodePublishStatesEnum.PREVIEW.ToString())
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PREVIEW.ToString()};
            }
            else
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PRODUCTION.ToString() };
            }
        }

        //Check whether any other catalog is in publish state or not
        public virtual bool IsImportProcessInProgress()
        {
            ZnodeLogging.LogMessage("Execution of IsImportProcessInProgress started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            View_ReturnBoolean importStatus = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportStatusForCatalogPublish")?.FirstOrDefault();

            ZnodeLogging.LogMessage("Execution of IsImportProcessInProgress done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return importStatus.Status.Value;
        }
        //Check Export file insertion is in progress or not.
        public virtual bool IsExportPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isExportPublishInProgress = _ExportProcessLogRepository.Table.Any(x => x.Status == ZnodeConstant.ExportStatusInprogress);
          
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isExportPublishInProgress;
        }
        #endregion Public Methods
    }
}
