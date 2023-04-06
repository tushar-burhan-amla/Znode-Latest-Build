using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class PublishStateService : BaseService, IPublishStateService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishStateApplicationTypeMapping> _publishStateMappingRepository;
        private readonly IZnodeRepository<ZnodePublishState> _publishStateRepository;

        #endregion Private Variables

        #region Constructor

        public PublishStateService()
        {
            _publishStateMappingRepository = new ZnodeRepository<ZnodePublishStateApplicationTypeMapping>();
            _publishStateRepository = new ZnodeRepository<ZnodePublishState>();
        }

        #endregion Constructor

        #region Public Methods

        //Returns publish state mapping list.
        public virtual PublishStateMappingListModel GetPublishStateApplicationTypeMappingList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate publishStateMappingList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<PublishStateMappingModel> objStoredProc = new ZnodeViewRepository<PublishStateMappingModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            List<PublishStateMappingModel> publishStateMappingList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishStateApplicationTypeMappings  @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("publishStateMappingList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, publishStateMappingList?.Count());
            PublishStateMappingListModel publishStateMappingListModel = new PublishStateMappingListModel() { PublishStateMappingList = publishStateMappingList };

            publishStateMappingListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return publishStateMappingListModel;
        }

        public virtual bool EnableDisableMapping(int mappingId, bool isEnabled)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter mappingId:", ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Verbose, new object[] { mappingId });
            bool result = false;

            PublishStateMappingModel publishStateMappingModel = GetPublishStateMapping(mappingId);

            if (HelperUtility.IsNotNull(publishStateMappingModel))
            {
                if (publishStateMappingModel.IsDefault)
                {
                    result = false;
                    throw new ZnodeException(ErrorCodes.NotPermitted, Api_Resources.DefaultPublishStateDisableErrorMessage);
                }
                else
                {
                    ZnodePublishStateApplicationTypeMapping publishStateMapping = _publishStateMappingRepository.GetById(mappingId);

                    if (HelperUtility.IsNotNull(publishStateMapping))
                    {
                        publishStateMapping.IsEnabled = isEnabled;
                        _publishStateMappingRepository.Update(publishStateMapping);
                        result = true;
                    }
                    else
                        result = false;
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private PublishStateMappingModel GetPublishStateMapping(int id)
        {
            ZnodeLogging.LogMessage("Input Parameter id", ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Verbose, id);
            PublishStateMappingModel model = (from publishState in _publishStateMappingRepository.Table
                                              join PS in _publishStateRepository.Table on publishState.PublishStateId equals PS.PublishStateId
                                              where publishState.IsActive && PS.IsActive && publishState.PublishStateMappingId == id
                                              select new PublishStateMappingModel
                                              {
                                                  PublishStateMappingId = publishState.PublishStateMappingId,
                                                  PublishStateId = publishState.PublishStateId,
                                                  ApplicationType = publishState.ApplicationType,
                                                  IsDefault = PS.IsDefaultContentState,
                                                  IsEnabled = PS.IsEnabled
                                              }).FirstOrDefault();

            ZnodeLogging.LogMessage("PublishStateMappingModel model: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, model);
            return model;
        }

        #endregion Private Methods
    }
}