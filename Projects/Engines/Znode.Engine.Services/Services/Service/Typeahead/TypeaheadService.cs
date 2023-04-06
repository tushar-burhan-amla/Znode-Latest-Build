using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class TypeaheadService : BaseService, ITypeaheadService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodeUserPortal> _userPortalRepository;
        private readonly IZnodeRepository<ZnodePimCatalog> _pimCatalogRepository;
        private readonly IZnodeRepository<ZnodeGlobalEntity> _globalEntityRepository;

        #endregion

        #region Constructor
        public TypeaheadService()
        {
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
            _pimCatalogRepository = new ZnodeRepository<ZnodePimCatalog>();
            _globalEntityRepository = new ZnodeRepository<ZnodeGlobalEntity>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the suggestions of typeahead.
        /// </summary>
        /// <param name="typeaheadreqModel">Typeahead Request model.</param>
        /// <returns>Returns suggestions list.</returns>
        public virtual TypeaheadResponselistModel GetTypeaheadList(TypeaheadRequestModel typeaheadreqModel)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            switch (typeaheadreqModel.Type)
            {
                case ZnodeTypeAheadEnum.StoreList:
                    return GetStoreList();
                case ZnodeTypeAheadEnum.CatalogList:
                    return GetCatologList(typeaheadreqModel);
                case ZnodeTypeAheadEnum.PIMCatalogList:
                    return GetPIMCatologList();
                case ZnodeTypeAheadEnum.AccountList:
                    return GetAccountList(typeaheadreqModel);
                case ZnodeTypeAheadEnum.EligibleReturnOrderNumberList:
                    return GetEligibleOrderNumberListForReturn(typeaheadreqModel);
                case ZnodeTypeAheadEnum.EntityList:
                    return GetEntityList();

            }
            return null;
        }

        //Get the store list according to logged in user id.
        private TypeaheadResponselistModel GetStoreList()
        {
            TypeaheadResponselistModel typeaheadResponselistModel = new TypeaheadResponselistModel();
            int userId = GetLoginUserId();
            List<ZnodeUserPortal> userPortals = _userPortalRepository.Table.Where(x => x.UserId == userId).ToList();
            if (HelperUtility.IsNotNull(userPortals) && userPortals.Count > 0)
            {
                if (userPortals.Any(x => x.PortalId == null))
                {
                    typeaheadResponselistModel.Typeaheadlist = (from n in _portalRepository.Table
                                                                select new TypeaheadResponseModel
                                                                {
                                                                    Id = n.PortalId,
                                                                    Name = n.StoreName,
                                                                    DisplayText = n.StoreName
                                                                }).OrderBy(s => s.Name).ToList();
                }
                else
                {
                    typeaheadResponselistModel.Typeaheadlist = (from n in _portalRepository.Table
                                                                join portalId in userPortals.Select(z => z.PortalId).ToList() on n.PortalId equals portalId
                                                                select new TypeaheadResponseModel
                                                                {
                                                                    Id = n.PortalId,
                                                                    Name = n.StoreName,
                                                                    DisplayText = n.StoreName
                                                                }).OrderBy(s => s.Name).ToList();
                }
            }
            return typeaheadResponselistModel;
        }

        //Get published catalog list
        private TypeaheadResponselistModel GetCatologList(TypeaheadRequestModel typeaheadreqModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            FilterCollection filters = null;
            IZnodeRepository<ZnodePublishCatalogEntity> _publishCatalogEntity = new ZnodeRepository<ZnodePublishCatalogEntity>(HelperMethods.Context);
            ZnodeLogging.LogMessage("TypeaheadRequestModel to get TypeaheadResponselistModel: ", string.Empty, TraceLevel.Verbose, typeaheadreqModel);
            //to bind filter by fieldname in where clause
            if (!string.IsNullOrEmpty(typeaheadreqModel.FieldName))
            {
                filters = new FilterCollection { new FilterTuple(typeaheadreqModel.FieldName, FilterOperators.Is, typeaheadreqModel.FieldName) };
            }
            //for getting the published category list
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return new TypeaheadResponselistModel
            {
                Typeaheadlist = (from _publishCatalog in _publishCatalogEntity.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter
                                 (filters.ToFilterDataCollection())?.WhereClause)
                                 join _pimCatalog in _pimCatalogRepository.Table on _publishCatalog.ZnodeCatalogId equals _pimCatalog.PimCatalogId
                                 select new TypeaheadResponseModel
                                 {
                                     Id = _publishCatalog.ZnodeCatalogId,
                                     Name = _publishCatalog.CatalogName,
                                     DisplayText = _publishCatalog.CatalogName
                                 })?.DistinctBy(p => p.Id)?.OrderBy(m=> m.Name).ToList()
            };
        }

        //Get PIM catalog list
        private TypeaheadResponselistModel GetPIMCatologList() => new TypeaheadResponselistModel
        {
            Typeaheadlist = (from n in _pimCatalogRepository.Table
                             select new TypeaheadResponseModel
                             {
                                 Id = n.PimCatalogId,
                                 Name = n.CatalogName,
                                 DisplayText = n.CatalogName
                             }).OrderBy(p => p.Name).ToList()
        };

        private TypeaheadResponselistModel GetEntityList() => new TypeaheadResponselistModel
        {
            Typeaheadlist = (from n in _globalEntityRepository.Table
                             select new TypeaheadResponseModel
                             {
                                 Id = n.GlobalEntityId,
                                 Name = n.EntityName,
                                 DisplayText = n.EntityName
                             }).OrderBy(p => p.Name).ToList()
        };

        //Get the account list according to portal id.
        protected virtual TypeaheadResponselistModel GetAccountList(TypeaheadRequestModel typeaheadreqModel)
        {
            TypeaheadResponselistModel typeaheadResponselistModel = new TypeaheadResponselistModel();

            List<AccountModel> accountList = GetAccountListByPortalId(typeaheadreqModel);

            if (accountList?.Count > 0)
            {
                typeaheadResponselistModel.Typeaheadlist = (from account in accountList
                                                            select new TypeaheadResponseModel
                                                            {
                                                                Id = account.AccountId,
                                                                Name = account.Name + " | " + account?.AccountCode,
                                                                DisplayText = account.Name + " | " + account?.AccountCode
                                                            }).OrderBy(s => s.Name).ToList();
            }

            return typeaheadResponselistModel;
        }

        //Get the eligible order number list for return
        protected virtual TypeaheadResponselistModel GetEligibleOrderNumberListForReturn(TypeaheadRequestModel typeaheadreqModel)
        {
            TypeaheadResponselistModel typeaheadResponselistModel = new TypeaheadResponselistModel();

            //Get userid of logged in user.
            int userId = GetLoginUserId();

            IList<string> returnEligibleOrderNumberList = GetEligibleOrderNumberList(typeaheadreqModel, userId);

            if (returnEligibleOrderNumberList?.Count > 0)
            {
                typeaheadResponselistModel.Typeaheadlist = (from orderNumber in returnEligibleOrderNumberList
                                                            select new TypeaheadResponseModel
                                                            {
                                                                Id = orderNumber.ToInteger(),
                                                                Name = orderNumber,
                                                                DisplayText = orderNumber
                                                            }).ToList();
            }
            return typeaheadResponselistModel;
        }

        //Get return eligible order number list by user id.
        protected virtual IList<string> GetEligibleOrderNumberList(TypeaheadRequestModel typeaheadreqModel, int userId)
        {
            //Get RMA Configuration
            RMAConfigurationModel rmaConfiguration = GetService<IRMAConfigurationService>().GetRMAConfiguration();
            IList<string> eligibleOrderNumberList = null;

            if (IsNull(rmaConfiguration) || rmaConfiguration?.MaxDays == 0)
                return eligibleOrderNumberList;

            FilterCollection filters = new FilterCollection();
            if (!string.IsNullOrEmpty(typeaheadreqModel.Searchterm))
                filters.Add(FilterKeys.OrderNumber, FilterOperators.Like, Convert.ToString(typeaheadreqModel.Searchterm));

            ZnodeLogging.LogMessage("Input parameters to Get Eligible Order Number List For Return:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, portalId = typeaheadreqModel.MappingId, maxDays = rmaConfiguration.MaxDays });
            IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
            objStoredProc.SetParameter("@WhereClause", DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection()), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PageSize", typeaheadreqModel.PageSize > 0 ? typeaheadreqModel.PageSize : ZnodeConstant.TypeAheadPageSizeForOrderReturn, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", typeaheadreqModel.MappingId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@MaxDays", rmaConfiguration.MaxDays, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@OrderNumber", string.Empty, ParameterDirection.Input, DbType.String);
            eligibleOrderNumberList = objStoredProc.ExecuteStoredProcedureList("Znode_GetEligibleOrderNumberListForReturn @UserId, @PortalId,@MaxDays,@OrderNumber,@WhereClause,@PageSize");
            return eligibleOrderNumberList;
        }

        /// <summary>
        /// Get account list by portal Id
        /// </summary>
        /// <param name="typeaheadreqModel"></param>
        /// <returns></returns>
        protected virtual List<AccountModel> GetAccountListByPortalId(TypeaheadRequestModel typeaheadreqModel)
        {
            FilterCollection filters = new FilterCollection();
            if (typeaheadreqModel.MappingId > 0)
                filters.Add(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(typeaheadreqModel.MappingId));

            if (!string.IsNullOrEmpty(typeaheadreqModel.Searchterm))
                filters.Add(FilterKeys.Name, FilterOperators.Like, Convert.ToString(typeaheadreqModel.Searchterm));


            //if the global level user creation is set to true then set portal id to null.
            if (!(filters?.Any(x => string.Equals(x.FilterName, ZnodePortalEnum.PortalId.ToString(), StringComparison.CurrentCultureIgnoreCase))).GetValueOrDefault() &&
                !DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                //Bind the Filter conditions for the authorized portal access.
                BindUserPortalFilter(ref filters);

            string whereclause = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection());

            IZnodeViewRepository<AccountModel> objStoredProc = new ZnodeViewRepository<AccountModel>();
            objStoredProc.SetParameter("@WhereClause", whereclause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PageSize", typeaheadreqModel.PageSize > 0 ? typeaheadreqModel.PageSize : ZnodeConstant.DefaultTypeAheadPageSize, ParameterDirection.Input, DbType.Int32);

            var list = objStoredProc.ExecuteStoredProcedureList("Znode_GetAccountList @WhereClause");

            ZnodeLogging.LogMessage("Account list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());

            List<AccountModel> listModel = list?.ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return listModel;
        }
        #endregion
    }
}
