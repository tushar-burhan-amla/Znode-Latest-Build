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
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class CurrencyService : BaseService, ICurrencyService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCurrency> _currencyRepository;
        private readonly IZnodeRepository<ZnodeCulture> _cultureRepository;
        #endregion

        #region Constructor
        public CurrencyService()
        {
            _currencyRepository = new ZnodeRepository<ZnodeCurrency>();
            _cultureRepository = new ZnodeRepository<ZnodeCulture>();
        }
        #endregion

        #region public Methods

        //Get a list of currencies
        public virtual CurrencyListModel GetCurrencies(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate currency list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Gets the entity list according to where clause, order by clause and pagination
            _currencyRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);

            CurrencyListModel currencyList = new CurrencyListModel
            {
                Currencies = _currencyRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).
                ToModel<CurrencyModel>()?.ToList()
            };
            currencyList?.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return currencyList;
        }

        //Get a list of currencies
        public virtual CurrencyListModel CurrencyCultureList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            // Updated the filter value for IsActive and IsDefault as boolean since the sp did not accept string value.

            string IsActiveValue = filters.Find(x => x.FilterName.Equals(FilterKeys.IsActive, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;
            if(!string.IsNullOrEmpty(IsActiveValue))
            { 
            IsActiveValue = Convert.ToBoolean(IsActiveValue) ? FilterKeys.ActiveTrue : FilterKeys.ActiveFalse;
            filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.IsActive, StringComparison.InvariantCultureIgnoreCase));
            filters.Add(FilterKeys.IsActive, FilterOperators.Equals, IsActiveValue);
            }
            string IsDefaultValue = filters.Find(x => x.FilterName.Equals(ZnodeConstant.IsDefault, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;
            if (!string.IsNullOrEmpty(IsDefaultValue))
            {
            IsDefaultValue = Convert.ToBoolean(IsDefaultValue) ? FilterKeys.ActiveTrue : FilterKeys.ActiveFalse;
            filters.RemoveAll(x => x.FilterName.Equals(ZnodeConstant.IsDefault, StringComparison.InvariantCultureIgnoreCase));
            filters.Add(ZnodeConstant.IsDefault, FilterOperators.Equals, IsDefaultValue);
            }

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate currencyListEntity list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<CurrencyModel> objStoredProc = new ZnodeViewRepository<CurrencyModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            List<CurrencyModel> currencyListEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetCurrency @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("currencyListEntity list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, currencyListEntity?.Count());
            CurrencyListModel currencyList = new CurrencyListModel
            {
                Currencies = currencyListEntity
            };
            currencyList?.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return currencyList;
        }

        //Get a list of culture
        public virtual CultureListModel GetCulture(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate culture list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Gets the entity list according to where clause, order by clause and pagination
            _cultureRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);

            CultureListModel cultureList = new CultureListModel
            {
                Culture = _cultureRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).
                ToModel<CultureModel>()?.ToList()
            };
            cultureList?.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return cultureList;
        }

        //Get a currency as per the filter passed
        public virtual CurrencyModel GetCurrency(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            try
            {
                //gets the where clause with filter Values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("Where clause generated", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose,whereClauseModel);
                return _currencyRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues).ToModel<CurrencyModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Get currency details
        public virtual CurrencyModel GetCurrencyDetail(int portalId)
        {
            IZnodeRepository<ZnodeCurrency> znodeCurrency = new ZnodeRepository<ZnodeCurrency>();
            IZnodeRepository<ZnodePortalUnit> znodePortalUnit = new ZnodeRepository<ZnodePortalUnit>();

            CurrencyModel currencyModel = (from _znodePortalUnit in znodePortalUnit.Table
                                           join _znodeCurrency in znodeCurrency.Table on _znodePortalUnit.CurrencyId equals _znodeCurrency.CurrencyId
                                           where _znodePortalUnit.PortalId == portalId
                                           select new CurrencyModel
                                           {
                                               CurrencyCode = _znodeCurrency.CurrencyCode,
                                               CurrencyId = _znodeCurrency.CurrencyId
                                           })?.FirstOrDefault();

            return currencyModel;
        }


        //Get a currency as per the filter passed
        public virtual CultureModel GetCultureCode(FilterCollection filters)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                //gets the where clause with filter Values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("Where clause generated", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                return _cultureRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues).ToModel<CultureModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Update currencies
        public virtual bool UpdateCurrency(DefaultGlobalConfigListModel globalConfigListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if (Equals(globalConfigListModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.DefaultGlobalConfigModelNotNull);
            try
            {
                FilterCollection filterList = new FilterCollection();
                filterList.Add(new FilterTuple(ZnodeConstant.CultureId.ToString(), ProcedureFilterOperators.In, string.Join(",", globalConfigListModel?.DefaultGlobalConfigs?.Select(x => x.SelectedIds).ToList())));

                //Gets the where clause with filter values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClauseModel to generate currency list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                List<ZnodeCulture> cutlureConfigurationList = _cultureRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
                ZnodeLogging.LogMessage("cutlureConfigurationList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, cutlureConfigurationList?.Count());
                filterList.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.CultureId.ToString(), StringComparison.CurrentCultureIgnoreCase));

                filterList.Add(new FilterTuple(ZnodeCurrencyEnum.CurrencyId.ToString(), ProcedureFilterOperators.In, string.Join(",", cutlureConfigurationList?.Select(x => x.CurrencyId).ToList())));

                //Gets the where clause with filter values.              
                whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClauseModel to generate currencyConfiguration list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                List<ZnodeCurrency> currencyConfigurationList = _currencyRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
                ZnodeLogging.LogMessage("currencyConfigurationList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, currencyConfigurationList?.Count());
                bool checkIsDefault = cutlureConfigurationList.Any(x => x.IsDefault);

                if (globalConfigListModel?.DefaultGlobalConfigs?.Count() > 0)
                {
                    foreach (var item in globalConfigListModel.DefaultGlobalConfigs)
                    {

                        if (Equals(item.Action, "SetActive"))
                            SetActive(cutlureConfigurationList, currencyConfigurationList);

                        else if (Equals(item.Action, "SetDeActive"))
                            return SetDeactive(cutlureConfigurationList, checkIsDefault, currencyConfigurationList, filterList);

                        else if (Equals(item.Action, "SetDefault"))
                            SetDefault(cutlureConfigurationList, currencyConfigurationList);

                        currencyConfigurationList?.ForEach(x => _currencyRepository.Update(x));
                        cutlureConfigurationList?.ForEach(x => _cultureRepository.Update(x));
                        
                        return true;
                    }
                }
                ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorCurrencyUpdate,ex.Message), ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }
        #endregion

        #region Private Methods

        private static void SetActive(List<ZnodeCulture> cultureConfigurationList, List<ZnodeCurrency> currencyConfigurationList)
        {
            currencyConfigurationList.ForEach(x => x.IsActive = true);
            cultureConfigurationList.ForEach(x => x.IsActive = true);           
        }

        private bool SetDeactive(List<ZnodeCulture> cultureConfigurationList, bool checkIsDefault, List<ZnodeCurrency> currencyConfigurationList, FilterCollection filterList)
        {
            currencyConfigurationList.RemoveAll(x => x.IsDefault || !x.IsActive);
            currencyConfigurationList.ForEach(x => x.IsActive = false);
            currencyConfigurationList?.ForEach(x => _currencyRepository.Update(x));
            List<ZnodeCurrency> CurrencyList = _currencyRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection()).WhereClause).ToList();
            bool checkIsActive = CurrencyList.Any(x => x.IsActive);
            if (!checkIsActive)
            {
                cultureConfigurationList.ForEach(x => x.IsActive = false);
                cultureConfigurationList?.ForEach(x => _cultureRepository.Update(x));
            }
            if (checkIsDefault)
                throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDeactivate");
            return true;
        }

        private void SetDefault(List<ZnodeCulture> cutlureConfigurationList, List<ZnodeCurrency> currencyConfigurationList)
        {
            //Get list of entity from table. 
            List<ZnodeCulture> defaultConfigurationList = _cultureRepository.Table.Where(x => x.IsDefault).ToList();
            List<ZnodeCurrency> defaultCurrencyConfigurationList = _currencyRepository.Table.Where(x => x.IsDefault).ToList();
            defaultConfigurationList.ForEach(x => x.IsDefault = false);
            defaultCurrencyConfigurationList.ForEach(x => x.IsDefault = false);

            if (cutlureConfigurationList.Any(x => !x.IsActive))
                throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDefault");

            defaultConfigurationList?.ForEach(x => _cultureRepository.Update(x));
            defaultCurrencyConfigurationList.ForEach(x => _currencyRepository.Update(x));
            cutlureConfigurationList?.FirstOrDefault(x => x.IsDefault = true);
            currencyConfigurationList?.FirstOrDefault(x => x.IsDefault = true);
        }

        #endregion
    }
}
