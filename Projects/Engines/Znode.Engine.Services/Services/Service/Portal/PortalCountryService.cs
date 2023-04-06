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
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class PortalCountryService : BaseService, IPortalCountryService
    {
        #region Private Member
        private readonly IZnodeRepository<ZnodePortalCountry> _portalCountryRepository;

        #endregion

        #region Constructor
        public PortalCountryService()
        {
            _portalCountryRepository = new ZnodeRepository<ZnodePortalCountry>();
        }
        #endregion

        #region Associate Country
        //Get list of associated countries to store.
        public virtual CountryListModel GetAssociatedCountryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<CountryModel> objStoredProc = new ZnodeViewRepository<CountryModel>();
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", 1, ParameterDirection.Input, DbType.Int32);

            CountryListModel countyListModel = new CountryListModel() { Countries = objStoredProc.ExecuteStoredProcedureList("Znode_GetPortalCountry @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@IsAssociated", 4, out pageListModel.TotalRowCount)?.ToList() };
            countyListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return countyListModel;
        }

        //Get list of unassociated countries to store.
        public virtual CountryListModel GetUnAssociatedCountryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            filters.Add(ZnodeConstant.IsAssociated, FilterOperators.Equals, ZnodeConstant.Zero);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<CountryModel> objStoredProc = new ZnodeViewRepository<CountryModel>();
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", 0, ParameterDirection.Input, DbType.Int32);

            CountryListModel countyListModel = new CountryListModel() { Countries = objStoredProc.ExecuteStoredProcedureList("Znode_GetPortalCountry @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@IsAssociated", 4, out pageListModel.TotalRowCount)?.ToList() };
            countyListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return countyListModel;
        }

        //Associate Countries to store.
        public virtual bool AssociateCountries(ParameterModelForPortalCountries model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model?.CountryCode) || model?.PortalId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.CountryCodeNotNull);
            ZnodeLogging.LogMessage("ParameterModelForPortalCountries Model: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose,model);
            if (model?.PortalCountryId > 0)
            {
                //Update already existing isDefault value of country to false.
                UpdateIsDefaultAlreadyExists(model.PortalId);

                var entity = model.ToEntity<ZnodePortalCountry>();

                //Update portal country.
                bool result = _portalCountryRepository.Update(entity);

                if (result)
                {
                    //Clear cache call
                    var clearCacheInitializer = new ZnodeEventNotifier<ZnodePortalCountry>(entity);
                }
                return result;
            }
            else
            {
                List<ZnodePortalCountry> entriesToInsert = new List<ZnodePortalCountry>();
                AssociateMultipleCountriesToPortal(model, entriesToInsert);
                bool result = _portalCountryRepository.Insert(entriesToInsert)?.Count() > 0;

                if (result && HelperUtility.IsNotNull(model))
                {
                    //Clear cache call
                    var clearCacheInitializer = new ZnodeEventNotifier<ZnodePortalCountry>(new ZnodePortalCountry { PortalId = model.PortalId });
                }
                return result;
            }
        }

        //UnAssociate Countries from store.
        public virtual bool UnAssociateCountries(ParameterModelForPortalCountries portalCountries)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(portalCountries) || string.IsNullOrEmpty(portalCountries.PortalCountryIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);

            ZnodeLogging.LogMessage("ParameterModelForPortalCountries portalCountries: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalCountries);
            //Get default portal country id.
            int defaultPortalCountryId = (GetDefaultPortalCountryEntity(portalCountries.PortalId)?.PortalCountryId).GetValueOrDefault();
            ZnodeLogging.LogMessage("defaultPortalCountryId: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, defaultPortalCountryId);
            //Check if default portal country id is greater,throws an exception default country can not be deleted and delete non default portal countries.
            if (defaultPortalCountryId > 0)
                DeleteNonDefaultPortalCountries(portalCountries, defaultPortalCountryId);

            //Delete mapping of countries against store.
            bool isDeleted = DeletePortalCountries(portalCountries.PortalCountryIds);

            if (isDeleted)
            {
                //Clear cache call
                var clearCacheInitializer = new ZnodeEventNotifier<ZnodePortalCountry>(new ZnodePortalCountry { PortalId = portalCountries.PortalId });
            }
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessCountriesUnassociated : Admin_Resources.ErrorCountriesUnassociated, string.Empty, TraceLevel.Info);
            return isDeleted;
        }
        #endregion

        #region Private Method
        //Update already existing isDefault value of country to false.
        private bool UpdateIsDefaultAlreadyExists(int portalId)
        {
            ZnodeLogging.LogMessage("portalId: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalId);
            //Get Default Portal Country Entity by portal id.
            ZnodePortalCountry portalCountry = GetDefaultPortalCountryEntity(portalId);
            if (HelperUtility.IsNotNull(portalCountry))
            {
                //Update default value to false.
                portalCountry.IsDefault = false;
                return _portalCountryRepository.Update(portalCountry);
            }
            return false;
        }

        //Delete non default portal countries and throws an exception default country can not be deleted.
        private void DeleteNonDefaultPortalCountries(ParameterModelForPortalCountries portalCountries, int defaultPortalCountryId)
        {
            ZnodeLogging.LogMessage("defaultPortalCountryId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, defaultPortalCountryId);
            //Check if portalCountryIds contains default portal country id.
            if (portalCountries.PortalCountryIds.Contains(System.Convert.ToString(defaultPortalCountryId)))
            {
                IList<int> defaultPortalCountry = new List<int>();
                defaultPortalCountry.Add(defaultPortalCountryId);

                //If portalCountryIds contains default portal country id,it removes default portal country id so that is will not be deleted.
                portalCountries.PortalCountryIds = string.Join(",", portalCountries.PortalCountryIds.Split(',').Select(int.Parse).AsEnumerable().Except(defaultPortalCountry).ToList());

                ZnodeLogging.LogMessage("PortalCountryIds  list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalCountries.PortalCountryIds?.Count());
                //If default Portal Country Id is greater throws an exception that default portal country id can not be deleted.
                if (!string.IsNullOrEmpty(portalCountries.PortalCountryIds))
                    ZnodeLogging.LogMessage(DeletePortalCountries(portalCountries.PortalCountryIds) ? Admin_Resources.SuccessCountriesUnassociated : Admin_Resources.ErrorCountriesUnassociated, string.Empty, TraceLevel.Info);

                throw new ZnodeException(ErrorCodes.DefaultDataDeletionError, Admin_Resources.DefaultPortalCountryDeleteError);
            }
        }

        //Delete portal countries from store. 
        private bool DeletePortalCountries(string portalCountryIds)
        {
            ZnodeLogging.LogMessage("portalCountryIds:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalCountryIds);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalCountryEnum.PortalCountryId.ToString(), ProcedureFilterOperators.In, portalCountryIds));

            return _portalCountryRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Get Default Portal Country Entity by portal id.
        private ZnodePortalCountry GetDefaultPortalCountryEntity(int portalId)
        {
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, portalId);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalCountryEnum.IsDefault.ToString(), ProcedureFilterOperators.Equals, ZnodeConstant.TrueValue));

            //Get country model which is default. 
            return _portalCountryRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Associate multiple Countries to Portal.
        private void AssociateMultipleCountriesToPortal(ParameterModelForPortalCountries model, List<ZnodePortalCountry> entriesToInsert)
        {
            ZnodeLogging.LogMessage("ParameterModelForPortalCountries model:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, model);
            string[] countryCodes = model.CountryCode.Split(',');
            IZnodeRepository<ZnodeCountry> _countryRepository = new ZnodeRepository<ZnodeCountry>();

            //Get default country code from global setting.
            string defaultCountryCode = _countryRepository.Table.Where(x => x.IsDefault)?.Select(x => x.CountryCode)?.FirstOrDefault();
            ZnodeLogging.LogMessage("defaultCountryCode:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, defaultCountryCode);
            foreach (string countryCode in countryCodes)
            {
                //Check if PortalCountryIds to associate contains default portalCountryId. 
                if (string.Equals(countryCode, defaultCountryCode, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    //If current portal already contains default country it will update it.
                    if ((GetDefaultPortalCountryEntity(model.PortalId)?.PortalCountryId).GetValueOrDefault() > 0)
                        //Update already existing isDefault value of country to false.
                        UpdateIsDefaultAlreadyExists(model.PortalId);

                    entriesToInsert.Add(new ZnodePortalCountry() { PortalId = model.PortalId, CountryCode = countryCode, IsDefault = true });
                }
                else
                    entriesToInsert.Add(new ZnodePortalCountry() { PortalId = model.PortalId, CountryCode = countryCode, IsDefault = model.IsDefault });
            }
            #endregion
        }
    }
}