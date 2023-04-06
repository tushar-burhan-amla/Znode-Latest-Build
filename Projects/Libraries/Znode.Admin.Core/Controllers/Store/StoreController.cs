using MvcSiteMapProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Admin.Areas.Portal
{
    public class StoreController : BaseController
    {
        #region Private Variable
        private readonly IDomainAgent _domainAgent;
        private readonly ISMTPAgent _smtpAgent;
        private readonly IPortalProfileAgent _portalProfileAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly IHelperAgent _helperAgent;
        private readonly IStoreUnitAgent _storeUnitAgent;
        private readonly IPriceAgent _priceAgent;
        private readonly ICatalogAgent _catalogAgent;
        private readonly IAccountAgent _accountAgent;
        private readonly IPortalCountryAgent _portalCountryAgent;
        private readonly IEcommerceCatalogAgent _ecommerceCatalogAgent;
        private readonly IWebSiteAgent _websiteAgent;
        private readonly IBrandAgent _brandAgent;
        private readonly ISMSAgent _smsAgent;
        private readonly IGeneralSettingAgent _generalSettingAgent;
        private const string _DisplayPublishInfo = "_DisplayPublishInfo";
        private const string _EditShippingView = "_EditShipping";
        private const string _ManagePartialView = "_ManagePartial";
        private const string _CreatePortalProfilePopupPanel = "~/Views/Store/_CreatePortalProfilePopupPanel.cshtml";
        private readonly string AssociatedPortalShipping = "_AssociatedPortalShippingList";
        private readonly string UnAssociatedPortalShipping = "_UnAssociatedPortalShippingList";
        private readonly string PublishCatalogListPanel = "_asideCatalogListPanel";
        private const string _ManageTagManager = "_ManageTagManager";
        private const string _EditTaxView = "_EditTax";
        private const string CreateEditAnalytics = "CreateEditAnalytics";
        private const string PortalTrackingPixelView = "_PortalTrackingPixel";
        private const string CreateEditRobotsTxt = "CreateEditRobotsTxt";
        private readonly IKlaviyoAgent _klaviyoAgent;
        #endregion

        #region Constructor
        public StoreController(IDomainAgent domainAgent, IHelperAgent helperAgent, ISMTPAgent smtpAgent, IPortalProfileAgent portalProfileAgent, IStoreAgent storeAgent, IStoreUnitAgent storeUnitAgent,
            IPriceAgent priceAgent, ICatalogAgent catalogAgent, IEcommerceCatalogAgent ecommerceCatalogAgent, IPortalCountryAgent portalCountryAgent, IWebSiteAgent websiteAgent, IAccountAgent accountAgent, IBrandAgent brandAgent, IGeneralSettingAgent generalSettingAgent, ISMSAgent smsAgent, IKlaviyoAgent klaviyoAgent)
        {
            _helperAgent = helperAgent;
            _domainAgent = domainAgent;
            _smtpAgent = smtpAgent;
            _portalProfileAgent = portalProfileAgent;
            _storeAgent = storeAgent;
            _storeUnitAgent = storeUnitAgent;
            _priceAgent = priceAgent;
            _catalogAgent = catalogAgent;
            _ecommerceCatalogAgent = ecommerceCatalogAgent;
            _portalCountryAgent = portalCountryAgent;
            _websiteAgent = websiteAgent;
            _accountAgent = accountAgent;
            _brandAgent = brandAgent;
            _generalSettingAgent = generalSettingAgent;
            _smsAgent = smsAgent;
            _klaviyoAgent = klaviyoAgent;
        }
        #endregion

        #region Public Methods
        #region Manage Portal

        //Get the index view for store
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleStoreAndReps", Key = "Setup", Area = "", ParentKey = "Home")]
        public virtual ActionResult Index() => View();


        //Get the list of all stores.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleStore", Key = "Store", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeStore.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeStore.ToString(), model);

            //Get the list of all stores.
            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeStore.ToString(), string.Empty, null, true, true, storeList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView(storeList);
        }

        //Get type method to create new store.
        [HttpGet]
        public virtual ActionResult CreateStore()
            => ActionView(AdminConstants.CreateEdit, _storeAgent.GetStoreInformation(new StoreViewModel()));

        //Post type method to create new store.
        [HttpPost]
        public virtual ActionResult CreateStore(StoreViewModel storeViewModel)
        {
            storeViewModel = _storeAgent.CreateStore(storeViewModel);
            if (IsNotNull(storeViewModel))
            {
                if (!storeViewModel.HasError)
                {
                    if (storeViewModel.IsSearchIndexCreated)
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    else
                        SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorCreateStoreSearchIndex));
                    return RedirectToAction<StoreController>(x => x.UpdateStore(storeViewModel.StoreCode));
                }
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(storeViewModel.ErrorMessage));
                    return RedirectToAction<StoreController>(x => x.CreateStore());
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
            return RedirectToAction<StoreController>(x => x.CreateStore());
        }

        //Get type method to update store.
        [HttpGet]
        public virtual ActionResult EditStore(int portalId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            StoreViewModel storeViewModel = _storeAgent.GetStore(portalId, new ExpandCollection { ZnodePortalEnum.ZnodeOmsOrderState.ToString().ToLower() });
            return ActionView(AdminConstants.CreateEdit, HelperUtility.IsNotNull(storeViewModel) ? storeViewModel : new StoreViewModel());
        }
        //Update Store by store code
        [HttpGet]
        public virtual ActionResult UpdateStore(string storeCode)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            StoreViewModel storeViewModel = _storeAgent.GetStore(storeCode, new ExpandCollection { ZnodePortalEnum.ZnodeOmsOrderState.ToString().ToLower() });
            return ActionView(AdminConstants.CreateEdit, IsNotNull(storeViewModel) ? storeViewModel : new StoreViewModel());
        }

        //Post type method to update store.
        [HttpPost]
        public virtual ActionResult UpdateStore(StoreViewModel storeViewModel)
        {
            string errorMessage = Admin_Resources.UpdateErrorMessage;
            if (IsNotNull(storeViewModel) && _storeAgent.UpdateStore(storeViewModel, out errorMessage))
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<StoreController>(x => x.UpdateStore((storeViewModel.StoreCode)));
            }
            SetNotificationMessage(GetErrorNotificationMessage(errorMessage));
            return RedirectToAction<StoreController>(x => x.EditStore((storeViewModel.PortalId)));
        }

        //Post type method to update store.
        [HttpPost]
        public virtual ActionResult EditStore(StoreViewModel storeViewModel)
        {
            string errorMessage = Admin_Resources.UpdateErrorMessage;
            if (HelperUtility.IsNotNull(storeViewModel))
            {
                if (_storeAgent.UpdateStore(storeViewModel, out errorMessage))
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<StoreController>(x => x.EditStore((storeViewModel.PortalId)));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(errorMessage));
            return RedirectToAction<StoreController>(x => x.EditStore((storeViewModel.PortalId)));
        }

        //Delete a store.
        public virtual ActionResult DeleteStore(string portalId)
        {
            if (!string.IsNullOrEmpty(portalId))
            {
                string message = string.Empty;
                bool status = _storeAgent.DeleteStore(portalId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete a store by storeCode.
        public virtual JsonResult DeleteStoreByStoreCode(string storeCode)
        {
            if (!string.IsNullOrEmpty(storeCode))
            {
                string message = string.Empty;
                bool status = _storeAgent.DeleteStore(storeCode, out message, true);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get store data to copy store.
        [HttpGet]
        public virtual ActionResult CopyStore(int portalId)
        {
            StoreViewModel store = _storeAgent.GetStore(portalId);
            if (IsNull(store))
                return RedirectToAction<PriceController>(x => x.List(null));

            store.CompanyName = $"Copy Of {store.CompanyName}";
            store.StoreName = $"Copy Of {store.StoreName}";
            store.StoreCode = $"CopyOf{store.StoreCode}";
            return ActionView(store);
        }
        //Method to Copy store.
        [HttpPost]
        public virtual ActionResult CopyStore(StoreViewModel storeViewModel)
        {
            if (_storeAgent.CopyStore(storeViewModel))
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.CopyMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCopy));

            return Json(new
            {
                status = storeViewModel.PortalId >= 1,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets css list based on selected theme in store.
        /// </summary>
        /// <param name="cmsThemeId">cmsThemeId to get css list assigned to theme.</param>
        /// <returns>Json value containing css list.</returns>
        [HttpGet]
        public virtual JsonResult GetCSSList(int cmsThemeId)
            => Json(_storeAgent.GetCSSList(cmsThemeId), JsonRequestBehavior.AllowGet);

        //Check store code already exist or not.
        [HttpGet]
        public virtual ActionResult IsStoreCodeExist(string storeCode)
            => Json(new { data = _storeAgent.IsStoreCodeExist(storeCode) }, JsonRequestBehavior.AllowGet);
        #endregion

        #region Manage URLs
        //Gets URL redirect list.
        public virtual ActionResult UrlList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeDomain.ToString(), model);
            //GetFilterTopass()
            DomainListViewModel domainList = _storeAgent.GetDomains(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            domainList.PortalId = portalId;
            domainList.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            var applicationTypes = _domainAgent.GetApplicationTypes();
            domainList.Domains?.ForEach(item => { item.ApplicationTypeList = applicationTypes; });
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, domainList.Domains, GridListType.ZnodeDomain.ToString(), string.Empty, null, true, true, domainList?.GridModel?.FilterColumn?.ToolMenuList);

            //set the total record count
            gridModel.TotalRecordCount = domainList.TotalResults;
            domainList.GridModel = gridModel;

            //returns the view
            return ActionView(domainList);
        }

        //Create Url
        [HttpGet]
        public virtual ActionResult CreateUrl(int portalId)
            => ActionView(AdminConstants.CreateEditUrlView, new DomainViewModel { PortalId = portalId, ApplicationTypeList = _domainAgent.GetApplicationTypes(), PortalName = _storeAgent.GetStore(portalId)?.StoreName });

        //Create Url.
        [HttpPost]
        public virtual ActionResult CreateUrl(DomainViewModel domainViewModel)
        {
            if (ModelState.IsValid)
            {
                domainViewModel = _domainAgent.CreateDomainUrl(domainViewModel);

                if (!domainViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<StoreController>(x => x.UrlList(null, domainViewModel.PortalId));
                }
            }
            domainViewModel.ApplicationTypeList = _domainAgent.GetApplicationTypes();
            SetNotificationMessage(GetErrorNotificationMessage(domainViewModel.ErrorMessage));
            return View(AdminConstants.CreateEditUrlView, domainViewModel);
        }

        //Edit url.
        public virtual ActionResult EditUrl(int portalId, int domainId, bool status, string data)
        {
            DomainViewModel domainViewModel = JsonConvert.DeserializeObject<DomainViewModel[]>(data)[0];
            bool exists = _domainAgent.CheckDomainNameExist(domainViewModel.DomainName, domainId);

            if (exists)
                return Json(new { status = false, message = PIM_Resources.DomainNameAlreadyExists }, JsonRequestBehavior.AllowGet);

            exists = _domainAgent.ValidateDomainIsDefault(portalId, domainViewModel.ApplicationType, domainViewModel.DomainId, domainViewModel.IsDefault);
            if (!exists)
                return Json(new { status = false, message = Admin_Resources.DomainsMarkedAsDefault }, JsonRequestBehavior.AllowGet);
            DomainViewModel Domain = _domainAgent.GetDomain(domainId);

            if (_domainAgent.ValidateDomainUrl(domainViewModel))
            {
                return Json(new { message = Admin_Resources.ValidationUrl }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                domainViewModel = _domainAgent.UpdateDomainUrl(domainViewModel);
                domainViewModel.ApplicationTypeList = _domainAgent.GetApplicationTypes();
                if (!domainViewModel.HasError)
                {
                    if (domainViewModel.IsDefault == true && Domain?.IsDefault == false)
                        return Json(new { status = true, message = Admin_Resources.UpdateMessageForDefaultUrl }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete Url.
        [HttpGet]
        public virtual JsonResult DeleteUrl(string domainId)
        {
            if (!string.IsNullOrEmpty(domainId))
            {
                string message = string.Empty;

                bool status = _domainAgent.DeleteDomainUrl(domainId, out message);
                if(status==true && message==Admin_Resources.DeleteNonDefaultDomain)
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Is domain name exist.
        [HttpPost]
        public virtual JsonResult IsDomainNameExist(string DomainName, int DomainId)
           => Json(!_domainAgent.CheckDomainNameExist(DomainName, DomainId), JsonRequestBehavior.AllowGet);

        /// <summary>
        /// This method will enable or disable the domain.
        /// </summary>
        /// <param name="domainIds">User Ids whose domain have to be enabled or disabled.</param>
        /// <param name="IsActive">To check if domain is locked or not.</param>
        /// <param name="PortalId">portalId</param>
        /// <returns>Returns if the customer account is enabled or disabled.</returns>
        public virtual ActionResult EnableDisableDomain(int PortalId, string DomainId, bool IsActive)
        {
            if (!string.IsNullOrEmpty(DomainId))
            {
                string message = string.Empty;
                bool status = _domainAgent.EnableDisableDomain(DomainId, PortalId, IsActive, out message);
                if (status && IsActive)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.EnableMessage));
                else if (status && !IsActive)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DisableMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return RedirectToAction<StoreController>(x => x.UrlList(null, PortalId));
        }


        #endregion

        #region Portal Profile
        //Get portal profile  list.
        public virtual ActionResult PortalProfileList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePortalProfile.ToString(), model);

            PortalProfileListViewModel portalProfileList = _portalProfileAgent.GetPortalProfiles(portalId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            portalProfileList.GridModel = FilterHelpers.GetDynamicGridModel(model, portalProfileList?.PortalProfiles, GridListType.ZnodePortalProfile.ToString(), null, null, true, true, portalProfileList?.GridModel?.FilterColumn?.ToolMenuList);
            //set the total record count
            portalProfileList.GridModel.TotalRecordCount = portalProfileList.TotalResults;

            portalProfileList.PortalId = portalId;
            portalProfileList.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            //returns the view
            return ActionView(portalProfileList);
        }

        //Create portal profile.
        public virtual ActionResult CreatePortalProfile(int portalId)
        {
            PortalProfileViewModel portalProfileViewModel = new PortalProfileViewModel();
            portalProfileViewModel.PortalId = portalId;
            portalProfileViewModel.Profiles = _portalProfileAgent.GetProfileList(portalId, null, null, null, null, null, 0);
            if (portalProfileViewModel?.Profiles?.Count < 1)
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorNoProfilesToAddToStore));
                return RedirectToAction<StoreController>(x => x.PortalProfileList(null, portalId));
            }
            portalProfileViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            PortalProfileListViewModel portalProfileListViewModel = new PortalProfileListViewModel();
            portalProfileListViewModel = _portalProfileAgent.GetPortalProfiles(portalId);
            if (portalProfileListViewModel?.PortalProfiles?.Count > 0)
            {
                portalProfileViewModel.IsDefaultAnonymousProfile = false;
                portalProfileViewModel.IsDefaultRegistedProfile = false;
                portalProfileViewModel.IsDefaultAnonymousProfileEnable = true;
                portalProfileViewModel.IsDefaultRegistedProfileEnable = true;
            }
            return ActionView(_CreatePortalProfilePopupPanel, portalProfileViewModel);
        }

        //Create portal profile.
        [HttpPost]
        public virtual ActionResult CreatePortalProfile(PortalProfileViewModel portalProfileViewModel)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_portalProfileAgent.CreatePortalProfile(portalProfileViewModel).HasError
                    ? GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate)
                    : GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));

                return RedirectToAction<StoreController>(x => x.PortalProfileList(null, portalProfileViewModel.PortalId));
            }
            return RedirectToAction<StoreController>(x => x.PortalProfileList(null, portalProfileViewModel.PortalId));
        }

        //Edit portal profile.
        public virtual ActionResult EditPortalProfile(int portalId = 0, int portalProfileId = 0, string data = null)
        {
            if (ModelState.IsValid)
            {
                PortalProfileViewModel portalProfileViewModel = _portalProfileAgent.UpdatePortalProfile(portalId, portalProfileId, data);
                if (!portalProfileViewModel.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = portalProfileViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);

        }

        //Delete portal profile.
        [HttpGet]
        public virtual JsonResult DeletePortalProfile(string portalProfileId)
        {
            if (!string.IsNullOrEmpty(portalProfileId))
            {
                bool status = _portalProfileAgent.DeletePortalProfile(portalProfileId);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteProfileErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteProfileErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Manage SMTP 
        //Get SMTP settings.
        [HttpGet]
        public virtual ActionResult GetSMTPDetails(int portalId = 0)
        {
            SMTPViewModel smtpViewModel = _smtpAgent.GetSmtp(portalId);
            smtpViewModel.PortalId = portalId;
            smtpViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            return View(smtpViewModel);
        }

        //Edit SMTP settings.
        [HttpGet]
        public virtual ActionResult EditStoreSMTP(int portalId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            SMTPViewModel smtpViewModel = _smtpAgent.GetSmtp(portalId);
            if (IsNotNull(smtpViewModel))
                smtpViewModel.PortalId = portalId;
            else
                smtpViewModel = new SMTPViewModel() { PortalId = portalId, PortalName = _storeAgent.GetStore(portalId)?.StoreName };

            return View(AdminConstants.CreateEditSMTPView, smtpViewModel);
        }

        //Edit SMTP settings.
        [HttpPost]
        public virtual ActionResult EditStoreSMTP(SMTPViewModel smtpViewModel)
        {
            if (ModelState.IsValid)
            {
                SMTPViewModel smtp = _smtpAgent.UpdateSmtp(smtpViewModel);
                SetNotificationMessage(smtp.HasError || HelperUtility.IsNull(smtp) ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                    : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));

                return RedirectToAction<StoreController>(x => x.EditStoreSMTP(smtpViewModel.PortalId));
            }
            return View(AdminConstants.CreateEditSMTPView, smtpViewModel);
        }
        #endregion

         #region Manage Unit Settings
        //Get portal unit settings.
        [HttpGet]
        public virtual ActionResult GetPortalUnitDetails(int portalId)
        {
            PortalUnitViewModel portalUnitViewModel = _storeUnitAgent.GetStoreUnit(portalId);
            portalUnitViewModel.PortalId = portalId;
            portalUnitViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            return View(portalUnitViewModel);
        }

        //Create/Edit store unit.
        [HttpGet]
        public virtual ActionResult CreateEditStoreUnit(int portalId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            if (portalId > 0)
            {
                PortalUnitViewModel portalUnitViewModel = _storeUnitAgent.GetStoreUnit(portalId);
                if (IsNotNull(portalUnitViewModel))
                {
                    portalUnitViewModel.OldCurrencyId = portalUnitViewModel.CurrencyTypeID.GetValueOrDefault();
                    portalUnitViewModel.OldCultureId = portalUnitViewModel.CultureId.GetValueOrDefault();
                    portalUnitViewModel.PortalId = portalId;
                    portalUnitViewModel.PortalName = string.IsNullOrEmpty(portalUnitViewModel.PortalName) ? _storeAgent.GetStore(portalId)?.StoreName : portalUnitViewModel.PortalName;
                }
                else
                    portalUnitViewModel = new PortalUnitViewModel() { PortalId = portalId, PortalName = _storeAgent.GetStore(portalId)?.StoreName };
                return View(portalUnitViewModel);
            }
            return View(new PortalUnitViewModel());
        }

        //Create/Edit store unit.
        [HttpPost]
        public virtual ActionResult CreateEditStoreUnit(PortalUnitViewModel portalUnitViewModel)
        {
            if (ModelState.IsValid)
            {
                PortalUnitViewModel unit = _storeUnitAgent.CreateUpdateStoreUnit(portalUnitViewModel);
                SetNotificationMessage((unit.HasError || Equals(unit, null))
                    ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                    : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<StoreController>(x => x.CreateEditStoreUnit(portalUnitViewModel.PortalId.GetValueOrDefault()));
            }
            portalUnitViewModel = _storeUnitAgent.GetStoreUnit(portalUnitViewModel.PortalId.GetValueOrDefault());
            portalUnitViewModel.PortalName = _storeAgent.GetStore(portalUnitViewModel.PortalId.GetValueOrDefault())?.StoreName;
            return View(portalUnitViewModel);
        }

        #endregion

        #region Shipping association

        // Edits Shipping section for store. 
        public virtual ActionResult EditStoreShipping(int portalId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            if (DefaultSettingHelper.IsDataSeparationAllowedforAppType())
                return View(_EditShippingView, new PortalShippingViewModel() { PortalId = portalId, PortalName = StoreAgent.CurrentStore.StoreName });
            else
                return View(_EditShippingView, _storeAgent.GetPortalShippingInformation(portalId, DefaultSettingHelper.GetCurrentOrDefaultAppType(0)));

        }
        //Edits Shipping section for store.
        [HttpPost]
        public virtual ActionResult EditStoreShipping(PortalShippingViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                PortalShippingViewModel portalShippingViewModel = _storeAgent.UpdatePortalShipping(model);
                if (HelperUtility.IsNotNull(portalShippingViewModel) && portalShippingViewModel.PortalId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<StoreController>(x => x.EditStoreShipping(portalShippingViewModel.PortalId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            _storeAgent.BindPageDropdown(model, model.PortalId);
            model.PortalName = StoreAgent.CurrentStore.StoreName;
            return View(_EditShippingView, model);
        }

        #endregion

        #region Tax association

        // Edit Tax section for store. 
        public virtual ActionResult EditStoreTax(int portalId = 0)
        {
            ActionResult action = GotoBackURL();
            return IsNotNull(action) ? action : View(_EditTaxView, _storeAgent.GetTaxPortalInformation(portalId));
        }

        //Edit Tax section for store.
        [HttpPost]
        public virtual ActionResult EditStoreTax(TaxPortalViewModel model)
        {
            if (IsNotNull(model))
            {
                TaxPortalViewModel taxPortalViewModel = _storeAgent.UpdateTaxPortal(model);
                if (taxPortalViewModel?.PortalId > 0 && !taxPortalViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<StoreController>(x => x.EditStoreTax(taxPortalViewModel.PortalId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            model.PortalName = StoreAgent.CurrentStore.StoreName;
            return View(_EditTaxView, model);
        }

        [HttpPost]
        public virtual ActionResult TestAvalaraConnection(TaxPortalViewModel model)
        {
            if (IsNotNull(model))
            {
                string taxPortalDetails = _storeAgent.TestAvalaraConnection(model);
                bool isConnectionSuccessful = taxPortalDetails?.ToLower().Contains("connection success") ?? false;

                return Json(new { Data = taxPortalDetails, HasError = !isConnectionSuccessful, Message = isConnectionSuccessful ? Admin_Resources.TestConnectionSuccess : Admin_Resources.TestConnectionFailure });
            }
            return Json(new { HasError = true, Message = Admin_Resources.TestConnectionFailure });
        }

        #endregion

        #region Clear Cache 
        [HttpGet]
        public virtual ActionResult ClearDemoWebsiteCache(int portalId, int domainId)
        {
            CacheListViewModel cacheListViewModel = _generalSettingAgent.CacheData();

            int applicationCacheId = (cacheListViewModel?.CacheList
                .FirstOrDefault(x => string.Equals(x.ApplicationType, ApplicationCacheTypeEnum.FullPageCache.ToString(), StringComparison.InvariantCultureIgnoreCase))
                ?.ApplicationCacheId).GetValueOrDefault();

            CacheViewModel cacheViewModel = _generalSettingAgent.RefreshCache(new CacheViewModel() { ApplicationCacheId = applicationCacheId, DomainIds = domainId.ToString() });

            SetNotificationMessage(cacheViewModel.HasError
                ? GetErrorNotificationMessage(Admin_Resources.CacheNotClearedMessage)
                : GetSuccessNotificationMessage(Admin_Resources.CacheClearedMessage));

            return RedirectToAction<StoreController>(x => x.UrlList(null, portalId));
        }
        #endregion

        #region Price Management
        //Get price management.
        [HttpGet]
        public virtual ActionResult GetPriceManagement(int portalId)
            => View(new PriceListViewModel() { PortalId = portalId, PortalName = _storeAgent.GetStore(portalId)?.StoreName });

        //Get tab structure for price.
        public virtual ActionResult GetTabStructure(int portalId, int? profileId)
            => PartialView(AdminConstants.TabStructurePath, _storeAgent.CreateTabStructure(portalId, profileId));

        //Get tab structure for price.
        public virtual ActionResult GetTabStructureForShippingOrigin(int portalId)
            => PartialView(AdminConstants.TabStructurePath, _storeAgent.CreateTabStructureForShippingOrigin(portalId));

        // Get Price list for store.
        public virtual ActionResult GetAssociatedPriceListForStore([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.BasePriceManagement.ToString(), model);

            _storeAgent.SetFiltersForPortalId(model.Filters, portalId);
            //Get Price list.
            PriceListViewModel priceList = _storeAgent.GetPriceList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model.
            priceList.PortalId = portalId;
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.BasePriceManagement.ToString(), string.Empty, null, true, true, priceList?.GridModel?.FilterColumn?.ToolMenuList);

            priceList.GridModel.TotalRecordCount = priceList.TotalResults;

            //Returns the attribute family list.
            return ActionView("_GetAssociatedPriceListForStore", priceList);
        }

        // Get Price list for profile.
        public virtual ActionResult GetAssociatedPriceListForProfile([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId, int profileId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ProfileBasePriceManagement.ToString(), model);

            _storeAgent.SetFiltersForProfileId(model.Filters, profileId, portalId);
            PriceListViewModel priceList = _storeAgent.GetPriceList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            priceList.Profiles = _storeAgent.GetPortalProfiles(portalId);
            priceList.PortalId = portalId;

            TempData[AdminConstants.ProfileId] = profileId;
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.ProfileBasePriceManagement.ToString(), string.Empty, null, true, true, priceList?.GridModel?.FilterColumn?.ToolMenuList);
            priceList.GridModel.TotalRecordCount = priceList.TotalResults;

            return ActionView("_GetAssociatedPriceListForProfile", priceList);
        }

        public virtual ActionResult GetShippingOriginForProduction(int portalId)
        {
            return PartialView("_editStoreShippingOrigin", _storeAgent.GetPortalShippingInformation(portalId, (byte)ZnodePublishStatesEnum.PRODUCTION));
        }
        public virtual ActionResult GetShippingOriginForPreview(int portalId)
        {
            return ActionView("_editStoreShippingOrigin", _storeAgent.GetPortalShippingInformation(portalId, (byte)ZnodePublishStatesEnum.PREVIEW));
        }

        public virtual ActionResult GetUnAssociatedPriceListForStore([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedPriceList.ToString(), model);

            _storeAgent.SetFiltersForPortalId(model.Filters, portalId);

            //Get Price list.
            PriceListViewModel priceList = _priceAgent.GetUnAssociatedPriceList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            priceList.PortalId = portalId;
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.UnAssociatedPriceList.ToString(), string.Empty, null, true);
            priceList.GridModel.TotalRecordCount = priceList.TotalResults;

            return ActionView("_GetUnAssociatedPriceListForStore", priceList);
        }

        public virtual ActionResult GetUnAssociatedPriceListForProfile([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId, int profileId)
        {
            _storeAgent.SetFiltersForProfileId(model.Filters, profileId, portalId);

            //Get Price list.
            PriceListViewModel priceList = _priceAgent.GetUnAssociatedPriceList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            priceList.PortalId = portalId;

            //Get the grid model.
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.UnAssociatedPriceList.ToString(), string.Empty, null, true);
            priceList.GridModel.TotalRecordCount = priceList.TotalResults;
            priceList.ProfileId = profileId;

            return ActionView("_GetUnAssociatedPriceListForProfile", priceList);
        }

        public virtual JsonResult AssociatePriceListToStore(int portalId, string priceListIds)
        {
            string errorMessage = Admin_Resources.ErrorAssociatePriceListToStore;
            bool status = false;
            if (!string.IsNullOrEmpty(priceListIds) && portalId > 0)
            {
                status = _priceAgent.AssociatePriceListToStore(portalId, priceListIds);
                errorMessage = status ? Admin_Resources.StorePriceSuccessMessage : Admin_Resources.ErrorAssociatePriceListToStore;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult AssociatePriceListToProfile(int profileId, string priceListIds, int portalId)
        {
            string errorMessage = Admin_Resources.ErrorAssociatePriceListToProfile;
            bool status = false;
            if (!string.IsNullOrEmpty(priceListIds) && portalId > 0)
            {
                status = _priceAgent.AssociatePriceListToProfile(profileId, priceListIds, portalId);
                errorMessage = status ? Admin_Resources.ProfilePriceSuccessMessage : Admin_Resources.ErrorAssociatePriceListToProfile;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult RemoveAssociatedPriceListToStore(string priceListPortalId)
        {
            if (!string.IsNullOrEmpty(priceListPortalId))
            {
                bool status = _priceAgent.RemoveAssociatedPriceListToStore(priceListPortalId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult RemoveAssociatedPriceListToProfile(string priceListProfileId)
        {
            if (!string.IsNullOrEmpty(priceListProfileId))
            {
                bool status = _priceAgent.RemoveAssociatedPriceListToProfile(priceListProfileId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Update associated price list precedence value for Store/Profile.
        public virtual ActionResult EditAssociatedPriceListPrecedence(string data)
        {
            bool status = _priceAgent.UpdateAssociatedPriceListPrecedence(data);
            return Json(new { status = status, message = status ? PIM_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Inventory Management

        //Get associated warehouses as per portalId
        [HttpGet]
        public virtual ActionResult GetAssociatedWarehouseList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId, int warehouseId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            PortalWarehouseViewModel viewModel = _storeAgent.GetAssociatedWarehouseList(portalId, warehouseId, model.Expands, model.Filters);
            viewModel.PortalId = portalId;
            viewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            return ActionView("_StoreWarehouseAssociation", viewModel);
        }

        //Associate warehouses to respective store
        [HttpPost]
        public virtual ActionResult AssociateWarehouseToStore(int portalId, int warehouseId, string alternateWarehouseIds)
        {
            bool isAssociated = _storeAgent.AssociateWarehouseToStore(portalId, warehouseId, alternateWarehouseIds);
            if (isAssociated)
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<StoreController>(m => m.GetAssociatedWarehouseList(null, portalId, warehouseId));
        }

        #endregion

        #region Catalog View
        //Get Associated portal catalog
        public virtual ActionResult GetAssociatedPortalCatalog([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            PortalCatalogListViewModel portalCatalogListViewModel = _ecommerceCatalogAgent.GetAssociatedPortalCatalogByPortalId(portalId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            portalCatalogListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, portalCatalogListViewModel.PortalCatalogs, GridListType.ZnodePortalCatalog.ToString(), string.Empty, null, true);
            portalCatalogListViewModel.GridModel.TotalRecordCount = portalCatalogListViewModel.TotalResults;
            portalCatalogListViewModel.PortalId = portalId;
            portalCatalogListViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            return ActionView("_GetAssociatedPortalCatalog", portalCatalogListViewModel);
        }

        //Get Catalog List
        public virtual ActionResult CatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            CatalogListViewModel catalogList = _catalogAgent.GetCatalogList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            catalogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catalogList.Catalogs, GridListType.ZnodePimCatalog.ToString(), string.Empty, null, true);

            catalogList.GridModel.TotalRecordCount = catalogList.TotalResults;
            return ActionView(catalogList);
        }

        //Edit Portal Catalog
        [HttpGet]
        public virtual ActionResult EditPortalCatalog(int portalCatalogId)
            => (portalCatalogId > 0) ? PartialView("_EditPortalCatalog", _ecommerceCatalogAgent.GetPortalCatalog(portalCatalogId)) : null;

        //Edit Portal Catalog
        [HttpPost]
        public virtual ActionResult EditPortalCatalog(PortalCatalogViewModel portalCatalogViewModel)
        {
            bool status = false;
            if (ModelState.IsValid)
                status = _ecommerceCatalogAgent.UpdatePortalCatalog(portalCatalogViewModel);
            return Json(new { status = status, message = status ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage, portalId = portalCatalogViewModel.PortalId }, JsonRequestBehavior.AllowGet);
        }

        //View portal catalog tree structure 
        [HttpGet]
        public virtual ActionResult ViewPortalCatalog(int publishCatalogId, int portalId = 0)
           => View(new PortalCatalogViewModel { PublishCatalogId = publishCatalogId, PortalId = portalId, PortalName = _storeAgent.GetStore(portalId)?.StoreName });

        //Get catalog tree structure
        [HttpGet]
        public virtual ActionResult GetCatalogTree(int portalCatalogId, int publishCategoryId)
           => Json(_ecommerceCatalogAgent.GetCatalogTree(portalCatalogId, publishCategoryId), JsonRequestBehavior.AllowGet);

        //Get Publish Catalog Details
        [HttpGet]
        public virtual ActionResult GetPublishCatalogDetails(int publishCatalogId)
            => (publishCatalogId > 0) ? PartialView(_DisplayPublishInfo, _ecommerceCatalogAgent.GetPublishCatalogDetails(publishCatalogId)) : null;

        //Get Publish Category Details
        [HttpGet]
        public virtual ActionResult GetPublishCategoryDetails(int publishCategoryId)
            => (publishCategoryId > 0) ? PartialView(_DisplayPublishInfo, _ecommerceCatalogAgent.GetPublishCategoryDetails(publishCategoryId)) : null;

        //Get Publish Product Details
        [HttpGet]
        public virtual ActionResult GetPublishProductDetails(int publishProductId, int portalId)
            => (publishProductId > 0 && portalId > 0) ? PartialView(_DisplayPublishInfo, _ecommerceCatalogAgent.GetPublishProductDetails(publishProductId, portalId)) : null;
        #endregion

        #region Portal Locale
        //Method to get active locale list as portal Id.
        public virtual ActionResult LocaleList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePortalLocale.ToString(), model);
            //Get the list of locales            
            LocaleListViewModel localeListViewModel = _storeAgent.GetLocales(portalId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            localeListViewModel.PortalId = portalId;
            localeListViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;

            //Get the grid model
            localeListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, localeListViewModel?.Locales, GridListType.ZnodePortalLocale.ToString(), string.Empty, null, true, true, localeListViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            localeListViewModel.GridModel.TotalRecordCount = localeListViewModel.TotalResults;

            //Returns the locale list view
            return ActionView(localeListViewModel);
        }

        //Action for Update Locale.
        [HttpPost]
        public virtual ActionResult UpdateLocale(DefaultGlobalConfigViewModel model)
        {
            string message = string.Empty;
            bool status = _storeAgent.UpdateLocale(model, out message);
            message = status ? Admin_Resources.UpdateMessage : message;
            if (status)
            {
                switch (model.Action)
                {
                    case ZnodeConstant.SetDefault:
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                        break;
                    case ZnodeConstant.SetActive:
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SetActive));
                        break;
                    case ZnodeConstant.SetDeactive:
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SetDeActive));
                        break;
                    default:
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                        break;
                }
                return RedirectToAction<StoreController>(x => x.LocaleList(null, model.PortalId));
            }
            SetNotificationMessage(GetErrorNotificationMessage(message));
            return RedirectToAction<StoreController>(x => x.LocaleList(null, model.PortalId));
        }
        #endregion

        #region Country Association
        // Get associated country list.
        public virtual ActionResult GetAssociatedCountryList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePortalCountry.ToString(), model);
            CountryListViewModel list = _portalCountryAgent.GetAssociatedOrUnAssociatedCountryList(model, portalId, true);
            list.PortalId = portalId;
            list.PortalName = _storeAgent.GetStore(portalId)?.StoreName;

            //Get the grid model.
            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.Countries, GridListType.ZnodePortalCountry.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("_PortalAssociatedCountryList", list);
        }

        //Get list of unassociate countries.
        public virtual ActionResult GetUnAssociatedCountryList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedPortalCountry.ToString(), model);
            CountryListViewModel list = _portalCountryAgent.GetAssociatedOrUnAssociatedCountryList(model, portalId, false);
            list.PortalId = portalId;

            //Get the grid model.
            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.Countries, GridListType.UnAssociatedPortalCountry.ToString(), string.Empty, null, true);

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("_UnAssociatedCountryList", list);
        }

        //Remove associated countries.
        public virtual JsonResult UnAssociateCountries(string portalCountryId, int portalId)
        {
            if (!string.IsNullOrEmpty(portalCountryId))
            {
                string message = Admin_Resources.UnassignError;
                bool status = _portalCountryAgent.UnAssociateCountries(portalCountryId, portalId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
        }

        //Associate countries to portal.
        public virtual JsonResult AssociateCountries(string countryCode, int portalId, bool isDefault = false, int portalCountryId = 0)
        {
            SetNotificationMessage(_portalCountryAgent.AssociateCountries(portalId, countryCode, isDefault, portalCountryId)
                ? portalCountryId > 0 ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetSuccessNotificationMessage(Admin_Resources.AssociatedSuccessMessage)
                : GetErrorNotificationMessage(Admin_Resources.AssociatedErrorMessage));
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion        

        #region Website configurations
        //Get WebSite Logo details
        public virtual ActionResult StoreSetting(int portalId, string fileName = "")
        {
            //Get the available WebSite Logo details.
            WebSiteLogoViewModel model = _websiteAgent.GetWebSiteLogoDetails(portalId);
            if (model?.PortalId > 0)
            {
                model.FileName = fileName;
                return Request.IsAjaxRequest() ? ActionView(_ManagePartialView, model) : ActionView(model);
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorPortalNotFound));
            return RedirectToAction<StoreController>(x => x.List(null));
        }

        //Publish CMS configuration.
        public virtual JsonResult PublishStoreSetting(int portalId, string targetPublishState = null, string publishContent = null)
        {
            if (portalId > 0)
            {
                string errorMessage;
                bool status = _websiteAgent.Publish(portalId, out  errorMessage, targetPublishState, publishContent);
                return Json(new { status = status, message = status ? string.Format(Admin_Resources.TextPublishInProgress, "Store") : errorMessage  }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = PIM_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
        }

        //Save the WebSite Logo details.
        [HttpPost]
        public virtual ActionResult SaveWebSiteLogo(WebSiteLogoViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool status = _websiteAgent.SaveWebSiteLogo(model);
                string message = status ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage;
                ActionResult action = GotoBackURL();

                if (IsNotNull(action))
                    SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

                return Json(new { status = status, message = message, Url = Equals(action, null) ? "" : "/Store/List" }, JsonRequestBehavior.AllowGet);
            }
            model.HasError = true;
            return ActionView(_ManagePartialView, model);
        }
        #endregion

        #region Tax
        //Get tax class list.
        public virtual ActionResult TaxList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePortalTaxClassList.ToString(), model);
            TaxClassListViewModel taxClassList = _storeAgent.GetTaxClassList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, true);
            taxClassList.PortalId = portalId;
            taxClassList.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            taxClassList.GridModel = FilterHelpers.GetDynamicGridModel(model, taxClassList.TaxClassList, GridListType.ZnodePortalTaxClassList.ToString(), string.Empty, null, true, true, taxClassList?.GridModel?.FilterColumn?.ToolMenuList);

            //set the total record count.
            taxClassList.GridModel.TotalRecordCount = taxClassList.TotalResults;

            //returns the view.
            return ActionView(taxClassList);
        }

        //Get unassociated Taxclass list.
        public virtual ActionResult GetUnAssociatedTaxList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePortalTaxClassAssociatedList.ToString(), model);
            TaxClassListViewModel taxClassList = _storeAgent.GetTaxClassList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, false);
            //Remove tool option.
            taxClassList?.GridModel?.FilterColumn?.ToolMenuList.Clear();
            //Get the grid model.
            taxClassList.GridModel = FilterHelpers.GetDynamicGridModel(model, taxClassList?.TaxClassList, GridListType.ZnodePortalTaxClassAssociatedList.ToString(), string.Empty, null, true, true, null);
            taxClassList.PortalId = portalId;
            //Set the total record count
            taxClassList.GridModel.TotalRecordCount = taxClassList.TotalResults;

            return PartialView("_UnAssociatedTaxList", taxClassList);
        }

        // Action for associate taxclass.
        public virtual JsonResult AssociateTaxClass(string taxClassIds, int portalId = 0)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (IsNotNull(taxClassIds) && portalId > 0)
            {
                status = _storeAgent.AssociateUnAssociateTaxClass(portalId, taxClassIds, false, out errorMessage);
                errorMessage = status ? Admin_Resources.SuccessAssociateStoreTax : errorMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Action for unassociate taxclass.
        public virtual ActionResult UnAssociateTaxClass(string taxClassId, int portalId = 0)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (IsNotNull(taxClassId) && portalId > 0)
            {
                status = _storeAgent.AssociateUnAssociateTaxClass(portalId, taxClassId, true, out errorMessage);
                errorMessage = status ? Admin_Resources.TaxUnassociatedSuccessfully : errorMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Set default taxclass for portal.
        public virtual ActionResult SetPortalDefaultTax(string taxClassId, int portalId = 0)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (IsNotNull(taxClassId) && portalId > 0)
            {
                status = _storeAgent.SetPortalDefaultTax(portalId, taxClassId, out errorMessage);
                SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.StoreDefaultTaxSuccessMessage) : GetErrorNotificationMessage(errorMessage));
                return Json(new { status = status }, JsonRequestBehavior.AllowGet);
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorUnableToSetDefaultTax));
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Payment
        //Get tax class list.
        public virtual ActionResult GetAssociatedPaymentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedPaymentListToPortal.ToString(), model);
            PaymentSettingListViewModel paymentSettingList = GetPaymentListAssociated(model, portalId);

            return ActionView("AssociatedPaymentList", paymentSettingList);
        }

        //Get associated .
        public virtual ActionResult GetAssociatedInvoiceManagementPaymentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedPaymentListToPortal.ToString(), model);
            model.Filters.Add(new FilterTuple(AdminConstants.FilterTupleForOfflinePayment, FilterOperators.Equals, "true"));

            PaymentSettingListViewModel paymentSettingList = GetPaymentListAssociated(model, portalId);
            paymentSettingList.IsUsedForOfflinePayment = true;
            return ActionView("AssociatedPaymentListForInvoice", paymentSettingList);
        }

        public virtual PaymentSettingListViewModel GetPaymentListAssociated(FilterCollectionDataModel model, int portalId)
        {
            bool isUsedForOfflinePayment = false;
            if (model.Filters?.Count > 0 && model.Filters.Any(x => string.Equals(x.FilterName, "IsUsedForOfflinePayment", StringComparison.InvariantCultureIgnoreCase)))
            {
                //Get filter value
                isUsedForOfflinePayment = Convert.ToBoolean(model.Filters.FirstOrDefault(x => string.Equals(x.FilterName, "IsUsedForOfflinePayment", StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
            }
            PaymentSettingListViewModel paymentSettingList = _storeAgent.GetPaymentSettingsList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);
            paymentSettingList.PortalId = portalId;
            paymentSettingList.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            var publishStateList = _storeAgent.GetApplicationType();
            paymentSettingList?.PaymentSettings?.ForEach(item => { item.PublishStateList = publishStateList; });
            paymentSettingList?.PaymentSettings?.ForEach(item => { item.IsUsedForOfflinePayment = isUsedForOfflinePayment; });

            //Get the grid model.
            paymentSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, paymentSettingList.PaymentSettings, GridListType.AssociatedPaymentListToPortal.ToString(), null, null, true, true, paymentSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            if (DefaultSettingHelper.IsDataSeparationAllowedforAppType() == false)
            {
                var removeGridColumn = paymentSettingList.GridModel.WebGridColumn.FirstOrDefault(x => x.ColumnName == "PublishState");
                paymentSettingList.GridModel.WebGridColumn.Remove(removeGridColumn);
                var removeFilterColumn = paymentSettingList.GridModel.FilterColumn.FilterColumnList.FirstOrDefault(x => x.ColumnName == "PublishState");
                paymentSettingList.GridModel.FilterColumn.FilterColumnList.Remove(removeFilterColumn);
            }

            paymentSettingList.GridModel.TotalRecordCount = paymentSettingList.TotalResults;

            return paymentSettingList;
        }

        //Get tax class list.
        public virtual ActionResult GetUnassociatedPaymentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedPaymentListToPortal.ToString(), model);

            PaymentSettingListViewModel paymentSettingList = GetPaymentListUnassociated(model, portalId);
            return ActionView("UnassociatedPaymentList", paymentSettingList);
        }

        public virtual PaymentSettingListViewModel GetPaymentListUnassociated(FilterCollectionDataModel model, int portalId)
        {
            PaymentSettingListViewModel paymentSettingList = _storeAgent.GetPaymentSettingsList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);
            paymentSettingList.PortalId = portalId;
            //Get the grid model.
            paymentSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, paymentSettingList.PaymentSettings, GridListType.UnassociatedPaymentListToPortal.ToString(), string.Empty, null, true, true, paymentSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            paymentSettingList.GridModel.TotalRecordCount = paymentSettingList.TotalResults;

            return paymentSettingList;
        }

        //Get unassociated payment
        public virtual ActionResult GetUnassociatedPaymentListForInvoice([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedPaymentListToPortal.ToString(), model);
            model.Filters.Add(new FilterTuple(AdminConstants.FilterTupleForOfflinePayment, FilterOperators.Equals, "true"));

            PaymentSettingListViewModel paymentSettingList = GetPaymentListUnassociated(model, portalId);

            return ActionView("UnassociatedOfflinePaymentList", paymentSettingList);
        }

        // Action for associate taxclass.
        public virtual JsonResult AssociatePaymentSetting(string paymentSettingId, int portalId = 0)
        {
            if (IsNotNull(paymentSettingId) && portalId > 0)
            {
                string message = string.Empty;
                bool status = _storeAgent.AssociatePaymentSettings(portalId, paymentSettingId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.AssociatedSuccessMessage : (string.IsNullOrEmpty(message)) ? Admin_Resources.AssociatedErrorMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.AssociatedErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        // Action for associate taxclass.
        public virtual JsonResult AssociateOfflinePaymentSetting(string paymentSettingId, int portalId = 0)
        {
            if (IsNotNull(paymentSettingId) && portalId > 0)
            {
                string message = string.Empty;
                bool status = _storeAgent.AssociatePaymentSettingsForInvoice(portalId, paymentSettingId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.AssociatedSuccessMessage : (string.IsNullOrEmpty(message)) ? Admin_Resources.AssociatedErrorMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.AssociatedErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        // Action for associate taxclass.
        public virtual JsonResult RemoveAssociatedPaymentSetting(string paymentSettingId, int portalId = 0, bool isUsedForOfflinePayment = false)
        {
            if (IsNotNull(paymentSettingId) && portalId > 0)
            {
                bool status = _storeAgent.RemoveAssociatedPaymentSettings(portalId, paymentSettingId, isUsedForOfflinePayment);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
        }

        //Update portal payment setting
        public virtual JsonResult UpdatePortalPaymentSettings(int paymentSettingId, int portalId, string data)
        {
            if (IsNotNull(paymentSettingId) && portalId > 0 && ModelState.IsValid && IsNotNull(data))
            {
                bool status = _storeAgent.UpdatePortalPaymentSettings(paymentSettingId, portalId, data);
                return Json(new { status = status, message = status ? Admin_Resources.UpdateMessage : Admin_Resources.ErrorPaymentNameAlreadyExist }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Associate Portal Shipping
        //Get Associated shipping list for portal.
        public virtual ActionResult GetAssociatedShippingList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAssociatedShippingListToPortal.ToString(), model);
            //Get portal shipping list.
            ShippingListViewModel shippingListViewModel = _storeAgent.GetAssociatedShippingList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            shippingListViewModel.PortalId = portalId;

            var publishStateList = _storeAgent.GetApplicationType();
            shippingListViewModel?.ShippingList?.ForEach(item => { item.PublishStateList = publishStateList; });

            //Get the grid model.
            shippingListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingListViewModel?.ShippingList, GridListType.ZnodeAssociatedShippingListToPortal.ToString(), string.Empty, null, true, true, shippingListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            if (DefaultSettingHelper.IsDataSeparationAllowedforAppType() == false)
            {
                var removeGridColumn = shippingListViewModel.GridModel.WebGridColumn.FirstOrDefault(x => x.ColumnName == "PublishState");
                shippingListViewModel.GridModel.WebGridColumn.Remove(removeGridColumn);
                var removeFilterColumn = shippingListViewModel.GridModel.FilterColumn.FilterColumnList.FirstOrDefault(x => x.ColumnName == "PublishState");
                shippingListViewModel.GridModel.FilterColumn.FilterColumnList.Remove(removeFilterColumn);
            }
            shippingListViewModel.GridModel.TotalRecordCount = shippingListViewModel.TotalResults;

            return ActionView(AssociatedPortalShipping, shippingListViewModel);
        }

        //Get UnAssociated shipping list for portal.
        public virtual ActionResult GetUnAssociatedShippingList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUnAssociatedShippingList.ToString(), model);
            //Get shipping list.
            ShippingListViewModel shippingListViewModel = _storeAgent.GetUnAssociatedShippingList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            shippingListViewModel.PortalId = portalId;
            //Get the grid model.
            shippingListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingListViewModel?.ShippingList, GridListType.ZnodeUnAssociatedShippingList.ToString(), string.Empty, null, true);
            shippingListViewModel.GridModel.TotalRecordCount = shippingListViewModel.TotalResults;

            return ActionView(UnAssociatedPortalShipping, shippingListViewModel);
        }

        //Associate UnAssociated shipping list to portal.
        public virtual JsonResult AssociateShipping(int portalId, string shippingIds)
        {
            if (!string.IsNullOrEmpty(shippingIds) && portalId > 0)
            {
                bool status = _storeAgent.AssociateShipping(portalId, shippingIds);
                return Json(new { status = status, message = status ? Admin_Resources.PortalShippingSuccessMessage : Admin_Resources.ErrorAssociationShippingToPortal }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorAssociationShippingToPortal }, JsonRequestBehavior.AllowGet);
        }

        //UnAssociate shipping from portal.
        public virtual JsonResult UnAssociateAssociatedShipping(string shippingId, int portalId)
        {
            if (!string.IsNullOrEmpty(shippingId) && portalId > 0)
            {
                bool status = _storeAgent.UnAssociateAssociatedShipping(shippingId, portalId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Portal Search Filter Setting

        [HttpGet]
        public virtual ActionResult GetSearchSetting(int portalId)
            => View(new PortalSortSettingListViewModel() { PortalId = portalId, PortalName = _storeAgent.GetStore(portalId)?.StoreName });

        //Get tab structure for search setting.
        public virtual ActionResult GetTabStructureForSearch(int portalId)
            => PartialView(AdminConstants.TabStructurePath, _storeAgent.CreateTabStructureForSearch(portalId));

        // Action for associate taxclass.
        public virtual JsonResult AssociateSortSetting(string sortSettingId, int portalId = 0)
        {
            if (IsNotNull(sortSettingId) && portalId > 0)
            {
                string message = string.Empty;
                bool status = _storeAgent.AssociateSortSettings(portalId, sortSettingId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.AssociatedSuccessfulMessageForSearchFilter : (string.IsNullOrEmpty(message)) ? Admin_Resources.AssociatedErrorMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.AssociatedErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        // Get Price list for store.
        public virtual ActionResult GetAssociatedSortForStore([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedSortListToPortal.ToString(), model);

            PortalSortSettingListViewModel sortSettingList = _storeAgent.GetPortalSortSettingsList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);
            sortSettingList.PortalId = portalId;
            sortSettingList.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            //Get the grid model.
            sortSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, sortSettingList.SortSettings, GridListType.AssociatedSortListToPortal.ToString(), null, null, true, true, sortSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            sortSettingList.GridModel.TotalRecordCount = sortSettingList.TotalResults;
            return ActionView("_GetAssociatedSortForStore", sortSettingList);
        }

        // Action for associate taxclass.
        public virtual JsonResult AssociatePageSetting(string pageSettingId, int portalId = 0)
        {
            if (IsNotNull(pageSettingId) && portalId > 0)
            {
                string message = string.Empty;
                bool status = _storeAgent.AssociatePageSettings(portalId, pageSettingId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.AssociatedSuccessfulMessageForSearchFilter : (string.IsNullOrEmpty(message)) ? Admin_Resources.AssociatedErrorMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.AssociatedErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get sort list.
        public virtual ActionResult GetUnassociatedSortList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedSortListToPortal.ToString(), model);

            PortalSortSettingListViewModel sortSettingList = _storeAgent.GetPortalSortSettingsList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);
            sortSettingList.PortalId = portalId;
            //Get the grid model.
            sortSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, sortSettingList.SortSettings, GridListType.UnassociatedSortListToPortal.ToString(), string.Empty, null, true, true, sortSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            sortSettingList.GridModel.TotalRecordCount = sortSettingList.TotalResults;
            return ActionView("_GetUnAssociatedSortForStore", sortSettingList);
        }
        // Action for associate sort.
        public virtual JsonResult RemoveAssociatedSortSetting(string portalSortSettingId, int portalId = 0)
        {
            if (IsNotNull(portalSortSettingId) && portalId > 0)
            {
                bool status = _storeAgent.RemoveAssociatedSortSettings(portalId, portalSortSettingId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessfulMessageForSearchFilter : Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
        }

        // Get Price list for store.
        public virtual ActionResult GetAssociatedPageForStore([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and pageing if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedPageListToPortal.ToString(), model);

            PortalPageSettingListViewModel pageSettingList = _storeAgent.GetPortalPageSettingsList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);
            pageSettingList.PortalId = portalId;
            pageSettingList.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            //Get the grid model.
            pageSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, pageSettingList.PageSettings, GridListType.AssociatedPageListToPortal.ToString(), null, null, true, true, pageSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            pageSettingList.GridModel.TotalRecordCount = pageSettingList.TotalResults;
            return ActionView("_GetAssociatedPageForStore", pageSettingList);
        }

        //Edit associated page setting.
        public virtual ActionResult EditAssociatedPageSetting(int portalPageSettingId, string data)
        {
            PortalPageSettingViewModel domainViewModel = JsonConvert.DeserializeObject<PortalPageSettingViewModel[]>(data)[0];
            string errorMessage = string.Empty;
            if (HelperUtility.IsNotNull(domainViewModel))
            {
                if (_storeAgent.UpdatePortalPageSetting(domainViewModel, out errorMessage))
                {
                    return Json(new { status = true, message = Admin_Resources.UpdatePortalPageSuccessMessage }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        // Action for associate sort.
        public virtual JsonResult RemoveAssociatedPageSetting(string portalPageSettingId, int portalId = 0)
        {
            if (IsNotNull(portalPageSettingId) && portalId > 0)
            {
                string errorMessage = string.Empty;
                bool status = _storeAgent.RemoveAssociatedPageSettings(portalId, portalPageSettingId, out errorMessage);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessfulMessageForSearchFilter : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
        }

        //Get sort list.
        public virtual ActionResult GetUnassociatedPageList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedPageListToPortal.ToString(), model);

            PortalPageSettingListViewModel pageSettingList = _storeAgent.GetPortalPageSettingsList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);
            pageSettingList.PortalId = portalId;
            //Get the grid model.
            pageSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, pageSettingList.PageSettings, GridListType.UnassociatedPageListToPortal.ToString(), string.Empty, null, true, true, pageSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            pageSettingList.GridModel.TotalRecordCount = pageSettingList.TotalResults;
            return ActionView("_GetUnAssociatedPageForStore", pageSettingList);
        }

        #endregion

        #region Manage SMS

        //Edit SMS settings.
        [HttpGet]
        public virtual ActionResult EditStoreSMSNotification(int portalId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            PortalSMSViewModel smsViewModel = new PortalSMSViewModel();
            smsViewModel = _smsAgent.GetSmsSettingViewData(null,portalId,true);
            smsViewModel.SmsProviderList = _smsAgent.GetSmsProviderList();
            if (IsNotNull(smsViewModel))
            {
                smsViewModel.PortalId = portalId;
                smsViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            }
            else
                smsViewModel = new PortalSMSViewModel() { PortalId = portalId, PortalName = _storeAgent.GetStore(portalId)?.StoreName };

            return View("CreateEditSms", smsViewModel);
        }

        //Edit SMS settings.
        [HttpPost]
        public virtual ActionResult EditStoreSMSNotification(PortalSMSViewModel smsViewModel)
        {
            if (ModelState.IsValid)
            {
                PortalSMSViewModel sms = _smsAgent.InsertUpdateSMSSetting(smsViewModel);
                SetNotificationMessage(sms.HasError || HelperUtility.IsNull(sms) ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                    : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));

                return RedirectToAction<StoreController>(x => x.EditStoreSMSNotification(smsViewModel.PortalId));
            }
            return View(AdminConstants.CreateEditSMTPView, smsViewModel);
        }

        /// <summary>
        /// Get Provider Type View
        /// </summary>
        /// <param name="providerName">string providerName</param>
        /// <param name="providerCode">string providerCode</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult GetProviderTypeForm(string providerName,int portalId, string currentProviderId, bool isSMSSettingEnabled = false)
        {
            PortalSMSViewModel smsviewmodel = _smsAgent.GetSmsSettingViewData(providerName, portalId, isSMSSettingEnabled);
           if(HelperUtility.IsNull(smsviewmodel))
            {
                smsviewmodel = new PortalSMSViewModel();
                smsviewmodel.SMSProviderId = Convert.ToInt32(currentProviderId);
            }
           
            return PartialView(AdminConstants.TwilioView, smsviewmodel);
        }
        #endregion

        //Get Analytics data.
        [HttpGet]
        public virtual ActionResult GetAnalytics(int portalId)
        {
            ActionResult action = GotoBackURL();
            return IsNotNull(action) ? action : View(CreateEditAnalytics, _storeAgent.GetAnalytics(portalId));
        }

        //save analytics data.
        [HttpPost]
        public virtual ActionResult SaveAnalytics(AnalyticsViewModel vieModel)
        {
            ActionResult action = GotoBackURL();
            if (ModelState.IsValid)
            {
                string errorMessage;
                SetNotificationMessage(_storeAgent.SaveAnalytics(vieModel, out errorMessage) ?
                   GetSuccessNotificationMessage(Admin_Resources.SaveMessage) : GetErrorNotificationMessage(IsNotNull(errorMessage) ? errorMessage : Admin_Resources.SaveErrorMessage));
                if (vieModel.HasError)
                {
                   return View(CreateEditAnalytics, _storeAgent.GetAnalytics(vieModel.PortalId));
                }
                return RedirectToAction<StoreController>(x => x.GetAnalytics(vieModel.PortalId));
            }
            return View(CreateEditAnalytics, vieModel);
        }

        /// <summary>
        /// Gets currency information.
        /// </summary>
        /// <param name="currencyId">Information for a particular currency</param>
        /// <returns>Json value containing currency information.</returns>
        [HttpGet]
        public virtual JsonResult GetCurrencyInformation(int currencyId, int oldCurrencyId, int cultureId)
            => Json(_storeUnitAgent.GetCurrencyInformationByCurrencyId(currencyId, oldCurrencyId, cultureId), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public virtual JsonResult GetCultureCodeList(int currencyId, int cultureId)
        => Json(_storeUnitAgent.GetCurrencyInformationByCultureCode(currencyId, cultureId), JsonRequestBehavior.AllowGet);

        public virtual ActionResult GetCatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            PortalCatalogListViewModel catalogList = _storeAgent.GetPublishCatalogList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            catalogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catalogList.PortalCatalogs, GridListType.ZnodeStoreCatalog.ToString(), string.Empty, null, true, true, catalogList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            catalogList.GridModel.TotalRecordCount = catalogList.TotalResults;
            //return ActionView(catalogList);
            return ActionView(PublishCatalogListPanel, catalogList);
        }

        public virtual ActionResult GetPortalPublishStatus(int portalId, string storeName, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            PublishPortalLogListViewModel publishPortalLogListViewModel = _storeAgent.GetPortalPublishStatus(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            publishPortalLogListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, publishPortalLogListViewModel.PublishPortalLog, GridListType.ZnodePublishPortalLog.ToString(), string.Empty, null, true, true, publishPortalLogListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            publishPortalLogListViewModel.GridModel.TotalRecordCount = publishPortalLogListViewModel.TotalResults;
            return ActionView("_PortalPublishStatus", publishPortalLogListViewModel);
        }

        //Get Email section.
        [HttpGet]
        public virtual ActionResult TestEmail(int portalId)
        {
            EmailViewModel emailViewModel = new EmailViewModel();
            emailViewModel.PortalId = portalId;
            return ActionView(emailViewModel);
        }


        //Method to send email.
        [HttpPost]
        public virtual ActionResult TestEmail(EmailViewModel emailViewModel)
        {
            if (ModelState.IsValid)
            {
                {
                    emailViewModel = _smtpAgent.SendTestEmail(emailViewModel);
                    if (!emailViewModel.HasError)
                        return Json(new { status = true, message = Admin_Resources.EmailSendSuccessMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false, message = Admin_Resources.EmailSendFailureMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get portal management data.
        [HttpGet]
        public virtual ActionResult GetPortalApproverDetailsById(int portalId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            PortalApprovalViewModel portalApprovalViewModel = _storeAgent.GetPortalApproverDetailsById(portalId, 0);
            portalApprovalViewModel.Portals = _storeAgent.GetPortalSelectList(portalId);

            portalApprovalViewModel.PortalId = portalId > 0 ? portalId : Convert.ToInt32(portalApprovalViewModel.Portals.FirstOrDefault().Value);

            //if (!portalApprovalViewModel.HasError)
            //    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);

            //return Json(new { status = false, message = Admin_Resources.UpdateError}, JsonRequestBehavior.AllowGet);
            return (Request.IsAjaxRequest()) ? PartialView("_PartialPortalApproverPanel", portalApprovalViewModel) :
           ActionView("GetPortalApprovalManagement", portalApprovalViewModel);

        }

        // Delete approval level by id
        public virtual JsonResult DeletePortalApproverUser(string userApproverId)
        {
            if (!string.IsNullOrEmpty(userApproverId))
            {
                string message = string.Empty;
                bool status = _accountAgent.DeleteApproverLevelById(userApproverId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.ErrorFailToDeleteParentAccount : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        #region Robots.txt
        //Get Robots.txt data.
        [HttpGet]
        public virtual ActionResult GetRobotsTxt(int portalId)
        {
            ActionResult action = GotoBackURL();
            return IsNotNull(action) ? action : View(CreateEditRobotsTxt, _storeAgent.GetRobotsTxt(portalId));
        }

        //Save Robots.txt data.
        public virtual ActionResult SaveRobotsTxt(RobotsTxtViewModel viewModel)
        {
            ActionResult action = GotoBackURL();
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_storeAgent.SaveRobotsTxt(viewModel) ?
                   GetSuccessNotificationMessage(Admin_Resources.SaveMessage) : GetErrorNotificationMessage(Admin_Resources.SaveErrorMessage));
                return RedirectToAction<StoreController>(x => x.GetRobotsTxt(viewModel.PortalId));
            }
            return View(CreateEditRobotsTxt, viewModel);
        }
        #endregion

        //Check StoreCode already present in DB.
        [HttpGet]
        public virtual JsonResult IsStoreCodeExists(string codeField)
        {
            bool isExist = _helperAgent.IsCodeExists(codeField, CodeFieldService.PortalService.ToString(), CodeFieldService.IsCodeExists.ToString());
            return Json(new { isExist = !isExist, message = Admin_Resources.ErrorStoreCodeExist }, JsonRequestBehavior.AllowGet);
        }

        //Get level list
        [HttpGet]
        public virtual ActionResult GetApproverOrder(int portalApprovalId)
        {
            //Get List of Available Template Area. 
            PortalApprovalViewModel portalApproverViewModel = new PortalApprovalViewModel();
            portalApproverViewModel.PortalApprovalId = portalApprovalId;
            //Convert View Result into the String.
            string partialPortalApproverView = RenderRazorViewToString("_PartialPortalApproverPanel", portalApproverViewModel);
            return Json(new { html = partialPortalApproverView, status = true, message = Admin_Resources.AreaNotFound }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get User Approver list based on search term.
        /// </summary>
        /// <param name="searchTerm">searchTerm</param>
        /// <param name="portalId">portalId</param>
        /// <param name="accountId">accountId</param>
        /// <returns>returns approver users name</returns>
        public virtual JsonResult GetApproverUsersByPortalId(string searchTerm, int portalId, string approvalUserIds)
        {
            JsonResult jsonResult = Json(_accountAgent.GetApproverUsersByName(searchTerm, portalId, 0, 0, approvalUserIds), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #region Approval Management Tab
        /// <summary>
        /// Get Approver Tab
        /// </summary>
        /// <param name="selectedApprovalTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual JsonResult GetApprovalList(int portalId, string selectedApprovalType, int selectedApprovalTypeId)
        {
            PortalApprovalViewModel portalApproverViewModel = new PortalApprovalViewModel();
            string partialPortalApproverView = string.Empty;
            portalApproverViewModel = _storeAgent.GetPortalApproverDetailsById(portalId, selectedApprovalTypeId);

            partialPortalApproverView = Equals(selectedApprovalType, ZnodePortalApprovalsLevelEnum.Store.ToString())
                                                    ? RenderRazorViewToString("PortalUserApproval", portalApproverViewModel)
                                                    : RenderRazorViewToString("PortalPaymentApproval", portalApproverViewModel);

            return Json(new { html = partialPortalApproverView }, JsonRequestBehavior.AllowGet);
        }

        //This method will create and update the approvers for the store.
        [HttpPost]
        public virtual ActionResult SaveUpdatePortalApprovalDetails(PortalApprovalViewModel portalApprovalViewModel)
        {
            string errorMessage;
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_storeAgent.SaveUpdatePortalApprovalDetails(portalApprovalViewModel, out errorMessage) ?
                   GetSuccessNotificationMessage(Admin_Resources.SaveMessage) : GetErrorNotificationMessage(IsNotNull(errorMessage) ? errorMessage : Admin_Resources.SaveErrorMessage));
                return RedirectToAction<StoreController>(x => x.GetPortalApproverDetailsById(portalApprovalViewModel.PortalId));

                //return new JsonResult { Data = status ? Admin_Resources.SaveMessage : Admin_Resources.SaveErrorMessage , JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return RedirectToAction<StoreController>(x => x.GetPortalApproverDetailsById(portalApprovalViewModel.PortalId));
        }
        #endregion
        public virtual ActionResult GetPaymentApproverOrder(int portalId, int PaymentDivCount, string paymentIds)
        {
            string[] paymentIdArray = JsonConvert.DeserializeObject<string[]>(paymentIds);
            return Json(new
            {
                html = RenderRazorViewToString("_PartialPortalPaymentApproval", new PortalPaymentApproverViewModel() { PaymentTypes = _storeAgent.GetPaymentTypeList(portalId, paymentIdArray), PaymentDivCount = PaymentDivCount }),
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult EditAssociatedShipping(int shippingId, int portalId, string data)
        {
            AssociatedPortalShippingViewModel associatedPortalShippingViewModel = JsonConvert.DeserializeObject<AssociatedPortalShippingViewModel[]>(data)[0];

            bool status = _storeAgent.UpdateShippingToPortal(portalId, Convert.ToString(shippingId), associatedPortalShippingViewModel.PublishState);

            return Json(new { status = status, message = status ? Admin_Resources.UpdateMessage : Admin_Resources.ErrorAssociationShippingToPortal }, JsonRequestBehavior.AllowGet);
        }

        #region Brand Association
        //Action to get Associated store Brand list.
        public virtual ActionResult BrandList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePortalBrandList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePortalBrandList.ToString(), model);

            BrandListViewModel brandList = _brandAgent.GetPortalBrandList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, true);
            brandList.PortalId = portalId;
            brandList.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            //Get the grid model.
            brandList.GridModel = FilterHelpers.GetDynamicGridModel(model, brandList?.Brands, GridListType.ZnodePortalBrandList.ToString(), string.Empty, null, true, true, brandList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            brandList.GridModel.TotalRecordCount = brandList.TotalResults;

            return ActionView(brandList);
        }

        //Action to get UnAssociated store Brand list.
        public virtual ActionResult GetUnAssociatedBrandList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            FilterHelpers.GetDefaultView(GridListType.ZnodePortalBrandAssociatedList.ToString(), model);
            BrandListViewModel brandList = _brandAgent.GetPortalBrandList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, false);
            //Remove tool option.
            brandList?.GridModel?.FilterColumn?.ToolMenuList.Clear();
            //Get the grid model.
            brandList.GridModel = FilterHelpers.GetDynamicGridModel(model, brandList?.Brands, GridListType.ZnodePortalBrandAssociatedList.ToString(), string.Empty, null, true, true, null);
            brandList.PortalId = portalId;
            //Set the total record count
            brandList.GridModel.TotalRecordCount = brandList.TotalResults;
            return PartialView("_UnAssociatedBrandList", brandList);
        }

        // Action for associate Brands.
        public virtual JsonResult AssociatePortalBrand(string brandIds, int portalId = 0)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (IsNotNull(brandIds) && portalId > 0)
            {
                status = _brandAgent.AssociateAndUnAssociatePortalBrand(brandIds, portalId, true, out errorMessage);
                errorMessage = status ? Admin_Resources.SuccessAssociateStoreBrand : errorMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Action for unassociate Brands.
        public virtual ActionResult UnAssociatePortalBrand(string brandId, int portalId = 0)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (IsNotNull(brandId) && portalId > 0)
            {
                status = _brandAgent.AssociateAndUnAssociatePortalBrand(brandId, portalId, false, out errorMessage);
                errorMessage = status ? Admin_Resources.BrandUnassociateSuccessfully : errorMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Update associated brand Data.
        public virtual ActionResult UpdateAssociatedPortalBrandDetail(string data)
        {
            bool status = false;
            status = _brandAgent.UpdateAssociatedPortalBrandDetail(data);
            return Json(new { status = status, message = status ? PIM_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Klaviyo 
        //Get Klaviyo settings.
        [HttpGet]
        public virtual ActionResult GetKlaviyoDetails(int portalId = 0, bool isActive = false)
        {
            KlaviyoViewModel klaviyoViewModel = _klaviyoAgent.GetKlaviyo(portalId, isActive);
            klaviyoViewModel.PortalId = portalId;
            klaviyoViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            return View(klaviyoViewModel);
        }

        //Edit Klaviyo settings.
        [HttpGet]
        public virtual ActionResult EditStoreKlaviyo(int portalId = 0)
        {
            ActionResult action = GotoBackURL();
            if (IsNotNull(action))
                return action;

            //KlaviyoViewModel klaviyoViewModel = _klaviyoAgent.GetKlaviyo(portalId);
            KlaviyoViewModel klaviyoViewModel = new KlaviyoViewModel();
            klaviyoViewModel.EmailProviderList = _klaviyoAgent.GetEmailProviderList();
            if (IsNotNull(klaviyoViewModel))
            {
                klaviyoViewModel.PortalId = portalId;
                klaviyoViewModel.PortalName = _storeAgent.GetStore(portalId)?.StoreName;
            }
            else
                klaviyoViewModel = new KlaviyoViewModel() { PortalId = portalId, PortalName = _storeAgent.GetStore(portalId)?.StoreName };

            return View(AdminConstants.CreateEditKlaviyoProvider, klaviyoViewModel);
        }

        //Edit Klaviyo settings.
        [HttpPost]
        public virtual ActionResult EditStoreKlaviyo(KlaviyoViewModel klaviyoViewModel)
        {
            if (ModelState.IsValid)
            {
                KlaviyoViewModel klaviyo = _klaviyoAgent.UpdateKlaviyo(klaviyoViewModel);
                SetNotificationMessage(klaviyo.HasError || IsNull(klaviyo) ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                    : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));

                return RedirectToAction<StoreController>(x => x.EditStoreKlaviyo(klaviyoViewModel.PortalId));
            }
            return RedirectToAction<StoreController>(x => x.EditStoreKlaviyo(klaviyoViewModel.PortalId));
        }

        /// <summary>
        /// Get Provider Type View
        /// </summary>
        /// <param name="providerName">string providerName</param>
        /// <param name="providerCode">string providerCode</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult GetProviderTypeKlaviyoForm(string providerName, int portalId, string currentProviderId, bool isActive = false)
        {
            KlaviyoViewModel klaviyoViewModel = _klaviyoAgent.GetEmailSettingViewData(providerName, portalId, isActive);
            if (HelperUtility.IsNull(klaviyoViewModel))
            {
                klaviyoViewModel = new KlaviyoViewModel();
                klaviyoViewModel.EmailProviderId = Convert.ToInt32(currentProviderId);
            }

            return PartialView(AdminConstants.CreateKlaviyo, klaviyoViewModel);
        }
        #endregion
    }
       #endregion
}