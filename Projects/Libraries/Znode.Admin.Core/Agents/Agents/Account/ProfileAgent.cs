using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class ProfileAgent : BaseAgent, IProfileAgent
    {

        #region Private Variables
        private readonly IProfileClient _profilesClient;
        private readonly ICatalogClient _catalogClient;
        private readonly IShippingClient _shippingClient;
        private readonly IPaymentClient _paymentClient;
        #endregion

        #region Constructor
        public ProfileAgent(IProfileClient profilesClient, ICatalogClient catalogClient, IPaymentClient paymentClient, IShippingClient shippingClient)
        {
            _profilesClient = GetClient<IProfileClient>(profilesClient);
            _catalogClient = GetClient<ICatalogClient>(catalogClient);
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _shippingClient = GetClient<IShippingClient>(shippingClient);
        }
        #endregion

        #region Public Methods

        //Get list of profiles.
        public virtual ProfileListViewModel GetProfileList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            //Get the sort collection for profilename asc.
            sortCollection = HelperMethods.SortAsc(ZnodeProfileEnum.ProfileName.ToString(), sortCollection);

            ZnodeLogging.LogMessage("Input parameters filters and sortCollection: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });

			if (IsNotNull(filters))
				filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.ParentProfileId, StringComparison.CurrentCultureIgnoreCase));
			else
				filters = new FilterCollection();
			filters.Add(FilterKeys.ParentProfileId, FilterOperators.Equals, "null");

			ProfileListModel profileList = _profilesClient.GetProfileList(filters, sortCollection, pageIndex, recordPerPage);

            ProfileListViewModel listViewModel = new ProfileListViewModel { List = profileList?.Profiles?.ToViewModel<ProfileViewModel>().ToList() };
            listViewModel.List?.ForEach(item => { item.TaxExemptList = GetBooleanList(); });
            SetListPagingData(listViewModel, profileList);

            //Set tool menu for profile list grid view.
            SetProfileListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return profileList?.Profiles?.Count > 0 ? listViewModel : new ProfileListViewModel() { List = new List<ProfileViewModel>() };
        }

        //Create profile.
        public virtual ProfileViewModel CreateProfile(ProfileViewModel profileViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                ProfileModel profileModel = _profilesClient.SaveProfile(profileViewModel.ToModel<ProfileModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(profileModel) ? profileModel.ToViewModel<ProfileViewModel>() : new ProfileViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                return (ProfileViewModel)GetViewModelWithErrorMessage(profileViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (ProfileViewModel)GetViewModelWithErrorMessage(profileViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get profile by profile Id.
        public virtual ProfileViewModel GetProfileById(int profileId)
            => _profilesClient.GetProfile(profileId).ToViewModel<ProfileViewModel>();


        //Update profile
        public virtual ProfileViewModel UpdateProfile(int profileId, string data)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                ProfileViewModel profileViewModel = JsonConvert.DeserializeObject<ProfileViewModel[]>(data)[0];
                ProfileModel profileModel = _profilesClient.UpdateProfile(profileViewModel.ToModel<ProfileModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(profileModel) ? profileModel.ToViewModel<ProfileViewModel>() : (ProfileViewModel)GetViewModelWithErrorMessage(new WarehouseViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (ProfileViewModel)GetViewModelWithErrorMessage(new WarehouseViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete profile by profile Id.
        public virtual bool DeleteProfile(string profileId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            try
            {
                //Delete profile.
                return _profilesClient.DeleteProfile(new ParameterModel { Ids = profileId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorDeleteUserProfile;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Get Profile List.
        public virtual List<SelectListItem> GetProfileList()
        {
            ProfileListViewModel profileTypeList = GetProfileList(null, null, null, null);

            List<SelectListItem> selectedProfileTypeList = new List<SelectListItem>();
            if (profileTypeList?.List?.Count > 0)
                profileTypeList.List.OrderBy(x => x.ProfileName).ToList().ForEach(item => { selectedProfileTypeList.Add(new SelectListItem() { Text = item.ProfileName, Value = item.ProfileId.ToString() }); });
            return selectedProfileTypeList;
        }

        #region Profile Based Catalog
        //Get list of profile Catalog.
        public virtual ProfileCatalogListViewModel GetProfileCatalogList(int profileId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sortCollection: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });
            //Get Catalog Associated to the Profile.
            ProfileCatalogListModel profileCatalogList = GetProfileCatalogs(profileId, 1, filters, sortCollection, pageIndex, recordPerPage);
            ProfileViewModel profileModel = GetProfileById(profileId);
            ProfileCatalogListViewModel profileCataloglistViewModel = new ProfileCatalogListViewModel { List = profileCatalogList?.ProfileCatalogs?.ToViewModel<ProfileCatalogViewModel>().ToList(), ProfileId = profileId, ProfileName = profileModel.ProfileName, ParentProfileId = profileModel.ParentProfileId };
            SetListPagingData(profileCataloglistViewModel, profileCatalogList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return profileCatalogList?.ProfileCatalogs?.Count > 0 ? profileCataloglistViewModel : new ProfileCatalogListViewModel() { List = new List<ProfileCatalogViewModel>(), ProfileId = profileId, ProfileName = profileModel.ProfileName, ParentProfileId = profileModel.ParentProfileId };
        }

        //Get list of profile unassociated catalog.
        public virtual ProfileCatalogListViewModel GetProfileUnAssociatedCatalogList(int profileId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sortCollection: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });
            //Get Catalog Associated to the Profile.
            ProfileCatalogListModel profileCatalogList = GetProfileCatalogs(profileId, 0, filters, sortCollection, pageIndex, recordPerPage);
            ProfileCatalogListViewModel profileCataloglistViewModel = new ProfileCatalogListViewModel { List = profileCatalogList?.ProfileCatalogs?.ToViewModel<ProfileCatalogViewModel>().ToList(), ProfileId = profileId, ProfileName = GetProfileById(profileId).ProfileName };
            SetListPagingData(profileCataloglistViewModel, profileCatalogList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return profileCatalogList?.ProfileCatalogs?.Count > 0 ? profileCataloglistViewModel : new ProfileCatalogListViewModel() { List = new List<ProfileCatalogViewModel>(), ProfileId = profileId, ProfileName = GetProfileById(profileId).ProfileName };
        }

        //Delete associated catalog to profile by profileId.
        public virtual bool DeleteAssociatedProfileCatalog(int profileId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;

            try
            {
                //Delete profile catalog.
                return _profilesClient.DeleteAssociatedProfileCatalog(profileId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorDeleteProfile;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Method to associate catalog to profile. 
        public virtual bool AssociateCatalogToProfile(int profileId, int pimCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            ProfileCatalogViewModel profileCatalogViewModel = new ProfileCatalogViewModel();
            try
            {
                return profileId > 0 && pimCatalogId > 0 ?
                    _profilesClient.AssociateCatalogToProfile(new ProfileCatalogModel { PimCatalogId = pimCatalogId, ProfileId = profileId }) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Profile Shipping
        //Get associated shipping list for profile.
        public virtual ShippingListViewModel GetAssociatedShippingList(int profileId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (profileId > 0)
                SetProfileFilter(filters, profileId);

            ZnodeLogging.LogMessage("Input parameters profileId, filters and sortCollection: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { profileId = profileId, filters = filters, sorts = sorts });
            ShippingListModel shippingListModel = _shippingClient.GetAssociatedShippingList(new ExpandCollection() { ZnodeProfileShippingEnum.ZnodeShipping.ToString() }, filters, sorts, pageIndex, recordPerPage);
            if (shippingListModel?.ShippingList?.Count > 0)
            {
                ShippingListViewModel shippingListViewModel = new ShippingListViewModel { ShippingList = shippingListModel?.ShippingList?.ToViewModel<ShippingViewModel>()?.ToList(), ProfileName = shippingListModel.ProfileName };

                shippingListViewModel.ShippingList?.ForEach(item =>
                {
                    if (IsNotNull(item.DestinationCountryCode))
                        item.DestinationCountryCode = shippingListViewModel?.ShippingList?.Where(x => x.ShippingId == item.ShippingId).Select(x => x.DestinationCountryCode).FirstOrDefault();
                    else
                        item.DestinationCountryCode = Admin_Resources.LabelAll;
                });
                SetListPagingData(shippingListViewModel, shippingListModel);

                //Set tool menu for associated shipping profile list grid view.
                SetAssociatedProfileShippingListToolMenu(shippingListViewModel);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return shippingListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>(), ProfileName = shippingListModel?.ProfileName };
        }

        //Get list of unassociated shipping for profile.
        public virtual ShippingListViewModel GetUnAssociatedShippingList(int profileId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (profileId > 0)
                SetProfileFilter(filters, profileId);
            ZnodeLogging.LogMessage("Input parameters profileId, filters and sortCollection: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { profileId = profileId, filters = filters, sorts = sorts });

            ShippingListModel shippingListModel = _shippingClient.GetUnAssociatedShippingList(null, filters, sorts, pageIndex, recordPerPage);
            if (shippingListModel?.ShippingList?.Count > 0)
            {
                ShippingListViewModel shippingListViewModel = new ShippingListViewModel { ShippingList = shippingListModel?.ShippingList?.ToViewModel<ShippingViewModel>()?.ToList() };

                shippingListViewModel.ShippingList?.ForEach(item =>
                {
                    if (IsNotNull(item.DestinationCountryCode))
                        item.DestinationCountryCode = shippingListViewModel?.ShippingList?.Where(x => x.ShippingId == item.ShippingId).Select(x => x.DestinationCountryCode).FirstOrDefault();
                    else
                        item.DestinationCountryCode = Admin_Resources.LabelAll;
                });


                SetListPagingData(shippingListViewModel, shippingListModel);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return shippingListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
        }

        //Associate shipping to profile.
        public virtual bool AssociateShipping(int profileId, string shippingIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _shippingClient.AssociateShipping(new PortalProfileShippingModel { ShippingIds = shippingIds, ProfileId = profileId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Remove associated shipping from profile.
        public virtual bool UnAssociateAssociatedShipping(string shippingId, int profileId)
              => _shippingClient.UnAssociateAssociatedShipping(new PortalProfileShippingModel() { ShippingIds = shippingId, ProfileId = profileId });

        //Update profile shipping
        public virtual bool UpdateProfileShipping(int shippingId, int profileId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Input parameters shippingId and profileId: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { shippingId = shippingId, profileId = profileId });
                PortalProfileShippingModel model = GetPortalProfileShippingModel(shippingId, profileId, data);
                return shippingId > 0 ? _shippingClient.UpdateProfileShipping(model) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion
        #region Payment
        //Get payment settings list.
        public virtual PaymentSettingListViewModel GetPaymentSettingsList(int profileId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            sorts = SetSort(sorts);
            SetProfileIdFilter(filters, profileId);
            SetIsAssociatedFilter(filters, isAssociated);
            ZnodeLogging.LogMessage("Input parameters profileId, filters and sorts: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { profileId = profileId, filters = filters, sorts = sorts });
            PaymentSettingListModel paymentSettingList = _paymentClient.GetPaymentSettings(null, filters, sorts, pageIndex, pageSize);
            PaymentSettingListViewModel listViewModel = new PaymentSettingListViewModel { PaymentSettings = paymentSettingList?.PaymentSettings?.ToViewModel<PaymentSettingViewModel>().ToList() };
            SetListPagingData(listViewModel, paymentSettingList);
            if (isAssociated)
                SetPaymentToolMenu(listViewModel);
            SetProfileId(listViewModel, profileId);
            ZnodeLogging.LogMessage("PaymentSettings list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, paymentSettingList?.PaymentSettings?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return paymentSettingList?.PaymentSettings?.Count > 0 ? listViewModel : new PaymentSettingListViewModel() { PaymentSettings = new List<PaymentSettingViewModel>() };
        }

        //Associate payment settings to portal.
        public virtual bool AssociatePaymentSettings(int profileId, string paymentSettingIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return !string.IsNullOrEmpty(paymentSettingIds) ? _paymentClient.AssociatePaymentSettings(new PaymentSettingAssociationModel { ProfileId = profileId, PaymentSettingId = paymentSettingIds }) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Remove associated payment settings to portal.
        public virtual bool RemoveAssociatedPaymentSettings(int profileId, string paymentSettingIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return !string.IsNullOrEmpty(paymentSettingIds) ? _paymentClient.RemoveAssociatedPaymentSettings(new PaymentSettingAssociationModel { ProfileId = profileId, PaymentSettingId = paymentSettingIds }) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get portal profile shipping model
        public PortalProfileShippingModel GetPortalProfileShippingModel(int shippingId, int profileId, string data)
        {
            PortalProfileShippingModel portalShippingDetail = JsonConvert.DeserializeObject<PortalProfileShippingModel[]>(data)[0];
            return new PortalProfileShippingModel()
            {
                PortalShippingId = shippingId,
                ProfileId = profileId,
                DisplayOrder = portalShippingDetail.DisplayOrder,
                PublishState = DefaultSettingHelper.GetCurrentOrDefaultAppType(portalShippingDetail.PublishState),
            };
        }

        public bool UpdateProfilePaymentSetting(int paymentSettingId, int profileId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                PaymentSettingAssociationModel model = GetPaymentSettingAssociationModel(paymentSettingId, profileId, data);
                return paymentSettingId > 0 ? _paymentClient.UpdateProfilePaymentSetting(model) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get portal profile shipping model
        public PaymentSettingAssociationModel GetPaymentSettingAssociationModel(int paymentSettingId, int profileId, string data)
        {
            PaymentSettingAssociationModel profilePaymentDetail = JsonConvert.DeserializeObject<PaymentSettingAssociationModel[]>(data)[0];
            return new PaymentSettingAssociationModel()
            {
                ProfilePaymentSettingId = paymentSettingId,
                ProfileId = profileId,
                DisplayOrder = profilePaymentDetail.DisplayOrder,
                PublishState = DefaultSettingHelper.GetCurrentOrDefaultAppType(profilePaymentDetail.PublishState),
            };
        }
        #endregion

        #endregion

        #region Private Method
        //Set tool menu for profile list grid view.
        private void SetProfileListToolMenu(ProfileListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ProfilesDeletePopup')", ControllerName = "Profiles", ActionName = "Delete" });
            }
        }


        //Get Catalog Associated to the Profile.
        private ProfileCatalogListModel GetProfileCatalogs(int profileId, int isAssociated, FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage)
        {
            if (filters.Count() > 0)
            {
                //Remove ProfileId and IsAssociated filter.
                filters.RemoveAll(x => x.Item1.Equals(FilterKeys.ProfileId, StringComparison.InvariantCultureIgnoreCase));
                filters.RemoveAll(x => x.Item1.Equals(FilterKeys.IsAssociated.ToString(), StringComparison.InvariantCultureIgnoreCase));
            }

            filters.Add(ZnodeProfileEnum.ProfileId.ToString(), FilterOperators.Equals, profileId.ToString());
            filters.Add(FilterKeys.IsAssociated.ToString(), FilterOperators.Equals, isAssociated.ToString());
            return _profilesClient.GetProfileCatalogList(filters, sortCollection, pageIndex, recordPerPage);
        }

        //Set tool menu for associated shipping profile list grid view.
        private void SetAssociatedProfileShippingListToolMenu(ShippingListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ProfileShippingDeletePopup',this)", ControllerName = "Profiles", ActionName = "UnAssociateAssociatedShipping" });
            }
        }

        //Set filter for profile id.
        private void SetProfileFilter(FilterCollection filters, int profileId)
        {
            if (IsNotNull(filters))
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.ProfileId, StringComparison.CurrentCultureIgnoreCase));
                filters.Add(FilterKeys.ProfileId, FilterOperators.Equals, profileId.ToString());
            }
        }

        //Set the isAssociated field.
        private void SetIsAssociatedFilter(FilterCollection filters, bool isAssociated)
        {
            if (IsNotNull(filters))
            {
                filters.RemoveAll(x => x.Item1 == FilterKeys.IsAssociated.ToString());
                filters.Add(new FilterTuple(FilterKeys.IsAssociated.ToString(), FilterOperators.Equals, isAssociated.ToString()));
            }
        }

        //Set the portal id field.
        private void SetProfileIdFilter(FilterCollection filters, int profileId)
        {
            if (IsNotNull(filters))
            {
                //If ProfileId is already present in filters, remove it.
                filters.RemoveAll(x => string.Equals(x.Item1, ZnodeProfileEnum.ProfileId.ToString(), StringComparison.InvariantCultureIgnoreCase));

                //Add New ProfileId into filters.
                filters.Add(new FilterTuple(ZnodeProfileEnum.ProfileId.ToString(), FilterOperators.Equals, profileId.ToString()));
            }
        }

        private static SortCollection SetSort(SortCollection sorts)
        {
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodePaymentSettingEnum.DisplayOrder.ToString(), DynamicGridConstants.ASCKey);
            }

            return sorts;
        }

        //Set portal id in model.
        private void SetProfileId(PaymentSettingListViewModel listViewModel, int profileId)
        {
            if (listViewModel?.PaymentSettings?.Count > 0)
                listViewModel.PaymentSettings.ForEach(x => x.ProfileId = profileId);
        }

        //Set payment tools.
        private void SetPaymentToolMenu(PaymentSettingListViewModel listViewModel)
        {
            if (IsNotNull(listViewModel))
            {
                listViewModel.GridModel = GetGridModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreTaxDeletePopup',this)", ControllerName = "Profiles", ActionName = "RemoveAssociatedPaymentSetting" });
            }
        }

        //Get the grid model for binding the tools.
        private GridModel GetGridModel()
        {
            GridModel gridModel = new GridModel();
            gridModel.FilterColumn = new FilterColumnListModel();
            gridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
            return gridModel;
        }
        #endregion
    }
}