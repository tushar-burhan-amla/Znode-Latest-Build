using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.ElasticSearch;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class MaintenanceService : BaseService, IMaintenanceService
    {
        #region Protected variables

        protected readonly IZnodeRepository<ZnodeCatalogIndex> _catalogIndexRepository;
        protected readonly IZnodeRepository<ZnodeCMSSearchIndex> _cmsSearchIndexRepository;

        #endregion Protected variables

        #region Constructor

        public MaintenanceService()
        {
            _catalogIndexRepository = new ZnodeRepository<ZnodeCatalogIndex>();
            _cmsSearchIndexRepository = new ZnodeRepository<ZnodeCMSSearchIndex>();
        }

        #endregion Constructor

        #region Public Methods

        //To delete published data & elastic index of all catalog & store.
        public virtual bool PurgeAllPublishedData()
        {
            ZnodeLogging.LogMessage("Execution started.", "Maintenance", TraceLevel.Info);
            ZnodeLogging.LogMessage($"Clear Data start process time {DateTime.UtcNow.TimeOfDay}", "Maintenance", TraceLevel.Info);
            try
            {
                //Perform the deletion of all catalog indices. #Step 1
                DeleteAllCatalogElasticIndex();

                //Perform the deletion of all portal cms indices. #Step 2
                DeleteAllCMSElasticIndex();

                //Perform the deletion of all published data of catalogs and stores. #Step 3
                bool status = PurgeAllPublishEntityData();

                if (status)
                    ZnodeLogging.LogMessage("Published data has been deleted successfully", "Maintenance", TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage("Published data deletion has been failed", "Maintenance", TraceLevel.Error);

                ClearCacheHelper.EnqueueEviction(new ManuallyClearAllPublishedDataEvent()
                {
                    Comment = $"From clicking CLEAR DATA under Maintenance page."
                });
                ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);
                ZnodeLogging.LogMessage($"Clear Data end process time {DateTime.UtcNow.TimeOfDay}", "Maintenance", TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Maintenance", TraceLevel.Error);
                throw ex;
            }
        }

        //Perform the deletion of all published data of catalogs and stores.
        protected virtual bool PurgeAllPublishEntityData()
        {
            ZnodeLogging.LogMessage("Execution started.", "Maintenance", TraceLevel.Info);

            try
            {
                //The custom time is used for the deletion of publish entity data than default time out.
                int purgePublishedDataSPTimeOut = ZnodeApiSettings.PurgePublishCatalogConnectionTime;

                IList<View_ReturnBoolean> result;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

                objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PimCatalogId", 0, ParameterDirection.Input, DbType.Int32);

                result = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteAllPublishedData @UserId,@PimCatalogId", purgePublishedDataSPTimeOut);

                bool status = result.FirstOrDefault().Status.GetValueOrDefault();

                ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);

                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Maintenance", TraceLevel.Error);
                throw ex;
            }
        }

        //Perform the deletion of all catalog indices.
        protected virtual void DeleteAllCatalogElasticIndex()
        {
            ZnodeLogging.LogMessage("Execution started.", "Maintenance", TraceLevel.Info);

            List<string> indexName = GetAllCatalogIndexName();

            GetService<IDefaultDataService>().DeleteElasticSearchIndex(indexName);

            ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);
        }

        //Perform the deletion of all portal cms indices.
        protected virtual void DeleteAllCMSElasticIndex()
        {
            ZnodeLogging.LogMessage("Execution started.", "Maintenance", TraceLevel.Info);

            List<string> indexName = GetAllCMSIndexName();

            GetService<IDefaultDataService>().DeleteElasticSearchIndex(indexName);

            ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);
        }

        #endregion Public Methods

        #region Private Methods

        //Get all catalog index name.
        private List<string> GetAllCatalogIndexName()
        {
            ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);

            List<string> catalogIndexNameList = (from index in _catalogIndexRepository.Table
                                                 select index.IndexName)?.ToList();

            ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);

            return catalogIndexNameList != null ? catalogIndexNameList : new List<string>();

        }

        //Get all portal cms index name
        private List<string> GetAllCMSIndexName()
        {
            ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);

            List<string> cmsSearchIndexName = (from index in _cmsSearchIndexRepository.Table
                                               select index.IndexName)?.ToList();

            ZnodeLogging.LogMessage("Execution done.", "Maintenance", TraceLevel.Info);

            return cmsSearchIndexName != null ? cmsSearchIndexName : new List<string>();
        }

        #endregion Private Methods
    }
}
