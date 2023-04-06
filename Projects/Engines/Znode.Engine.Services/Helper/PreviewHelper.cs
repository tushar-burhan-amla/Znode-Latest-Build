using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Services
{
    public static class PreviewHelper
    {


        // Delete Publish Logs.
        public static void DeletePublishPreviewLog(int versionId)
        {
            IZnodeRepository<ZnodePublishPreviewLogEntity> _publishPreviewLogEntity = new ZnodeRepository<ZnodePublishPreviewLogEntity>(HelperMethods.Context);

            List<ZnodePublishPreviewLogEntity> logEntity = _publishPreviewLogEntity.Table.Where(x => x.VersionId == versionId)?.ToList();
            _publishPreviewLogEntity.Delete(logEntity);

        }



        /// <summary>
        /// Execute SP and get result
        /// </summary>
        /// <param name="id">Id (catalog/Product)</param>
        /// <param name="parameterName1">parameter Name</param>
        /// <param name="userId">User Id</param>
        /// <param name="spName">Name of SP</param>
        /// <param name="versionId">Version Id</param>
        /// <param name="parameterName2">Parameter name</param>
        /// <param name="associateProductTypeName">Product Type</param>
        /// <returns>Dataset</returns>
        public static DataSet ExecuteSP(int id, string parameterName1, int userId, string spName, int? versionId = null, string parameterName2 = "", string associateProductTypeName = "", List<int> LocaleIds = null, ZnodePublishStatesEnum revisionType = ZnodePublishStatesEnum.PREVIEW)
        {
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter(parameterName1, id, ParameterDirection.Input, SqlDbType.Int);

            if (versionId > 0)
                executeSpHelper.GetParameter("@VersionId", versionId, ParameterDirection.Input, SqlDbType.Int);

            executeSpHelper.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@PublishStateId", (byte)revisionType, ParameterDirection.Input, SqlDbType.Int);

            if (LocaleIds?.Count > 0)
                executeSpHelper.SetTableValueParameter("@LocaleId", PublishHelper.ConvertKeywordListToDataTable(LocaleIds), ParameterDirection.Input, SqlDbType.Structured, "dbo.TransferId");

            if (!string.IsNullOrEmpty(parameterName2) && !string.IsNullOrEmpty(associateProductTypeName))
                executeSpHelper.GetParameter(parameterName2, associateProductTypeName, ParameterDirection.Input, SqlDbType.Text);
            return executeSpHelper.GetSPResultInDataSet(spName);
        }

        /// <summary>
        /// Execute SP with Json Parameters and get result.
        /// </summary>
        /// <param name="id">Id (catalog/Product)</param>
        /// <param name="parameterName1">parameter Name</param>
        /// <param name="userId">User Id</param>
        /// <param name="spName">Name of SP</param>
        /// <param name="versionId">Version Id</param>
        /// <param name="parameterName2">Parameter name</param>
        /// <param name="associateProductTypeName">Product Type</param>
        /// <returns>Dataset</returns>
        public static DataSet ExecuteSPWithJSONParameters(int id, string parameterName1, int userId, string spName, int? versionId = null, string parameterName2 = "", string associateProductTypeName = "", List<int> LocaleIds = null, ZnodePublishStatesEnum revisionType = ZnodePublishStatesEnum.PREVIEW)
        {
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter(parameterName1, id, ParameterDirection.Input, SqlDbType.Int);

            if (versionId > 0)
                executeSpHelper.GetParameter("@VersionId", versionId, ParameterDirection.Input, SqlDbType.Int);

            executeSpHelper.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@PublishStateId", (byte)revisionType, ParameterDirection.Input, SqlDbType.Int);

            if (LocaleIds?.Count > 0)
                executeSpHelper.GetParameter("@LocaleId", PublishHelper.ConvertKeywordListToDataTable(LocaleIds)?.ToJson(), ParameterDirection.Input, SqlDbType.NVarChar);

            if (!string.IsNullOrEmpty(parameterName2) && !string.IsNullOrEmpty(associateProductTypeName))
                executeSpHelper.GetParameter(parameterName2, associateProductTypeName, ParameterDirection.Input, SqlDbType.Text);
            return executeSpHelper.GetSPResultInDataSet(spName);
        }

        //Log Progress notification
        public static void LogProgressNotification(Guid jobId, int progressMark, bool isCompleted = false, bool isFailed = false, Exception ex = null)
        {

            IZnodeRepository<ZnodePublishProgressNotifierEntity> _PublishProgressNotifier = new ZnodeRepository<ZnodePublishProgressNotifierEntity>(HelperMethods.Context);
            ZnodePublishProgressNotifierEntity progressNotifierEntity = _PublishProgressNotifier.Table.FirstOrDefault(x => x.JobId == jobId.ToString());

            if (IsNotNull(progressNotifierEntity))
            {

                progressNotifierEntity.ProgressMark = progressMark;
                progressNotifierEntity.IsCompleted = isCompleted;
                progressNotifierEntity.IsFailed = isFailed;
                progressNotifierEntity.ExceptionMessage = IsNotNull(ex) ? ex.GetBaseException().Message : null;

                _PublishProgressNotifier.Update(progressNotifierEntity);
            }

        }


    }

    public class PublishProcessor
    {
        #region Private Methods

        private List<string> EnumerateCommaSeparatedString(string commaSeparatedString)
        {
            return !string.IsNullOrEmpty(commaSeparatedString) ? commaSeparatedString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
        }

        private List<int> GetAvailableLocalesForPortal(int portalId)
        {
            return new ZnodeRepository<ZnodePortalLocale>()?.Table?.Where(x => x.PortalId == portalId)?.Select(x => x.LocaleId)?.ToList();
        }

        private List<PublishStateMappingModel> GetAvailablePublishStateMappings()
        {
            IZnodeRepository<ZnodePublishStateApplicationTypeMapping> _publishStateMappingRepository = new ZnodeRepository<ZnodePublishStateApplicationTypeMapping>();
            IZnodeRepository<ZnodePublishState> _publishStateRepository = new ZnodeRepository<ZnodePublishState>();

            List<PublishStateMappingModel> publishStateMappings = (from publishStateApplicationTypeMapping in _publishStateMappingRepository.Table
                                                                   join PS in _publishStateRepository.Table on publishStateApplicationTypeMapping.PublishStateId equals PS.PublishStateId
                                                                   where publishStateApplicationTypeMapping.IsActive
                                                                   select new PublishStateMappingModel
                                                                   {
                                                                       PublishStateMappingId = publishStateApplicationTypeMapping.PublishStateMappingId,
                                                                       ApplicationType = publishStateApplicationTypeMapping.ApplicationType,
                                                                       PublishStateCode = PS.PublishStateCode,
                                                                       Description = publishStateApplicationTypeMapping.Description,
                                                                       IsDefault = PS.IsDefaultContentState,
                                                                       IsEnabled = publishStateApplicationTypeMapping.IsEnabled,
                                                                       PublishStateId = publishStateApplicationTypeMapping.PublishStateId,
                                                                       PublishState = PS.PublishStateCode
                                                                   }).ToList();


            return publishStateMappings;
        }

        public bool IsWebstorePreviewEnabled()
        {
            return GetAvailablePublishStateMappings()?.Count(x => !x.IsDefault && x.IsEnabled) > 0;
        }

        private bool NonDefaultPublishStateExists(out ZnodePublishStatesEnum publishState)
        {
            string nonDefaultPublishState = GetAvailablePublishStateMappings()?.Where(x => !x.IsDefault && x.IsEnabled)?.FirstOrDefault()?.PublishStateCode;

            if (!string.IsNullOrEmpty(nonDefaultPublishState))
                return Enum.TryParse(nonDefaultPublishState, out publishState);

            publishState = 0;
            return false;
        }

        private ZnodePublishStatesEnum GetDefaultPublishState()
        {
            IZnodeRepository<ZnodePublishState> _publishStateRepository = new ZnodeRepository<ZnodePublishState>();
            string publishStateCode = _publishStateRepository.Table.Where(x => x.IsContentState && x.IsDefaultContentState)?.FirstOrDefault()?.PublishStateCode;

            ZnodePublishStatesEnum publishState;

            if (!string.IsNullOrEmpty(publishStateCode) && Enum.TryParse(publishStateCode, true, out publishState))
                return publishState;
            else
                return ZnodePublishStatesEnum.PRODUCTION;
        }

        #endregion Private Methods
    }
}