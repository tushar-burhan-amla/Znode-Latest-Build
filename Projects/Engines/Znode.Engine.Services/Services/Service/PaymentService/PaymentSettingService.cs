using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
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
using Znode.Libraries.Observer;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class PaymentSettingService : BaseService, IPaymentSettingService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePaymentSetting> _paymentSettingRepository;
        private readonly IZnodeRepository<ZnodePortalPaymentSetting> _portalPaymentSettingRepository;
        private readonly IZnodeRepository<ZnodeProfilePaymentSetting> _profilePaymentSettingRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderRepository;
        private readonly IZnodeRepository<ZnodePaymentType> _paymentTypeRepository;
        private readonly IZnodeRepository<ZnodePaymentGateway> _paymentGatewayRepository;
        #endregion

        #region Constructor
        public PaymentSettingService()
        {
            _paymentSettingRepository = new ZnodeRepository<ZnodePaymentSetting>();
            _portalPaymentSettingRepository = new ZnodeRepository<ZnodePortalPaymentSetting>();
            _profilePaymentSettingRepository = new ZnodeRepository<ZnodeProfilePaymentSetting>();
            _orderRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _paymentTypeRepository = new ZnodeRepository<ZnodePaymentType>();
            _paymentGatewayRepository = new ZnodeRepository<ZnodePaymentGateway>();
        }
        #endregion

        #region Public Methods
        //Create new Payment Setting
        public virtual PaymentSettingModel CreatePaymentSetting(PaymentSettingModel paymentSettingsModel)
        {
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingModel having PaymentSettingId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, paymentSettingsModel.PaymentSettingId);

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(paymentSettingsModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingNotNull);

            // Checks the condition if paymentDisplayName is null or it is duplicate then throws exception
            ValidatePaymentDisplayName(paymentSettingsModel.PaymentSettingId, paymentSettingsModel.PaymentDisplayName);

            //To set paymentTypeId by paymentCode & gatewayId by gatewayCode
            SetPaymentTypeAndGatewayIdByCode(paymentSettingsModel);

            ZnodePaymentSetting paymentSetting = _paymentSettingRepository.Insert(paymentSettingsModel.ToEntity<ZnodePaymentSetting>());
            ZnodeLogging.LogMessage("Inserted paymentSetting with id ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, paymentSettingsModel.PaymentSettingId);
            ZnodeLogging.LogMessage(IsNotNull(paymentSetting) ? Admin_Resources.SuccessPaymentSettingCreate : Admin_Resources.ErrorPaymentSettingCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (IsNotNull(paymentSetting))
                return paymentSetting.ToModel<PaymentSettingModel>();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return paymentSettingsModel;
        }

        //Update Payment Setting
        public virtual bool UpdatePaymentSetting(PaymentSettingModel paymentSettingsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingModel having PaymentSettingId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, paymentSettingsModel.PaymentSettingId);

            if (IsNull(paymentSettingsModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (paymentSettingsModel.PaymentSettingId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            // Checks the condition if paymentDisplayName is null or it is duplicate then throws exception
            ValidatePaymentDisplayName(paymentSettingsModel.PaymentSettingId, paymentSettingsModel.PaymentDisplayName);

            //To set paymentTypeId by paymentCode & gatewayId by gatewatCode
            SetPaymentTypeAndGatewayIdByCode(paymentSettingsModel);

            bool isPaymentSettingUpdated = _paymentSettingRepository.Update(paymentSettingsModel.ToEntity<ZnodePaymentSetting>());
            ZnodeLogging.LogMessage(isPaymentSettingUpdated ? Admin_Resources.SuccessPaymentSettingUpdate : Admin_Resources.ErrorPaymentSettingUpdate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return isPaymentSettingUpdated;
        }

        //Get Payment Setting by Payment Setting id.
        public virtual PaymentSettingModel GetPaymentSetting(int paymentSettingId, NameValueCollection expands, int portalId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter paymentSettingId and portalId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { paymentSettingId, portalId });
            if (paymentSettingId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePaymentSettingEnum.PaymentSettingId.ToString(), FilterOperators.Equals, paymentSettingId.ToString()));
                PaymentSettingModel paymentSetting = _paymentSettingRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection()), GetExpands(expands))?.ToModel<PaymentSettingModel>();

                if (portalId > 0 && IsNotNull(paymentSetting))
                {
                    SetPortalPaymentDisplayNameAndExternalId(portalId, paymentSetting);
                }

                return paymentSetting;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }

        //Get paged Payment Setting list
        public virtual PaymentSettingListModel GetPaymentSettingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            UpdatePaymentSettingIdFilter(filters);

            int userId = GetUserIdFromFilter(filters);
            int portalId = GetPortalIdFromFilter(filters);
            int profileId = GetProfileIdFromFilter(filters);
            bool isAssociated = GetIsAssociatedFromFilter(filters);
            bool isUsedForOfflinePayment = false;

            if (filters?.Count > 0 && filters.Any(x => string.Equals(x.FilterName, "IsUsedForOfflinePayment", StringComparison.InvariantCultureIgnoreCase)))
            {
                isUsedForOfflinePayment = Convert.ToBoolean(filters.FirstOrDefault(x => string.Equals(x.FilterName, "IsUsedForOfflinePayment", StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
                //Remove profileId Filter from filters list
                filters.RemoveAll(x => string.Equals(x.FilterName, "IsUsedForOfflinePayment", StringComparison.InvariantCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("userId,portalId,profileId,isAssociated ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { userId, portalId, profileId, isAssociated });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("Where condition in GetPaymentSettingList method:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<PaymentSettingModel> objStoredProc = new ZnodeViewRepository<PaymentSettingModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", isAssociated ? "1" : "0", ParameterDirection.Input, DbType.Int32);

            IList<PaymentSettingModel> list = null;

            if (isUsedForOfflinePayment)
                list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPaymentSettingForOfflinePayment @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT, @PortalId, @ProfileId, @UserId,@IsAssociated", 4, out pageListModel.TotalRowCount);
            else
                list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPaymentSetting @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT, @PortalId, @ProfileId, @UserId,@IsAssociated", 4, out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("PaymentSettingModel list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, list?.Count());
            PaymentSettingListModel paymentSettingListModel = new PaymentSettingListModel();
            paymentSettingListModel.PaymentSettings = list?.Count > 0 ? list?.ToList() : null;

            paymentSettingListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return paymentSettingListModel;
        }

        //Get UserId From Filter.
        private int GetUserIdFromFilter(FilterCollection filters)
        {
            int userId = 0;
            if (filters?.Count > 0 && filters.Any(x => x.FilterName.ToLower() == ZnodeUserEnum.UserId.ToString().ToLower()))
            {
                //Get filter value
                userId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName.ToLower() == ZnodeUserEnum.UserId.ToString().ToLower())?.FilterValue);
                //Remove userId Filter from filters list
                filters.RemoveAll(x => x.FilterName.ToLower() == ZnodeUserEnum.UserId.ToString().ToLower());
            }
            ZnodeLogging.LogMessage("userId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, userId);
            return userId;
        }

        //Get portalId From Filter.
        private int GetPortalIdFromFilter(FilterCollection filters)
        {
            int portalId = 0;
            if (filters?.Count > 0 && filters.Any(x => string.Equals(x.FilterName, ZnodePortalPaymentSettingEnum.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                //Get filter value
                portalId = Convert.ToInt32(filters.FirstOrDefault(x => string.Equals(x.FilterName, ZnodePortalPaymentSettingEnum.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
                //Remove portalId Filter from filters list
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodePortalPaymentSettingEnum.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, portalId);
            return portalId;
        }

        //Get profileId From Filter.
        private int GetProfileIdFromFilter(FilterCollection filters)
        {
            int profileId = 0;
            if (filters?.Count > 0 && filters.Any(x => string.Equals(x.FilterName, ZnodeProfileEnum.ProfileId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                //Get filter value
                profileId = Convert.ToInt32(filters.FirstOrDefault(x => string.Equals(x.FilterName, ZnodeProfileEnum.ProfileId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
                //Remove profileId Filter from filters list
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeProfileEnum.ProfileId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("profileId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, profileId);
            return profileId;
        }

        //Get profileId From Filter.
        private bool GetIsAssociatedFromFilter(FilterCollection filters)
        {
            bool profileId = false;
            if (filters?.Count > 0 && filters.Any(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase)))
            {
                //Get filter value
                profileId = Convert.ToBoolean(filters.FirstOrDefault(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
                //Remove profileId Filter from filters list
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("IsAssociated:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, profileId);
            return profileId;
        }

        //Delete Payment Setting  by payment Setting Id.
        public virtual bool DeletePaymentSetting(ParameterModel paymentSettingId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (paymentSettingId.Ids.Count() < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePaymentSettingEnum.PaymentSettingId.ToString(), ProcedureFilterOperators.In, paymentSettingId.Ids.ToString()));

            bool status = false;

            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause generated", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClause);
            if (!CheckOrderExistForPaymentMethod(whereClause))
            {
                status = _portalPaymentSettingRepository.Delete(whereClause.WhereClause, whereClause.FilterValues);
                ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPortalPaymentSettingDelete : Admin_Resources.ErrorPortalPaymentSettingDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                status = _profilePaymentSettingRepository.Delete(whereClause.WhereClause, whereClause.FilterValues);
                ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessProfilePaymentSettingDelete : Admin_Resources.ErrorProfilePaymentSettingDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                //Delete Payment Setting
                status = _paymentSettingRepository.Delete(whereClause.WhereClause, whereClause.FilterValues);
                ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPaymentSettingDelete : Admin_Resources.ErrorPaymentSettingDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorPaymentSettingDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.IsUsed, Admin_Resources.PaymentDeleteErrorMessage);
            }
            if (!status)
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidPaymentSettingId);

            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return status;
        }

        public virtual bool IsActivePaymentSettingPresent(PaymentSettingModel paymentSettingsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingModel having PaymentSettingId,PaymentTypeId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { paymentSettingsModel.PaymentSettingId, paymentSettingsModel.PaymentTypeId });

            if (IsNull(paymentSettingsModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingNotNull);

            //As we allow to save paymentsetting in deactivate mode
            if (!paymentSettingsModel.IsActive) return false;

            if (paymentSettingsModel.PaymentSettingId < 1)
                return _paymentSettingRepository.Table.Any(x => x.PaymentGatewayId == x.PaymentGatewayId && x.PaymentTypeId == paymentSettingsModel.PaymentTypeId
                                                    && x.PaymentName.ToLower() == paymentSettingsModel.PaymentName.ToLower());
            else
            {
                //get active payment setting
                ZnodePaymentSetting activePaymentsetting = _paymentSettingRepository.Table.FirstOrDefault(x => x.PaymentGatewayId == x.PaymentGatewayId && x.PaymentTypeId == paymentSettingsModel.PaymentTypeId
                                                            && x.PaymentName.ToLower() == paymentSettingsModel.PaymentName.ToLower());
                if (IsNotNull(activePaymentsetting))
                    // return false if active payment setting Id is equal to provided payment setting Id
                    return activePaymentsetting.PaymentSettingId != paymentSettingsModel.PaymentSettingId;

                return false;
            }
        }

        //to check active payment setting present by paymentCode
        public virtual bool IsActivePaymentSettingPresentByPaymentCode(PaymentSettingModel paymentSettingsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingModel having PaymentSettingId,PaymentTypeId,PaymentCode", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { paymentSettingsModel.PaymentSettingId, paymentSettingsModel.PaymentTypeId, paymentSettingsModel.PaymentCode });

            if (IsNull(paymentSettingsModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingNotNull);

            //As we allow to save paymentsetting in deactivate mode
            if (!paymentSettingsModel.IsActive) return false;

            if (paymentSettingsModel.PaymentSettingId < 1)
                return _paymentSettingRepository.Table.Any(x => x.PaymentGatewayId == x.PaymentGatewayId && x.PaymentTypeId == paymentSettingsModel.PaymentTypeId
                                                    && x.PaymentCode.ToLower() == paymentSettingsModel.PaymentCode.ToLower());
            else
            {
                //get active payment setting
                ZnodePaymentSetting activePaymentsetting = _paymentSettingRepository.Table.FirstOrDefault(x => x.PaymentGatewayId == x.PaymentGatewayId && x.PaymentTypeId == paymentSettingsModel.PaymentTypeId
                                                            && x.PaymentCode.ToLower() == paymentSettingsModel.PaymentCode.ToLower());
                if (IsNotNull(activePaymentsetting))
                    // return false if active payment setting Id is equal to provided payment setting Id
                    return activePaymentsetting.PaymentSettingId != paymentSettingsModel.PaymentSettingId;

                return false;
            }
        }

        //Get captured payment details.
        public virtual bool GetCapturedPaymentDetails(int omsOrderId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (omsOrderId > 0)
            {
                ZnodeLogging.LogMessage("Input Parameter omsOrderId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, omsOrderId);
                IOrderService service = GetService<IOrderService>();
                NameValueCollection expands = new NameValueCollection();
                expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsPaymentState.ToString(), ZnodeOmsOrderDetailEnum.ZnodeOmsPaymentState.ToString());
                expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString());
                expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString(), ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString());
                expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString(), ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString());
                expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString(), ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString());
                expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsHistories.ToString(), ZnodeOmsOrderDetailEnum.ZnodeOmsHistories.ToString());
                expands.Add("isfromorderreceipt", "isfromorderreceipt");
                OrderModel orderModel = service.GetOrderById(omsOrderId, null, expands);

                var _erpInc = new ERPInitializer<OrderModel>(orderModel, "ARPayment");
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return true;
        }

        //Get portal payment display name and external Id
        public virtual void SetPortalPaymentDisplayNameAndExternalId(int portalId, PaymentSettingModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter portalId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { portalId });
            ZnodePortalPaymentSetting portalPaymentSetting = _portalPaymentSettingRepository.Table.Where(x => x.PaymentSettingId == model.PaymentSettingId && x.PortalId == portalId).FirstOrDefault() ?? null;
            if (IsNotNull(portalPaymentSetting))
            {
                model.PaymentDisplayName = !string.IsNullOrEmpty(portalPaymentSetting.PaymentDisplayName) ? portalPaymentSetting.PaymentDisplayName : model.PaymentDisplayName;
                model.PaymentExternalId = portalPaymentSetting.PaymentExternalId;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //To check order exist for payment method
        public virtual bool CheckOrderExistForPaymentMethod(EntityWhereClauseModel whereClause)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<ZnodeOmsOrderDetail> orders = _orderRepository.Table.Where(whereClause.WhereClause, whereClause.FilterValues).ToList() ?? null;
            ZnodeLogging.LogMessage("orders list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, orders?.Count());
            return IsNotNull(orders) && orders?.Count > 0;
        }
        #region Portal/Profile
        //Associate payment settings to portal.
        public virtual bool AssociatePaymentSettings(PaymentSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having ProfileId and PortalId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { associationModel.ProfileId, associationModel.ProfileId.ToString() });
            if (IsNull(associationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingAssociationNotNull);

            if (string.IsNullOrEmpty(associationModel.PaymentSettingId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PaymentSettingIdNotNull);
            if (associationModel.ProfileId == 0 && associationModel.PortalId == 0)
                throw new ZnodeException(ErrorCodes.NullModel, Api_Resources.ErrorPortalIdProfileId);

            bool result = false;
            if (associationModel.PortalId > 0 && associationModel.ProfileId > 0)
            {
                InsertIntoPortalPaymentSetting(associationModel);
                result = InsertIntoProfilePaymentSetting(associationModel);
            }
            else if (associationModel.PortalId > 0)
                result = InsertIntoPortalPaymentSetting(associationModel);
            else if (associationModel.ProfileId > 0)
                result = InsertIntoProfilePaymentSetting(associationModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return result;
        }

        //Associate payment settings to portal.
        public virtual bool AssociatePaymentSettingsForInvoice(PaymentSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having ProfileId and PortalId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { associationModel.ProfileId, associationModel.ProfileId.ToString() });
            if (IsNull(associationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingAssociationNotNull);

            if (string.IsNullOrEmpty(associationModel.PaymentSettingId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PaymentSettingIdNotNull);
            if (associationModel.ProfileId == 0 && associationModel.PortalId == 0)
                throw new ZnodeException(ErrorCodes.NullModel, Api_Resources.ErrorPortalIdProfileId);

            bool result = false;
            if (associationModel.PortalId > 0 && associationModel.ProfileId > 0)
            {
                InsertIntoPortalPaymentSettingForInvoice(associationModel);
                result = InsertIntoProfilePaymentSetting(associationModel);
            }
            else if (associationModel.PortalId > 0)
                result = InsertIntoPortalPaymentSettingForInvoice(associationModel);
            else if (associationModel.ProfileId > 0)
                result = InsertIntoProfilePaymentSetting(associationModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return result;
        }

        //Remove associated payment settings to portal.
        public virtual bool RemoveAssociatedPaymentSettings(PaymentSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having ProfileId and PortalId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { associationModel.ProfileId, associationModel.ProfileId.ToString() });
            if (IsNull(associationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingAssociationNotNull);

            if (string.IsNullOrEmpty(associationModel.PaymentSettingId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PaymentSettingIdNotNull);

            bool result = false;
            if (associationModel.PortalId > 0)
                result = RemoveFromPortalPaymentSetting(associationModel);

            if (associationModel.ProfileId > 0 && !associationModel.IsUserForOfflinePayment)
                result = RemoveFromProfilePaymentSetting(associationModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return result;
        }

        //Update portal payment settings.
        public virtual bool UpdatePortalPaymentSettings(PaymentSettingPortalModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having PaymentSettingId and PortalId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model.PaymentSettingId, model.PortalId.ToString() });
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingNotNull);

            if (model.PaymentSettingId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PaymentSettingIdNotNullOrZero);

            bool result = false;

            if (model.PortalId > 0)
                result = UpdatePortalPaymentDetails(model);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return result;
        }

        //Update payment display name and code.
        public virtual bool UpdatePortalPaymentDetails(PaymentSettingPortalModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(model))
            {
                ZnodePortalPaymentSetting payment = _portalPaymentSettingRepository.Table.Where(x => x.PaymentSettingId != model.PaymentSettingId && x.PortalId == model.PortalId && x.PaymentDisplayName.ToLower() == model.PaymentDisplayName.ToLower()).FirstOrDefault() ?? null;
                if (IsNull(payment))
                {
                    ZnodePublishStatesEnum PublishStateEnum = (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), model.PublishState, true);
                    payment = _portalPaymentSettingRepository.Table.Where(x => x.PaymentSettingId == model.PaymentSettingId && x.PortalId == model.PortalId).FirstOrDefault();
                    payment.PaymentDisplayName = model?.PaymentDisplayName;
                    payment.PaymentExternalId = model?.PaymentExternalId;
                    payment.IsApprovalRequired = model?.IsApprovalRequired ?? false;
                    payment.IsOABRequired = model.IsOABRequired;
                    payment.PublishStateId = (byte)PublishStateEnum;
                    _portalPaymentSettingRepository.Update(payment);
                    return true;
                }
            }
            return false;
        }

        //Check whether to call payment API by paymentTypeCode.
        public virtual bool CallToPaymentAPI(string paymentTypeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(paymentTypeCode))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymenttypeNotNull);

            return _paymentTypeRepository.Table.FirstOrDefault(x => x.Code.ToLower() == paymentTypeCode.ToLower())?.IsCallToPaymentAPI ?? false;
        }

        //Update profile payment settings.
        public virtual bool UpdateProfilePaymentSettings(PaymentSettingAssociationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having ProfilePaymentSettingId and ProfileId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model.ProfilePaymentSettingId, model.ProfileId });
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingModelNotNull);

            if (model.ProfilePaymentSettingId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, "Profile Payment Setting Id cannot be 0 or null.");

            if (model.ProfileId > 0)
            {
                ZnodePublishStatesEnum PublishStateEnum = (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), model.PublishState, true);
                ZnodeProfilePaymentSetting profilePaymentSetting = _profilePaymentSettingRepository.Table.Where(x => x.PaymentSettingId == model.ProfilePaymentSettingId && x.ProfileId == model.ProfileId)?.FirstOrDefault();
                profilePaymentSetting.PublishStateId = (byte)PublishStateEnum;
                profilePaymentSetting.DisplayOrder = model.DisplayOrder;
                _profilePaymentSettingRepository.Update(profilePaymentSetting);
                return true;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }
        #endregion

        /// <summary>
        /// Is payment display name already exist
        /// </summary>
        /// <param name="PaymentSettingValidationModel"></param>
        /// <returns>Return True False Response</returns>
        public virtual bool IsPaymentDisplayNameExists(PaymentSettingValidationModel paymentSettingValidationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters paymentDisplayName and paymentSettingId from PaymentSettingModel:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentDisplayName = paymentSettingValidationModel.PaymentDisplayName, PaymentSettingId = paymentSettingValidationModel.PaymentSettingId });
            if (string.IsNullOrEmpty(paymentSettingValidationModel.PaymentDisplayName))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentDisplayNameNotNull);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return IsNotNull(_paymentSettingRepository.Table.FirstOrDefault(x => x.PaymentDisplayName.Equals(paymentSettingValidationModel.PaymentDisplayName, StringComparison.InvariantCultureIgnoreCase) && x.PaymentSettingId != paymentSettingValidationModel.PaymentSettingId)) ? true : false;
        }

        //Get Payment Setting list for user
        public virtual PaymentSettingListModel GetPaymentSettingByUserDetails(UserPaymentSettingModel userPaymentSettingModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Payment.ToString(), TraceLevel.Info);
            int rowCount = 0;

            ZnodeLogging.LogMessage("userId,portalId ", ZnodeLogging.Components.Payment.ToString(), TraceLevel.Verbose, new object[] { userPaymentSettingModel?.UserId, userPaymentSettingModel?.PortalId });

            IZnodeViewRepository<PaymentSettingModel> objStoredProc = new ZnodeViewRepository<PaymentSettingModel>();
            objStoredProc.SetParameter("@PortalId", userPaymentSettingModel?.PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userPaymentSettingModel?.UserId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RowsCount", rowCount, ParameterDirection.Output, DbType.Int32);
            IList<PaymentSettingModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPaymentSettingByUserDetails @PortalId,@UserId,@RowsCount OUT", 2, out rowCount);

            ZnodeLogging.LogMessage("PaymentSettingModel list count:", ZnodeLogging.Components.Payment.ToString(), TraceLevel.Verbose, list?.Count());
            PaymentSettingListModel paymentSettingListModel = new PaymentSettingListModel
            {
                PaymentSettings = list?.Count > 0 ? list?.ToList() : null
            };

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Payment.ToString(), TraceLevel.Info);
            return paymentSettingListModel;
        }

        #endregion

        #region private Methods

        //Update filter value if PaymentSettingId passed with in operator
        private void UpdatePaymentSettingIdFilter(FilterCollection filters)
        {
            //Check whether PaymentSettingId passed with in operator
            if (filters?.Count > 0 && filters.Any(x => x.FilterName.ToLower() == ZnodePaymentSettingEnum.PaymentSettingId.ToString().ToLower() && x.FilterOperator == FilterOperators.In))
            {
                //Get filter value
                string filterValue = filters.FirstOrDefault(x => x.FilterName.ToLower() == ZnodePaymentSettingEnum.PaymentSettingId.ToString().ToLower() && x.FilterOperator == FilterOperators.In)?.FilterValue;

                if (!string.IsNullOrEmpty(filterValue))
                {
                    //Remove Payment Setting Filters with IN operator from filters list
                    filters.RemoveAll(x => x.FilterName.ToLower() == ZnodePaymentSettingEnum.PaymentSettingId.ToString().ToLower() && x.FilterOperator == FilterOperators.In);

                    //Add Payment Setting Filters
                    filters.Add(ZnodePaymentSettingEnum.PaymentSettingId.ToString(), FilterOperators.In, filterValue.Replace('_', ','));
                }
            }
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodePaymentSettingEnum.ZnodePaymentType.ToString().ToLower())) SetExpands(ZnodePaymentSettingEnum.ZnodePaymentType.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Insert into portal payment setting.
        protected virtual bool InsertIntoPortalPaymentSetting(PaymentSettingAssociationModel associationModel)
        {
            List<ZnodePortalPaymentSetting> entriesToInsert = new List<ZnodePortalPaymentSetting>();
            List<ZnodePortalPaymentSetting> entriesToUpdate = new List<ZnodePortalPaymentSetting>();

            List<ZnodePaymentSetting> lstPaymentSetting = GetListPaymentSetting(associationModel);
            List<int> lstAssociatedPaymentType = GetAssociatePaymentTypeByPortalId(associationModel.PortalId);

            ZnodeLogging.LogMessage("lstAssociatedPaymentType:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, lstAssociatedPaymentType);
            foreach (string item in associationModel.PaymentSettingId.Split(','))
            {
                int paymentTypeId = lstPaymentSetting.Where(x => x.PaymentSettingId == Convert.ToInt32(item)).Select(x => x.PaymentTypeId).FirstOrDefault();
                int paymentIdCheck = Convert.ToInt32(item);
                if (paymentIdCheck > 0)
                {
                    if (CheckPortalPaymentAssociationExists(associationModel.PortalId, paymentIdCheck))
                        throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.ErrorPortalPaymentAlreadyExist);

                    ZnodePortalPaymentSetting portalPaymentSettingRepository = _portalPaymentSettingRepository.Table.FirstOrDefault(x => x.PortalId == associationModel.PortalId && x.PaymentSettingId == paymentIdCheck);
                    if (IsNotNull(portalPaymentSettingRepository))
                    {
                        portalPaymentSettingRepository.IsUsedForWebStorePayment = true;
                        entriesToUpdate.Add(portalPaymentSettingRepository);
                        continue;
                    }
                }
                else
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.MessageNumericValueAllowed);
                ZnodePublishStatesEnum PublishStateEnum = (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), associationModel.PublishState, true);

                entriesToInsert.Add(new ZnodePortalPaymentSetting() { PortalId = associationModel.PortalId, PaymentSettingId = Convert.ToInt32(item), PublishStateId = (byte)PublishStateEnum, IsUsedForWebStorePayment = true });
                lstAssociatedPaymentType.Add(paymentTypeId);
            }
            ZnodeLogging.LogMessage("lstAssociatedPaymentType:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, lstAssociatedPaymentType);
            return UpdateOrInsertIntoStorePaymentSetting(entriesToInsert, entriesToUpdate);

        }

        //Insert into portal payment setting.
        protected virtual List<ZnodePaymentSetting> GetListPaymentSetting(PaymentSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having PaymentSettingId and ProfileId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { associationModel.PaymentSettingId, associationModel.ProfileId.ToString() });

            //Get the Payment Setting Details based on requested Payment Settings.
            int[] paymentSettingId = associationModel.PaymentSettingId.Split(',').Select(Int32.Parse).ToArray();
            List<ZnodePaymentSetting> lstPaymentSetting = _paymentSettingRepository.Table.Where(x => paymentSettingId.Contains(x.PaymentSettingId)).ToList();
            ZnodeLogging.LogMessage("lstPaymentSetting list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, lstPaymentSetting?.Count());
            return lstPaymentSetting;
        }


        //Insert into portal payment setting.
        protected virtual bool UpdateOrInsertIntoStorePaymentSetting(List<ZnodePortalPaymentSetting> entriesToInsert, List<ZnodePortalPaymentSetting> entriesToUpdate)
        {
            if (entriesToInsert.Count > 0)
                entriesToInsert = _portalPaymentSettingRepository.Insert(entriesToInsert)?.ToList();
            if (entriesToUpdate.Count > 0)
                _portalPaymentSettingRepository.BatchUpdate(entriesToUpdate);
            return entriesToInsert?.Count > 0 || entriesToUpdate?.Count > 0;
        }

        //Insert into portal payment setting.
        protected virtual bool InsertIntoPortalPaymentSettingForInvoice(PaymentSettingAssociationModel associationModel)
        {
            List<ZnodePortalPaymentSetting> entriesToInsert = new List<ZnodePortalPaymentSetting>();
            List<ZnodePortalPaymentSetting> entriesToUpdate = new List<ZnodePortalPaymentSetting>();

            List<ZnodePaymentSetting> lstPaymentSetting = GetListPaymentSetting(associationModel);
            List<int> lstAssociatedPaymentType = GetAssociatePaymentTypeByPortalId(associationModel.PortalId);

            foreach (string item in associationModel.PaymentSettingId.Split(','))
            {
                int paymentTypeId = lstPaymentSetting.Where(x => x.PaymentSettingId == Convert.ToInt32(item)).Select(x => x.PaymentTypeId).FirstOrDefault();
                int paymentIdCheck = Convert.ToInt32(item);
                if (paymentIdCheck > 0)
                {
                    if (CheckPortalPaymentAssociationExistsForOfflinePayment(associationModel.PortalId, paymentIdCheck))
                        throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.ErrorPortalPaymentAlreadyExist);

                    ZnodePortalPaymentSetting portalPaymentSettingRepository = _portalPaymentSettingRepository.Table.FirstOrDefault(x => x.PortalId == associationModel.PortalId && x.PaymentSettingId == paymentIdCheck);
                    if (IsNotNull(portalPaymentSettingRepository))
                    {
                        portalPaymentSettingRepository.IsUsedForOfflinePayment = true;
                        entriesToUpdate.Add(portalPaymentSettingRepository);
                        continue;
                    }
                }
                else
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.MessageNumericValueAllowed);
                ZnodePublishStatesEnum PublishStateEnum = (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), associationModel.PublishState, true);

                entriesToInsert.Add(new ZnodePortalPaymentSetting() { PortalId = associationModel.PortalId, PaymentSettingId = Convert.ToInt32(item), PublishStateId = (byte)PublishStateEnum, IsUsedForOfflinePayment = true });
                lstAssociatedPaymentType.Add(paymentTypeId);
            }
            ZnodeLogging.LogMessage("lstAssociatedPaymentType:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, lstAssociatedPaymentType);

            return UpdateOrInsertIntoStorePaymentSetting(entriesToInsert, entriesToUpdate);

        }

        //Get the Associated Payment Type Ids by Portal Id.
        private List<int> GetAssociatePaymentTypeByPortalId(int portalId)
        {
            //Get the Associated Payment Types based on the Portal.
            return (from portalPayment in _portalPaymentSettingRepository.Table
                    join paymentSetting in _paymentSettingRepository.Table on portalPayment.PaymentSettingId equals paymentSetting.PaymentSettingId
                    where portalPayment.PortalId == portalId
                    select paymentSetting.PaymentTypeId).ToList();
        }

        //Insert into profile payment setting.
        private bool InsertIntoProfilePaymentSetting(PaymentSettingAssociationModel associationModel)
        {
            List<ZnodeProfilePaymentSetting> entriesToInsert = new List<ZnodeProfilePaymentSetting>();
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having PaymentSettingId and ProfileId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { associationModel.PaymentSettingId, associationModel.ProfileId.ToString() });
            foreach (string item in associationModel.PaymentSettingId.Split(','))
            {
                int paymentIdCheck = Convert.ToInt32(item);

                if (paymentIdCheck > 0)
                {
                    if (CheckProfilePaymentAssociationExists(associationModel.ProfileId, paymentIdCheck))
                        throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.ErrorProfilePaymentAlreadyExist);
                }
                else
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.MessageNumericValueAllowed);

                entriesToInsert.Add(new ZnodeProfilePaymentSetting() { ProfileId = associationModel.ProfileId, PaymentSettingId = Convert.ToInt32(item) });
            }
            entriesToInsert = _profilePaymentSettingRepository.Insert(entriesToInsert)?.ToList();
            ZnodeLogging.LogMessage("Inserted PaymentSetting in Profile with id:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, associationModel.PaymentSettingId);
            return entriesToInsert?.Count > 0;
        }

        //Delete entries from portal payment settings.
        protected virtual bool RemoveFromPortalPaymentSetting(PaymentSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having PaymentSettingId and PortalId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { associationModel.PaymentSettingId, associationModel.ProfileId.ToString() });
            FilterCollection filters = GetPaymentSettingIdFilter(associationModel.PaymentSettingId);

            filters.Add(new FilterTuple(ZnodePortalPaymentSettingEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, associationModel.PortalId.ToString()));
            ZnodeLogging.LogMessage("Deleting PaymentSetting from portal with id:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, associationModel.PaymentSettingId);
            return RemovePortalPaymentSetting(filters, associationModel);

        }

        protected virtual bool RemovePortalPaymentSetting(FilterCollection filters, PaymentSettingAssociationModel associationModel)
        {

            List<string> listPaymentSettingId = associationModel.PaymentSettingId.Split(',').ToList();
            try
            {
                foreach (string paymentSettingIds in listPaymentSettingId)
                {
                    int paymentSettingId = Convert.ToInt32(paymentSettingIds);
                    ZnodePortalPaymentSetting portalPaymentSettingRepository = _portalPaymentSettingRepository.Table.FirstOrDefault(x => x.PortalId == associationModel.PortalId && x.PaymentSettingId == paymentSettingId);
                    EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                    if (IsNotNull(portalPaymentSettingRepository))
                    {
                        if (!portalPaymentSettingRepository.IsUsedForWebStorePayment && associationModel.IsUserForOfflinePayment)
                        {
                            _portalPaymentSettingRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
                        }
                        else if (!portalPaymentSettingRepository.IsUsedForOfflinePayment && !associationModel.IsUserForOfflinePayment)
                        {
                            _portalPaymentSettingRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
                        }
                        else if (associationModel.IsUserForOfflinePayment)
                        {
                            portalPaymentSettingRepository.IsUsedForOfflinePayment = false;
                            _portalPaymentSettingRepository.Update(portalPaymentSettingRepository);
                        }
                        else
                        {
                            portalPaymentSettingRepository.IsUsedForWebStorePayment = false;
                            _portalPaymentSettingRepository.Update(portalPaymentSettingRepository);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in RemovePortalPaymentSetting:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error, ex.Message);
                throw;
            }
        }


        // To check associated profile payment method.
        protected virtual bool CheckProfilePaymentAssociationExists(int profileId, int paymentSettingId)
            => _profilePaymentSettingRepository.Table.Any(x => x.ProfileId == profileId && x.PaymentSettingId == paymentSettingId);
        //To Check associated portal payment method.
        protected virtual bool CheckPortalPaymentAssociationExists(int portalId, int paymentSettingId)
           => _portalPaymentSettingRepository.Table.Any(x => x.PortalId == portalId && x.PaymentSettingId == paymentSettingId && x.IsUsedForWebStorePayment);

        protected virtual bool CheckPortalPaymentAssociationExistsForUpdate(int portalId, int paymentSettingId)
            => _portalPaymentSettingRepository.Table.Any(x => x.PortalId == portalId && x.PaymentSettingId == paymentSettingId && x.IsUsedForWebStorePayment);

        //To Check associated portal payment method.
        protected virtual bool CheckPortalPaymentAssociationExistsForOfflinePayment(int portalId, int paymentSettingId)
           => _portalPaymentSettingRepository.Table.Any(x => x.PortalId == portalId && x.PaymentSettingId == paymentSettingId && x.IsUsedForOfflinePayment);

        //Delete entries from profile payment settings.
        private bool RemoveFromProfilePaymentSetting(PaymentSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PaymentSettingAssociationModel having PaymentSettingId and ProfileId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { associationModel.PaymentSettingId, associationModel.ProfileId.ToString() });
            FilterCollection filters = GetPaymentSettingIdFilter(associationModel.PaymentSettingId);
            filters.Add(new FilterTuple(ZnodeProfilePaymentSettingEnum.ProfileId.ToString(), ProcedureFilterOperators.Equals, associationModel.ProfileId.ToString()));
            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Deleting PaymentSetting from profile with id:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, associationModel.PaymentSettingId);
            return _profilePaymentSettingRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
        }

        //Get payment setting ids filter.
        private FilterCollection GetPaymentSettingIdFilter(string PaymentSettingIds)
            => new FilterCollection() { new FilterTuple(ZnodePortalPaymentSettingEnum.PaymentSettingId.ToString(), ProcedureFilterOperators.In, PaymentSettingIds) };

        //Set payment type Id and gateway Id by paymentcode and gatewaycode
        private void SetPaymentTypeAndGatewayIdByCode(PaymentSettingModel paymentSettingsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter PaymentSettingModel having PaymentTypeCode and GatewayCode", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, new object[] { paymentSettingsModel?.PaymentTypeCode, paymentSettingsModel?.GatewayCode });
            if (IsNotNull(paymentSettingsModel) && !string.IsNullOrEmpty(paymentSettingsModel.PaymentTypeCode))
            {
                paymentSettingsModel.PaymentTypeId = _paymentTypeRepository.Table.Where(x => x.Code.ToLower() == paymentSettingsModel.PaymentTypeCode.ToLower()).FirstOrDefault().PaymentTypeId;
                ZnodeLogging.LogMessage("PaymentTypeId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, paymentSettingsModel.PaymentTypeId);
                if (string.Equals(paymentSettingsModel.PaymentTypeCode, ZnodeConstant.PayPalExpress, StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentSettingsModel.PaymentGatewayId = _paymentGatewayRepository.Table.Where(x => x.GatewayCode.ToLower() == ZnodeConstant.PayPalExpress.ToLower()).FirstOrDefault()?.PaymentGatewayId;
                    ZnodeLogging.LogMessage("PaymentGatewayId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, paymentSettingsModel.PaymentGatewayId);
                }
                if (!string.IsNullOrEmpty(paymentSettingsModel.GatewayCode))
                {
                    paymentSettingsModel.PaymentGatewayId = _paymentGatewayRepository.Table.Where(x => x.GatewayCode.ToLower() == paymentSettingsModel.GatewayCode.ToLower()).FirstOrDefault().PaymentGatewayId;
                    ZnodeLogging.LogMessage("PaymentGatewayId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, paymentSettingsModel.PaymentGatewayId);
                }
            }

        }

        /// <summary>
        /// Is payment display name already exist
        /// </summary>
        /// <param name="paymentSettingModel"></param>
        private void ValidatePaymentDisplayName(int paymentSettingId, string paymentDisplayName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters paymentDisplayName and paymentSettingId from PaymentSettingModel:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentDisplayName = paymentDisplayName, PaymentSettingId = paymentSettingId });
            PaymentSettingValidationModel paymentSettingValidationModel = new PaymentSettingValidationModel()
            {
                PaymentSettingId = paymentSettingId,
                PaymentDisplayName = paymentDisplayName
            };
            if (IsPaymentDisplayNameExists(paymentSettingValidationModel))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.PaymentDisplayNameAlreadyExist);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }
        #endregion
    }
}
