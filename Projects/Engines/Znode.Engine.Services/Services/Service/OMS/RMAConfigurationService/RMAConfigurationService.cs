using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class RMAConfigurationService : BaseService, IRMAConfigurationService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeRmaConfiguration> _rmaConfigurationRepository;
        private readonly IZnodeRepository<ZnodeRmaReasonForReturn> _reasonForReturnRepository;
        private readonly IZnodeRepository<ZnodeRmaRequestStatu> _requestStatusRepository;

        #endregion

        #region Public Constructor
        public RMAConfigurationService()
        {
            _rmaConfigurationRepository = new ZnodeRepository<ZnodeRmaConfiguration>();
            _reasonForReturnRepository = new ZnodeRepository<ZnodeRmaReasonForReturn>();
            _requestStatusRepository = new ZnodeRepository<ZnodeRmaRequestStatu>();
        }
        #endregion

        #region RMA configuration
        //Create/Update RMA Configuration.
        public virtual RMAConfigurationModel CreateRMAConfiguration(RMAConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ModelNotNull);

            RMAConfigurationModel rmaConfiguration = new RMAConfigurationModel();

            //If RmaConfigurationId is greater data will be updated else inserted.
            if (model?.RmaConfigurationId > 0)
            {
                //Update request status.
                bool isRMSConfigurationUpdated = _rmaConfigurationRepository.Update(model.ToEntity<ZnodeRmaConfiguration>());
                ZnodeLogging.LogMessage(isRMSConfigurationUpdated ? string.Format(Admin_Resources.SuccessRMAConfiguratorUpdate, model.DisplayName)  : Admin_Resources.ErrorRMAConfiguratorUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                rmaConfiguration = isRMSConfigurationUpdated ? model : new RMAConfigurationModel();
            }
            else
            {
                ZnodeRmaConfiguration entity = _rmaConfigurationRepository.Insert(model.ToEntity<ZnodeRmaConfiguration>());
                ZnodeLogging.LogMessage(IsNotNull(rmaConfiguration) ? string.Format(Admin_Resources.SuccessRMAConfiguratorCreate, entity.RmaConfigurationId) : Admin_Resources.ErrorRMAConfiguratorCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                rmaConfiguration = IsNotNull(entity) ? rmaConfiguration : new RMAConfigurationModel();
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return rmaConfiguration;
        }

        //Get RMA Configuration data.
        public virtual RMAConfigurationModel GetRMAConfiguration()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            RMAConfigurationModel rmaConfigurationModel = _rmaConfigurationRepository.GetEntity(string.Empty)?.ToModel<RMAConfigurationModel>();
            ZnodeLogging.LogMessage("RMAConfigurationModel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaConfigurationModel);
            return IsNull(rmaConfigurationModel) ? new RMAConfigurationModel() : rmaConfigurationModel;
        }
        #endregion

        #region Request status
        //Update request status.
        public virtual bool UpdateRequestStatus(RequestStatusModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter RequestStatusModel model", ZnodeLogging.Components.OMS.ToString(),TraceLevel.Verbose, new object[] { model?.RmaRequestStatusId });
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.RmaRequestStatusId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            //Update request status.
            bool isRequestStatusUpdated = _requestStatusRepository.Update(model.ToEntity<ZnodeRmaRequestStatu>());
            ZnodeLogging.LogMessage(isRequestStatusUpdated ? Admin_Resources.SuccessRequestUpdate : Admin_Resources.ErrorRequestUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isRequestStatusUpdated;
        }

        //Delete request status by requestStatusId.
        public virtual bool DeleteRequestStatus(ParameterModel requestStatusId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter ParameterModel", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { requestStatusId?.Ids });
            bool status = false;
            if (string.IsNullOrEmpty(requestStatusId?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeRmaRequestStatuEnum.RmaRequestStatusId.ToString(), ProcedureFilterOperators.In, requestStatusId.Ids.ToString()));

            //Check and get active Request Status.
            List<int> activeIds = GetActiveRequestStatus(requestStatusId.Ids);
            ZnodeLogging.LogMessage("Active request status Ids: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, activeIds);

            if (activeIds.Count > 0)
            {
                //Delete Request Status which are not Active.
                DeleteInActiveData(requestStatusId?.Ids, activeIds);
                throw new ZnodeException(ErrorCodes.NotDeleteActiveRecord, Admin_Resources.RequestStatusActiveCanNotDelete);
            }

            status = _requestStatusRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessRequestDelete : Admin_Resources.ErrorRequestDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        //Get request status list.
        public virtual RequestStatusListModel GetRequestStatusList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("PageListModel to generate RMA request status list: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeRmaRequestStatu> requestStatusList = _requestStatusRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("requestStatusList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, requestStatusList?.Count());
            RequestStatusListModel listModel = new RequestStatusListModel();

            listModel.RequestStatusList = requestStatusList?.Count > 0 ? requestStatusList.ToModel<RequestStatusModel>().ToList() : new List<RequestStatusModel>();
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get request status data by requestStatusId.
        public virtual RequestStatusModel GetRequestStatus(int requestStatusId)
            => _requestStatusRepository.Table.FirstOrDefault(x => x.RmaRequestStatusId == requestStatusId)?.ToModel<RequestStatusModel>();
        #endregion

        #region Reason For Return
        //Create Reason For Return.
        public virtual RequestStatusModel CreateReasonForReturn(RequestStatusModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeRmaReasonForReturn reasonForReturn = _reasonForReturnRepository.Insert(model.ToEntity<ZnodeRmaReasonForReturn>());
            ZnodeLogging.LogMessage(IsNotNull(reasonForReturn) ? string.Format(Admin_Resources.SuccessReasonReturnCreate, reasonForReturn.RmaReasonForReturnId) : Admin_Resources.ErrorReasonReturnCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return IsNotNull(reasonForReturn) ? reasonForReturn.ToModel<RequestStatusModel>() : new RequestStatusModel();
        }

        //Update Reason For Return.
        public virtual bool UpdateReasonForReturn(RequestStatusModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.RmaReasonForReturnId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            //Update reason for return.
            bool isReasonForReturnUpdated = _reasonForReturnRepository.Update(model.ToEntity<ZnodeRmaReasonForReturn>());
            ZnodeLogging.LogMessage(isReasonForReturnUpdated ? string.Format(Admin_Resources.SuccessReasonReturnUpdate, model.RmaReasonForReturnId) : Admin_Resources.ErrorReasonReturnUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isReasonForReturnUpdated;
        }

        //Get reason for return data by reasonForReturnId.
        public virtual RequestStatusModel GetReasonForReturn(int reasonForReturnId)
          => _reasonForReturnRepository.Table.FirstOrDefault(x => x.RmaReasonForReturnId == reasonForReturnId)?.ToModel<RequestStatusModel>();

        //Get paged reason for return list.
        public virtual RequestStatusListModel GetReasonForReturnList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("PageListModel to get Rma reason for return list: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeRmaReasonForReturn> reasonForReturnList = _reasonForReturnRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("reasonForReturnList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, reasonForReturnList?.Count());
            RequestStatusListModel listModel = new RequestStatusListModel();

            listModel.RequestStatusList = reasonForReturnList?.Count > 0 ? reasonForReturnList.ToModel<RequestStatusModel>().ToList() : new List<RequestStatusModel>();
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Delete  reason for return by reasonForReturnId.
        public virtual bool DeleteReasonForReturn(ParameterModel reasonForReturnId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool status = false;
            if (reasonForReturnId.Ids.Count() < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeRmaReasonForReturnEnum.RmaReasonForReturnId.ToString(), ProcedureFilterOperators.In, reasonForReturnId.Ids.ToString());

            //Check and get default reason for return ids.
            List<int> defaultIds = GetDefaultReasonForReturn(reasonForReturnId.Ids);
            ZnodeLogging.LogMessage("Reason for return default Ids: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, defaultIds);

            if (defaultIds.Count > 0)
            {
                ZnodeLogging.LogMessage("defaultIds to delete reason for return:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, defaultIds);
                //Delete Reason For Return which are not Default.
                DeleteUnDefaultData(reasonForReturnId?.Ids, defaultIds);
                throw new ZnodeException(ErrorCodes.DefaultDataDeletionError, Admin_Resources.ErrorReasonReturnDeleteAsDefault);
            }

            status = _reasonForReturnRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessReasonReturnDelete : Admin_Resources.ErrorReasonReturnDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return status;
        }
        #endregion

        #region Private Method
        //Check and get active Request Status.
        private List<int> GetActiveRequestStatus(string rmaRequestStatusIds)
        {
            IEnumerable<int> ids = rmaRequestStatusIds.Split(',').Select(int.Parse);
            ZnodeLogging.LogMessage("RequestStatusIds:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, ids);
            return _requestStatusRepository.Table.Where(x => ids.Contains(x.RmaRequestStatusId) && x.IsActive)?.Select(x => x.RmaRequestStatusId)?.ToList();
        }

        //Check and get default Reason For Return.
        private List<int> GetDefaultReasonForReturn(string rmaReasonForReturnIds)
        {
            IEnumerable<int> ids = rmaReasonForReturnIds.Split(',').Select(int.Parse);
            ZnodeLogging.LogMessage("ReasonForReturnIds:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, ids);
            return _reasonForReturnRepository.Table.Where(x => ids.Contains(x.RmaReasonForReturnId) && x.IsDefault.HasValue ? x.IsDefault.Value : false)?.Select(x => x.RmaReasonForReturnId)?.ToList();
        }

        //Delete Request Status or Reason For Return which are not Default.
        private void DeleteUnDefaultData(string id, List<int> defaultIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            List<int> unDefaultIds = id.Split(',').Select(int.Parse).Except(defaultIds).ToList();
            if (unDefaultIds.Count > 0)
            {
                FilterCollection filters = new FilterCollection();

                filters.Add(ZnodeRmaReasonForReturnEnum.RmaReasonForReturnId.ToString(), ProcedureFilterOperators.In, string.Join(",", unDefaultIds));
                ZnodeLogging.LogMessage("Records to be deleted with Ids: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, unDefaultIds);
                _reasonForReturnRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Delete Request Status which are not Active.
        private void DeleteInActiveData(string id, List<int> activeIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<int> inActiveIds = id.Split(',').Select(int.Parse).AsEnumerable().Except(activeIds).ToList();
            ZnodeLogging.LogMessage("inActiveIds:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, inActiveIds);
            if (inActiveIds.Count > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeRmaRequestStatuEnum.RmaRequestStatusId.ToString(), ProcedureFilterOperators.In, string.Join(",", inActiveIds)));
                ZnodeLogging.LogMessage("Records to be deleted with Ids: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, inActiveIds);
                _requestStatusRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }
        #endregion
    }
}
