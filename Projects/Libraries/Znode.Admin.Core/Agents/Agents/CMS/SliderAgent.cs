using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class SliderAgent : BaseAgent, ISliderAgent
    {
        #region Private Variables
        private readonly ISliderClient _sliderClient;
        private readonly ILocaleAgent _localeAgent;
        #endregion

        #region Constructor
        public SliderAgent(ISliderClient sliderClient)
        {
            _sliderClient = GetClient<ISliderClient>(sliderClient);
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
        }
        #endregion

        #region public virtual Methods
        #region Slider
        //Get the list slider.
        public virtual SliderListViewModel GetSliders(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {     
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sorts: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            SliderListModel sliderList = _sliderClient.GetSliders(filters, sorts, pageIndex, pageSize);
            SliderListViewModel listViewModel = new SliderListViewModel { Sliders = sliderList?.Sliders?.ToViewModel<SliderViewModel>().ToList() };

            SetListPagingData(listViewModel, sliderList);

            //Set tool option menus for slider grid.
            SetSliderListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return sliderList?.Sliders?.Count > 0 ? listViewModel : new SliderListViewModel() { Sliders = new List<SliderViewModel>() };
        }
        //Create slider.
        public virtual SliderViewModel CreateSlider(SliderViewModel sliderViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _sliderClient.CreateSlider(sliderViewModel?.ToModel<SliderModel>())?.ToViewModel<SliderViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (SliderViewModel)GetViewModelWithErrorMessage(sliderViewModel, Admin_Resources.AlreadyExistSliderName);
                    default:
                        return (SliderViewModel)GetViewModelWithErrorMessage(sliderViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (SliderViewModel)GetViewModelWithErrorMessage(sliderViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get slider by cms slider Id
        public virtual SliderViewModel GetSlider(int cmsSliderId) => _sliderClient.GetSlider(cmsSliderId).ToViewModel<SliderViewModel>();

        //Update slider.
        public virtual SliderViewModel UpdateSlider(int cmsSliderId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            SliderViewModel sliderViewModel = JsonConvert.DeserializeObject<SliderViewModel[]>(data)[0];
            try
            {
                return _sliderClient.UpdateSlider(sliderViewModel?.ToModel<SliderModel>())?.ToViewModel<SliderViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (SliderViewModel)GetViewModelWithErrorMessage(sliderViewModel, Admin_Resources.AlreadyExistSliderName);
                    default:
                        return (SliderViewModel)GetViewModelWithErrorMessage(sliderViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (SliderViewModel)GetViewModelWithErrorMessage(sliderViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete slider.
        public virtual bool DeleteSlider(string cmsPortalSliderId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;

            try
            {
                return _sliderClient.DeleteSlider(new ParameterModel { Ids = cmsPortalSliderId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorDeleteSlider;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Publish slider.
        [Obsolete("To be discontinued in one of the upcoming versions.")]
        public virtual bool PublishSlider(string cmsPortalSliderId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = PIM_Resources.ErrorPublished;
            try
            {
                return Convert.ToBoolean(_sliderClient.PublishSlider(new ParameterModel { Ids = cmsPortalSliderId })?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = ex.Message;
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Publish slider.
        public virtual bool PublishSlider(string cmsPortalSliderId, int portalId, int localeId, out string errorMessage, string targetPublishSlider = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = PIM_Resources.ErrorPublishedFailed;
            try
            {
                return Convert.ToBoolean(_sliderClient.PublishSlider(new ParameterModel { Ids = cmsPortalSliderId, PortalId = portalId, LocaleId = localeId, TargetPublishState = targetPublishSlider, TakeFromDraftFirst = takeFromDraftFirst })?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = ex.Message;
                        return false;
                    case ErrorCodes.StoreNotPublishedForAssociatedEntity:
                        errorMessage = ex.Message;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorPublished;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Check whether the Slider Name already exists.
        public virtual bool CheckSliderNameExist(string sliderName, int cmsSliderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { sliderName = sliderName, cmsSliderId = cmsSliderId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSSliderEnum.Name.ToString(), FilterOperators.Is, sliderName.Trim()));
            ZnodeLogging.LogMessage("Input parameters of method GetSliders: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters});
            //Get the Slider List based on the slider name filter.
            SliderListModel sliderList = _sliderClient.GetSliders(filters, null, null, null);
            if (IsNotNull(sliderList) && IsNotNull(sliderList.Sliders))
            {
                if (cmsSliderId > 0)
                {
                    //Set the status in case the Slider is open in edit mode.
                    SliderModel slider = sliderList.Sliders.Find(x => x.CMSSliderId == cmsSliderId);
                    if (IsNotNull(slider))
                        return !Equals(slider.Name.Trim(), sliderName.Trim());
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return sliderList.Sliders.FindIndex(x => x.Name == sliderName) != -1;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return false;
        }
        #endregion

        #region Banner
        //Get Slider Banner list.
        public virtual BannerListViewModel GetBannerList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            BannerListModel bannerList = _sliderClient.GetBannerList(null, filters, sorts, pageIndex, recordPerPage);
            BannerListViewModel bannerListViewModel = new BannerListViewModel { Banners = bannerList?.Banners?.ToViewModel<BannerViewModel>().ToList() };
            SliderModel sliderModel = _sliderClient.GetSlider(Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == ZnodeCMSSliderEnum.CMSSliderId.ToString()).FilterValue));
            bannerListViewModel.IsWidgetAssociated = sliderModel.IsWidgetAssociated;
            bannerListViewModel.BannersCreated = (IsNotNull(bannerList.Banners) && bannerList.Banners.Count > 0) ? true : false;
            bannerListViewModel.Name = sliderModel?.Name;
            SetListPagingData(bannerListViewModel, bannerList);

            //Set tool option menus for banner grid.
            SetBannerListToolMenu(bannerListViewModel);
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return bannerList?.Banners?.Count > 0 ? bannerListViewModel : new BannerListViewModel() { Banners = new List<BannerViewModel>(), Name = bannerListViewModel?.Name };
        }

        //Create banner.
        public virtual BannerViewModel CreateBanner(BannerViewModel bannerViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _sliderClient.CreateBanner(bannerViewModel?.ToModel<BannerModel>())?.ToViewModel<BannerViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (BannerViewModel)GetViewModelWithErrorMessage(bannerViewModel, Admin_Resources.AlreadyExistBannerTitle);
                    default:
                        return (BannerViewModel)GetViewModelWithErrorMessage(bannerViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (BannerViewModel)GetViewModelWithErrorMessage(bannerViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get banner by cms slider banner Id
        public virtual BannerViewModel GetBanner(int cmsSliderBannerId, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsSliderBannerId = cmsSliderBannerId, localeId = localeId });
            if (cmsSliderBannerId > 0)
            {
                localeId = localeId == 0 ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : localeId;
                BannerViewModel bannerViewModel = _sliderClient.GetBanner(cmsSliderBannerId, new FilterCollection() { new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()) }).ToViewModel<BannerViewModel>();
                if (IsNotNull(bannerViewModel))
                {
                    bannerViewModel.Locales = _localeAgent.GetLocalesList(localeId);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return bannerViewModel;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new BannerViewModel { HasError = true };
        }

        //Update banner.
        public virtual BannerViewModel UpdateBanner(BannerViewModel bannerViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _sliderClient.UpdateBanner(bannerViewModel?.ToModel<BannerModel>())?.ToViewModel<BannerViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (BannerViewModel)GetViewModelWithErrorMessage(bannerViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Inline edit and update the banner sequence for respective banner
        public virtual BannerViewModel UpdateBannerSequence(int cmsSliderBannerId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            BannerViewModel bannerSequenceViewModel = new BannerViewModel();
            try
            {
                bannerSequenceViewModel = JsonConvert.DeserializeObject<BannerViewModel[]>(data)[0];
                BannerModel bannerModel = _sliderClient.GetBanner(cmsSliderBannerId, new FilterCollection() { new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, DefaultSettingHelper.DefaultLocale) });
                if (IsNotNull(bannerModel))
                {
                    if (bannerModel.BannerSequence != bannerSequenceViewModel.BannerSequence)
                    {
                        bannerModel.BannerSequence = bannerSequenceViewModel?.BannerSequence;
                        ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                        return _sliderClient.UpdateBanner(bannerModel)?.ToViewModel<BannerViewModel>(); 
                    }
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return bannerSequenceViewModel;
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return new BannerViewModel { HasError = true };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (BannerViewModel)GetViewModelWithErrorMessage(bannerSequenceViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete banner.
        public virtual bool DeleteBanner(string cmsSliderBannerId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _sliderClient.DeleteBanner(new ParameterModel { Ids = cmsSliderBannerId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Check whether the Banner Name already exists.
        public virtual bool CheckBannerNameExist(string bannerName, int cmsSliderBannerId, int cmsSliderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { bannerName = bannerName, cmsSliderBannerId = cmsSliderBannerId, cmsSliderId = cmsSliderId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSSliderBannerLocaleEnum.Title.ToString(), FilterOperators.Is, bannerName.Replace("'", "''")));
            filters.Add(new FilterTuple(ZnodeCMSSliderEnum.CMSSliderId.ToString(), FilterOperators.Equals, cmsSliderId.ToString()));
            ZnodeLogging.LogMessage("Input parameters of method GetBannerList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters});
            //Get the Banner List based on the Banner name filter.
            BannerListModel bannerList = _sliderClient.GetBannerList(null, filters, null, null, null);
            if (IsNotNull(bannerList) && IsNotNull(bannerList.Banners))
            {
                if (cmsSliderBannerId > 0)
                {
                    //Set the status in case the Banner is open in edit mode.
                    BannerModel banner = bannerList.Banners.Find(x => x.CMSSliderBannerId == cmsSliderBannerId);
                    if (IsNotNull(banner))
                        return !Equals(banner.Title, bannerName);
                }
                return bannerList.Banners.FindIndex(x => x.Title == bannerName) != -1;
            }
            return false;
        }
        #endregion

        #endregion

        #region Private Methods.
        //Set tool option menus for slider grid.
        private void SetSliderListToolMenu(SliderListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('SliderDeletePopup')", ControllerName = "WebSite", ActionName = "DeleteSlider" });
            }
        }

        //Set tool option menus for banner grid.
        private void SetBannerListToolMenu(BannerListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('BannerDeletePopup')", ControllerName = "WebSite", ActionName = "DeleteBanner" });
            }
        }
        #endregion
    }
}