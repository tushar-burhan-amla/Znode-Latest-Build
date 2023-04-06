using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class PublishCategoryDataService : BaseService, IPublishCategoryDataService, IPublishProcessValidationService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishCatalogLog> _publishCatalogLogRepository;
        private readonly IZnodeRepository<ZnodeExportProcessLog> _znodeExportProcessLog;

        #endregion Private Variables

        #region Constructor

        public PublishCategoryDataService()
        {
            _publishCatalogLogRepository = new ZnodeRepository<ZnodePublishCatalogLog>();
            _znodeExportProcessLog = new ZnodeRepository<ZnodeExportProcessLog>();
        }

        #endregion Constructor

        #region Public Methods

        //Check whether any catalog is in publish state or not
        public virtual bool IsCatalogPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isCatalogPublishInProgress = _publishCatalogLogRepository.Table.Any(x => x.PublishStateId == publishStateId);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isCatalogPublishInProgress;
        }

        //Perform single category publish operation by calling master store procedure
        public virtual DataSet ProcessSingleCategoryPublish(int pimCategoryId, int pimCatalogId, string revisionType, out bool status, out int isAssociate)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                /*Call master sp and wait to finish sp operation to perform single category publish operation such as 
                adding data into publish table depending on revision type and category code.
                NOTE : if Revisiontype : Null - only production, Preview - only preview, Production - preview & production 
                if sp return IsAssociate = 0 - category is not associated to any catalog */

                DataSet resultDataSet = null;
                status = false;
                isAssociate  = 0;

                ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();

                executeSpHelper.GetParameter("@PimCategoryId", pimCategoryId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@RevisionType", revisionType, ParameterDirection.Input, SqlDbType.NVarChar);
                executeSpHelper.GetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@IsAssociate", isAssociate, ParameterDirection.Output, SqlDbType.Int);
                //If category publish from catalog manage
                if (pimCatalogId > 0)
                    executeSpHelper.GetParameter("@PimCatalogId", pimCatalogId, ParameterDirection.Input, SqlDbType.Int);

                resultDataSet = executeSpHelper.GetSPResultInDataSet("Znode_PublishSingleCategoryEntity", 3, out isAssociate);

                if (resultDataSet?.Tables?.Count > 1  && resultDataSet?.Tables[2]?.Rows?.Count > 0)
                {
                    status = resultDataSet.Tables[2].AsEnumerable().Select(dataRow => (bool)dataRow.Field<object>("Status")).FirstOrDefault();
                }

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                return resultDataSet;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        public virtual bool IsExportPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isExportPublishInProgress = _znodeExportProcessLog.Table.Any(x => x.Status == ZnodeConstant.ExportStatusInprogress || x.Status == ZnodeConstant.SearchIndexStartedStatus);


            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isExportPublishInProgress;
        }
        #endregion endregion
    }
}
