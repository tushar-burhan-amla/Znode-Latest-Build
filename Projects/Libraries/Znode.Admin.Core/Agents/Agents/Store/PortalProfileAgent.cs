using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Models;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Newtonsoft.Json;
using System;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class PortalProfileAgent : BaseAgent, IPortalProfileAgent
    {
        #region Private Variables
        private readonly IPortalProfileClient _portalProfileClient;
        private readonly IProfileClient _profileClient;
        private readonly IProfileAgent _profileAgent;
        #endregion

        #region Constructor
        public PortalProfileAgent(IPortalProfileClient portalProfileClient, IProfileClient profileClient)
        {
            _portalProfileClient = GetClient<IPortalProfileClient>(portalProfileClient);
            _profileClient = GetClient<IProfileClient>(profileClient);
            _profileAgent = new ProfileAgent(GetClient<ProfileClient>(), GetClient<CatalogClient>(), GetClient<PaymentClient>(), GetClient<ShippingClient>());
        }
        #endregion

        #region public Methods
        public virtual PortalProfileListViewModel GetPortalProfiles(int portalId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter portalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalId });

            //checks if the filter collection null
            if (IsNull(filters))
                filters = new FilterCollection();

            if (portalId > 0)
            {
                filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
            }

            //Get the sort collection for profilename asc.
            sortCollection = HelperMethods.SortAsc(ZnodeProfileEnum.ProfileName.ToString(), sortCollection);

            ZnodeLogging.LogMessage("Input parameters filters and sortCollection of GetPortalProfiles method:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { filters, sortCollection });

            PortalProfileListModel listModel = _portalProfileClient.GetPortalProfiles(null, filters, sortCollection, pageIndex, recordPerPage);
            PortalProfileListViewModel listViewModel = new PortalProfileListViewModel { PortalProfiles = listModel?.PortalProfiles?.ToViewModel<PortalProfileViewModel>()?.ToList(), TotalResults = listModel?.TotalResults.GetValueOrDefault() ?? 0 };
            listViewModel.PortalProfiles?.ForEach(item => { item.IsDefaultAnonymousProfileList = GetBooleanList(); item.IsDefaultRegistedProfileList = GetBooleanList(); });

            SetPortalProfileData(portalId, listViewModel);

            SetListPagingData(listViewModel, listModel);

            //Set tool menu for Portal Profile list grid view.
            SetPortalProfileListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return IsNotNull(listViewModel?.PortalProfiles) ? listViewModel : new PortalProfileListViewModel() { PortalProfiles = new List<PortalProfileViewModel>(), PortalId = portalId };
        }

        public virtual PortalProfileViewModel GetPortalProfile(int portalProfileId, ExpandCollection expands)
            => portalProfileId > 0 ? _portalProfileClient.GetPortalProfile(portalProfileId, expands)?.ToViewModel<PortalProfileViewModel>() : new PortalProfileViewModel();

        public virtual PortalProfileViewModel CreatePortalProfile(PortalProfileViewModel portalProfileViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                PortalProfileModel portalProfileModel = _portalProfileClient.CreatePortalProfile(portalProfileViewModel.ToModel<PortalProfileModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return IsNotNull(portalProfileModel) ? portalProfileModel.ToViewModel<PortalProfileViewModel>() : new PortalProfileViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new PortalProfileViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (PortalProfileViewModel)GetViewModelWithErrorMessage(portalProfileViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        public virtual PortalProfileViewModel UpdatePortalProfile(int portalId, int portalProfileId, string data)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameters portalId and portalProfileId: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalId, portalProfileId });

                PortalProfileViewModel portalProfileViewModel = GetPortalProfileViewModel(portalId, portalProfileId, data);
                PortalProfileModel portalProfileModel = _portalProfileClient.UpdatePortalProfile(portalProfileViewModel.ToModel<PortalProfileModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return IsNotNull(portalProfileModel) ? portalProfileModel.ToViewModel<PortalProfileViewModel>() : new PortalProfileViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new PortalProfileViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return new PortalProfileViewModel { HasError = true, ErrorMessage = ex.Message };
            }
        }

        public virtual bool DeletePortalProfile(string portalProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return _portalProfileClient.DeletePortalProfile(new ParameterModel() { Ids = portalProfileId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual List<SelectListItem> GetProfileList(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId, expands, filters and sorts:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalId, expands, filters, sorts });

            List<SelectListItem> profileList = PortalProfileViewModelMap.ToListItems(_profileClient.GetProfileList(filters, sorts, pageIndex, pageSize).Profiles);
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            //checks if the filter collection null
            if (IsNull(filters))
                filters = new FilterCollection();

            if (portalId > 0)
                filters.Add(new FilterTuple(ZnodePortalProfileEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));

            if (portalProfileId > 0)
            {
                filters.Add(new FilterTuple(ZnodePortalProfileEnum.PortalProfileID.ToString(), FilterOperators.Equals, portalProfileId.ToString()));
                PortalProfileListViewModel portalProfile = GetPortalProfiles(filters);
                if (portalProfile.PortalProfiles.Any(p => p.ProfileId > 0))
                {
                    foreach (SelectListItem profiles in profileList)
                    {
                        if (portalProfile.PortalProfiles.Any(p => p.ProfileId.ToString() == profiles.Value))
                            selectListItems.Add(new SelectListItem { Value = profiles.Value.ToString(), Text = profiles.Text });
                    }
                    ZnodeLogging.LogMessage("selectListItems count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, selectListItems?.Count);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                    return selectListItems;
                }
            }

            PortalProfileListViewModel portalProfileList = GetPortalProfiles(filters);
            foreach (SelectListItem profiles in profileList)
            {
                if (!portalProfileList.PortalProfiles.Any(p => p.ProfileId.ToString() == profiles.Value))
                    selectListItems.Add(new SelectListItem { Value = profiles.Value.ToString(), Text = profiles.Text });
            }
            ZnodeLogging.LogMessage("selectListItems count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, selectListItems?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return selectListItems;
        }


        #endregion

        #region Private Methods
        //Set tool menu for portal Profile list grid view.
        private void SetPortalProfileListToolMenu(PortalProfileListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PortalProfileDeletePopup')", ControllerName = "Store", ActionName = "DeletePortalProfile" });
            }
        }

        //Get Portal Profiles.
        private PortalProfileListViewModel GetPortalProfiles(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Input parameter filters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] {filters});
            List<PortalProfileViewModel> portalProfiles = _portalProfileClient.GetPortalProfiles(null, filters, null, null, null)?.PortalProfiles?.ToViewModel<PortalProfileViewModel>()?.ToList();
            return new PortalProfileListViewModel { PortalProfiles = IsNull(portalProfiles) ? new List<PortalProfileViewModel>() : portalProfiles };
        }

        //Get PortalProfileViewModel from json data.
        private PortalProfileViewModel GetPortalProfileViewModel(int portalId, int portalProfileId, string data)
        {
            return new PortalProfileViewModel()
            {
                PortalId = portalId,
                ProfileId = JsonConvert.DeserializeObject<PortalProfileViewModel[]>(data)[0].ProfileId,
                IsDefaultRegistedProfile = JsonConvert.DeserializeObject<PortalProfileViewModel[]>(data)[0].IsDefaultRegistedProfile,
                IsDefaultAnonymousProfile = JsonConvert.DeserializeObject<PortalProfileViewModel[]>(data)[0].IsDefaultAnonymousProfile,
                PortalProfileID = portalProfileId,
                ProfileNumber= JsonConvert.DeserializeObject<PortalProfileViewModel[]>(data)[0].ProfileNumber,
                ProfileName = JsonConvert.DeserializeObject<PortalProfileViewModel[]>(data)[0].ProfileName,
                ParentProfileId = JsonConvert.DeserializeObject<PortalProfileViewModel[]>(data)[0].ParentProfileId,
            };
        }

        //Set portal profile data
        private void SetPortalProfileData(int portalId, PortalProfileListViewModel listViewModel)
        {
            listViewModel.PortalId = portalId;
            listViewModel.ProfileCount = listViewModel.TotalResults > 0 ? listViewModel.TotalResults : Convert.ToInt32(listViewModel?.PortalProfiles?.Count);
            listViewModel.Profiles = _profileAgent.GetProfileList();
            listViewModel.ActiveProfileCount = Convert.ToInt32(listViewModel?.Profiles?.Count);
        }
        #endregion
    }
}