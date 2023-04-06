using System;
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
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class CountryService : BaseService, ICountryService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCountry> _countryRepository;
        #endregion

        #region Constructor
        public CountryService()
        {
            _countryRepository = new ZnodeRepository<ZnodeCountry>();
        }
        #endregion

        #region Public Methods

        //Get a list of countries
        public virtual CountryListModel GetCountries(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("PageListModel for generating countryList", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel);
            //Gets the entity list according to where clause, order by clause and pagination
            IList<ZnodeCountry> countryListEntity = _countryRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("countryListEntity list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, countryListEntity?.Count());
            //Todo :  Need to replace mapping with Automapper.
            //maps the entity list to model
            CountryListModel countryList = CountryMap.ToListModel(countryListEntity);

            countryList.Countries = countryList?.Countries?.Count > 0 ? countryList?.Countries?.ToList() : null;

            countryList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            return countryList;
        }
        [Obsolete]
        //Not Used                                                                                                                                                                                  
        //Get a country as per the filter passed
        public virtual CountryModel GetCountry(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            try
            {
                //gets the where clause with filter Values.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("Where clause generated", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                return CountryMap.ToModel(_countryRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        //Update countries
        public virtual bool UpdateCountry(DefaultGlobalConfigListModel globalConfigListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if (Equals(globalConfigListModel, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.DefaultGlobalConfigModelNotNull);
            try
            {
                FilterCollection filterList = new FilterCollection();
                filterList.Add(new FilterTuple(ZnodeCountryEnum.CountryId.ToString(), ProcedureFilterOperators.In, string.Join(",", globalConfigListModel?.DefaultGlobalConfigs?.Select(x => x.SelectedIds).ToList())));

                //Gets the where clause with filter values.            
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause generated to get countryConfiguration List", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, whereClauseModel);
                List<ZnodeCountry> countryConfigurationList = _countryRepository.GetEntityList(whereClauseModel.WhereClause).ToList();
                ZnodeLogging.LogMessage("countryConfiguration list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, countryConfigurationList?.Count());
                bool checkIsDefault = countryConfigurationList.Any(x => x.IsDefault);

                if (globalConfigListModel?.DefaultGlobalConfigs?.Count() > 0)
                {

                   DefaultGlobalConfigModel defaultGlobalConfigModel =  globalConfigListModel.DefaultGlobalConfigs.FirstOrDefault();

                    switch (defaultGlobalConfigModel.Action)
                    {
                        case "SetActive":
                            Activate(countryConfigurationList);
                            break;
                        case "SetDeActive":
                            Deactivate(countryConfigurationList, checkIsDefault);
                            break;
                        case "SetDefault":
                            SetDefault(countryConfigurationList);
                            break;
                      
                    }
                    countryConfigurationList?.ForEach(x => _countryRepository.Update(x));
                    return true;
                }
                ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorCountryUpdate, ex.Message), ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }
        #endregion

        #region Private Methods

        private static void Activate(List<ZnodeCountry> countryConfigurationList) => countryConfigurationList?.ForEach(x => x.IsActive = true);

        private bool Deactivate(List<ZnodeCountry> countryConfigurationList, bool checkIsDefault)
        {
            countryConfigurationList.RemoveAll(x => x.IsDefault || !x.IsActive);
            countryConfigurationList.ForEach(x => x.IsActive = false);
            countryConfigurationList?.ForEach(x => _countryRepository.Update(x));
            if (checkIsDefault)
                throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDeactivate");
            return true;
        }

        private void SetDefault(List<ZnodeCountry> countryConfigurationList)
        {
            //Get list of entity from table. 
            List<ZnodeCountry> defaultConfigurationList = _countryRepository.Table.Where(x => x.IsDefault).ToList();
            ZnodeLogging.LogMessage("defaultConfigurationList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, defaultConfigurationList?.Count());
            defaultConfigurationList.ForEach(x => x.IsDefault = false);

            if (countryConfigurationList.Any(x => !x.IsActive))
                throw new ZnodeException(ErrorCodes.InvalidData, "ErrorDefault");

            defaultConfigurationList?.ForEach(x => _countryRepository.Update(x));
            countryConfigurationList?.FirstOrDefault(x => x.IsDefault = true);
        }
        #endregion
    }
}
