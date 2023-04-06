using MvcSiteMapProvider;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class ProfilesController : BaseController
    {
        #region Private Variables
        private readonly IProfileAgent _profileAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly string AssociatedProfileShipping = "_AssociatedProfileShippingList";
        private readonly string UnAssociatedProfileShipping = "_UnAssociatedProfileShippingList";
        #endregion

        #region Public Constructor
        public ProfilesController(IProfileAgent profileAgent, IStoreAgent storeAgent)
        {
            _profileAgent = profileAgent;
            _storeAgent = storeAgent;
        }
        #endregion

        #region Public Methods
        //Action for show create profile view.
        public virtual ActionResult Create() => View(Request.IsAjaxRequest() ? "_AddProfileAsidePanel" : AdminConstants.CreateEditProfileView, new ProfileViewModel());

        //action for create profile.
        [HttpPost]
        public virtual ActionResult Create(ProfileViewModel profileViewModel)
        {
            if (ModelState.IsValid)
            {
                profileViewModel = _profileAgent.CreateProfile(profileViewModel);
                if (!profileViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<ProfilesController>(x => x.List(null));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(profileViewModel.ErrorMessage));
            return RedirectToAction<ProfilesController>(x => x.Create());
        }


        //Action for edit profile.
        public virtual JsonResult Edit(int profileId, string data)
        {
            if (ModelState.IsValid && IsNotNull(data))
            {
                ProfileViewModel profileViewModel = _profileAgent.UpdateProfile(profileId, data);
                if (!profileViewModel.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Action for profilelist.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleZnodeProfile", Key = "Profile", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeProfile.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeProfile.ToString(), model);
            //Get the list of profiles            
            ProfileListViewModel profileListViewModel = _profileAgent.GetProfileList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            profileListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, profileListViewModel.List, GridListType.ZnodeProfile.ToString(), "", null, true, true, profileListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            profileListViewModel.GridModel.TotalRecordCount = profileListViewModel.TotalResults;

            //Returns the profile list view
            return ActionView(profileListViewModel);
        }

        // Action for delete profile by profileId.
        public virtual ActionResult Delete(string profileId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(profileId))
            {
                status = _profileAgent.DeleteProfile(profileId, out message);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

    //Action for profileCataloglist.
    public virtual ActionResult GetProfileCatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int profileId, int portalId = 0)
    {
      //Get the list of profile catalog.            
      ProfileCatalogListViewModel profileCatalogListViewModel = _profileAgent.GetProfileCatalogList(profileId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.     
      profileCatalogListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, profileCatalogListViewModel.List, IsNull(profileCatalogListViewModel.ParentProfileId) ? GridListType.ZnodeProfileAssociatedCatalogList.ToString() : GridListType.ZnodeProfileDefaultCatalogList.ToString(), "", null, true, true, IsNull(profileCatalogListViewModel.ParentProfileId) ? profileCatalogListViewModel?.GridModel?.FilterColumn?.ToolMenuList : null);
      //Set the total record count.
      profileCatalogListViewModel.GridModel.TotalRecordCount = profileCatalogListViewModel.TotalResults;      
      profileCatalogListViewModel.PortalId = portalId;
      //Returns the profile catalog list view.
      return ActionView("ProfileCatalogList", profileCatalogListViewModel);
    }

        //Action for profileUnAssociatedCataloglist.
        public virtual ActionResult GetProfileUnAssociatedCatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int profileId)
        {
            //Get the list of profile catalog.            
            ProfileCatalogListViewModel profileCatalogListViewModel = _profileAgent.GetProfileUnAssociatedCatalogList(profileId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            profileCatalogListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, profileCatalogListViewModel.List, GridListType.UnAssociatedProfileCatalogList.ToString(), "", null, true, true, profileCatalogListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count.
            profileCatalogListViewModel.GridModel.TotalRecordCount = profileCatalogListViewModel.TotalResults;

            //Returns the catalog list view.           
            return PartialView("_unassociatedProfileCatalogList", profileCatalogListViewModel);
        }

        // Action for delete catalog associated to profile by profileCatalogId.
        public virtual ActionResult DeleteAssociatedProfileCatalog(int profileId)
        {
            string message = string.Empty;
            bool status = false;
            if (profileId > 0)
            {
                status = _profileAgent.DeleteAssociatedProfileCatalog(profileId, out message);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Action to associate catalog to profile.
        public virtual JsonResult AssociateCatalogToProfile(int profileId, int pimCatalogId)
        {
            string errorMessage = Admin_Resources.ErrorAssociateCatalogToProfile;

            if (pimCatalogId > 0 && profileId > 0)
            {
                bool status = _profileAgent.AssociateCatalogToProfile(profileId, pimCatalogId);
                errorMessage = status ? Admin_Resources.ProfileCatalogSuccessMessage : Admin_Resources.ErrorAssociateCatalogToProfile;
                return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #region Profile Shipping
        //Get Associated shipping list for profile.
        public virtual ActionResult GetAssociatedShippingList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int profileId, int portalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAssociatedShippingListToProfile.ToString(), model);
            //Get profile shipping list.
            ShippingListViewModel shippingListViewModel = _profileAgent.GetAssociatedShippingList(profileId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            shippingListViewModel.ProfileId = profileId;

            var publishStateList = _storeAgent.GetApplicationType();
            shippingListViewModel?.ShippingList?.ForEach(item => { item.PublishStateList = publishStateList; });

            //Get the grid model.
            shippingListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingListViewModel?.ShippingList, GridListType.ZnodeAssociatedShippingListToProfile.ToString(), string.Empty, null, true, true, shippingListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            if (DefaultSettingHelper.IsDataSeparationAllowedforAppType() == false)
            {
                var removeGridColumn = shippingListViewModel.GridModel.WebGridColumn.FirstOrDefault(x => x.ColumnName == "PublishState");
                shippingListViewModel.GridModel.WebGridColumn.Remove(removeGridColumn);
                var removeFilterColumn = shippingListViewModel.GridModel.FilterColumn.FilterColumnList.FirstOrDefault(x => x.ColumnName == "PublishState");
                shippingListViewModel.GridModel.FilterColumn.FilterColumnList.Remove(removeFilterColumn);
            }

            shippingListViewModel.GridModel.TotalRecordCount = shippingListViewModel.TotalResults;
            shippingListViewModel.PortalId = portalId;
            return ActionView(AssociatedProfileShipping, shippingListViewModel);
        }

        //Get UnAssociated shipping list for profile.
        public virtual ActionResult GetUnAssociatedShippingList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int profileId, int portalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUnAssociatedShippingList.ToString(), model);
            //Get shipping list.
            ShippingListViewModel shippingListViewModel = _profileAgent.GetUnAssociatedShippingList(profileId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            shippingListViewModel.ProfileId = profileId;

            //Get the grid model.
            shippingListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingListViewModel?.ShippingList, GridListType.ZnodeUnAssociatedShippingList.ToString(), string.Empty, null, true);
            shippingListViewModel.GridModel.TotalRecordCount = shippingListViewModel.TotalResults;
            shippingListViewModel.PortalId = portalId;
            return ActionView(UnAssociatedProfileShipping, shippingListViewModel);
        }

        //Associate UnAssociated shipping list to profile.
        public virtual JsonResult AssociateShipping(int profileId, string shippingIds)
        {
            if (!string.IsNullOrEmpty(shippingIds) && profileId > 0)
            {
                bool status = _profileAgent.AssociateShipping(profileId, shippingIds);
                return Json(new { status = status, message = status ? Admin_Resources.ProfileShippingSuccessMessage : Admin_Resources.ErrorAssociationShippingToProfile }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorAssociationShippingToProfile }, JsonRequestBehavior.AllowGet);
        }

        //UnAssociate shipping from profile.
        public virtual JsonResult UnAssociateAssociatedShipping(string shippingId, int profileId)
        {
            if (!string.IsNullOrEmpty(shippingId) && profileId > 0)
            {
                bool status = _profileAgent.UnAssociateAssociatedShipping(shippingId, profileId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Update profile shipping
        public virtual ActionResult UpdateProfileShipping(int shippingId = 0, int profileId = 0, string data = null)
        {
            if (ModelState.IsValid)
            {
                bool success = _profileAgent.UpdateProfileShipping(shippingId, profileId, data);
                return Json(new { status = success, message = success ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Payment
        //Get tax class list.
        public virtual ActionResult GetAssociatedPaymentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int profileId, int portalId = 0)
        {
            PaymentSettingListViewModel paymentSettingList = _profileAgent.GetPaymentSettingsList(profileId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);
            paymentSettingList.ProfileId = profileId;
            paymentSettingList.ProfileName = _profileAgent.GetProfileById(profileId)?.ProfileName;

            var publishStateList = _storeAgent.GetApplicationType();
            paymentSettingList?.PaymentSettings?.ForEach(item => { item.PublishStateList = publishStateList; });
            //Get the grid model.

            paymentSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, paymentSettingList.PaymentSettings, GridListType.AssociatedPaymentListToProfile.ToString(), string.Empty, null, true, true, paymentSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            if (DefaultSettingHelper.IsDataSeparationAllowedforAppType() == false)
            {
                var removeGridColumn = paymentSettingList.GridModel.WebGridColumn.FirstOrDefault(x => x.ColumnName == "PublishState");
                paymentSettingList.GridModel.WebGridColumn.Remove(removeGridColumn);
                var removeFilterColumn = paymentSettingList.GridModel.FilterColumn.FilterColumnList.FirstOrDefault(x => x.ColumnName == "PublishState");
                paymentSettingList.GridModel.FilterColumn.FilterColumnList.Remove(removeFilterColumn);
            }

            paymentSettingList.GridModel.TotalRecordCount = paymentSettingList.TotalResults;
            paymentSettingList.PortalId = portalId;
            return ActionView("AssociatedPaymentList", paymentSettingList);
        }

        //Get tax class list.
        public virtual ActionResult GetUnassociatedPaymentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int profileId, int portalId = 0)
        {
            PaymentSettingListViewModel paymentSettingList = _profileAgent.GetPaymentSettingsList(profileId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);
            paymentSettingList.ProfileId = profileId;
            //Get the grid model.
            paymentSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, paymentSettingList.PaymentSettings, GridListType.UnassociatedPaymentListToPortal.ToString(), string.Empty, null, true, true, paymentSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            paymentSettingList.GridModel.TotalRecordCount = paymentSettingList.TotalResults;
            paymentSettingList.PortalId = portalId;
            return ActionView("UnassociatedPaymentList", paymentSettingList);
        }

        // Action for associate taxclass.
        public virtual JsonResult AssociatePaymentSetting(string paymentSettingId, int profileId = 0)
        {
            if (IsNotNull(paymentSettingId) && profileId > 0)
            {
                bool status = _profileAgent.AssociatePaymentSettings(profileId, paymentSettingId);
                return Json(new { status = status, message = status ? Admin_Resources.AssociatedSuccessMessage : Admin_Resources.AssociatedErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.AssociatedErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        // Action for associate taxclass.
        public virtual JsonResult RemoveAssociatedPaymentSetting(string paymentSettingId, int profileId = 0)
        {
            if (IsNotNull(paymentSettingId) && profileId > 0)
            {
                bool status = _profileAgent.RemoveAssociatedPaymentSettings(profileId, paymentSettingId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
        }

        //Update profile payment settings.
        public virtual ActionResult UpdateProfilePaymentSetting(int paymentSettingId = 0, int profileId = 0, string data = null)
        {
            if (ModelState.IsValid)
            {
                bool success = _profileAgent.UpdateProfilePaymentSetting(paymentSettingId, profileId, data);
                return Json(new { status = success, message = success ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
    }
}