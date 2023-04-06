using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
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
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;
using System.Data;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Caching.Events.Models;

namespace Znode.Engine.Services
{
    public partial class PortalService : BaseService, IPortalService
    {
        #region Private Variables
        private IZnodeRepository<ZnodePortalPageSetting> _portalPageSettingRepository;
        private IZnodeRepository<ZnodePageSetting> _pageSettingRepository;
        private IZnodeRepository<ZnodePortalSortSetting> _portalSortSettingRepository;
        private IZnodeRepository<ZnodeSortSetting> _sortSettingRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodePortalFeature> _portalFeatureRepository;
        private readonly IZnodeRepository<ZnodePortalFeatureMapper> _portalFeatureMapperRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodeWarehouse> _warehouseRepository;
        private readonly IZnodeRepository<ZnodePortalWarehouse> _portalWarehouseRepository;
        private readonly IZnodeRepository<ZnodePortalAlternateWarehouse> _portalAlternateWarehouseRepository;
        private readonly IZnodeRepository<ZnodeCMSPortalTheme> _cmsPortalTheme;
        private readonly IZnodeRepository<ZnodeCMSTheme> _cmsTheme;
        private readonly IZnodeRepository<ZnodePortalSmtpSetting> _portalSmtpSetting;
        private readonly IZnodeRepository<ZnodePortalLocale> _portalLocaleRepository;
        private readonly IZnodeRepository<ZnodeLocale> _locales;
        private readonly IZnodeRepository<ZnodePortalProfile> _portalProfileRepository;
        private readonly IZnodeRepository<ZnodeCurrency> _currencyRepository;
        private readonly IZnodeRepository<ZnodePortalRecommendationSetting> _recommendationRepository;
        private readonly IZnodeRepository<ZnodePortalUnit> _portalUnitRepository;
        private readonly IZnodeRepository<ZnodeShippingPortal> _shippingPortalRepository;
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        private readonly IZnodeRepository<ZnodePortalTaxClass> _taxClassPortalRepository;
        private readonly IZnodeRepository<ZnodeTaxPortal> _taxPortalRepository;
        private readonly IZnodeRepository<ZnodePortalPixelTracking> _portalPixelTracking;
        private readonly IZnodeRepository<ZnodeGoogleTagManager> _tagManager;
        private readonly IZnodeRepository<ZnodeTaxClass> _taxClassRepository;
        private readonly IZnodeRepository<ZnodeRobotsTxt> _robotsTxtRepository;
        private readonly IZnodeRepository<ZnodeCulture> _cultureRepository;
        private readonly IZnodeRepository<ZnodePortalApproval> _portalApprovalRepository;
        private readonly IZnodeRepository<ZnodeUserApprover> _userApproverRepository;
        private readonly IZnodeRepository<ZnodePortalApprovalType> _portalApprovalTypeRepository;
        private readonly IZnodeRepository<ZnodePortalApprovalLevel> _portalApprovalLevelRepository;
        private readonly IZnodeRepository<ZnodePortalPaymentGroup> _portalPaymentGroupRepository;
        private readonly IZnodeRepository<ZnodePortalPaymentApprover> _portalPaymentApproversRepository;
        private readonly IZnodeRepository<ZnodePublishPortalLog> _publishPortalLogRepository;
        private readonly IZnodeRepository<ZnodePublishWebstoreEntity> _publishWebstoreEntity;

        private readonly IZnodeRepository<ZnodePublishPortalCustomCssEntity> _publishPortalCustomCss;


        #endregion

        #region Constructor
        public PortalService()
        {
            _pageSettingRepository = new ZnodeRepository<ZnodePageSetting>();
            _portalPageSettingRepository = new ZnodeRepository<ZnodePortalPageSetting>();
            _sortSettingRepository = new ZnodeRepository<ZnodeSortSetting>();
            _portalSortSettingRepository = new ZnodeRepository<ZnodePortalSortSetting>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _portalFeatureRepository = new ZnodeRepository<ZnodePortalFeature>();
            _portalFeatureMapperRepository = new ZnodeRepository<ZnodePortalFeatureMapper>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _warehouseRepository = new ZnodeRepository<ZnodeWarehouse>();
            _portalWarehouseRepository = new ZnodeRepository<ZnodePortalWarehouse>();
            _portalAlternateWarehouseRepository = new ZnodeRepository<ZnodePortalAlternateWarehouse>();
            _cmsPortalTheme = new ZnodeRepository<ZnodeCMSPortalTheme>();
            _cmsTheme = new ZnodeRepository<ZnodeCMSTheme>();
            _portalSmtpSetting = new ZnodeRepository<ZnodePortalSmtpSetting>();
            _portalLocaleRepository = new ZnodeRepository<ZnodePortalLocale>();
            _locales = new ZnodeRepository<ZnodeLocale>();
            _portalProfileRepository = new ZnodeRepository<ZnodePortalProfile>();
            _currencyRepository = new ZnodeRepository<ZnodeCurrency>();
            _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            _shippingPortalRepository = new ZnodeRepository<ZnodeShippingPortal>();
            _domainRepository = new ZnodeRepository<ZnodeDomain>();
            _taxClassPortalRepository = new ZnodeRepository<ZnodePortalTaxClass>();
            _portalPixelTracking = new ZnodeRepository<ZnodePortalPixelTracking>();
            _taxPortalRepository = new ZnodeRepository<ZnodeTaxPortal>();
            _tagManager = new ZnodeRepository<ZnodeGoogleTagManager>();
            _taxClassRepository = new ZnodeRepository<ZnodeTaxClass>();
            _robotsTxtRepository = new ZnodeRepository<ZnodeRobotsTxt>();
            _cultureRepository = new ZnodeRepository<ZnodeCulture>();
            _portalApprovalRepository = new ZnodeRepository<ZnodePortalApproval>();
            _userApproverRepository = new ZnodeRepository<ZnodeUserApprover>();
            _portalApprovalTypeRepository = new ZnodeRepository<ZnodePortalApprovalType>();
            _portalApprovalLevelRepository = new ZnodeRepository<ZnodePortalApprovalLevel>();
            _portalPaymentGroupRepository = new ZnodeRepository<ZnodePortalPaymentGroup>();
            _portalPaymentApproversRepository = new ZnodeRepository<ZnodePortalPaymentApprover>();
            _publishPortalLogRepository = new ZnodeRepository<ZnodePublishPortalLog>();
            _recommendationRepository = new ZnodeRepository<ZnodePortalRecommendationSetting>();

            _publishPortalCustomCss = new ZnodeRepository<ZnodePublishPortalCustomCssEntity>(HelperMethods.Context);
            _publishWebstoreEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
        }

        #endregion

        #region Public Methods
        public virtual PortalListModel GetPortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //Set Authorized Portal filter based on user portal access.
            BindUserPortalFilter(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("Where condition in GetPortalList method:",ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose,pageListModel?.ToDebugString());
            IZnodeViewRepository<PortalModel> objStoredProc = new ZnodeViewRepository<PortalModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            List<PortalModel> publishPortalLogs = objStoredProc.ExecuteStoredProcedureList("Znode_GetStoreList @WhereClause,@Rows,@PageNo,@Order_By,@UserId,@RowCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("publishPortalLogs list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, publishPortalLogs?.Count());
            ZnodeLogging.LogMessage("Method GetPortalList-PublishPortalLogs list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, publishPortalLogs?.Count());
            PortalListModel publishPortalLogList = new PortalListModel { PortalList = publishPortalLogs };
            publishPortalLogList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return publishPortalLogList;
        }

#if DEBUG
        //Get all portals for select list item
        public virtual PortalListModel GetDevPortalList()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<ZnodePublishWebstoreEntity> webstoreEntity  = GetService<IPublishedPortalDataService>().GetWebstoreEntity();

            return new PortalListModel(){ PortalList = webstoreEntity?.GroupBy(m => m.PortalId).Select(x => x.First())?.ToModel<PortalModel>().ToList() };
        }
#endif
        //Get all portals on Catalog Id
        public virtual PortalListModel GetPortalListByCatalogId(int catalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            var portals = from portalCatalog in _portalCatalogRepository.Table
                          join portal in _portalRepository.Table on portalCatalog.PortalId equals portal.PortalId 
                          where portalCatalog.PublishCatalogId == catalogId
                          select portal;
            ZnodeLogging.LogMessage("Method GetPortalListByCatalogId- Portals:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portals);

            return new PortalListModel()
            {
                PortalList = IsNotNull(portals) ? portals.ToModel<PortalModel>().ToList() : new List<PortalModel>()
            };
        }

        public virtual PortalModel GetPortal(int portalId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, portalId);
            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.PortalIdNotLessThanOne);

            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("Method GetPortal - WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClause);
            PortalModel portal = PortalMap.ToModel(_portalRepository.GetEntity(whereClause, GetExpands(expands)));

            //Set the default Media Server URL.
            SetDefaultServer(portal);

            //Bind Portal Details for Theme,CSS & Publish CatalogId.
            BindPortalDetails(portal);

            //Bind the Portal selected & Available Features.
            BindPortalFeatures(whereClause, portal);

            //Get name of portal of which content pages and manage messages are copied for current portal.
            GetCopyContentPortalName(portal);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portal;
        }

        public virtual PortalModel GetPortal(string storeCode, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("storeCode:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, storeCode);
            if (string.IsNullOrEmpty(storeCode))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.StoreCodeNotNull);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.StoreCode.ToString(), ProcedureFilterOperators.Is, storeCode.ToString()));
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause for generating portal model", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClause);
            PortalModel portal = PortalMap.ToModel(_portalRepository.GetEntity(whereClause.WhereClause, GetExpands(expands), whereClause.FilterValues));

            //Check for User Portal Access.
            CheckUserPortalAccess(portal.PortalId);
            //Set the default Media Server URL.
            SetDefaultServer(portal);
            //Bind Portal Details for Theme,CSS & Publish CatalogId.
            BindPortalDetails(portal);

            filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portal.PortalId.ToString()));
            string whereClauseForFeature = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseForFeature);
            //Bind the Portal selected & Available Features.
            BindPortalFeatures(whereClauseForFeature, portal);

            //Get name of portal of which content pages and manage messages are copied for current portal.
            GetCopyContentPortalName(portal);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portal;
        }

        private string GetPortalDomainName()
        {
            const string headerDomainName = "Znode-DomainName";
            var headers = HttpContext.Current.Request.Headers;
            string domainName = headers[headerDomainName];
            return domainName;
        }

        public virtual bool IsCodeExists(HelperParameterModel parameterModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return _portalRepository.Table.Any(a => a.StoreCode == parameterModel.CodeField);
        }
        public virtual PortalModel CreatePortal(PortalModel portalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(portalModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            //Create new portal.
            ZnodePortal portal = _portalRepository.Insert(portalModel.ToEntity<ZnodePortal>());
            ZnodeLogging.LogMessage("Inserted portal with id ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portal?.PortalId);
            //Save Data into Portal feature table.
            if (portal?.PortalId > 0)
            {    //Save the Portal Associated Details.
                MapPortalDetails(portalModel, portal.PortalId);

                portalModel = PortalMap.ToModel(portal);

            }

            ZnodeLogging.LogMessage(IsNull(portal) ? Admin_Resources.ErrorCreatingPortal : Admin_Resources.SuccessPortalCreatedMessage, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalModel;
        }

        public virtual bool UpdatePortal(PortalModel portalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PortalModel having id:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, portalModel.PortalId);
            if (IsNull(portalModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            if (portalModel.PortalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PortalIdNotLessThanOne);
            if (_portalRepository.Update(portalModel.ToEntity<ZnodePortal>()))

            {
                //Update the Portal Mapping Details like, Portal Features,Catalog, Theme & CSS.
                UpdatePortalMappingDetails(portalModel);

                var websiteDomains = GetWebsiteDomains(portalModel.PortalId);
                var websitePreviewDomains = GetWebsitePreviewDomains(portalModel.PortalId);
                var allWebsiteDomains = websiteDomains.Concat(websitePreviewDomains);

                var portalUpdateEventEntries = allWebsiteDomains.Select(d => new PortalUpdateEventEntry()
                {
                    PortalId = d.PortalId,
                    PortalDomainName = d.DomainName
                });

                ClearCacheHelper.EnqueueEviction(new PortalUpdateEvent()
                {
                    Comment = $"From portal service updating portal with portal id '{portalModel.PortalId}'.",
                    PortalUpdateEventEntries = portalUpdateEventEntries.ToArray()
                });

                return true;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return false;
        }

        public virtual bool DeletePortal(ParameterModel portalIds, bool isDeleteByStoreCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(portalIds?.Ids))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PortalIdNotLessThanOne);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = GetSPParameters(portalIds, isDeleteByStoreCode);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePortalByPortalId @PortalId,@StoreCode,@Status OUT", 2, out status);
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return deleteResult.FirstOrDefault().Status.GetValueOrDefault();

        }
        public virtual IZnodeViewRepository<View_ReturnBoolean> GetSPParameters(ParameterModel portalIds, bool isDeleteByStoreCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            if (isDeleteByStoreCode)
            {
                objStoredProc.SetParameter("PortalId", string.Empty, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("StoreCode", portalIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            }
            else
            {

                objStoredProc.SetParameter("PortalId", portalIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("StoreCode", string.Empty, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return objStoredProc;
        }

        //Copy the existing store.
        public virtual bool CopyStore(PortalModel portalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(portalModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePortalEnum.PortalId.ToString(), portalModel.PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePortalEnum.StoreName.ToString(), portalModel.StoreName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePortalEnum.CompanyName.ToString(), portalModel.CompanyName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePortalEnum.StoreCode.ToString(), portalModel.StoreCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_CopyPortal @PortalId, @StoreName,@CompanyName,@UserId,@StoreCode,@Status OUT", 5, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessCopyingStore, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorCopingStore, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return false;
            }

        }

        //Get the portal features.
        public virtual List<PortalFeatureModel> GetPortalFeatures() => PortalMap.ToPortalFeatureModel(_portalFeatureRepository.GetEntityList(string.Empty));

        #region Inventory Management

        //Get the associated wherehouse list.
        public virtual PortalWarehouseModel GetAssociatedWarehouseList(int portalId, FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.PortalIdNotLessThanOne);

            string mainwhereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("mainwhereClause generated for associatedMainWarehouse:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, mainwhereClause);

            List<string> navigationProperties = GetExpands(expands);

            PortalWarehouseModel portalWarehouseModel = new PortalWarehouseModel();

            ZnodePortalWarehouse associatedMainWarehouse = _portalWarehouseRepository.GetEntity(mainwhereClause, navigationProperties);
            if (IsNotNull(associatedMainWarehouse))
            {
                portalWarehouseModel = associatedMainWarehouse.ToModel<PortalWarehouseModel>();

                filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePortalAlternateWarehouseEnum.PortalWarehouseId.ToString(), FilterOperators.Equals, portalWarehouseModel.PortalWarehouseId.ToString()));
                EntityWhereClauseModel alternateWhereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, alternateWhereClause);
                portalWarehouseModel.AlternateWarehouses = _portalAlternateWarehouseRepository.GetEntityList(alternateWhereClause.WhereClause, navigationProperties, alternateWhereClause.FilterValues).ToModel<PortalAlternateWarehouseModel>().ToList();
            }

            portalWarehouseModel.WarehouseList = _warehouseRepository.GetEntityList("").ToModel<WarehouseModel>().ToList();
            ZnodeLogging.LogMessage("portalWarehouseModel.WarehouseList list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalWarehouseModel.WarehouseList?.Count());
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalWarehouseModel;
        }

        public virtual bool AssociateWarehouseToStore(PortalWarehouseModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, ZnodeConstant.NullModelError);

            //delete previous association
            DeleteAssociatedWarehouse(model);

            ZnodePortalWarehouse mainWarehouse = _portalWarehouseRepository.Insert(model.ToEntity<ZnodePortalWarehouse>());
            ZnodeLogging.LogMessage("Inserted Warehouse with id ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, mainWarehouse.WarehouseId);
            if (IsNotNull(mainWarehouse) && model?.AlternateWarehouses?.Count() > 0)
            {
                model.AlternateWarehouses.ForEach(a => a.PortalWarehouseId = mainWarehouse.PortalWarehouseId);
                IEnumerable<ZnodePortalAlternateWarehouse> alternateWarehouse = _portalAlternateWarehouseRepository.Insert(model.AlternateWarehouses.ToEntity<ZnodePortalAlternateWarehouse>().ToList());
                ZnodeLogging.LogMessage("Inserted Warehouse with id ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, mainWarehouse.WarehouseId);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessWareHouseAssociation, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return true;
        }

        #endregion

        #region Portal Locale
        //Get a list of active locales.
        public virtual LocaleListModel LocaleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<LocaleModel> objStoredProc = new ZnodeViewRepository<LocaleModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<LocaleModel> localeEntityList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPortalLocale @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("localeEntity list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, localeEntityList?.Count());
            LocaleListModel localeListModel = new LocaleListModel { Locales = localeEntityList?.ToList() };
            localeListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return localeListModel;

        }

        //Update Associated Portal Locale Details.
        public virtual bool UpdateLocale(DefaultGlobalConfigListModel globalConfigListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (Equals(globalConfigListModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.GlobalConfigNotNull);
            try
            {
                FilterCollection filterList = new FilterCollection();
                filterList.Add(new FilterTuple(ZnodePortalLocaleEnum.LocaleId.ToString(), ProcedureFilterOperators.In, string.Join(",", globalConfigListModel?.DefaultGlobalConfigs?.Select(x => x.LocaleId)?.ToList())));
                filterList.Add(new FilterTuple(ZnodePortalLocaleEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, globalConfigListModel?.DefaultGlobalConfigs?.Select(y => y.PortalId).FirstOrDefault().ToString()));

                //Gets the where clause with filter values.                 
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);
                List<ZnodePortalLocale> portalLocaleList = _portalLocaleRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
                ZnodeLogging.LogMessage("portalLocaleList list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalLocaleList?.Count());
                bool checkIsDefault = portalLocaleList.Any(x => x.IsDefault);

                //Remove Locale ids from portal locale list to activate locales.
                RemoveLocaleIds(globalConfigListModel, portalLocaleList);

                //Map Parameters for portal locale.
                List<DefaultGlobalConfigModel> DefaultGlobalConfigListModel = MapParametersForPortalLocales(globalConfigListModel, portalLocaleList);
                ZnodeLogging.LogMessage(" Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return UpdateLocaleAsPerAction(globalConfigListModel, whereClauseModel, portalLocaleList, checkIsDefault, DefaultGlobalConfigListModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"{Admin_Resources.ErrorLocaleUpdate}", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        #endregion

        #region Shipping Association
        //Get portal shipping data by portalId.
        public virtual PortalShippingModel GetPortalShippingInformation(int portalId, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId > 0)
            {
                filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));

                string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
                ZnodeLogging.LogMessage(" WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClause);
                ZnodeShippingPortal shippingPortal = _shippingPortalRepository.GetEntity(whereClause);

                if (IsNotNull(shippingPortal))
                    //Decrypt shipping information.
                    DecryptShippingData(shippingPortal);

                return shippingPortal.ToModel<PortalShippingModel>();
            }
            return new PortalShippingModel();
        }

        //Update portal shipping.
        public virtual bool UpdatePortalShipping(PortalShippingModel portalShippingModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(portalShippingModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            //Encrypt Shipping information.
            EncryptShippingData(portalShippingModel);

            bool isPortalShippingUpdated = true;
            //Update or insert portal shipping data on the basis of ShippingPortalId.
            if (portalShippingModel.ShippingPortalId < 1)
            {
                _shippingPortalRepository.Insert(portalShippingModel.ToEntity<ZnodeShippingPortal>());
            }
            else
                isPortalShippingUpdated = _shippingPortalRepository.Update(portalShippingModel.ToEntity<ZnodeShippingPortal>());

            ZnodeLogging.LogMessage(isPortalShippingUpdated ? Admin_Resources.SuccessPortalShipping : Admin_Resources.ErrorPortalShipping , ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return isPortalShippingUpdated;
        }

        #endregion

        #region Tax Association
        //Get portal tax data by portalId.
        public virtual TaxPortalModel GetTaxPortalInformation(int portalId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId > 0)
            {
                List<string> navigationProperties = GetExpandsForPortal(expands);

                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));

                string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
                ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClause);
                ZnodeTaxPortal taxPortal = _taxPortalRepository.GetEntity(whereClause, navigationProperties);

                TaxPortalModel taxPortalModel = taxPortal?.ToModel<TaxPortalModel>();
                if (IsNull(taxPortalModel))
                    return new TaxPortalModel { PortalId = portalId, PortalName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName };

                taxPortalModel.PortalName = taxPortal?.ZnodePortal?.StoreName;
                return taxPortalModel;
            }
            return null;
        }

        //Update portal tax.
        public virtual bool UpdateTaxPortal(TaxPortalModel taxPortalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(taxPortalModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            bool isPortalTaxUpdated = true;
            //Update or insert portal tax data on the basis of TaxPortalId.
            if (taxPortalModel.TaxPortalId < 1)
                _taxPortalRepository.Insert(taxPortalModel.ToEntity<ZnodeTaxPortal>());
            else
                isPortalTaxUpdated = _taxPortalRepository.Update(taxPortalModel.ToEntity<ZnodeTaxPortal>());

            ZnodeLogging.LogMessage(isPortalTaxUpdated ? Admin_Resources.SuccessPortalTax : Admin_Resources.ErrorPortalTax, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return isPortalTaxUpdated;
        }

        #endregion              

        #region Tax
        //Get associate/unassociated tax class to portal.
        public virtual bool AssociateAndUnAssociateTaxClass(TaxClassPortalModel taxClassPortalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (taxClassPortalModel.IsUnAssociated)
                return UnAssociateTaxClass(taxClassPortalModel);
            else
                return AssociateTaxClass(taxClassPortalModel);
        }

        //set default tax class to portal.
        public virtual bool SetPortalDefaultTax(TaxClassPortalModel taxClassPortalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (!IsTaxClassEnabled(Convert.ToInt32(taxClassPortalModel.TaxClassIds)))
                throw new ZnodeException(ErrorCodes.SetDefaultDataError, Admin_Resources.ErrorDefaultTaxUnassigned);

            try
            {
                ResetPortalDefaultTax(taxClassPortalModel);
                return UpdatePortalDefaultTax(taxClassPortalModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #region Portal Tracking Pixel
        //Get portal tracking pixel based on portal id.
        public virtual PortalTrackingPixelModel GetPortalTrackingPixel(int portalId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PortalIdNotLessThanOne);

            FilterCollection filter = new FilterCollection();
            filter.Add(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString());

            PortalTrackingPixelModel portalTrackingPixelModel = _portalPixelTracking.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClause(filter.ToFilterDataCollection()), GetExpands(expands))?.ToModel<PortalTrackingPixelModel>();
            if (IsNull(portalTrackingPixelModel))
                return new PortalTrackingPixelModel() { PortalId = portalId, StoreName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName };
            ZnodeLogging.LogMessage("PortalId and StoreName:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, new object[] { portalTrackingPixelModel.PortalId, portalTrackingPixelModel.StoreName });
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalTrackingPixelModel;
        }

        //Save portal tracking pixel.
        public virtual bool SavePortalTrackingPixel(PortalTrackingPixelModel portalTrackingPixelModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(portalTrackingPixelModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            //If PortalPixelTrackingId is greater than zero Update Portal Tracking Pixel else Create Portal Tracking Pixel. 
            return portalTrackingPixelModel.PortalId > 0 && portalTrackingPixelModel.PortalPixelTrackingId > 0 ?
                _portalPixelTracking.Update(portalTrackingPixelModel?.ToEntity<ZnodePortalPixelTracking>()) :
                _portalPixelTracking.Insert(portalTrackingPixelModel?.ToEntity<ZnodePortalPixelTracking>())?.PortalPixelTrackingId > 0;
        }
        #endregion

        //Get a Portal Publish Status list.
        public virtual PublishPortalLogListModel GetPortalPublishStatus(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            int portalId = 0;
            if (IsNotNull(filters))
                portalId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodePortalEnum.PortalId.ToString().ToLower())?.FirstOrDefault()?.Item3);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<PublishPortalLogModel> objStoredProc = new ZnodeViewRepository<PublishPortalLogModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            List<PublishPortalLogModel> publishPortalLogs = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishPortalStatus @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("publishPortalLogs list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, publishPortalLogs?.Count());
            PublishPortalLogListModel publishPortalLogList = new PublishPortalLogListModel { PublishPortalLogList = publishPortalLogs, StoreName = _portalRepository.GetById(portalId)?.StoreName };
            publishPortalLogList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return publishPortalLogList;
        }

        #region Robots.txt
        //Get robots.txt data on the basis of portal id.
        public virtual RobotsTxtModel GetRobotsTxt(int portalId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PortalIdNotLessThanOne);

            FilterCollection filter = new FilterCollection();
            filter.Add(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString());

            ZnodeRobotsTxt robotTxtEntity = _robotsTxtRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClause(filter.ToFilterDataCollection()), GetExpandsForPortal(expands));

            RobotsTxtModel robotsTxtModel = robotTxtEntity?.ToModel<RobotsTxtModel>();

            if (IsNull(robotsTxtModel))
                return new RobotsTxtModel() { PortalId = portalId, StoreName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName };

            robotsTxtModel.StoreName = robotTxtEntity?.ZnodePortal?.StoreName;
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return robotsTxtModel;
        }

        //Save robots.txt.
        public virtual bool SaveRobotsTxt(RobotsTxtModel robotsTxtModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(robotsTxtModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (robotsTxtModel.PortalId > 0)
            {
                //Update data if RobotsTxtId is greater than 0 else insert data.
                return robotsTxtModel.RobotsTxtId > 0 ? _robotsTxtRepository.Update(robotsTxtModel?.ToEntity<ZnodeRobotsTxt>()) :
                    (IsNotNull(robotsTxtModel.RobotsTxtContent) ? _robotsTxtRepository.Insert(robotsTxtModel?.ToEntity<ZnodeRobotsTxt>())?.RobotsTxtId > 0 : true);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return false;
        }
        #endregion

        #region Approval Routing
        public virtual PortalApprovalModel GetPortalApprovalDetails(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter portalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, portalId);
            PortalApprovalModel portalApprovalModel = _portalApprovalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.ToModel<PortalApprovalModel>();
            if (IsNotNull(portalApprovalModel))
            {
                ZnodeLogging.LogMessage("PortalApprovalTypeName,PortalApprovalId and UserApprover:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalApprovalModel.PortalApprovalTypeName, portalApprovalModel.PortalApprovalId, portalApprovalModel.UserApprover });
                portalApprovalModel.PortalApprovalTypeName = _portalApprovalTypeRepository.Table.FirstOrDefault(x => x.PortalApprovalTypeId == portalApprovalModel.PortalApprovalTypeId)?.ApprovalTypeName;

                if (portalApprovalModel.PortalApprovalId > 0)
                {
                    //if (!Equals(portalApprovalModel.PortalApprovalTypeId, Convert.ToInt32(ZnodePortalApprovalsLevelEnum.Payment + 1)))
                    portalApprovalModel.UserApprover = GetPortalApproverList(portalApprovalModel.PortalApprovalId);
                    // else
                    portalApprovalModel.PortalPaymentUserApproverList = GetPortalPaymentApproverList(portalApprovalModel);
                }
                //return portalApprovalModel;
            }
            else
                portalApprovalModel = new PortalApprovalModel();

            portalApprovalModel.PortalApprovalTypes = _portalApprovalTypeRepository.Table.Select(x => new SelectListItem { Value = x.PortalApprovalTypeId.ToString(), Text = x.ApprovalTypeName, Selected = x.PortalApprovalTypeId == portalApprovalModel.PortalApprovalTypeId ? true : false }).ToList();
            portalApprovalModel.PortalApprovalLevel = _portalApprovalLevelRepository.Table.Select(x => new SelectListItem { Value = x.PortalApprovalLevelId.ToString(), Text = x.ApprovalLevelName, Selected = x.PortalApprovalLevelId == portalApprovalModel.PortalApprovalLevelId ? true : false }).ToList();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalApprovalModel;
        }


        public virtual bool SaveUpdatePortalApprovalDetails(PortalApprovalModel portalApprovalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalApprovalModel properties:PortalApprovalId and PortalId", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, new object[] { portalApprovalModel?.PortalApprovalId, portalApprovalModel.PortalId });
            if (IsNull(portalApprovalModel) && portalApprovalModel.PortalId < 1)
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ApprovalDetailsNotNull);

            bool addedUpdated = false;

            //Save/Update the Portal Approval details 
            portalApprovalModel = SavePortalApprovalDetails(portalApprovalModel);

            if (portalApprovalModel?.PortalApprovalId > 0)
            {
                //Disable all the Active approvers against the portal.
                List<ZnodeUserApprover> listToUpdate = _userApproverRepository.Table.Where(x => x.PortalApprovalId == portalApprovalModel.PortalApprovalId && x.IsActive == true)?.ToList();
                listToUpdate.Select(x => { x.IsActive = false; return x; })?.ToList();
                listToUpdate.ForEach(x => _userApproverRepository.Update(x));

                if (portalApprovalModel.PortalPaymentUserApproverList?.Count > 0)
                {
                    ZnodePortalPaymentGroup portalPaymentGroup = null;
                    List<ZnodePortalPaymentApprover> portalPaymentApproverList = new List<ZnodePortalPaymentApprover>();
                    foreach (var portalPaymentApproval in portalApprovalModel.PortalPaymentUserApproverList)
                    {
                        //Save Portal Payment Group details.
                        if (portalPaymentApproval.PortalPaymentGroupId > 0)
                        {
                            //Delete old Payment approvers.
                            FilterCollection filters = new FilterCollection();
                            filters.Add(new FilterTuple(ZnodePortalPaymentApproverEnum.PortalPaymentGroupId.ToString(), FilterOperators.Equals, portalPaymentApproval.PortalPaymentGroupId.ToString()));
                            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection());
                            ZnodeLogging.LogMessage(" WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClause);
                            _portalPaymentApproversRepository.Delete(whereClause);

                            //Save updated Payment approvers.
                            portalPaymentApproverList = SavePaymentApproverDetails(portalPaymentApproval.PortalPaymentGroupId.GetValueOrDefault(), portalPaymentApproval.PaymentSettingIds);
                            portalPaymentApproval.UserApprover.Select(x => { x.PortalPaymentGroupId = portalPaymentApproval.PortalPaymentGroupId; return x; })?.ToList();
                        }
                        else
                        {
                            //Create the Portal Payment group.
                            portalPaymentGroup = SavePaymentGroupDetails(portalApprovalModel.PortalApprovalId);
                            ZnodeLogging.LogMessage(Admin_Resources.GetPortalPaymentDetails, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                            //Save Portal Payment Approver details.
                            if (portalPaymentGroup?.PortalPaymentGroupId > 0)
                            {
                                portalPaymentApproverList = SavePaymentApproverDetails(portalPaymentGroup.PortalPaymentGroupId, portalPaymentApproval.PaymentSettingIds);
                                portalPaymentApproval.UserApprover.Select(x => { x.PortalPaymentGroupId = portalPaymentGroup.PortalPaymentGroupId; return x; })?.ToList();
                            }
                        }
                        if (portalPaymentApproval.ApprovalUserIds?.Count() > 0)
                            addedUpdated = SaveUserApproversDetails(portalApprovalModel.PortalApprovalId, portalPaymentApproval.UserApprover);

                        ZnodeLogging.LogMessage(Admin_Resources.SuccessPortalPaymentApprovalSave, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                    }
                    if (portalApprovalModel.EnableApprovalManagement.Equals(false))
                    {
                        addedUpdated = true;
                    }
                }
                //Save User Approver details.
                else
                {
                    //Save Portal User Approvers.
                    if (portalApprovalModel.ApprovalUserIds?.Count() > 0)
                    {
                        List<ZnodePortalPaymentGroup> listPortalPaymentGroyupToUpdate = _portalPaymentGroupRepository.Table.Where(y => y.PortalApprovalId == portalApprovalModel.PortalApprovalId)?.ToList();

                        portalApprovalModel.PortalApprovalTypeName = _portalApprovalTypeRepository.Table.FirstOrDefault(x => x.PortalApprovalTypeId == portalApprovalModel.PortalApprovalTypeId)?.ApprovalTypeName;
                        if (portalApprovalModel.PortalApprovalTypeName.Equals(Admin_Resources.TextStore))
                        {
                            listPortalPaymentGroyupToUpdate.Select(y => { y.isActive = false; return y; })?.ToList();
                            listPortalPaymentGroyupToUpdate.ForEach(y => _portalPaymentGroupRepository.Update(y));
                        }
                        addedUpdated = SaveUserApproversDetails(portalApprovalModel.PortalApprovalId, portalApprovalModel.UserApprover);
                    }
                    else
                    {
                        if (portalApprovalModel.EnableApprovalManagement.Equals(false))
                        {
                            addedUpdated = true;
                        }
                    }
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessUserDetailsSave, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                }
            }

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            var clearCacheInitializer = new ZnodeEventNotifier<PortalApprovalModel>(portalApprovalModel);

            return addedUpdated;
        }

        //Save Portal Payment Group details.
        public virtual ZnodePortalPaymentGroup SavePaymentGroupDetails(int portalApprovalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodePortalPaymentGroup> _portalPaymentGroupRepository = new ZnodeRepository<ZnodePortalPaymentGroup>();
            return _portalPaymentGroupRepository.Insert(new ZnodePortalPaymentGroup { PortalApprovalId = portalApprovalId, isActive = true });
        }

        public virtual PortalApprovalModel SavePortalApprovalDetails(PortalApprovalModel portalApprovalModel)
        {

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalApprovalModel Properties:PortalId,EnableApprovalManagement,PortalApprovalTypeId and PortalApprovalLevelId", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, new object[] { portalApprovalModel.PortalId, portalApprovalModel.EnableApprovalManagement, portalApprovalModel.PortalApprovalTypeId, portalApprovalModel.PortalApprovalLevelId });
            if (portalApprovalModel.PortalApprovalId < 1)
            {
                ZnodePortalApproval portalApproval = _portalApprovalRepository.Insert(new ZnodePortalApproval
                {
                    PortalId = portalApprovalModel.PortalId,
                    EnableApprovalManagement = portalApprovalModel.EnableApprovalManagement,
                    PortalApprovalTypeId = portalApprovalModel.PortalApprovalTypeId,
                    PortalApprovalLevelId = portalApprovalModel.PortalApprovalLevelId,
                    OrderLimit = portalApprovalModel.OrderLimit.GetValueOrDefault()
                });
                portalApprovalModel.PortalApprovalId = portalApproval.PortalApprovalId;
                ZnodeLogging.LogMessage(Admin_Resources.PortalApprovalDetailsSave, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            }
            else
            {
                _portalApprovalRepository.Update(portalApprovalModel.ToEntity<ZnodePortalApproval>());
                ZnodeLogging.LogMessage(Admin_Resources.PortalApprovalDetailsUpdate, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalApprovalModel;
        }

        //Get Approval Type name by id
        public virtual string GetPortalApprovalTypeName(int PortalApprovalTypeId)
          => _portalApprovalTypeRepository.Table.Where(x => x.PortalApprovalTypeId == PortalApprovalTypeId)?.Select(x => x.ApprovalTypeName)?.FirstOrDefault();

        //Save Portal Payment Approver details.
        public virtual List<ZnodePortalPaymentApprover> SavePaymentApproverDetails(int portalPaymentGroupId, string[] paymentTypeIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<ZnodePortalPaymentApprover> portalPaymentApproverList = new List<ZnodePortalPaymentApprover>();
            foreach (var item in paymentTypeIds)
            {
                portalPaymentApproverList.Add(new ZnodePortalPaymentApprover
                {
                    PaymentSettingId = Convert.ToInt32(item),
                    PortalPaymentGroupId = portalPaymentGroupId
                });
            }
            ZnodeLogging.LogMessage("portalPaymentApproverList list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalPaymentApproverList?.Count());
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return _portalPaymentApproversRepository.Insert(portalPaymentApproverList)?.ToList();
        }

        //Save User Approver details.
        public virtual bool SaveUserApproversDetails(int portalApprovalId, List<UserApproverModel> userApproversList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            userApproversList.Select(c => { c.PortalApprovalId = portalApprovalId; return c; }).ToList();

            //Insert/Update user approvers.
            try
            {
                return userApproversList.Any() ? _userApproverRepository.Insert(userApproversList.ToEntity<ZnodeUserApprover>().ToList())?.ToList()?.Count > 0 : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.ExceptionalError,Admin_Resources.ErrorUpdate);
            }
        }

        public virtual List<UserApproverModel> GetPortalApproverList(int portalApprovalId)
            => _userApproverRepository.Table.Where(x => x.PortalApprovalId == portalApprovalId && x.PortalPaymentGroupId == null && x.IsActive == true)?.Include(ZnodeConstant.ZnodeUser)?.ToModel<UserApproverModel>()?.ToList();

        public virtual List<PortalPaymentApproverModel> GetPortalPaymentApproverList(PortalApprovalModel portalApprovalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<PortalPaymentApproverModel> portalPaymentGroupList = _portalPaymentGroupRepository.Table.Where(x => x.PortalApprovalId == portalApprovalModel.PortalApprovalId && x.isActive == true)?.ToModel<PortalPaymentApproverModel>()?.ToList();//portalApprovalModel.UserApprover?.Select(x => x.PortalPaymentGroupId).Distinct()?.ToArray();
            ZnodeLogging.LogMessage("portalPaymentGroupList list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalPaymentGroupList?.Count());
            if (portalPaymentGroupList?.Count > 0)
            {
                List<int> portalPaymentGroupIds = portalPaymentGroupList.Select(x => x.PortalPaymentGroupId.HasValue ? x.PortalPaymentGroupId.Value : 0)?.ToList();           
                List<ZnodeUserApprover> userApproverList = _userApproverRepository.Table.Where(x => x.PortalApprovalId == portalApprovalModel.PortalApprovalId && portalPaymentGroupIds.Contains(x.PortalPaymentGroupId.HasValue? x.PortalPaymentGroupId.Value: 0) && x.IsActive == true)?.Include(ZnodeConstant.ZnodeUser)?.ToList();
                List<ZnodePortalPaymentApprover> paymentSettingList = _portalPaymentApproversRepository.Table.Where(x => portalPaymentGroupIds.Contains(x.PortalPaymentGroupId))?.ToList();
                if(userApproverList?.Count > 0 && paymentSettingList?.Count > 0)
                {
                    foreach (var portalPaymentGroup in portalPaymentGroupList)
                    {
                        portalPaymentGroup.UserApprover = userApproverList.Where(x => x.PortalPaymentGroupId == portalPaymentGroup.PortalPaymentGroupId)?.ToModel<UserApproverModel>()?.ToList();
                        portalPaymentGroup.PaymentSettingIds = paymentSettingList.Where(x => x.PortalPaymentGroupId == portalPaymentGroup.PortalPaymentGroupId)?.Select(x => x.PaymentSettingId.ToString())?.ToArray();
                    }
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalPaymentGroupList;
        }

        //Get Portal Approval type
        public PortalApprovalTypeListModel GetPortalApprovalTypeList() => new PortalApprovalTypeListModel
        {
            PortalApprovalTypes = _portalApprovalTypeRepository.Table.ToModel<PortalApprovalTypeModel>().ToList()
        };
        //Get Portal Approval level
        public PortalApprovalLevelListModel GetPortalApprovalLevelList() => new PortalApprovalLevelListModel
        {
            PortalApprovalLevels = _portalApprovalLevelRepository.Table.ToModel<PortalApprovalLevelModel>().ToList()
        };
        #endregion

        #region Portal Search Filter Settings
        public virtual PortalSortSettingListModel GetPortalSortSettingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<FilterTuple> modifiedFilterCollection = null;
            if (filters != null)
                modifiedFilterCollection = new List<FilterTuple>(filters);
            bool isAssociated = GetIsAssociatedFromFilter(filters);
            //Remove filter sortname for unassociated list based on isAssociated value.
            filters = GetUpdatedFiltersforPageAndSort(filters, isAssociated, ZnodeSortSettingEnum.SortName.ToString());            
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Updates SortName type Filter to SortDisplayName
            GetUpdatedSortingType(sorts, ZnodeSortSettingEnum.SortName.ToString().ToLower(), ZnodePortalSortSettingEnum.SortDisplayName.ToString().ToLower());
            List<PortalSortSettingModel> list = _portalSortSettingRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<PortalSortSettingModel>()?.ToList();
            ZnodeLogging.LogMessage("PortalSortSettingModel list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, list?.Count());
            List<int?> sortSettingIds = list.Select(m => m.SortSettingId).ToList();
            ZnodeLogging.LogMessage("sortSettingIds list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, sortSettingIds?.Count());
            if (!isAssociated && IsNotNull(sortSettingIds))
            {
                filters = GetUpdatedFiltersforPageAndSort(filters, isAssociated, ZnodePortalSortSettingEnum.PortalId.ToString());
                filters = GetModifiedFiltersforPageAndSort(modifiedFilterCollection, filters, ZnodeSortSettingEnum.SortName.ToString());
                GetUpdatedSortingType(sorts, ZnodePortalSortSettingEnum.SortDisplayName.ToString().ToLower(), ZnodeSortSettingEnum.SortName.ToString().ToLower());
                list = _sortSettingRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<PortalSortSettingModel>().Where(m => !sortSettingIds.Contains(m.SortSettingId)).ToList();
            }
            else if (IsNotNull(sortSettingIds))
            {
                List<PortalSortSettingModel> listSortSetting = _sortSettingRepository.Table.Where(m => sortSettingIds.Contains(m.SortSettingId)).ToModel<PortalSortSettingModel>().ToList();
                list.ForEach(m => m.SortValue = listSortSetting.Where(x => x.SortSettingId == m.SortSettingId).Select(n => n.SortValue).FirstOrDefault());
            }
            PortalSortSettingListModel pageSettingListModel = new PortalSortSettingListModel();
            pageSettingListModel.SortSettings = list?.Count > 0 ? list?.ToList() : null;
            pageListModel.TotalRowCount = list.Count;

            pageSettingListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return pageSettingListModel;
        }

        //Get paged Page Setting list
        public virtual PortalPageSettingListModel GetPortalPageSettingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<FilterTuple> modifiedFilterCollection = null;
            if (filters != null)
                modifiedFilterCollection = new List<FilterTuple>(filters);
            bool isAssociated = GetIsAssociatedFromFilter(filters);                        
            //Remove filter pagename for unassociated list based on isAssociated value.
            filters = GetUpdatedFiltersforPageAndSort(filters, isAssociated, ZnodePageSettingEnum.PageName.ToString());
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<PortalPageSettingModel> list = _portalPageSettingRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<PortalPageSettingModel>()?.ToList();
            ZnodeLogging.LogMessage("PortalPageSettingModel list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, list?.Count());
            List<int?> pageSettingIds = list.Select(m => m.PageSettingId).ToList();
            ZnodeLogging.LogMessage("pageSettingIds list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageSettingIds?.Count());
            if (!isAssociated && IsNotNull(pageSettingIds))
            {
                filters = GetUpdatedFiltersforPageAndSort(filters, isAssociated, ZnodePortalPageSettingEnum.PortalId.ToString());
                filters = GetModifiedFiltersforPageAndSort(modifiedFilterCollection, filters, ZnodePageSettingEnum.PageName.ToString());
                list = _pageSettingRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<PortalPageSettingModel>().Where(m => !pageSettingIds.Contains(m.PageSettingId)).ToList();
            }
            else if (IsNotNull(pageSettingIds))
            {
                List<PortalPageSettingModel> listPageSetting = _pageSettingRepository.Table.Where(m => pageSettingIds.Contains(m.PageSettingId)).ToModel<PortalPageSettingModel>().ToList();
                ZnodeLogging.LogMessage("listPageSetting list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, listPageSetting?.Count());
                list.ForEach(m => m.PageValue = listPageSetting.Where(x => x.PageSettingId == m.PageSettingId).Select(n => n.PageValue).FirstOrDefault());
            }
            PortalPageSettingListModel pageSettingListModel = new PortalPageSettingListModel();
            pageSettingListModel.PageSettings = list?.Count > 0 ? list?.ToList() : null;

            pageSettingListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return pageSettingListModel;
        }

        //Load portal page setting from lookups if there is no record in the table ZnodePortalPageSetting
        public virtual bool InsertPortalPageSettingFromLookups(int portalId)
        {
            int portalPageCount = _portalPageSettingRepository.Table.Count(m => m.PortalId == portalId);
            if (portalPageCount == 0)
            {
                PageSettingAssociationModel associationModel = new PageSettingAssociationModel();
                associationModel.PortalId = portalId;
                associationModel.PageSettingIds = string.Join(",", _pageSettingRepository.Table.Select(m => m.PageSettingId).ToArray());
                return InsertIntoPortalPageSetting(associationModel, true);
            }
            return false;
        }

        //Remove associated sort settings to portal.
        public virtual bool RemoveAssociatedSortSettings(SortSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("associationModel properties PortalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, associationModel.PortalId);
            if (IsNull(associationModel))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.SortSettingAssociationModelNotNull);

            if (string.IsNullOrEmpty(associationModel.PortalSortSettingIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.SortSettingIdNotNullOrEmpty);

            bool result = false;
            if (associationModel.PortalId > 0)
                result = RemoveFromPortalSortSetting(associationModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return result;
        }

        //Remove associated sort settings to portal.
        public virtual bool RemoveAssociatedPageSettings(PageSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("associationModel properties PortalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, associationModel.PortalId);
            if (IsNull(associationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PageSettingAssociationModelNotNull);

            if (string.IsNullOrEmpty(associationModel.PortalPageSettingIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PageSettingIdNotNullOrEmpty);

            bool result = false;
            if (associationModel.PortalId > 0)
                result = RemoveFromPortalPageSetting(associationModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return result;
        }

        //Associate sort settings to portal.
        public virtual bool AssociateSortSettings(SortSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("associationModel properties PortalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, associationModel.PortalId);
            if (IsNull(associationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.SortSettingAssociationModelNotNull);

            if (string.IsNullOrEmpty(associationModel.SortSettingIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.SortSettingIdNotNullOrEmpty);

            bool result = false;
            if (associationModel.PortalId > 0)
                result = InsertIntoPortalSortSetting(associationModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return result;
        }

        //Associate sort settings to portal.
        public virtual bool AssociatePageSettings(PageSettingAssociationModel associationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("associationModel properties PortalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, associationModel.PortalId);
            if (IsNull(associationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PageSettingAssociationModelNotNull);

            if (string.IsNullOrEmpty(associationModel.PageSettingIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PageSettingIdNotNullOrEmpty);

            bool result = false;
            if (associationModel.PortalId > 0)
                result = InsertIntoPortalPageSetting(associationModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return result;
        }

        //Update portal page setting
        public virtual bool UpdatePortalPageSetting(PortalPageSettingModel portalPageSettingModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalPageSettingModel properties PortalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, portalPageSettingModel.PortalId);
            if (IsNull(portalPageSettingModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PageSettingAssociationModelNotNull);
            if(portalPageSettingModel.IsDefault == true)
            {
                List<ZnodePortalPageSetting> portalPageList = _portalPageSettingRepository.Table.Where(x => x.IsDefault == true && x.PortalId == portalPageSettingModel.PortalId)?.ToList();
                portalPageList?.ForEach(portalPage =>
                {
                    portalPage.IsDefault = portalPage.PortalPageSettingId == portalPageSettingModel.PortalPageSettingId;
                });                
                ZnodePortalPageSetting isDefaultPortalPage = _portalPageSettingRepository.Table.FirstOrDefault(x => x.PortalPageSettingId == portalPageSettingModel.PortalPageSettingId);
                List<ZnodePortalPageSetting> settingsToUpdate = new List<ZnodePortalPageSetting>();
                if(portalPageList?.Count() > 0)
                {
                    settingsToUpdate.AddRange(portalPageList);
                }                
                if (IsNotNull(isDefaultPortalPage))
                {
                    isDefaultPortalPage.IsDefault = true;
                    settingsToUpdate.Add(isDefaultPortalPage);
                }
                if(settingsToUpdate.Count > 0)
                {
                    _portalPageSettingRepository.BatchUpdate(settingsToUpdate);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                ZnodePortalPageSetting isDefaultportalPage = _portalPageSettingRepository.Table.FirstOrDefault(x => x.IsDefault == true);
                if(IsNotNull(isDefaultportalPage) && isDefaultportalPage.PortalPageSettingId== portalPageSettingModel.PortalPageSettingId)
                {
                    throw new ZnodeException(ErrorCodes.AtLeastSelectOne, Admin_Resources.PortalPageAtLeastOneDefault);
                }
                else
                {
                    _portalPageSettingRepository.Update(portalPageSettingModel.ToEntity<ZnodePortalPageSetting>());
                    return true;
                }
            }
        }

        #endregion

        //Get barcode scanner details
        public virtual BarcodeReaderModel GetBarcodeScanner()
        {            
            return new BarcodeReaderModel() { LicenseKey = ZnodeApiSettings.BarcodeScannerLicenseKey, BarcodeFormates = ZnodeConstant.BarcodeFormates, EnableSpecificSearch = ZnodeApiSettings.EnableBarcodeSpecificSearch };
        }
        #endregion

        #region Private Methods

        //Set the default Media Server URL.
        private void SetDefaultServer(PortalModel model)
        {
            if (IsNull(model))
                model = new PortalModel();

            IMediaConfigurationService mediaConfiguration = GetService<IMediaConfigurationService>();
            MediaConfigurationModel configuration = mediaConfiguration.GetDefaultMediaConfiguration();

            string serverPath = GetMediaServerUrl(configuration);
            if (IsNotNull(configuration))
                model.MediaServerUrl = serverPath;
            model.MediaServerThumbnailUrl = $"{serverPath}{configuration.ThumbnailFolderName}";
            ZnodeLogging.LogMessage("model properties MediaServerUrl and MediaServerThumbnailUrl", ZnodeLogging.Components.Portal.ToString(),TraceLevel.Verbose, new object[] { model?.MediaServerUrl, model.MediaServerThumbnailUrl });
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {

                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodePortalEnum.ZnodeDomains.ToString().ToLower()))
                        SetExpands(ZnodePortalEnum.ZnodeDomains.ToString(), navigationProperties);
                    if (Equals(key, ZnodePortalFeatureMapperEnum.ZnodePortalFeature.ToString().ToLower()))
                        SetExpands(ZnodePortalFeatureMapperEnum.ZnodePortalFeature.ToString(), navigationProperties);
                    if (Equals(key, ZnodePortalEnum.ZnodeOmsOrderState.ToString().ToLower()))
                        SetExpands(ZnodePortalEnum.ZnodeOmsOrderState.ToString(), navigationProperties);
                    if (Equals(key, ZnodePortalAlternateWarehouseEnum.ZnodeWarehouse.ToString().ToLower()))
                        SetExpands(ZnodePortalAlternateWarehouseEnum.ZnodeWarehouse.ToString(), navigationProperties);
                    if (Equals(key, ZnodePortalEnum.ZnodePortalCatalogs.ToString().ToLower()))
                        SetExpands(ZnodePortalEnum.ZnodePortalCatalogs.ToString(), navigationProperties);
                    if (Equals(key, ZnodePortalEnum.ZnodePortalLocales.ToString().ToLower()))
                        SetExpands(ZnodePortalEnum.ZnodePortalLocales.ToString(), navigationProperties);
                    if (Equals(key, ZnodePortalProfileEnum.ZnodePortal.ToString().ToLower()))
                        SetExpands(ZnodePortalProfileEnum.ZnodePortal.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Method to set locale as a default.
        private void SetDefault(List<ZnodePortalLocale> localeConfigurationList, DefaultGlobalConfigModel defaultGlobalConfigModel)
        {
            //Get list of entity from table. 
            List<ZnodePortalLocale> defaultConfigurationList = _portalLocaleRepository.Table.Where(x => x.PortalId == defaultGlobalConfigModel.PortalId && x.IsDefault).ToList();
            ZnodeLogging.LogMessage("defaultConfigurationList list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, defaultConfigurationList?.Count());
            defaultConfigurationList.ForEach(x => x.IsDefault = false);
            defaultConfigurationList?.ForEach(x => _portalLocaleRepository.Update(x));
            localeConfigurationList?.FirstOrDefault(x => x.IsDefault = true);
            localeConfigurationList?.ForEach(x => _portalLocaleRepository.Update(x));
        }

        //Method to deactivate locales
        private bool Deactivate(string whereClauseModel, List<ZnodePortalLocale> portalLocaleList, bool checkIsDefault)
        {
            ZnodeLogging.LogMessage("Input Parameter whereClauseModel:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);
            ZnodeLogging.LogMessage("portalLocaleList list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalLocaleList?.Count());
            portalLocaleList.RemoveAll(x => x.IsDefault);
            portalLocaleList?.ForEach(x => _portalLocaleRepository.Delete(x));
            //If CheckIsDefault is equal to true it will throw exception.
            if (checkIsDefault)
                throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDeactivate");
            _portalLocaleRepository.Delete(whereClauseModel);
            return true;
        }

        //Update Locale as per action Name
        private bool UpdateLocaleAsPerAction(DefaultGlobalConfigListModel globalConfigListModel, EntityWhereClauseModel whereClauseModel, List<ZnodePortalLocale> portalLocaleList, bool checkIsDefault, List<DefaultGlobalConfigModel> DefaultGlobalConfigListModel)
        {
            if (DefaultGlobalConfigListModel?.Count() > 0)
            {
                foreach (var item in DefaultGlobalConfigListModel)
                {
                    if (Equals(item.Action, "SetActive"))
                    {
                        if (string.IsNullOrEmpty(item.PortalLocaleId))
                            _portalLocaleRepository.Insert(DefaultGlobalConfigListModel.ToEntity<ZnodePortalLocale>());
                        else
                            _portalLocaleRepository.Update(item.ToEntity<ZnodePortalLocale>());
                    }
                    else if (Equals(item.Action, "SetDeActive"))
                    {
                        ZnodeLogging.LogMessage("Parameter for Deactivate", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel.WhereClause, portalLocaleList, checkIsDefault });
                        return Deactivate(whereClauseModel.WhereClause, portalLocaleList, checkIsDefault);
                    }
                    else if (Equals(item.Action, "SetDefault"))
                        SetDefault(portalLocaleList, item);
                }
                return true;
            }
            else
            {
                foreach (var item in globalConfigListModel?.DefaultGlobalConfigs)
                {
                    if (Equals(item.Action, "SetActive"))
                        _portalLocaleRepository.Insert(item.ToEntity<ZnodePortalLocale>());
                    else if (Equals(item.Action, "SetDefault") && DefaultGlobalConfigListModel?.Count <= 0)
                        throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDefault");
                }
                return true;
            }
        }

        //Map Parameters for portal Locale
        private static List<DefaultGlobalConfigModel> MapParametersForPortalLocales(DefaultGlobalConfigListModel globalConfigListModel, List<ZnodePortalLocale> portalLocaleList)
        {
            List<DefaultGlobalConfigModel> DefaultGlobalConfigListModel = new List<DefaultGlobalConfigModel>();

            portalLocaleList?.ForEach(item =>
            {
                var locales = globalConfigListModel?.DefaultGlobalConfigs.Where(x => x.PortalId == item.PortalId)?.FirstOrDefault();
                DefaultGlobalConfigModel defaultGlobalConfigModel = new DefaultGlobalConfigModel();
                if (IsNotNull(locales))
                {
                    defaultGlobalConfigModel.PortalId = locales.PortalId;
                    defaultGlobalConfigModel.Action = locales?.Action;
                }
                defaultGlobalConfigModel.PortalLocaleId = item?.PortalLocaleId.ToString();
                defaultGlobalConfigModel.LocaleId = item?.LocaleId.ToString();
                defaultGlobalConfigModel.IsDefault = item.IsDefault;
                DefaultGlobalConfigListModel.Add(defaultGlobalConfigModel);
            });
            ZnodeLogging.LogMessage("DefaultGlobalConfigListModel list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, DefaultGlobalConfigListModel?.Count());
            return DefaultGlobalConfigListModel;
        }

        //Remove Locale ids to activate locales.
        private static void RemoveLocaleIds(DefaultGlobalConfigListModel globalConfigListModel, List<ZnodePortalLocale> portalLocaleList)
        {
            if (globalConfigListModel.DefaultGlobalConfigs.FirstOrDefault()?.Action == "SetActive" && portalLocaleList.Count > 0)
            {
                List<int> localeids = portalLocaleList.Select(x => x.LocaleId).ToList();
                ZnodeLogging.LogMessage("localeids list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, localeids?.Count());
                globalConfigListModel.DefaultGlobalConfigs.RemoveAll(x => localeids.Contains(Convert.ToInt32(x.LocaleId)));
                portalLocaleList.Clear();
            }
        }

        //Bind Portal Details for Theme,CSS & Publish CatalogId.
        private void BindPortalDetails(PortalModel portal)
        {
            if (IsNotNull(portal))
            {
                IZnodeRepository<ZnodePimCatalog> _pimCatalogRepository = new ZnodeRepository<ZnodePimCatalog>();

                ZnodeCMSPortalTheme cmsPortalTheme = _cmsPortalTheme.Table.Where(x => x.PortalId == portal.PortalId)?.FirstOrDefault();
                if (IsNotNull(cmsPortalTheme))
                {
                    portal.CMSThemeId = cmsPortalTheme.CMSThemeId;
                    portal.CMSThemeCSSId = cmsPortalTheme.CMSThemeCSSId.GetValueOrDefault();
                }

                var catalogDetail = from portalCatalog in _portalCatalogRepository.Table
                                    join pimCatalog in _pimCatalogRepository.Table on portalCatalog.PublishCatalogId equals pimCatalog.PimCatalogId
                                    where portalCatalog.PortalId == portal.PortalId
                                    select new
                                    {
                                        pimCatalog.CatalogName,
                                        portalCatalog.PublishCatalogId
                                    };

                ZnodeLogging.LogMessage("catalogDetail:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, catalogDetail);
                portal.PublishCatalogId = catalogDetail?.FirstOrDefault()?.PublishCatalogId;
                portal.CatalogName = catalogDetail?.FirstOrDefault()?.CatalogName;

            }
        }

        //Insert into user portal.
        private void InsertUserPortal(PortalModel portalModel)
        {
            int loginUserId = GetLoginUserId();
            ZnodeLogging.LogMessage("loginUserId and PortalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { loginUserId, portalModel.PortalId });

            if (portalModel.PortalId > 0 && loginUserId > 0)
            {
                IZnodeRepository<ZnodeUserPortal> _portalCountryRepository = new ZnodeRepository<ZnodeUserPortal>();

                var list = _portalCountryRepository.Table.Where(x => x.UserId == loginUserId).ToList();
                ZnodeLogging.LogMessage("UserPortal list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, list?.Count());
                if (!list.Any(x => x.PortalId == null))
                    ZnodeLogging.LogMessage(_portalCountryRepository.Insert(new ZnodeUserPortal() { UserId = loginUserId, PortalId = portalModel.PortalId })?.UserPortalId > 0 ? Admin_Resources.SuccessUserPortalInsert : Admin_Resources.ErrorUserPortalInsert, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            }
        }

        //Bind the Portal selected & Available Features.
        private void BindPortalFeatures(string whereClause, PortalModel portal)
        {
            if (IsNotNull(portal))
            {
                //Set Expand for Portal Features
                List<string> navigationProperties = new List<string>();
                navigationProperties.Add(ZnodePortalFeatureMapperEnum.ZnodePortalFeature.ToString());

                //Get the Associated Portal Features.
                portal.SelectedPortalFeatures = PortalMap.ToPortalFeatureListModel(_portalFeatureMapperRepository.GetEntityList(whereClause, navigationProperties));

                //Set all available Portal Features.
                portal.AvailablePortalFeatures = GetPortalFeatures();
                ZnodeLogging.LogMessage("SelectedPortalFeatures and AvailablePortalFeatures:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { portal.SelectedPortalFeatures, portal.AvailablePortalFeatures });
            }
        }

        //Save the Portal Associated Details like PortalFeatures, PortalCatalog,Theme & CSS Details.
        private void MapPortalDetails(PortalModel portalModel, int portalId)
        {
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalId);
            portalModel.PortalId = portalId;
            if (portalModel?.PortalFeatureIds?.Length > 0)
                _portalFeatureMapperRepository.Insert(PortalMap.ToPortalFeatureMapperListModel(portalModel.PortalId, portalModel.PortalFeatureIds));

            //Insert Portal Catalog Details.
            _portalCatalogRepository.Insert(portalModel.ToEntity<ZnodePortalCatalog>());

            //Insert Portal Theme & CSS Details.
            _cmsPortalTheme.Insert(portalModel.ToEntity<ZnodeCMSPortalTheme>());

            //Insert Publish state as draft for newly created store
            _publishPortalLogRepository.Insert(new ZnodePublishPortalLog { PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT, PortalId = portalId});

            //If default currency is not null set default currency of global setting to current portal.
            if (!string.IsNullOrEmpty(portalModel.DefaultCurrency))
            {
                //Insert default portal units.
                ZnodeCurrency currencyEntity = _currencyRepository.Table.FirstOrDefault(x => x.CurrencyCode == portalModel.DefaultCurrency);
                ZnodeCulture cultureEntity = _cultureRepository.Table.FirstOrDefault(x => x.CultureCode == portalModel.DefaultCulture);
                _portalUnitRepository.Insert(new ZnodePortalUnit() { PortalId = portalId, CurrencyId = currencyEntity?.CurrencyId, WeightUnit = portalModel.DefaultWeightUnit, DimensionUnit = portalModel.DefaultDimensionUnit, CurrencySuffix = currencyEntity?.CurrencyCode, CultureId = cultureEntity?.CultureId }); //TODO-U323
            }

            //Insert into portal Country.
            InsertPortalCountry(portalModel);

            //Insert default portal SMTP details.
            InsertDefaultPortalSMTPDetails(portalId);

            //Copy content page and manage message of another portal to this portal.
            CopyContent(portalId, portalModel.CopyContentPortalId.GetValueOrDefault());

            //Insert default portal Locale details.
            DefaultGlobalConfigListModel defaultGlobalConfigListModel = new DefaultGlobalConfigListModel();
            defaultGlobalConfigListModel.DefaultGlobalConfigs.Add(new DefaultGlobalConfigModel() { PortalId = portalId, LocaleId = GetDefaultLocaleId().ToString(), Action = "SetActive", IsDefault = true });
            UpdateLocale(defaultGlobalConfigListModel);

            InsertUserPortal(portalModel);

            //Insert portal page setting from lookups if there is no record in the table ZnodePortalPageSetting
            InsertPortalPageSettingFromLookups(portalId);
        }

        //Insert default portal SMTP details.
        private void InsertDefaultPortalSMTPDetails(int portalId)
        {
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalId);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, _portalRepository.GetEntity("")?.PortalId.ToString()));

            //Get Portal smtp details.
            ZnodePortalSmtpSetting portalSmtpSetting = _portalSmtpSetting.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            if (IsNotNull(portalSmtpSetting))
            {
                portalSmtpSetting.PortalId = portalId;

                //Explicitly marking this setting false as it should be the default value regardless of what Portal with (Id = 1) has.
                portalSmtpSetting.DisableAllEmails = false;

                //Insert it into the portal setting.
                _portalSmtpSetting.Insert(portalSmtpSetting);
            }
        }

        //Update the Portal Mapping Details like, Portal Features,Catalog, Theme & CSS.
        private void UpdatePortalMappingDetails(PortalModel portalModel)
        {
            ZnodeLogging.LogMessage("Input Parameters PortalModel having PortalId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { portalModel?.PortalId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.In, portalModel.PortalId.ToString()));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage(" WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClause);
            //Update the Associate Portal Features.
            UpdatePortalFeatureMappings(portalModel, whereClause);

            //Update the Associate Portal Catalog Details.
            UpdatePortalCatalogMapping(portalModel);

            //Update the Associate Portal Theme & CSS Details.
            UpdatePortalThemeMapping(portalModel);
        }

        //Update the Associate Portal Features.
        private void UpdatePortalFeatureMappings(PortalModel portalModel, string whereClause)
        {
            //Delete the Existing portal Features mapping.
            _portalFeatureMapperRepository.Delete(whereClause);
            ZnodeLogging.LogMessage("Input Parameters PortalModel having PortalFeatureIds and PortalId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { portalModel?.PortalFeatureIds, portalModel?.PortalId });
            //Insert new portal Features Mappings
            if (portalModel.PortalFeatureIds?.Length > 0)
                _portalFeatureMapperRepository.Insert(PortalMap.ToPortalFeatureMapperListModel(portalModel.PortalId, portalModel.PortalFeatureIds));

            //Clear portal cache 
            var clearCacheInitializer = new ZnodeEventNotifier<PortalModel>(portalModel);
        }

        //Update the Associate Portal Catalog Details.
        private void UpdatePortalCatalogMapping(PortalModel portalModel)
        {
            ZnodeLogging.LogMessage("Input Parameters PortalModel having PublishCatalogId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { portalModel?.PublishCatalogId });
            if (portalModel.PublishCatalogId > 0)
            {
                //Get existing Portal Catalog Details.
                ZnodePortalCatalog portalCatalog = _portalCatalogRepository.Table.Where(x => x.PortalId == portalModel.PortalId)?.FirstOrDefault();

                //Check whether the Existing Catalog Id changed in the request. To Update the Catalog Details.
                if (portalCatalog?.PublishCatalogId != portalModel.PublishCatalogId)
                {
                    //Delete all the associated mapping for the Category & Product Widget.
                    DeletePortalWidgetConfiguration(portalModel.PortalId);

                    portalCatalog.PublishCatalogId = portalModel.PublishCatalogId.Value;

                    //Update the Portal Catalog Details
                    _portalCatalogRepository.Update(portalCatalog);
                }
            }
        }

        //Update the Associate Portal Theme & CSS Details.
        private void UpdatePortalThemeMapping(PortalModel portalModel)
        {
            ZnodeLogging.LogMessage("Input Parameters PortalModel having CMSThemeId and CMSThemeCSSId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { portalModel.CMSThemeId, portalModel.CMSThemeCSSId });
            if (portalModel.CMSThemeId > 0 && portalModel.CMSThemeCSSId > 0)
            {
                //Get the Associated Portal Theme & CSS Details to update.
                ZnodeCMSPortalTheme cmsPortalTheme = _cmsPortalTheme.Table.Where(x => x.PortalId == portalModel.PortalId)?.FirstOrDefault();
                if (IsNotNull(cmsPortalTheme))
                {
                    cmsPortalTheme.CMSThemeCSSId = portalModel.CMSThemeCSSId;
                    cmsPortalTheme.CMSThemeId = portalModel.CMSThemeId.GetValueOrDefault();
                }

                //Update new portal Theme & CSS Association.
                _cmsPortalTheme.Update(cmsPortalTheme);
            }
        }

        //This method is used to get media path from media Id.
        private MediaManagerModel GetMediaPath(int mediaId)
         => GetService<IMediaManagerServices>().GetMediaByID(mediaId, null);

        //Copy content page and manage message of another portal to this portal.
        private void CopyContent(int portalId, int copyContentPortalId)
        {
            ZnodeLogging.LogMessage("Input Parameters portalId and copyContentPortalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { portalId , copyContentPortalId });
            try
            {
                if (portalId > 0 && copyContentPortalId > 0)
                {
                    IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                    objStoredProc.SetParameter("CopyPortalId", copyContentPortalId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter(ZnodePortalEnum.PortalId.ToString(), portalId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                    int status = 0;
                    objStoredProc.ExecuteStoredProcedureList("Znode_CopyPortalMessageAndContentPages @CopyPortalId, @PortalId, @UserId ,@Status OUT", 3, out status);
                    ZnodeLogging.LogMessage(status == 1 ? Admin_Resources.SuccessCopyContent: Admin_Resources.ErrorCopyContent, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.ExceptionalError, ex.Message);
            }
        }

        //Method to encrypt shipping information.
        private void EncryptShippingData(PortalShippingModel portalShippingModel)
        {
            ZnodeEncryption encryption = new ZnodeEncryption();

            portalShippingModel.FedExAccountNumber = !string.IsNullOrEmpty(portalShippingModel.FedExAccountNumber) ? encryption.EncryptData(portalShippingModel.FedExAccountNumber) : null;
            portalShippingModel.FedExMeterNumber = !string.IsNullOrEmpty(portalShippingModel.FedExMeterNumber) ? encryption.EncryptData(portalShippingModel.FedExMeterNumber) : null;
            portalShippingModel.FedExProductionKey = !string.IsNullOrEmpty(portalShippingModel.FedExProductionKey) ? encryption.EncryptData(portalShippingModel.FedExProductionKey) : null;
            portalShippingModel.FedExSecurityCode = !string.IsNullOrEmpty(portalShippingModel.FedExSecurityCode) ? encryption.EncryptData(portalShippingModel.FedExSecurityCode) : null;
            portalShippingModel.UpsUsername = !string.IsNullOrEmpty(portalShippingModel.UpsUsername) ? encryption.EncryptData(portalShippingModel.UpsUsername) : null;
            portalShippingModel.UpsPassword = !string.IsNullOrEmpty(portalShippingModel.UpsPassword) ? encryption.EncryptData(portalShippingModel.UpsPassword) : null;
            portalShippingModel.UpsKey = !string.IsNullOrEmpty(portalShippingModel.UpsKey) ? encryption.EncryptData(portalShippingModel.UpsKey) : null;

            portalShippingModel.FedExLTLAccountNumber = !string.IsNullOrEmpty(portalShippingModel.FedExLTLAccountNumber) ? encryption.EncryptData(portalShippingModel.FedExLTLAccountNumber) : null;
            portalShippingModel.LTLUPSAccessLicenseNumber = !string.IsNullOrEmpty(portalShippingModel.LTLUPSAccessLicenseNumber) ? encryption.EncryptData(portalShippingModel.LTLUPSAccessLicenseNumber) : null;
            portalShippingModel.LTLUPSAccountNumber = !string.IsNullOrEmpty(portalShippingModel.LTLUPSAccountNumber) ? encryption.EncryptData(portalShippingModel.LTLUPSAccountNumber) : null;
            portalShippingModel.LTLUPSUsername = !string.IsNullOrEmpty(portalShippingModel.LTLUPSUsername) ? encryption.EncryptData(portalShippingModel.LTLUPSUsername) : null;
            portalShippingModel.LTLUPSPassword = !string.IsNullOrEmpty(portalShippingModel.LTLUPSPassword) ? encryption.EncryptData(portalShippingModel.LTLUPSPassword) : null;
        }

        //Method to decrypt shipping information.
        private void DecryptShippingData(ZnodeShippingPortal shippingPortal)
        {
            ZnodeEncryption encryption = new ZnodeEncryption();

            shippingPortal.FedExAccountNumber = !string.IsNullOrEmpty(shippingPortal.FedExAccountNumber) ? encryption.DecryptData(shippingPortal.FedExAccountNumber) : null;
            shippingPortal.FedExMeterNumber = !string.IsNullOrEmpty(shippingPortal.FedExMeterNumber) ? encryption.DecryptData(shippingPortal.FedExMeterNumber) : null;
            shippingPortal.FedExProductionKey = !string.IsNullOrEmpty(shippingPortal.FedExProductionKey) ? encryption.DecryptData(shippingPortal.FedExProductionKey) : null;
            shippingPortal.FedExSecurityCode = !string.IsNullOrEmpty(shippingPortal.FedExSecurityCode) ? encryption.DecryptData(shippingPortal.FedExSecurityCode) : null;
            shippingPortal.UPSUserName = !string.IsNullOrEmpty(shippingPortal.UPSUserName) ? encryption.DecryptData(shippingPortal.UPSUserName) : null;
            shippingPortal.UPSPassword = !string.IsNullOrEmpty(shippingPortal.UPSPassword) ? encryption.DecryptData(shippingPortal.UPSPassword) : null;
            shippingPortal.UPSKey = !string.IsNullOrEmpty(shippingPortal.UPSKey) ? encryption.DecryptData(shippingPortal.UPSKey) : null;

            shippingPortal.FedExLTLAccountNumber = !string.IsNullOrEmpty(shippingPortal.FedExLTLAccountNumber) ? encryption.DecryptData(shippingPortal.FedExLTLAccountNumber) : null;
            shippingPortal.LTLUPSAccessLicenseNumber = !string.IsNullOrEmpty(shippingPortal.LTLUPSAccessLicenseNumber) ? encryption.DecryptData(shippingPortal.LTLUPSAccessLicenseNumber) : null;
            shippingPortal.LTLUPSAccountNumber = !string.IsNullOrEmpty(shippingPortal.LTLUPSAccountNumber) ? encryption.DecryptData(shippingPortal.LTLUPSAccountNumber) : null;
            shippingPortal.LTLUPSUsername = !string.IsNullOrEmpty(shippingPortal.LTLUPSUsername) ? encryption.DecryptData(shippingPortal.LTLUPSUsername) : null;
            shippingPortal.LTLUPSPassword = !string.IsNullOrEmpty(shippingPortal.LTLUPSPassword) ? encryption.DecryptData(shippingPortal.LTLUPSPassword) : null;
        }

        //Get name of portal of which content pages and manage messages are copied for current portal.
        private void GetCopyContentPortalName(PortalModel portal)
        {
            //If CopyContentPortalId is greater than 0, get its portal name.
            if (portal.CopyContentPortalId.GetValueOrDefault() > 0)
                portal.CopyContentPortalName = _portalRepository.Table.Where(x => x.PortalId == portal.CopyContentPortalId)?.Select(x => x.StoreName)?.FirstOrDefault();
        }

        //Delete the Portal Widget Configuration of Product & Category.
        private void DeletePortalWidgetConfiguration(int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameters portalId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, portalId);
            FilterCollection filters = new FilterCollection();
            IZnodeRepository<ZnodeCMSWidgetProduct> _cmsWidgetProductRepository = new ZnodeRepository<ZnodeCMSWidgetProduct>();
            IZnodeRepository<ZnodeCMSWidgetCategory> _cmsWidgetCategory = new ZnodeRepository<ZnodeCMSWidgetCategory>();

            //Set Filter Values for Deletion of Portal Widget Configuration.
            filters.Add(new FilterTuple(ZnodeCMSWidgetProductEnum.CMSMappingId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));
            filters.Add(new FilterTuple(ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString(), ProcedureFilterOperators.Contains, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage(" WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);
            //Delete the Associated Product Widget Configuration for the portal.
            _cmsWidgetProductRepository.Delete(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

            //Delete the Associated Category Widget Configuration for the portal.
            _cmsWidgetCategory.Delete(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
        }

        //Delete previously associated where house.
        private void DeleteAssociatedWarehouse(PortalWarehouseModel model)
        {
            ZnodeLogging.LogMessage("Input Parameters PortalWarehouseModel having PortalId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, model.PortalId);
            int portalWarehouseId = _portalWarehouseRepository.Table.Where(a => a.PortalId == model.PortalId).Select(a => a.PortalWarehouseId).FirstOrDefault();
            ZnodeLogging.LogMessage("portalWarehouseId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, portalWarehouseId);
            if (portalWarehouseId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePortalAlternateWarehouseEnum.PortalWarehouseId.ToString(), FilterOperators.Equals, portalWarehouseId.ToString()));
                string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
                ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClause);
                _portalAlternateWarehouseRepository.Delete(whereClause);
                _portalWarehouseRepository.Delete(whereClause);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessPreviousWarehouseDissocaition, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            }
        }

        //Clears the cache of all the web store domain for a specific portal ID.
        private IList<ZnodeDomain> GetWebsiteDomains(int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameters portalId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, portalId);
            FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeDomainEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()),
            new FilterTuple(ZnodeDomainEnum.ApplicationType.ToString(),FilterOperators.Like,ApplicationTypesEnum.WebStore.ToString())};

            PageListModel pageListModel = new PageListModel(filter, null, null);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<ZnodeDomain> domains = _domainRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues);
            ZnodeLogging.LogMessage("domains count :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, domains.Count());

            return domains;
        }

        //Clears the cache of all the web store domain for a specific portal ID.
        private IList<ZnodeDomain> GetWebsitePreviewDomains(int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameters portalId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, portalId);
            FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeDomainEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()),
            new FilterTuple(ZnodeDomainEnum.ApplicationType.ToString(),FilterOperators.Like,ApplicationTypesEnum.WebstorePreview.ToString())};

            PageListModel pageListModel = new PageListModel(filter, null, null);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<ZnodeDomain> domains = _domainRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues);

            return domains;
        }

        //Reset portal default tax.
        private void ResetPortalDefaultTax(TaxClassPortalModel taxClassPortalModel)
        {
            FilterCollection filters = SetPortalIdAndIsDefaultFilter(taxClassPortalModel.PortalId, ZnodeConstant.TrueValue);
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);
            //Update default tax.
            ZnodePortalTaxClass znodePortalTaxClass = _taxClassPortalRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.FirstOrDefault();
            if (IsNotNull(znodePortalTaxClass))
            {
                znodePortalTaxClass.IsDefault = false;
                _taxClassPortalRepository.Update(znodePortalTaxClass);
            }
        }

        //Set portal default tax.
        private bool UpdatePortalDefaultTax(TaxClassPortalModel taxClassPortalModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalTaxClassEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, taxClassPortalModel.PortalId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalTaxClassEnum.TaxClassId.ToString(), ProcedureFilterOperators.Equals, taxClassPortalModel.TaxClassIds.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);
            //Update default tax.
            ZnodePortalTaxClass znodePortalTaxClass = _taxClassPortalRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.FirstOrDefault();
            if (IsNotNull(znodePortalTaxClass))
            {
                znodePortalTaxClass.IsDefault = true;
                return _taxClassPortalRepository.Update(znodePortalTaxClass);
            }
            return false;
        }

        //Associate taxclass to portal.
        private bool AssociateTaxClass(TaxClassPortalModel taxClassPortalModel)
        {
            if (IsValidInput(taxClassPortalModel))
            {
                List<ZnodePortalTaxClass> portalTaxClassList = GetTaxClassPortalModelList(taxClassPortalModel);
                if (!IsDefaultExist(taxClassPortalModel) && portalTaxClassList?.Count > 0)
                    portalTaxClassList[0].IsDefault = true;

                portalTaxClassList = _taxClassPortalRepository.Insert(portalTaxClassList)?.ToList();
                ZnodeLogging.LogMessage((IsNotNull(portalTaxClassList) ? Admin_Resources.SuccessTaxClassAssociation : Admin_Resources.ErrorTaxClassAssociation), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return IsNotNull(portalTaxClassList);
            }
            return false;
        }

        //Check default tax when associate tax to portal.
        private bool IsDefaultExist(TaxClassPortalModel taxClassPortalModel)
        {
            FilterCollection filters = SetPortalIdAndIsDefaultFilter(taxClassPortalModel.PortalId, ZnodeConstant.TrueValue);
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);
            ZnodePortalTaxClass znodePortalTaxClass = _taxClassPortalRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.FirstOrDefault();
            return IsNotNull(znodePortalTaxClass);
        }

        //Check for the input is valid or not.
        private bool IsValidInput(TaxClassPortalModel taxClassPortalModel)
        {
            ZnodeLogging.LogMessage("Input Parameters TaxClassPortalModel having TaxClassIds:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, taxClassPortalModel.TaxClassIds);
            if (!string.IsNullOrEmpty(taxClassPortalModel.TaxClassIds))
            {
                IEnumerable<int?> associatedRuleTypes = GetAssociatedRuleType(taxClassPortalModel);

                //only taxes which having same tax rule type gets associated to portal. 
                if (associatedRuleTypes?.Count() >= 1)
                {
                    //Get the associated tax rule type of portal.
                    IZnodeRepository<ZnodeTaxRule> _taxRuleRepository = new ZnodeRepository<ZnodeTaxRule>();
                    List<int?> portalRuleTypes = _taxClassPortalRepository.Table
                                                                    .Where(x => x.PortalId == taxClassPortalModel.PortalId)
                                                                    .Join(_taxRuleRepository.Table,
                                                                          c => c.TaxClassId,
                                                                          r => r.TaxClassId,
                                                                          (c, r) => r.TaxRuleTypeId)
                                                                    .ToList();
                    if (portalRuleTypes.Count > 0)
                    {
                        //Check for the rule type of tax and tax assigned to portal.
                        bool result = portalRuleTypes.Intersect(associatedRuleTypes).Any();
                        if (!result)
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        //Get associated rule type.
        private IEnumerable<int?> GetAssociatedRuleType(TaxClassPortalModel taxClassPortalModel)
        {
            if (!string.IsNullOrEmpty(taxClassPortalModel.TaxClassIds))
            {
                IZnodeRepository<ZnodeTaxRule> _taxRuleRepository = new ZnodeRepository<ZnodeTaxRule>();
                List<string> taxClassIdsList =new List<string>(taxClassPortalModel.TaxClassIds.Split(',')).Distinct().ToList();
                IEnumerable<int?> portalTaxClassList = _taxRuleRepository.Table.Where(x => taxClassIdsList.Contains(x.TaxClassId.ToString())).Select(y => y.TaxClassId).Distinct().ToList();
                if (taxClassIdsList.Count() != portalTaxClassList.Count())
                {
                    return null;
                }
                else
                {
                    ZnodeLogging.LogMessage("portalTaxClassList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalTaxClassList?.Count());                  
                    return portalTaxClassList.Distinct(); 
                }
            }
            return null;
        }

        //Set filter for PortalId and isDefault.
        private static FilterCollection SetPortalIdAndIsDefaultFilter(int portalId, string isDefault)
        {
            ZnodeLogging.LogMessage("Input Parameters portalId and isDefault:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { portalId, isDefault });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalTaxClassEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalTaxClassEnum.IsDefault.ToString(), ProcedureFilterOperators.Equals, isDefault));
            return filters;
        }

        //Check is tax class is enabled or not.
        private bool IsTaxClassEnabled(int taxClassId)
        {
            ZnodeTaxClass taxClass = _taxClassRepository.GetById(taxClassId);
            if (IsNotNull(taxClass))
                return taxClass.IsActive;
            return false;
        }
        //Unassociate taxclass from portal.
        private bool UnAssociateTaxClass(TaxClassPortalModel taxClassPortalModel)
        {
            EntityWhereClauseModel whereClauseModel = GetPortalTaxWhereClause(taxClassPortalModel.TaxClassIds, taxClassPortalModel.PortalId);
            ZnodeLogging.LogMessage("WhereClause : ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel);
            bool status = _taxClassPortalRepository.Delete(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
            ZnodeLogging.LogMessage((status ? Admin_Resources.SuccessTaxClassDissociation : Admin_Resources.ErrorTaxClassDissociation), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return status;
        }

        //Get TaxClassPortalModel list.
        private List<ZnodePortalTaxClass> GetTaxClassPortalModelList(TaxClassPortalModel taxClassPortalModel)
        {
            List<ZnodePortalTaxClass> taxClassPortalModelList = new List<ZnodePortalTaxClass>();
            if (IsNotNull(taxClassPortalModel))
            {
                List<string> taxClassIs = taxClassPortalModel.TaxClassIds?.Split(',')?.ToList();
                taxClassIs?.ForEach(item =>
                {
                    taxClassPortalModelList.Add(new ZnodePortalTaxClass { TaxClassId = Convert.ToInt32(item), PortalId = taxClassPortalModel.PortalId, IsDefault = false });
                });
                ZnodeLogging.LogMessage("taxClassIs list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, taxClassIs?.Count());
            }

            return taxClassPortalModelList;
        }

        //Get whereclause for portal tax class.
        private EntityWhereClauseModel GetPortalTaxWhereClause(string taxClassIds, int portalId)
            => DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetPortalTaxClassFilter(taxClassIds, portalId).ToFilterDataCollection());

        //Get filter for portal tax class.
        private FilterCollection GetPortalTaxClassFilter(string taxClassIds, int portalId)
            => new FilterCollection() {
            new FilterTuple(ZnodePortalTaxClassEnum.TaxClassId.ToString(), FilterOperators.In, taxClassIds),
            new FilterTuple(ZnodePortalTaxClassEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString())
            };

        //Insert into portal Country.
        private static void InsertPortalCountry(PortalModel portalModel)
        {
            if (portalModel.CopyContentPortalId == null || portalModel.CopyContentPortalId == 0)
            {
                ZnodeLogging.LogMessage("Input Parameters PortalModel having CopyContentPortalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalModel.CopyContentPortalId);
                IZnodeRepository<ZnodePortalCountry> _portalCountryRepository = new ZnodeRepository<ZnodePortalCountry>();
                ZnodeLogging.LogMessage(_portalCountryRepository.Insert(new ZnodePortalCountry() { CountryCode = DefaultGlobalConfigSettingHelper.DefaultCountry, PortalId = portalModel.PortalId, IsDefault = true })?.PortalCountryId > 0 ? "Portal country inserted successfully." : "Fail to insert portal country.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
        }

        //Get expands and add them to navigation properties.
        private List<string> GetExpandsForPortal(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                    if (Equals(key, ZnodeGoogleTagManagerEnum.ZnodePortal.ToString().ToLower())) SetExpands(ZnodeGoogleTagManagerEnum.ZnodePortal.ToString(), navigationProperties);
            }
            return navigationProperties;
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpandsForParentTheme()
        {
            List<string> navigationProperties = new List<string>();
            SetExpands(ZnodeCMSThemeEnum.ZnodeCMSTheme2.ToString(), navigationProperties);
            return navigationProperties;
        }

        //Get Approver Details
        private List<UserApproverModel> GetApprovers(int portalApprovalId)
        {

            return (from userApprover in _userApproverRepository.Table
                    where userApprover.PortalApprovalId == portalApprovalId
                    select new UserApproverModel
                    {
                        UserApproverId = userApprover.UserApproverId,
                        UserId = userApprover.UserId,
                        ToBudgetAmount = userApprover.ToBudgetAmount,
                        FromBudgetAmount = userApprover.FromBudgetAmount,
                        ApproverOrder = userApprover.ApproverOrder,
                        ApproverLevelId = userApprover.ApproverLevelId.Value,
                        ApproverUserId = userApprover.ApproverUserId,
                        IsNoLimit = userApprover.IsNoLimit
                    }).ToList();
        }

        //Delete entries from portal sort settings.
        private bool RemoveFromPortalSortSetting(SortSettingAssociationModel associationModel)
        {
            FilterCollection filters = GetSortSettingIdFilter(associationModel.PortalSortSettingIds);
            filters.Add(new FilterTuple(ZnodePortalPageSettingEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, associationModel.PortalId.ToString()));
            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, entityWhereClauseModel);
            return _portalSortSettingRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
        }

        //Delete entries from portal sort settings.
        private bool RemoveFromPortalPageSetting(PageSettingAssociationModel associationModel)
        {
            FilterCollection filters = GetPageSettingIdFilter(associationModel.PortalPageSettingIds);
            filters.Add(new FilterTuple(ZnodePortalPageSettingEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, associationModel.PortalId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalPageSettingEnum.IsDefault.ToString(), ProcedureFilterOperators.NotEquals, "true"));
            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, entityWhereClauseModel);
            return _portalPageSettingRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
        }

        //Get Page setting ids filter.
        private FilterCollection GetPageSettingIdFilter(string portalPageSettingIds)
            => new FilterCollection() { new FilterTuple(ZnodePortalPageSettingEnum.PortalPageSettingId.ToString(), ProcedureFilterOperators.In, portalPageSettingIds) };

        //Get Page setting ids filter.
        private FilterCollection GetSortSettingIdFilter(string portalSortSettingIds)
            => new FilterCollection() { new FilterTuple(ZnodePortalSortSettingEnum.PortalSortSettingId.ToString(), ProcedureFilterOperators.In, portalSortSettingIds) };

        //Get IsAssociated From Filter.
        private bool GetIsAssociatedFromFilter(FilterCollection filters)
        {
            bool isAssociated = false;
            if (filters?.Count > 0 && filters.Any(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase)))
            {
                //Get filter value
                isAssociated = Convert.ToBoolean(filters.FirstOrDefault(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
                //Remove IsAssociated Filter from filters list
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("isAssociated:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, isAssociated);
            return isAssociated;
        }
        //Get PortalId From Filter.
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
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalId);
            return portalId;
        }
        //Remove filter for Page and Sort.
        private FilterCollection GetUpdatedFiltersforPageAndSort(FilterCollection filters, bool isAssociated, string type)
        {
            if (!isAssociated && filters?.Count > 0 && filters.Any(x => string.Equals(x.FilterName, type, StringComparison.InvariantCultureIgnoreCase)))
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, type, StringComparison.InvariantCultureIgnoreCase));
            }
            return filters;
        }

        //Add filter for Page and Sort.
        private FilterCollection GetModifiedFiltersforPageAndSort(List<FilterTuple> modfilters, FilterCollection filters, string type)
        {
            if (modfilters?.Count > 0 && modfilters.Any(x => string.Equals(x.FilterName, type, StringComparison.InvariantCultureIgnoreCase)))
            {
                filters.Add(type, Convert.ToString(modfilters.FirstOrDefault(x => string.Equals(x.FilterName, type, StringComparison.InvariantCultureIgnoreCase))?.FilterOperator), Convert.ToString(modfilters.FirstOrDefault(x => string.Equals(x.FilterName, type, StringComparison.InvariantCultureIgnoreCase))?.FilterValue));
            }
            return filters;
        }
        //Insert into portal sort setting.
        private bool InsertIntoPortalSortSetting(SortSettingAssociationModel associationModel)
        {
            List<ZnodePortalSortSetting> entriesToInsert = new List<ZnodePortalSortSetting>();

            //Get the Sort Setting Details based on requested Sort Settings.
            int[] sortSettingIds = associationModel.SortSettingIds.Split(',').Select(Int32.Parse).ToArray();
            List<ZnodeSortSetting> lstSortSetting = _sortSettingRepository.Table.Where(x => sortSettingIds.Contains(x.SortSettingId)).ToList();
            ZnodeLogging.LogMessage("lstPageSetting list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, lstSortSetting?.Count());
            foreach (string item in associationModel.SortSettingIds.Split(','))
            {
                string sortDisplayName = lstSortSetting.Where(x => x.SortSettingId == Convert.ToInt32(item)).Select(x => x.SortName).FirstOrDefault();
                int displayOrder = lstSortSetting.Where(x => x.SortSettingId == Convert.ToInt32(item)).Select(x => x.DisplayOrder).FirstOrDefault();
                entriesToInsert.Add(new ZnodePortalSortSetting() { PortalId = associationModel.PortalId, SortSettingId = Convert.ToInt32(item), SortDisplayName = sortDisplayName, DisplayOrder = displayOrder });
            }
            if (entriesToInsert.Count > 0)
                entriesToInsert = _portalSortSettingRepository.Insert(entriesToInsert)?.ToList();

            return entriesToInsert?.Count > 0;
        }

        //Insert into portal sort setting.
        private bool InsertIntoPortalPageSetting(PageSettingAssociationModel associationModel, bool isDefaultLoad = false)
        {
            List<ZnodePortalPageSetting> entriesToInsert = new List<ZnodePortalPageSetting>();

            //Get the Page Setting Details based on requested Page Settings.
            int[] pageSettingIds = associationModel.PageSettingIds.Split(',').Select(Int32.Parse).ToArray();
            List<ZnodePageSetting> lstPageSetting = _pageSettingRepository.Table.Where(x => pageSettingIds.Contains(x.PageSettingId)).ToList();
            ZnodeLogging.LogMessage("lstPageSetting list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, lstPageSetting?.Count());
            foreach (string item in associationModel.PageSettingIds.Split(','))
            {
                ZnodePageSetting pageSetting = lstPageSetting.FirstOrDefault(x => x.PageSettingId == Convert.ToInt32(item));
                if(IsNotNull(pageSetting))
                {                    
                    entriesToInsert.Add(new ZnodePortalPageSetting() { PortalId = associationModel.PortalId, PageSettingId = Convert.ToInt32(item), PageDisplayName = pageSetting.PageName, DisplayOrder = pageSetting.DisplayOrder, IsDefault = isDefaultLoad && pageSetting.PageValue == 16 });
                }                
            }
            if (entriesToInsert.Count > 0)
                entriesToInsert = _portalPageSettingRepository.Insert(entriesToInsert)?.ToList();

            return entriesToInsert?.Count > 0;
        }
        // Updates sorting type filter
        private NameValueCollection GetUpdatedSortingType(NameValueCollection sorts, string removeFilter, string addFilter)
        {
            if (sorts.AllKeys.First() == removeFilter)
            {
                string sortingOrder = sorts[removeFilter];
                sorts.Remove(removeFilter);
                sorts.Add(addFilter, sortingOrder);
            }
            return sorts;
        }
        #endregion
    }
}
