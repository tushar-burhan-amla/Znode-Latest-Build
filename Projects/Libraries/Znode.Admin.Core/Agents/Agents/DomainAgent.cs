using Znode.Engine.Api.Client;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Extensions;
using System.Linq;
using System.Collections.Generic;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System;
using System.Web.Mvc;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Znode.Engine.Admin.Agents
{
    public class DomainAgent : BaseAgent, IDomainAgent
    {
        #region Private Variables
        private readonly IDomainClient _domainClient;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Domain agent.
        /// </summary>
        public DomainAgent(IDomainClient domainClient)
        {
            _domainClient = GetClient<IDomainClient>(domainClient);
        }
        #endregion

        #region Public Methods
        //Method to get domain list.
        public virtual DomainListViewModel GetDomains(int portalId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection= sortCollection });
            //checks if the filter collection null
            filters = IsNotNull(filters) ? filters : new FilterCollection();
            if (portalId > 0)
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, new { filters = filters });
            DomainListModel domainList = _domainClient.GetDomains(filters, sortCollection, pageIndex, recordPerPage);

            if (domainList?.Domains?.Count > 0)
            {
                foreach (DomainModel item in domainList.Domains)
                {
                    item.Status = item.IsActive;
                    item.IsActive = !item.IsActive;
                }
            }
            DomainListViewModel listViewModel = new DomainListViewModel { Domains = domainList?.Domains?.ToViewModel<DomainViewModel>().ToList() };
            SetListPagingData(listViewModel, domainList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return domainList?.Domains?.Count > 0 ? listViewModel : new DomainListViewModel() { Domains = new List<DomainViewModel>() };
        }

        //Method to get domain by domain id.
        public virtual DomainViewModel GetDomain(int domainId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            DomainModel domainModel = _domainClient.GetDomain(domainId);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return IsNotNull(domainModel) ? domainModel.ToViewModel<DomainViewModel>() : new DomainViewModel();
        }

        //Method to create url.
        public virtual DomainViewModel CreateDomainUrl(DomainViewModel domainViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                DomainModel domainModel = domainViewModel.ToModel<DomainModel>();
                domainModel.ApiKey = Guid.NewGuid().ToString();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeDomainEnum.PortalId.ToString(), FilterOperators.Equals, domainViewModel.PortalId + ""));
                filters.Add(new FilterTuple(ZnodeDomainEnum.ApplicationType.ToString(), FilterOperators.EndsWith, domainViewModel.ApplicationType));
                ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters});
                List<DomainModel> Domains = _domainClient.GetDomains(filters, null, null, null)?.Domains;
                ZnodeLogging.LogMessage("Domains list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, Domains?.Count());
                bool IsPreviewUrlExists= Convert.ToBoolean( Domains?.Any(s => s.ApplicationType.Contains(ApplicationTypesEnum.WebstorePreview.ToString())));
                if ((IsNotNull(Domains) && Domains.Count == 0) || IsNull(Domains)|| (IsPreviewUrlExists == false && domainViewModel.ApplicationType== ApplicationTypesEnum.WebstorePreview.ToString()))
                    domainModel.IsDefault = true;
                else
                    domainModel.IsDefault = false;
                
                domainModel = _domainClient.CreateDomain(domainModel);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return IsNotNull(domainModel) ? domainModel.ToViewModel<DomainViewModel>() : new DomainViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorDomainNameAlreadyExists);
                    default:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Method to update domain.
        public virtual DomainViewModel UpdateDomainUrl(DomainViewModel domainViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                domainViewModel.IsActive = domainViewModel.Status;
                DomainModel domainModel = _domainClient.UpdateDomain(domainViewModel.ToModel<DomainModel>());
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return IsNotNull(domainModel) ? domainModel.ToViewModel<DomainViewModel>() : (DomainViewModel)GetViewModelWithErrorMessage(new DomainViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.ErrorDomainNameAlreadyExists);
                    default:
                        return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (DomainViewModel)GetViewModelWithErrorMessage(domainViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        public virtual bool DeleteDomainUrl(string domainId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorFailedToDelete;
            try
            { 
                return _domainClient.DeleteDomain(new ParameterModel() { Ids = domainId });
            } catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Admin_Resources.ErrorFailToDeleteUrl;
                        return false;
                    case ErrorCodes.SetDefaultDataError:
                        message = Admin_Resources.ErrorDefaultDomain;
                        return false;
                    case ErrorCodes.NonDefaultUrlDeleteError:
                        message = Admin_Resources.DeleteNonDefaultDomain;
                        return true;
                    default:
                        message = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            } catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        public virtual List<SelectListItem> GetApplicationTypes()
        {
            List<SelectListItem> applicationType = new List<SelectListItem>();

            applicationType.Add(new SelectListItem() { Text = ApplicationTypesEnum.WebStore.ToString(), Value = ApplicationTypesEnum.WebStore.ToString() });
            applicationType.Add(new SelectListItem() { Text = ApplicationTypesEnum.WebstorePreview.ToString(), Value = ApplicationTypesEnum.WebstorePreview.ToString() });
            ZnodeLogging.LogMessage("applicationType list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, applicationType?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return applicationType;
        }

        //Check whether the domain name is already exists or not.
        public virtual bool CheckDomainNameExist(string domainName, int domainId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //Remove trailing and replace domain protocol name.
            if (!string.IsNullOrEmpty(domainName))
                domainName = ReplaceDomainProtocol(domainName);

            FilterCollection filters = new FilterCollection();

            filters.Add(new FilterTuple(ZnodeDomainEnum.DomainName.ToString(), FilterOperators.Contains, domainName));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the Domain List based on the domain name filter.
            DomainListViewModel listViewModel = new DomainListViewModel { Domains = _domainClient.GetDomains(filters, null, null, null)?.Domains?.ToViewModel<DomainViewModel>().ToList() };

            if (IsNotNull(listViewModel) && IsNotNull(listViewModel.Domains))
            {
                listViewModel.Domains.ForEach(x => x.DomainName = ReplaceDomainProtocol(x.DomainName));
                if (domainId > 0)
                {
                    //Set the status in case the Domain is open in edit mode.
                    DomainViewModel domain = listViewModel.Domains.Find(x => x.DomainId == domainId);
                    if (IsNotNull(domain))
                        return !Equals(domain.DomainName.ToLower(), domainName.ToLower());
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return listViewModel.Domains.FindIndex(x => x.DomainName == domainName) != -1;
            }
            return false;
        }

        //Enable disable domain.
        public virtual bool EnableDisableDomain(string domainId, int portalId, bool isActive, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorMessageEnableDisable;
            try
            {
                return _domainClient.EnableDisableDomain(new DomainModel { DomainIds = domainId, PortalId = portalId, IsActive = isActive });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                return false;
            }
        }

        //Validate Domains based on 'IsDefault' value.
        public virtual bool ValidateDomainIsDefault(int portalId, string applicationType, int domainId, bool defaultChecked)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            bool validate = false;
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeDomainEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });
            List<DomainModel> domains = _domainClient.GetDomains(filters, null, null, null)?.Domains;
            ZnodeLogging.LogMessage("domains list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, domains?.Count());
            if (!defaultChecked)
            {               
                if ((IsNotNull(domains) && domains.FindAll(m => m.ApplicationType == applicationType).Count == 0) || IsNull(domains))
                    validate = false;
                CheckDomainsIsDefault(domains, applicationType, ref validate, defaultChecked, domainId);
            }
            else
            {

                if ((IsNotNull(domains) && domains.FindAll(m => m.ApplicationType == applicationType).Count == 0) || IsNull(domains))
                    validate = true;
                CheckDomainsIsDefault(domains, applicationType, ref validate, defaultChecked, domainId);
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return validate;
        }

        //Set filters for domain.
        public virtual void SetFilters(FilterCollection filters, string filterKey, string filterOperator, string filterValue)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });
            if (IsNotNull(filters))
            {
                //If filterKey already present in filters remove it.
                filters.RemoveAll(x => x.Item1 == filterKey);

                //Add new filterKey into filters.
                string[] filterValues = filterValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string filter in filterValues)
                {
                    filters.Add(new FilterTuple(filterKey, filterOperator, filter));
                }
            }
            else
                new FilterCollection().Add(new FilterTuple(filterKey, filterOperator, filterValue));
        }

        public bool ValidateDomainUrl(DomainViewModel domainViewModel)
        {
            Regex domainUrlRegex = new Regex(ZnodeConstant.DomainUrlRegex);
            if (!domainUrlRegex.IsMatch(domainViewModel.DomainName))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region private method
        //Replace the domain protocol for the domain name.
        private string ReplaceDomainProtocol(string domainName)
        {
            // Remove any trailing "/"
            if (domainName.EndsWith("/"))
                domainName = domainName.Remove(domainName.Length - 1);
            if (domainName.Contains("www."))
                domainName = domainName.Replace("www.", string.Empty);
            if (domainName.Contains("http://") || domainName.Contains("https://"))
                domainName = domainName.Replace("http://", string.Empty).Replace("https://", string.Empty);

            return domainName;
        }

        //Check Domains based on 'IsDefault' value.
        private void CheckDomainsIsDefault(List<DomainModel> Domains,string applicationType,ref bool validate, bool defaultChecked, int domainId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if ((IsNotNull(Domains) && Domains.Count > 0))
            {
                DomainModel model = Domains.Find(m => m.DomainId == domainId);
                if (IsNotNull(model))
                {
                    model.ApplicationType = applicationType;
                    model.IsDefault = defaultChecked;
                }
                List<string> Applicationtypes = Domains.Select(m => m.ApplicationType).Distinct().ToList();
                ZnodeLogging.LogMessage("Applicationtypes list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, Applicationtypes?.Count());
                foreach (string apptype in Applicationtypes)
                {
                    int IsDefaultCount = Domains.Where(m => m.IsDefault == true && m.ApplicationType == apptype).Select(m => m.ApplicationType).ToList().Count;
                    validate = IsDefaultCount == 0 ? false : true;
                    if (!validate)
                        break;
                }
            }
        }
        #endregion
    }
}
