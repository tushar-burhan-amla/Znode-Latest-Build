using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class PortalCountryAgent : BaseAgent, IPortalCountryAgent
    {
        #region Private Variables
        private readonly IPortalCountryClient _portalCountryClient;
        #endregion

        #region Constructor
        public PortalCountryAgent(IPortalCountryClient portalCountryClient)
        {
            _portalCountryClient = GetClient<IPortalCountryClient>(portalCountryClient);
        }
        #endregion

        #region Public Methods
        #region Country Association
        //Get list of associated and unassociate countries on the basis of flag.
        public virtual CountryListViewModel GetAssociatedOrUnAssociatedCountryList(FilterCollectionDataModel model, int portalId, bool isAssociatedList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId and isAssociatedList:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalId, isAssociatedList });

            CountryListModel list = new CountryListModel();

            //Get the sort collection for portal country id id desc.
            SortCollection sortCollection = new SortCollection();
            sortCollection = HelperMethods.SortDesc(ZnodePortalCountryEnum.PortalCountryId.ToString(), sortCollection);

            //Set Filters for portalId.
            SetFilters(model.Filters, portalId);

            ZnodeLogging.LogMessage("sortCollection and Filters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { model.SortCollection, model.Filters });

            list = isAssociatedList ?
                //Get list of associated category.
                _portalCountryClient.GetAssociatedCountryList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage)

                //Get list of unassociated categories.
                : _portalCountryClient.GetUnAssociatedCountryList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            CountryListViewModel listViewModel = new CountryListViewModel { Countries = list?.Countries?.ToViewModel<CountryViewModel>()?.ToList() };
            SetListPagingData(listViewModel, list);

            if (isAssociatedList)
                //Set tool options for this grid.
                SetPortalCountryListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return list?.Countries?.Count > 0 ? listViewModel : new CountryListViewModel() { Countries = new List<CountryViewModel>() };
        }

        //Remove associated countries.
        public virtual bool UnAssociateCountries(string portalCountryId, int portalId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _portalCountryClient.UnAssociateCountries(new ParameterModelForPortalCountries { PortalCountryIds = portalCountryId, PortalId = portalId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DefaultDataDeletionError:
                        message = ex.ErrorMessage;
                        return false;
                    default:
                        message = Admin_Resources.UnassignError;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.UnassignError;
                return false;
            }
        }

        //Associate countries to portal.
        public virtual bool AssociateCountries(int portalId, string countryCode, bool isDefault, int portalCountryId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return _portalCountryClient.AssociateCountries(new ParameterModelForPortalCountries { PortalId = portalId, CountryCode = countryCode, IsDefault = isDefault, PortalCountryId = portalCountryId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion
        #endregion

        #region Private Method
        private void SetFilters(FilterCollection filters, int portalId)
        {
            filters.RemoveAll(x => x.FilterName == ZnodePortalEnum.PortalId.ToString());
            if (portalId > 0)
                filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, Convert.ToString(portalId));
        }

        //Set tool option menus for portal country list grid.
        private void SetPortalCountryListToolMenu(CountryListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('CountryDelete',this)", ControllerName = "Store", ActionName = "UnAssociateCountries" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = GlobalSetting_Resources.IsDefault, JSFunctionName = "Store.prototype.DefaultSubmit('','Store','AssociateCountries','GetAssociatedCountryList')" });
            }
        }
        #endregion
    }
}