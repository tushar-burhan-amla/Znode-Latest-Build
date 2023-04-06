using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Engine.Shipping.Usps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ShippingService : BaseService, IShippingService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeShipping> _shippingRepository;
        private readonly IZnodeRepository<ZnodeShippingSKU> _shippingSKURepository;
        private readonly IZnodeRepository<ZnodeShippingServiceCode> _shippingServiceCodeRepository;
        private readonly IZnodeRepository<ZnodeShippingRule> _shippingRuleRepository;
        private readonly IZnodeRepository<ZnodeProfileShipping> _znodeProfileShippingRepository;
        private readonly IZnodeRepository<ZnodePortalShipping> _znodePortalShippingRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValue> _pimAttributeValueRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValueLocale> _pimAttributeValueLocaleRepository;
        private readonly IZnodeRepository<ZnodeState> _stateRepository;
        #endregion

        #region Constructor
        public ShippingService()
        {
            _shippingRepository = new ZnodeRepository<ZnodeShipping>();
            _shippingSKURepository = new ZnodeRepository<ZnodeShippingSKU>();
            _shippingServiceCodeRepository = new ZnodeRepository<ZnodeShippingServiceCode>();
            _shippingRuleRepository = new ZnodeRepository<ZnodeShippingRule>();
            _znodeProfileShippingRepository = new ZnodeRepository<ZnodeProfileShipping>();
            _znodePortalShippingRepository = new ZnodeRepository<ZnodePortalShipping>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _pimAttributeValueRepository = new ZnodeRepository<ZnodePimAttributeValue>();
            _pimAttributeValueLocaleRepository = new ZnodeRepository<ZnodePimAttributeValueLocale>();
            _stateRepository = new ZnodeRepository<ZnodeState>();
        }
        #endregion

        #region Public Methods
        //Create new shipping
        public virtual ShippingModel CreateShipping(ShippingModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("ShippingModel having ShippingTypeId, ShippingCode.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose,new object[] { model?.ShippingTypeId, model?.ShippingCode });

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ShippingModelNotNull);

            if (IsShippingAlreadyExist(model.ShippingTypeId, model.ProfileId, model.ShippingCode, model.DestinationCountryCode, model.StateCode, model.CountyFIPS))
            {
                throw new ZnodeException(ErrorCodes.AlreadyExist, string.Format(Admin_Resources.ShippingAlreadyExist, model.ShippingCode));
            }
            
            model.ProfileId = Equals(model.ProfileId, 0) ? null : model.ProfileId;
            ZnodeShipping shipping = _shippingRepository.Insert(model.ToEntity<ZnodeShipping>());
            model.ShippingId = shipping.ShippingId;
            ZnodeLogging.LogMessage("ProfileId, ShippingId for creating shipping.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { model?.ProfileId, model?.ShippingId });

            if (model.ShippingId > 0 && !string.IsNullOrEmpty(model.ImportedSkus))
                InsertTaxClassSku(model);

            ZnodeLogging.LogMessage((!Equals(shipping, null) ? Admin_Resources.SuccessShippingCreate : Admin_Resources.ErrorShippingCreate), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            return shipping.ToModel<ShippingModel>();
        }

        //Update  shipping
        public virtual bool UpdateShipping(ShippingModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            if (model.ShippingId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            if (IsShippingExistsOnUpdate(model.ShippingTypeId, model.ProfileId, model.ShippingId, model.ShippingCode, model.DestinationCountryCode, model.StateCode, model.CountyFIPS))
            { 
                throw new ZnodeException(ErrorCodes.AlreadyExist, string.Format(Admin_Resources.ShippingAlreadyExist, model.ShippingCode));
            }

            //Update Shipping
            model.ProfileId = Equals(model.ProfileId, 0) ? null : model.ProfileId;
            bool isShippingUpdated = _shippingRepository.Update(model.ToEntity<ZnodeShipping>());

            if (!string.IsNullOrEmpty(model.ImportedSkus))
                InsertTaxClassSku(model);

            ZnodeLogging.LogMessage(isShippingUpdated ? String.Format(Admin_Resources.SuccessShippingUpdate, model.ShippingId): String.Format(Admin_Resources.ErrorShippingUpdate, model.ShippingId), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return isShippingUpdated;
        }

        //Get shipping by shipping id.
        public virtual ShippingModel GetShipping(int shippingId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters shippingId ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { shippingId });

            ShippingModel shippingModel;
            shippingModel = _shippingRepository.GetById(shippingId).ToModel<ShippingModel, ZnodeShipping>();
           
            if (IsNotNull(shippingModel))
            {
                FilterCollection filterList = new FilterCollection();
                FilterTuple fileNameTuple = new FilterTuple(ZnodeShippingServiceCodeEnum.Code.ToString(), ProcedureFilterOperators.Is, shippingModel?.ShippingCode);
                filterList.Add(fileNameTuple);
                fileNameTuple = new FilterTuple(ZnodeShippingServiceCodeEnum.ShippingTypeId.ToString(), ProcedureFilterOperators.Equals, Convert.ToString(shippingModel?.ShippingTypeId));
                filterList.Add(fileNameTuple);

                //gets the where clause with filter Values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
                shippingModel.ShippingServiceCodeId = (_shippingServiceCodeRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ShippingServiceCodeId).GetValueOrDefault();
                ZnodeLogging.LogMessage("ShippingServiceCodeId ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { shippingModel?.ShippingServiceCodeId });

                shippingModel.DestinationCountryCode = IsNotNull(shippingModel.DestinationCountryCode) ? shippingModel.DestinationCountryCode : "All";
                return shippingModel;
                
            }
             else return shippingModel;
        }

        //Get paged shipping list
        public virtual ShippingListModel GetShippingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            //for statecode we have used textbox, in that text box user can insert either StateName or StateCode we dont know weather user entered StateName or StateCode
            //if we need to get statecode from front end we will have to make extra api call to get state code every time, to eliminate that call we pass stateName value in filter parameter and get the statecode then that apply state & country code filter on shipping method
            string countryCode = string.Empty;
            string stateCode = string.Empty;
            CheckShippingDestinationFilter(filters, ref countryCode, ref stateCode);
            
            int userId = GetUserIdFromFilter(filters);
            ZnodeLogging.LogMessage("userId returned from GetUserIdFromFilter method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, userId);

            int portalId = GetPortalIdFromFilter(filters);
            ZnodeLogging.LogMessage("portalId returned from GetPortalIdFromFilter method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, portalId);

            int profileId = GetProfileIdFromFilter(filters);
            ZnodeLogging.LogMessage("profileId returned from GetProfileIdFromFilter method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, profileId);


            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters  ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<ShippingModel> objStoredProc = new ZnodeViewRepository<ShippingModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            List<ShippingModel> shippinglist = objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingList @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT, @ProfileId,@PortalId, @UserId", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("shippinglist count ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippinglist?.Count);

            ShippingListModel shippingListModel = new ShippingListModel();
            shippingListModel.ShippingList = shippinglist?.Count > 0 ? (string.IsNullOrEmpty(countryCode)) ? shippinglist : GetShippingByCountryAndStateCode(countryCode, stateCode, shippinglist) : new List<ShippingModel>();
            shippingListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingListModel;
        }

        //Get Shipping list by userId and PortalId
        public virtual List<ShippingModel> GetShippingListByUserDetails(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            // For guest users, -1 will be assigned in the userId variable.
            userId = userId.Equals(0) ? -1 : userId;
            IZnodeViewRepository<ShippingModel> objStoredProc = new ZnodeViewRepository<ShippingModel>();
            int rowCount = 0;

            objStoredProc.SetParameter("@RowsCount", rowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            return objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingListByUserDetails @PortalId,@UserId,@RowsCount OUT", 2, out rowCount)?.ToList();
        }

        //Delete shipping  by shippingId.
        public virtual bool DeleteShipping(ParameterModel shippingId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Shipping to be deleted ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingId?.Ids);

            //Checks shipping id.
            if (string.IsNullOrEmpty(shippingId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShippingIdNotNullOrEmpty);

                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter(ZnodeShippingEnum.ShippingId.ToString(), shippingId.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                View_ReturnBoolean result = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteShipping @ShippingId,  @Status OUT", 1, out status).FirstOrDefault();

                //SP will return status as 1 if shipping as well as all its associated items deleted successfully.
                if (Equals(status, 1))
                {
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessShippingDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return true;
                }
                else if (result?.Id == 2)
                {
                throw new ZnodeException(ErrorCodes.DefaultDataDeletionError, string.Format(Admin_Resources.ErrorShippingDeleteAsUsedInDownloadableAndProducts,shippingId.Ids));
                }
            
                else
                {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, string.Format(Admin_Resources.ErrorShippingDeleteAsUsedInOrders, shippingId.Ids));
                }
            
            
            
        }

        #region Shipping SKU

        //Get shipping sku list.
        public virtual ShippingSKUListModel GetShippingSKUList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters  ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<ShippingSKUModel> objStoredProc = new ZnodeViewRepository<ShippingSKUModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);

            //Gets the entity list according to where clause, order by clause and pagination
            IList<ShippingSKUModel> associatedSKUs = objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingSKUDetail  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedSKUs count ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, associatedSKUs?.Count);

            ShippingSKUListModel shippingSKUListModel = new ShippingSKUListModel { ShippingSKUList = associatedSKUs?.ToList() };
            shippingSKUListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingSKUListModel;
        }

        #endregion

        #region Service Code Public Methods
        //Get shipping by shipping id.
        public virtual ShippingServiceCodeModel GetShippingServiceCode(int shippingServiceCodeId) => (_shippingServiceCodeRepository.GetById(shippingServiceCodeId)).ToModel<ShippingServiceCodeModel>();

        //Get paged shipping list
        public virtual ShippingServiceCodeListModel GetShippingServiceCodeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //maps the entity list to model
            IList<ZnodeShippingServiceCode> shippingServiceCodeList = _shippingServiceCodeRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("shippingServiceCodeList count ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingServiceCodeList?.Count);

            ShippingServiceCodeListModel listModel = new ShippingServiceCodeListModel();
            listModel.ShippingServiceCodes = shippingServiceCodeList?.Count > 0 ? shippingServiceCodeList.ToModel<ShippingServiceCodeModel>().ToList() : new List<ShippingServiceCodeModel>();

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return listModel;
        }

        #endregion

        #region Shipping Rule

        //Get shipping rule list.
        public virtual ShippingRuleListModel GetShippingRuleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //get expands
            var navigationProperties = GetExpands(expands);

            //Gets the entity list according to where clause, order by clause and pagination
            IList<ZnodeShippingRule> associatedRules = _shippingRuleRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, navigationProperties, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedRules count ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, associatedRules?.Count);

            ShippingRuleListModel shippingRuleListModel = new ShippingRuleListModel { ShippingRuleList = associatedRules?.ToModel<ShippingRuleModel>().ToList() };
            shippingRuleListModel?.ShippingRuleList?.ForEach(item => { item.ShippingRuleTypeCodeLocale = GetDefaultValueLocale(item); });
            ZnodeLogging.LogMessage("ShippingRuleList count ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingRuleListModel?.ShippingRuleList?.Count);
            shippingRuleListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            return shippingRuleListModel;
        }



        //Get shipping rule by shipping rule id.
        public virtual ShippingRuleModel GetShippingRule(int shippingRuleId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters shippingRuleId ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingRuleId);

            if (shippingRuleId > 0)
                return _shippingRuleRepository.GetById(shippingRuleId).ToModel<ShippingRuleModel>();

            return null;
        }

        //Add shipping rule.
        public virtual ShippingRuleModel AddShippingRule(ShippingRuleModel shippingRuleModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            if (IsNull(shippingRuleModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (IsShippingRuleExist(shippingRuleModel?.ShippingId, shippingRuleModel.ShippingRuleTypeCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist,string.Format(Admin_Resources.ShippingRuleAlreadyExist, shippingRuleModel.ShippingRuleTypeCode));
                  
            if (Equals(shippingRuleModel.ShippingRuleTypeCode, ZnodeConstant.FlatRatePerItem) || Equals(shippingRuleModel.ShippingRuleTypeCode, ZnodeConstant.FixedRatePerItem))
            {
                shippingRuleModel.LowerLimit = null;
                shippingRuleModel.UpperLimit = null;
            }

            shippingRuleModel = _shippingRuleRepository.Insert(shippingRuleModel?.ToEntity<ZnodeShippingRule>())?.ToModel<ShippingRuleModel>();
            ZnodeLogging.LogMessage("shippingRuleModel inserted with id ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingRuleModel?.ShippingRuleId);

            ZnodeLogging.LogMessage(IsNotNull(shippingRuleModel) ? string.Format(Admin_Resources.SuccessShippingRuleAdd, shippingRuleModel?.ShippingRuleTypeCode) : string.Format(Admin_Resources.ErrorShippingRuleAdd, shippingRuleModel.ShippingRuleTypeCode), ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            
            return shippingRuleModel;
        }

        //Update shipping rule.
        public virtual bool UpdateShippingRule(ShippingRuleModel shippingRuleModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("shippingRuleModel with ShippingRuleId,ShippingRuleTypeCode ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose,new object[] { shippingRuleModel?.ShippingRuleId });

            bool status = false;
            if (IsNull(shippingRuleModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (Equals(shippingRuleModel.ShippingRuleTypeCode, ZnodeConstant.FlatRatePerItem) || Equals(shippingRuleModel.ShippingRuleTypeCode, ZnodeConstant.FixedRatePerItem))
            {
                shippingRuleModel.LowerLimit = null;
                shippingRuleModel.UpperLimit = null;
            }

            if (shippingRuleModel?.ShippingRuleId > 0)
                status = _shippingRuleRepository.Update(shippingRuleModel.ToEntity<ZnodeShippingRule>());
            
            ZnodeLogging.LogMessage(status ? string.Format(Admin_Resources.SuccessAssociatedRuleUpdate, shippingRuleModel.ShippingRuleTypeCode) : string.Format(Admin_Resources.ErrorAssociatedRuleUpdate, shippingRuleModel.ShippingRuleTypeCode), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return status;
        }

        //Delete shipping rule.
        public virtual bool DeleteShippingRule(ParameterModel shippingRuleId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Shipping rule Ids to be deleted ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { shippingRuleId });

            bool status = false;
            if (shippingRuleId.Ids.Count() < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeShippingRuleEnum.ShippingRuleId.ToString(), ProcedureFilterOperators.In, shippingRuleId.Ids.ToString()));

            status = _shippingRuleRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(status ? string.Format(Admin_Resources.SuccessAssociatedRuleDelete, shippingRuleId.Ids.ToString()) : string.Format(Admin_Resources.ErrorAssociatedRuleDelete, shippingRuleId.Ids.ToString()), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            
            return status;
        }

        #endregion

        #region Service Rule Type

        //Get shipping rule type list
        public virtual ShippingRuleTypeListModel GetShippingRuleTypeList(FilterCollection filters, NameValueCollection sorts)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            int shippingId = Convert.ToInt32(filters.Where(x => x.FilterName.Equals(ZnodeShippingEnum.ShippingId.ToString().ToLower()))?.Select(y => y.FilterValue)?.FirstOrDefault());

            filters.RemoveAll(x => x.Item1 == ZnodeShippingEnum.ShippingId.ToString().ToLower());

            List<string> associatedRuleTypeCodeList = GetAssociatedShippingRuleTypeCodes(shippingId);
            ZnodeLogging.LogMessage("associatedRuleTypeCodeList count returned from GetAssociatedShippingRuleTypeCodes.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, associatedRuleTypeCodeList?.Count);

            associatedRuleTypeCodeList?.ForEach(item => { filters.Add(new FilterTuple(ZnodeShippingRuleEnum.ShippingRuleTypeCode.ToString(), FilterOperators.NotEquals, item)); });

            ShippingRuleTypeListModel listModel = new ShippingRuleTypeListModel();
            BrandListModel shippingRuleTypeList = GetService<IBrandService>().GetBrandCodes(ZnodeConstant.ShippingCost);
            listModel.ShippingRuleTypeList = shippingRuleTypeList?.BrandCodes?.Where(e => !associatedRuleTypeCodeList.Contains(e.AttributeDefaultValueCode))?.ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return listModel;
        }

        #endregion

        #region Portal/Profile Shipping
        //Get list of unassociated Portal/Profile shipping list.
        public virtual ShippingListModel GetAssociatedShippingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            int portalId = 0;
            int profileId = 0;

            ShippingListModel shippingListModel = new ShippingListModel();
            if (filters.Any(x => string.Equals(x.FilterName, FilterKeys.ProfileId, StringComparison.CurrentCultureIgnoreCase)))
            {
                profileId = GetProfileId(filters);
                ZnodeLogging.LogMessage("profileId returned from GetProfileId method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, profileId);
                shippingListModel.ProfileName = GetProfileName(profileId);
                ZnodeLogging.LogMessage("ProfileName returned from GetProfileName method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingListModel.ProfileName);

            }
            else
            {
                portalId = GetPortalId(filters);
                ZnodeLogging.LogMessage("portalId returned from GetPortalId method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, profileId);
                shippingListModel.PortalName = GetPortalName(portalId);
                ZnodeLogging.LogMessage("portalId returned from GetProfileName method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingListModel.PortalName);

            }

            //Remove PortalId/ProfileId filters.
            RemovePortalIdProfileIdFilters(filters);
            ZnodeLogging.LogMessage("RemovePortalIdProfileIdFilters method call executed", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose,null);

            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);
            ZnodeLogging.LogMessage("CheckUserPortalAccess method call executed", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, null);


            IZnodeViewRepository<ShippingModel> objStoredProc = new ZnodeViewRepository<ShippingModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", true, ParameterDirection.Input, DbType.Int32);

            IList<ShippingModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT, @ProfileId,@PortalId, @UserId,@IsAssociated", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Shipping list count", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, list.Count);

            shippingListModel.ShippingList = list?.Count > 0 ? list?.ToList() : new List<ShippingModel>();

            shippingListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingListModel;
        }

        //Get list of unassociated shipping list to Portal/Profile.
        public virtual ShippingListModel GetUnAssociatedShippingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            int portalId = 0;
            int profileId = 0;

            if (filters.Any(x => string.Equals(x.FilterName, FilterKeys.ProfileId, StringComparison.CurrentCultureIgnoreCase)))
            {
                profileId = GetProfileId(filters);
                ZnodeLogging.LogMessage("profileId returned from GetProfileId method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, profileId);

            }
            else
            {
                portalId = GetPortalId(filters);
                ZnodeLogging.LogMessage("portalId returned from GetPortalId method ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, profileId);

            }


            //Remove PortalId/ProfileId filters.
            RemovePortalIdProfileIdFilters(filters);
            ZnodeLogging.LogMessage("RemovePortalIdProfileIdFilters method call executed", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, null);

            IZnodeViewRepository<ShippingModel> objStoredProc = new ZnodeViewRepository<ShippingModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", false, ParameterDirection.Input, DbType.Int32);

            IList<ShippingModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetShippingList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT, @ProfileId, @PortalId, @UserId,@IsAssociated", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Shipping list count", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, list.Count);
            ShippingListModel shippingListModel = new ShippingListModel();
            shippingListModel.ShippingList = list?.Count > 0 ? list?.ToList() : new List<ShippingModel>();

            shippingListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingListModel;
        }

        //Method to associate shipping to Portal/Profile.
        public virtual bool AssociateShipping(PortalProfileShippingModel portalProfileShippingModel)
        {
            bool status = false;
            bool isProfileShippingAlreadyExist = false;
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            if (IsNull(portalProfileShippingModel))
            throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.PortalProfileShippingModelNotLessThanZero);

            string[] shippingIds = portalProfileShippingModel.ShippingIds?.Split(',');
            ZnodeLogging.LogMessage("Shipping Ids to be associated :", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingIds);

            if (IsInValidShippingIds(shippingIds))
                throw new ZnodeException(ErrorCodes.InvalidData,Api_Resources.AssociateShippingIdNotNullOrEmpty);
            
            //If profile id is greater associate shipping to profile else to portal.
            if (portalProfileShippingModel.ProfileId > 0)
            {
                //getting Existing Associated Shipping Data from request
                List<int> associatedProfileShippingIds = GetAssociatedShippingByProfile(portalProfileShippingModel.ProfileId, shippingIds);

                //setting flag if Shipping profile already Exist
                if (associatedProfileShippingIds.Count > 0) isProfileShippingAlreadyExist = true;

                //getting New Associated Shipping Data which need to insert into DB
                shippingIds = GetUnAssociateShippingIds(associatedProfileShippingIds, shippingIds);

                //checking if shipping Id's is null then All shipping Id's Already Exist else insert to Database
                if (shippingIds.Length == 0) {
                    throw new ZnodeException(ErrorCodes.AlreadyExist,Api_Resources.ShippingIdExist);
                }
                    status = AssociateShippingToProfile(portalProfileShippingModel, shippingIds);
                               
                if (isProfileShippingAlreadyExist && status)
                {
                    throw new ZnodeException(ErrorCodes.AlreadyExist, string.Format(Api_Resources.AssociateShippingId, string.Join(",", associatedProfileShippingIds), string.Join(",", shippingIds)));
                }
                      return status;
                }
            else
                return AssociateShippingToPortal(portalProfileShippingModel, shippingIds);
        }

        //Method to get Existing Associated Shipping Data by Profile
        private bool IsInValidShippingIds(string[] shippingIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("IsValidShippingIds: validating requesting Shipping ids", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, null);
            bool status = false;
            if (shippingIds == null) return status = true;

            //to check the "" for Array index[] values
            if (shippingIds.Any(x => string.IsNullOrEmpty(x))) status = true;
            return status;
        }

        //Method to get Existing Associated Shipping Data by Profile
        protected virtual List<int> GetAssociatedShippingByProfile(int profileId, string[] shippingIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("GetAssociatedShippingByProfile having ProfileId , ShippingIds :", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose,null);

            List<int> associatedDataList = new List<int>();

            List<int?> entityList = _znodeProfileShippingRepository.Table.Where(x => x.ProfileId == profileId).Select(x => x.ShippingId ).ToList();

            foreach (string shippingId in shippingIds)
            {
                int intTypeShippingId = Convert.ToInt32(shippingId);
                if (entityList.Contains(intTypeShippingId)) associatedDataList.Add(intTypeShippingId);
            }
            return associatedDataList;
        }

        //Method to get Existing Associated Shipping Data by Portal ID
        protected virtual List<int> GetAssociatedShippingByPortal(int portalId, string[] shippingIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("GetAssociatedShippingByProfile having PortalId , ShippingIds :", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, null);

            List<int> associatedDataList = new List<int>();

            List<int?> entityList = _znodePortalShippingRepository.Table.Where(x => x.PortalId == portalId).Select(x => x.ShippingId).ToList();

            foreach (string shippingId in shippingIds)
            {
                int intTypeShippingId = Convert.ToInt32(shippingId);
                if (entityList.Contains(intTypeShippingId)) associatedDataList.Add(intTypeShippingId);
            }
            return associatedDataList;
        }

        //Method to get Un-Associated Shipping Data which need to insert into DB
        protected virtual string[] GetUnAssociateShippingIds(List<int> associatedProfileShippingIds, string[] shippingIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("GetUnAssociateShippingIds having associatedProfileShippingIds , ShippingIds :", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, null);

            List<int> unAssociatedShippingIds = new List<int>();

            foreach (string shippingId in shippingIds)
            {
                    int intTypeShippingId = Convert.ToInt32(shippingId);
                    if (!associatedProfileShippingIds.Contains(intTypeShippingId)) unAssociatedShippingIds.Add(intTypeShippingId);
            }
            string[] newShippingIds = unAssociatedShippingIds.Select(x => x.ToString()).ToArray();
            return newShippingIds;
        }

        //Remove associated shipping to Portal/Profile.
        public virtual bool UnAssociateAssociatedShipping(PortalProfileShippingModel portalProfileShippingModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("portalProfileShippingModel having ShippingId,ProfileId :", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { portalProfileShippingModel?.ShippingIds, portalProfileShippingModel?.ProfileId });

            if (string.IsNullOrEmpty(portalProfileShippingModel?.ShippingIds))
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.PortalProfileShippingModelNotLessThanOne);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeProfileShippingEnum.ShippingId.ToString(), ProcedureFilterOperators.In, portalProfileShippingModel.ShippingIds));

            //If profile id is greater unassociate shipping associated to profile else associated to portal.
            if (portalProfileShippingModel.ProfileId > 0)
            {
                filter.Add(ZnodeProfileShippingEnum.ProfileId.ToString(), FilterOperators.Equals, portalProfileShippingModel.ProfileId.ToString());
                return _znodeProfileShippingRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            }
            else
            {
                filter.Add(ZnodePortalShippingEnum.PortalId.ToString(), FilterOperators.Equals, portalProfileShippingModel.PortalId.ToString());
                return _znodePortalShippingRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            }
        }
        #endregion

        //Check whether the address is valid or not.
        public virtual BooleanModel IsShippingAddressValid(AddressModel addressModel)
            => new UspsAgent().IsAddressValid(addressModel, addressModel?.PortalId > 0 ? addressModel.PortalId : PortalId);

        //Get recommended address
        public virtual AddressListModel RecommendedAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            if (Equals(addressModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.AddressModelNotNull);

            if (addressModel.PortalId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PortalIdNotLessThanOne);

            if (ZnodeConfigManager.GetSiteConfigFeatureValue(addressModel.PortalId, ZnodeConstant.AddressValidation))
            {
                return new UspsAgent().RecommendedAddress(addressModel, addressModel?.PortalId > 0 ? addressModel.PortalId : PortalId);
            }
            else
            {
                return null;
            }
        }

        // Update associated shipping. 
        public bool UpdateShippingToPortal(PortalProfileShippingModel portalProfileShippingModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("ProfileShippingId for portalProfileShippingModel ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, portalProfileShippingModel?.ProfileShippingId);

            int shippingId = Convert.ToInt32(portalProfileShippingModel.ShippingIds);
            ZnodeLogging.LogMessage("shippingId :", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, shippingId);

            ZnodePublishStatesEnum PublishStateEnum = (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), portalProfileShippingModel.PublishState, true);

            if (_znodePortalShippingRepository.Table.Any(x => x.PortalId == portalProfileShippingModel.PortalId && x.ShippingId == shippingId))
            {
                try
                {
                    ZnodePortalShipping znodePortalShipping = _znodePortalShippingRepository.Table.FirstOrDefault(x => x.PortalId == portalProfileShippingModel.PortalId && x.ShippingId == shippingId);
                    znodePortalShipping.PublishStateId = (byte)PublishStateEnum;
                    _znodePortalShippingRepository.Update(znodePortalShipping);
                    return true;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdateShippingToPortal, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error, ex);
                    throw new ZnodeException(ErrorCodes.AssociationUpdateError, ex.Message);
                }
            }
            return false;
        }
        //Update profile shipping.
        public virtual bool UpdateProfileShipping(PortalProfileShippingModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (model.PortalShippingId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            if (model.ProfileId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            ZnodeLogging.LogMessage("PortalProfileShippingModel with ProfileId ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, model?.ProfileId);

            if (model.ProfileId > 0)
            {
                ZnodeProfileShipping profileshipping = _znodeProfileShippingRepository.Table.Where(x => x.ShippingId == model.PortalShippingId && x.ProfileId == model.ProfileId)?.FirstOrDefault();
                ZnodePublishStatesEnum PublishStateEnum = (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), model.PublishState, true);
                profileshipping.PublishStateId = (byte)PublishStateEnum;
                profileshipping.DisplayOrder = model.DisplayOrder;
                _znodeProfileShippingRepository.Update(profileshipping);
                return true;
            }
            return false;
        }

        public bool ValidateRecommendedAddressModel(AddressModel model)
        {
            if (string.IsNullOrEmpty(model.Address1) || string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.CountryName) || string.IsNullOrEmpty(model.StateName) || string.IsNullOrEmpty(model.CityName) || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.PostalCode))
                return false;
            else
                return true;
        }
        #endregion

        #region Private Method

        //Check Shipping Destination Filter exist if exist then remove it from Filter condition
        private void CheckShippingDestinationFilter(FilterCollection filters, ref string countryCode, ref string stateCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters countryCode, stateCode.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose,new object[] { countryCode, stateCode });

            string shippingDestinationCountryCode = filters.Find(x => string.Equals(x.FilterName, FilterKeys.ShippingDestinationCountryCode.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue;
            string shippingDestinationStateCode = filters.Find(x => string.Equals(x.FilterName, FilterKeys.ShippingDestinationStateCode.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue;
            ZnodeLogging.LogMessage("shippingDestinationCountryCode, shippingDestinationStateCode", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { shippingDestinationCountryCode, shippingDestinationStateCode });

            //Shipping Destination CountryCode is present in filter then remove and assign its value to countryCode.
            if (!string.IsNullOrEmpty(shippingDestinationCountryCode))
            {
                filters.Remove(filters.Where(filterTuple => string.Equals(filterTuple.Item1, FilterKeys.ShippingDestinationCountryCode.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FirstOrDefault());
                countryCode = shippingDestinationCountryCode;
            }

            //Shipping Destination StateCode is present in filter then remove and assign its value to stateCode.
            if (!string.IsNullOrEmpty(shippingDestinationStateCode))
            {
                filters.Remove(filters.Where(filterTuple => string.Equals(filterTuple.Item1, FilterKeys.ShippingDestinationStateCode.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FirstOrDefault());
                stateCode = GetStateCode(shippingDestinationStateCode);
                ZnodeLogging.LogMessage("stateCode returned from GetStateCode method", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { stateCode });

            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

        }

        // Get state code by stateName
        private string GetStateCode(string stateName)
        {
            stateName = !string.IsNullOrEmpty(stateName) ? stateName.Trim() : null;
            if ((stateName?.Length > 3))
            {
                //to state code by stateName
                string stateCode = Convert.ToString((from state in _stateRepository.Table
                                                     where state.StateName == stateName
                                                     select state.StateCode).FirstOrDefault());
                return stateCode ?? string.Empty;
            }
            return stateName ?? string.Empty;
        }

        //Get Shipping methods By CountryCode  & StateCode filter 
        private List<ShippingModel> GetShippingByCountryAndStateCode(string countryCode, string stateCode, List<ShippingModel> shippinglist)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippinglist?.Where(shipping => (string.Equals(shipping.DestinationCountryCode, countryCode, StringComparison.CurrentCultureIgnoreCase) || shipping.DestinationCountryCode == null)
              && (string.Equals(shipping.StateCode, stateCode, StringComparison.CurrentCultureIgnoreCase) || shipping.StateCode == null))?.ToList() ?? new List<ShippingModel>();
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodeShippingEnum.ZnodeShippingType.ToString().ToLower())) SetExpands(ZnodeShippingEnum.ZnodeShippingType.ToString(), navigationProperties);
                    if (Equals(key, ZnodeProfileShippingEnum.ZnodeShipping.ToString().ToLower())) SetExpands(ZnodeProfileShippingEnum.ZnodeShipping.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Checks if same shipping rule exists or not
        private bool IsShippingRuleExist(int? shippingID, string shippingRuleTypeCode)
           => _shippingRuleRepository.Table.Any(x => x.ShippingId == shippingID && x.ShippingRuleTypeCode == shippingRuleTypeCode);

        //Check if shipping is already exists or not.
        private bool IsShippingAlreadyExist(int? shippingTypeId, int? profileId, string ShippingServiceCode, string DestinationCountryCode, string StateCode, string CountyFIPS)
        {
            ZnodeLogging.LogMessage("Check if shipping already exists", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, null);
            return _shippingRepository.Table.Any(x => x.ShippingTypeId == shippingTypeId && x.ShippingCode == ShippingServiceCode && x.DestinationCountryCode == DestinationCountryCode && x.StateCode == StateCode && x.CountyFIPS == CountyFIPS);
        }

        //Check if shipping Already exists on update
        private bool IsShippingExistsOnUpdate(int? shippingTypeId, int? profileId, int? shippingId, string ShippingServiceCode, string DestinationCountryCode, string StateCode, string CountyFIPS)
        {
            List<ZnodeShipping> entityList = _shippingRepository.Table.Where(x => x.ShippingTypeId == shippingTypeId && x.ShippingCode == ShippingServiceCode && x.DestinationCountryCode == DestinationCountryCode && x.StateCode == StateCode && x.CountyFIPS == CountyFIPS).Select(x => x).ToList();

            if (entityList.Count == 0) return false;

            if (entityList.Count == 1 && entityList.ToList()[0].ShippingId == shippingId) return false;

            return true;
        }

        //Insert tax class SKUs.
        private void InsertTaxClassSku(ShippingModel shippingModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            string[] skus = shippingModel.ImportedSkus.Split(',');

            ShippingSKUListModel shippingSkuListModel = new ShippingSKUListModel();
            shippingSkuListModel.ShippingSKUList = new List<ShippingSKUModel>();
            GetValidImportedSKUs(shippingModel, skus, shippingSkuListModel);

            IEnumerable<ZnodeShippingSKU> list = null;
            if (shippingSkuListModel?.ShippingSKUList?.Count > 0)
                list = _shippingSKURepository.Insert(shippingSkuListModel.ShippingSKUList.ToEntity<ZnodeShippingSKU>().ToList());
        }

        //Get valid imported SKUs.
        private void GetValidImportedSKUs(ShippingModel shippingModel, string[] skus, ShippingSKUListModel shippingSKUList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            //Get list of valid SKUs.
            string[] validSkus = _pimAttributeValueLocaleRepository.Table
           .Join(_pimAttributeValueRepository.Table, attributeValueLocale => attributeValueLocale.PimAttributeValueId, attributeValue => attributeValue.PimAttributeValueId, (attributeValueLocale, attributeValue) => new { attributeValueLocale, attributeValue })
           .Join(_pimAttributeRepository.Table, attributeValueInfo => attributeValueInfo.attributeValue.PimAttributeId, attribute => attribute.PimAttributeId, (attributeValueInfo, attribute) => new { attributeValueInfo, attribute })
           .Where(m => m.attribute.AttributeCode == ZnodeConstant.ProductSKU).Select(m => m.attributeValueInfo.attributeValueLocale.AttributeValue).ToArray();

            foreach (string sku in skus)
            {
                //check if the sku is valid and does not already exists.
                if (validSkus.Any(x => x == sku) && (!_shippingSKURepository.Table.Any(x => x.SKU == sku && x.ShippingRuleId == shippingModel.ShippingId)))
                    shippingSKUList.ShippingSKUList.Add(new ShippingSKUModel { SKU = sku, ShippingRuleId = shippingModel.ShippingId });
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

        }

        //Get associated shipping rule type Ids.
        private List<string> GetAssociatedShippingRuleTypeCodes(int shippingId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter shippingId ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new object[] { shippingId });

            //Create filter for znodeShippingRule
            FilterCollection shippingRuleFilter = new FilterCollection { new FilterTuple(ZnodeShippingRuleEnum.ShippingId.ToString(), FilterOperators.Equals, shippingId.ToString()) };
            EntityWhereClauseModel shippingRuleWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(shippingRuleFilter.ToFilterDataCollection());

            return _shippingRuleRepository.GetEntityList(shippingRuleWhereClauseModel.WhereClause).Select(item => item.ShippingRuleTypeCode).ToList();
        }

        //Get default locale.
        private string GetDefaultValueLocale(ShippingRuleModel shippingRules)
        {
            PIMAttributeDefaultValueModel pimAttributeDefaultValueModel = GetService<IBrandService>().GetBrandCodes(ZnodeConstant.ShippingCost).BrandCodes?.Where(x => x.AttributeDefaultValueCode.Equals(shippingRules?.ShippingRuleTypeCode))?.FirstOrDefault();
            string defaultValueLocale = pimAttributeDefaultValueModel?.ValueLocales?.FirstOrDefault(locale => locale.LocaleId == GetDefaultLocaleId())?.DefaultAttributeValue;
            return string.IsNullOrEmpty(defaultValueLocale) ? pimAttributeDefaultValueModel?.AttributeDefaultValueCode : defaultValueLocale;
        }

        //Get profileId from filter.
        private int GetProfileId(FilterCollection filters)
                => Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, FilterKeys.ProfileId, StringComparison.CurrentCultureIgnoreCase)).FilterValue);

        //Get portalId from filter.
        private int GetPortalId(FilterCollection filters)
               => Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.CurrentCultureIgnoreCase)).FilterValue);

        //Remove PortalId/ProfileId filters.
        private void RemovePortalIdProfileIdFilters(FilterCollection filters)
        {
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.CurrentCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.ProfileId, StringComparison.CurrentCultureIgnoreCase));
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
            return profileId;
        }

        //Associate Shipping to Profile.
        private bool AssociateShippingToProfile(PortalProfileShippingModel portalProfileShippingModel, string[] shippingIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            List<ZnodeProfileShipping> shippingIdsToAdd = new List<ZnodeProfileShipping>();

            foreach (string shippingId in shippingIds)
                shippingIdsToAdd.Add(new ZnodeProfileShipping() { ProfileId = portalProfileShippingModel.ProfileId, ShippingId = Convert.ToInt32(shippingId) });

            return _znodeProfileShippingRepository.Insert(shippingIdsToAdd)?.Count() > 0;
        }

        
        //Associate Shipping to Portal.
        private bool AssociateShippingToPortal(PortalProfileShippingModel portalProfileShippingModel, string[] shippingIds)
        {
            bool isPortalShippingAlreadyExist = false;
            bool status = false;

            List<int> associatedPortalShippingIds = GetAssociatedShippingByPortal(portalProfileShippingModel.PortalId, shippingIds);
            //setting flag if Shipping portal already Exist
            if (associatedPortalShippingIds.Count > 0) isPortalShippingAlreadyExist = true;

            //getting New Associated Shipping Data which need to insert into DB
            shippingIds = GetUnAssociateShippingIds(associatedPortalShippingIds, shippingIds);

            //checking if shipping Id's is null then All shipping Id's Already Exist else insert to Database
            if (shippingIds.Length == 0)
            {
                throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.ShippingIdExist);
            }
            //insert into database if new shipping id's available
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            List<ZnodePortalShipping> shippingIdsToAdd = new List<ZnodePortalShipping>();
            ZnodePublishStatesEnum publishState = (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), portalProfileShippingModel.PublishState, true);

            foreach (string shippingId in shippingIds)
                shippingIdsToAdd.Add(new ZnodePortalShipping() { PortalId = portalProfileShippingModel.PortalId, ShippingId = Convert.ToInt32(shippingId), PublishStateId = (byte)publishState });

            status = _znodePortalShippingRepository.Insert(shippingIdsToAdd)?.Count() > 0;

            if (isPortalShippingAlreadyExist && status)
            {
                throw new ZnodeException(ErrorCodes.AlreadyExist, string.Format(Api_Resources.AssociateShippingId, string.Join(",", associatedPortalShippingIds), string.Join(",", shippingIds)));
            }

            return status;
        }

        //Get Profile name on the basis of profile id.
        private string GetProfileName(int profileId)
        {
            if (profileId > 0)
            {
                IZnodeRepository<ZnodeProfile> _znodeProfile = new ZnodeRepository<ZnodeProfile>();
                return _znodeProfile.Table.FirstOrDefault(x => x.ProfileId == profileId)?.ProfileName;
            }
            return string.Empty;
        }

        //Get Portal name on the basis of portal id.
        private string GetPortalName(int portalId)
        {
            if (portalId > 0)
            {
                IZnodeRepository<ZnodePortal> _znodePortal = new ZnodeRepository<ZnodePortal>();
                return _znodePortal.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName;
            }
            return string.Empty;
        }


        #endregion
    }
}
