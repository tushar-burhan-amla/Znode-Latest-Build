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
    public class LocaleService : BaseService, ILocaleService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeLocale> _localeRepository;
        #endregion

        #region  public constructor for creating repository instance.
        public LocaleService()
        {
            //Initialize instance of repository.
            _localeRepository = new ZnodeRepository<ZnodeLocale>();
        }
        #endregion

        #region Public Methods
        //Get a list of Locales
        public virtual LocaleListModel GetLocaleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if (sorts?.Count > 0 && string.IsNullOrEmpty(sorts[0]))
                sorts.Add(ZnodeLocaleEnum.IsDefault.ToString(), "DESC");

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate localeListEntity list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Gets the entity list according to where clause, order by clause and pagination
            IList<ZnodeLocale> localeListEntity = _localeRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("localeListEntity list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, localeListEntity?.Count());
            //Todo :  Need to replace mapping with Automapper.
            //maps the entity list to model
            LocaleListModel localeList = LocaleMap.ToListModel(localeListEntity);
            localeList.Locales = localeList?.Locales?.Count > 0 ? localeList?.Locales?.ToList() : null;
            ZnodeLogging.LogMessage("Locales:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, localeList.Locales);
            localeList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return localeList;
        }

        //Get a Locale.
        public virtual LocaleModel GetLocale(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

                ZnodeLogging.LogMessage("whereClauseModel generated", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                return LocaleMap.ToModel(_localeRepository.GetEntity(whereClauseModel.WhereClause));
        }

        //Update Locales.
        public virtual bool UpdateLocale(DefaultGlobalConfigListModel globalConfigListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if (Equals(globalConfigListModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.DefaultGlobalConfigNotNull);
            try
            {
                FilterCollection filterList = new FilterCollection();
                filterList.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), ProcedureFilterOperators.In, string.Join(",", globalConfigListModel?.DefaultGlobalConfigs?.Select(x => x.SelectedIds).ToList())));

                //Gets the where clause with filter values.                 
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClauseModel to generate localeConfigurationList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                List<ZnodeLocale> localeConfigurationList = _localeRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
                ZnodeLogging.LogMessage("localeConfigurationList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, localeConfigurationList?.Count());
                bool checkIsDefault = localeConfigurationList.Any(x => x.IsDefault);

                if (globalConfigListModel?.DefaultGlobalConfigs?.Count() > 0)
                {
                    foreach (var item in globalConfigListModel.DefaultGlobalConfigs)
                    {
                        if (Equals(item.Action, "SetActive"))
                            SetActive(localeConfigurationList);

                        else if (Equals(item.Action, "SetDeActive"))
                            return SetDeActive(localeConfigurationList, checkIsDefault);

                        else if (Equals(item.Action, "SetDefault"))
                            SetDefault(localeConfigurationList);

                        localeConfigurationList?.ForEach(x => _localeRepository.Update(x));
                    }
                    //Remove associations of locales.
                    RemoveAssociationOfLocales();
                    return true;
                }
                ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorLocaleUpdate, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        #endregion

        #region Private Methods
        private static void SetActive(List<ZnodeLocale> localeConfigurationList) => localeConfigurationList.ForEach(x => x.IsActive = true);


        private bool SetDeActive(List<ZnodeLocale> localeConfigurationList, bool checkIsDefault)
        {
            localeConfigurationList.RemoveAll(x => x.IsDefault || !x.IsActive);
            localeConfigurationList.ForEach(x => x.IsActive = false);
            localeConfigurationList?.ForEach(x => _localeRepository.Update(x));
            //Remove associations of locales.
            RemoveAssociationOfLocales();
            if (checkIsDefault)
                throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDeactivate");
            return true;
        }

        private void SetDefault(List<ZnodeLocale> localeConfigurationList)
        {
            //Get list of entity from table. 
            List<ZnodeLocale> defaultConfigurationList = _localeRepository.Table.Where(x => x.IsDefault).ToList();

            ZnodeLogging.LogMessage("defaultConfigurationList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, defaultConfigurationList?.Count());
            defaultConfigurationList.ForEach(x => x.IsDefault = false);

            if (localeConfigurationList.Any(x => !x.IsActive))
                throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDefault");

            defaultConfigurationList?.ForEach(x => _localeRepository.Update(x));
            localeConfigurationList?.FirstOrDefault(x => x.IsDefault = true);
        }

        //Remove associations of locales.
        private void RemoveAssociationOfLocales()
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(FilterKeys.Status, null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> output = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateGlobalLocale  @Status OUT", 0, out status);
        }

        #endregion
    }
}
