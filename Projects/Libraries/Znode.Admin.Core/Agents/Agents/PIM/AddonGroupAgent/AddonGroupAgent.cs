using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
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
    public class AddonGroupAgent : BaseAgent, IAddonGroupAgent
    {
        #region Private Variables

        private readonly IAddonGroupClient _addonGroupClient;

        #endregion Private Variables

        public AddonGroupAgent(IAddonGroupClient addonGroupClient)
        {
            _addonGroupClient = GetClient<IAddonGroupClient>(addonGroupClient);
        }

        //Get addon group by addonGroupId.
        public virtual AddonGroupViewModel GetAddonGroup(int addonGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (addonGroupId > 0)
            {
                ExpandCollection expands = new ExpandCollection() { ZnodePimAddonGroupEnum.ZnodePimAddonGroupLocales.ToString() };
                FilterCollection filters = new FilterCollection() { new FilterTuple(ZnodePimAddonGroupEnum.PimAddonGroupId.ToString(), FilterOperators.Equals, addonGroupId.ToString()),
                                                                    new FilterTuple(ZnodePimAddonGroupLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValueForAddon().ToString())};

                AddonGroupViewModel addonGroup = _addonGroupClient.GetAddonGroup(filters, expands).ToViewModel<AddonGroupViewModel>();
                addonGroup.DisplayTypeValue = (AddonType)Enum.Parse(typeof(AddonType), addonGroup.DisplayType);
                addonGroup.Locale = LocaleModelMap.ToLocaleListItem(DefaultSettingHelper.GetActiveLocaleList());
                SetLocaleValueForDropdown(addonGroup.Locale, GetLocaleValueForAddon().ToString());
                ZnodeLogging.LogMessage("Output Parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info,new { addonGroup = addonGroup });
                return addonGroup;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return new AddonGroupViewModel();
        }

        //Create addon group.
        public virtual AddonGroupViewModel CreateAddonGroup(AddonGroupViewModel addonGroup)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (!Equals(addonGroup, null))
            {
                addonGroup.PimAddonGroupLocales[0].LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

                addonGroup.DisplayType = addonGroup.DisplayTypeValue.ToString();
                try
                {
                    addonGroup = _addonGroupClient.CreateAddonGroup(addonGroup.ToModel<AddonGroupModel>()).ToViewModel<AddonGroupViewModel>();
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    if (Equals(ex.ErrorCode, ErrorCodes.AlreadyExist))
                        addonGroup.ErrorMessage = PIM_Resources.ErrorAddonGroupExists;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    return (AddonGroupViewModel)GetViewModelWithErrorMessage(addonGroup, PIM_Resources.ErrorFailedToCreate);
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return addonGroup;
        }

        //Update addon group.
        public virtual AddonGroupViewModel UpdateAddonGroup(AddonGroupViewModel addonGroup)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNotNull(addonGroup))
            {
                addonGroup.DisplayType = addonGroup.DisplayTypeValue.ToString();
                addonGroup.PimAddonGroupLocales[0].LocaleId = Convert.ToInt32(GetLocaleValueForAddon());
                addonGroup.PimAddonGroupLocales[0].PimAddonGroupId = addonGroup.PimAddonGroupId;
                try
                {
                    return _addonGroupClient.UpdateAddonGroup(addonGroup.ToModel<AddonGroupModel>()).ToViewModel<AddonGroupViewModel>();
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    addonGroup.HasError = true;
                    if (Equals(ex.ErrorCode, ErrorCodes.AlreadyExist))
                        addonGroup.ErrorMessage = PIM_Resources.ErrorAddonGroupExists;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    return (AddonGroupViewModel)GetViewModelWithErrorMessage(addonGroup, PIM_Resources.UpdateErrorMessage);
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonGroup;
        }

        //Get addon group list.
        public virtual AddonGroupListViewModel GetAddonGroupList(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            AddonGroupListModel addonGroups = new AddonGroupListModel();

            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            if (filters.Exists(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString());
            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValueForAddon().ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sortCollection });

            addonGroups = _addonGroupClient.GetAddonGroupList(expands, filters, sortCollection, pageIndex, pageSize);

            AddonGroupListViewModel viewModel = new AddonGroupListViewModel
            {
                AddonGroups = addonGroups.AddonGroups.ToViewModel<AddonGroupViewModel>().ToList(),
                Locale = LocaleModelMap.ToLocaleListItem(DefaultSettingHelper.GetActiveLocaleList())
            };
            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(viewModel.Locale, GetLocaleValueForAddon().ToString());

            viewModel.AddonGroups.ForEach(addonGroup => addonGroup.DisplayType = EnumHelper<AddonType>.GetDisplayValue((AddonType)Enum.Parse(typeof(AddonType), addonGroup.DisplayType)));

            SetListPagingData(viewModel, addonGroups);

            //Set tool menu for Addon Group list grid view.
            SetAddonGroupListToolMenu(viewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonGroups.AddonGroups.Count > 0 ? viewModel : new AddonGroupListViewModel { AddonGroups = new List<AddonGroupViewModel>(), Locale = viewModel.Locale };
        }

        //methos to set locale value save in cookies for addon group.
        public virtual int GetLocaleValueForAddon()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_addOnCulture"))
            {
                string cookieValue = CookieHelper.GetCookieValue<string>("_addOnCulture");
                localeId = string.IsNullOrEmpty(cookieValue) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : Convert.ToInt32(cookieValue);
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_addOnCulture", Convert.ToString(localeId));
            }
            ZnodeLogging.LogMessage("Output Parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,new { localeId = localeId });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return localeId;
        }

        //Method to delete addon group by addonGroupId.
        public virtual bool DeleteAddonGroup(string addonGroupId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            errorMessage = PIM_Resources.ErrorFailedAddonGroupDelete;
            try
            {
                if (!string.IsNullOrEmpty(addonGroupId))
                    return _addonGroupClient.DeleteAddonGroup(new ParameterModel { Ids = addonGroupId });
            }
            catch (ZnodeException znodeException)
            {
                ZnodeLogging.LogMessage(znodeException, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (znodeException.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = PIM_Resources.ErrorDeleteAddonGroup;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                errorMessage = PIM_Resources.ErrorDeleteAddonGroup;
            }
            return false;
        }

        //Associate products to addon groups.
        public virtual bool AssociateAddonGroupProduct(int addonGroupId, string associatedProductIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            AddonGroupProductListModel addonGroupProductList = new AddonGroupProductListModel();
            addonGroupProductList.AddonGroupProducts = new List<AddonGroupProductModel>();
            int[] associatedProductIdsArray = associatedProductIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            foreach (int associatedProductId in associatedProductIdsArray)
            {
                AddonGroupProductModel addonGroupProductModel = new AddonGroupProductModel() { PimAddonGroupId = addonGroupId, PimChildProductId = associatedProductId };
                addonGroupProductList.AddonGroupProducts.Add(addonGroupProductModel);
            }

            if (addonGroupProductList.AddonGroupProducts?.Count() > 0)
                return _addonGroupClient.AssociateAddonGroupProduct(addonGroupProductList);
            return false;
        }

        public virtual ProductDetailsListViewModel GetAssociatedAddonGroupProduct(int addonGroupId, int localeId, SortCollection sortCollection = null, ExpandCollection expands = null, FilterCollection filters = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ProductDetailsListModel addonGroupProducts = new ProductDetailsListModel();

            //Add locale ID filter.
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection() { new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()) };
            else
            {
                filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId);
                filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));
            }
            filters.RemoveAll(x => x.FilterName == ZnodeConstant.IsCallForAttribute);
            filters.Add(new FilterTuple(ZnodeConstant.IsCallForAttribute, FilterOperators.Equals, ZnodeConstant.TrueValue));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sortCollection });

            addonGroupProducts = _addonGroupClient.GetAssociatedAddonGroupProductAssociation(addonGroupId, expands, filters, sortCollection, pageIndex, pageSize);
            addonGroupProducts?.AttributeColumnName?.Remove(AdminConstants.ProductImage);
            ProductDetailsListViewModel viewModel = new ProductDetailsListViewModel
            {
                ProductDetailList = addonGroupProducts.ProductDetailList.ToViewModel<ProductDetailsViewModel>().ToList(),
                Locale = PIMAttributeFamilyViewModelMap.ToLocaleListItem(addonGroupProducts.Locale),
                AttrubuteColumnName = addonGroupProducts?.AttributeColumnName,
                XmlDataList = addonGroupProducts?.XmlDataList ?? new List<dynamic>()
            };

            SetListPagingData(viewModel, addonGroupProducts);

            SetAddonGroupProductListToolMenu(viewModel);

            return viewModel;
        }

        public virtual ProductDetailsListViewModel GetUnassociatedAddonGroupProduct(int addonGroupId, int localeId, SortCollection sortCollection = null, ExpandCollection expands = null, FilterCollection filters = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ProductDetailsListModel unassociatedProducts = new ProductDetailsListModel();

            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));
            filters.RemoveAll(x => x.FilterName == ZnodeConstant.IsCallForAttribute);
            filters.Add(new FilterTuple(ZnodeConstant.IsCallForAttribute, FilterOperators.Equals, ZnodeConstant.TrueValue));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters= filters, sorts = sortCollection });
            //Set display order sort if not present.
            SetDisplayOrderSortIfNotPresent(ref sortCollection);

            unassociatedProducts = _addonGroupClient.GetUnassociatedAddonGroupProductAssociation(addonGroupId, expands, filters, sortCollection, pageIndex, pageSize);

            ProductDetailsListViewModel viewModel = new ProductDetailsListViewModel
            {
                ProductDetailList = unassociatedProducts.ProductDetailList.ToViewModel<ProductDetailsViewModel>().ToList(),
                Locale = PIMAttributeFamilyViewModelMap.ToLocaleListItem(unassociatedProducts.Locale),
                AttrubuteColumnName = unassociatedProducts?.AttributeColumnName,
                XmlDataList = unassociatedProducts?.XmlDataList ?? new List<dynamic>()
            };

            SetListPagingData(viewModel, unassociatedProducts);

            SetAddonGroupProductListToolMenu(viewModel);

            return viewModel;
        }

        public virtual bool DeleteAddonGroupProducts(string addonGroupProductIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            errorMessage = "Failed to delete products associated to Addon group.";
            try
            {
                if (!string.IsNullOrEmpty(addonGroupProductIds))
                    return _addonGroupClient.DeleteAddonGroupProductAssociation(new ParameterModel { Ids = addonGroupProductIds });
            }
            catch (ZnodeException znodeException)
            {
                ZnodeLogging.LogMessage(znodeException, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (znodeException.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = PIM_Resources.ErrorDeleteAddonGroup;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                errorMessage = PIM_Resources.ErrorDeleteAddonGroup;
            }
            return false;
        }

        #region Private Method

        //Set tool menu for Addon Group list grid view.
        private void SetAddonGroupListToolMenu(AddonGroupListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AddonGroupDeletePopup',this)", ControllerName = "AddonGroup", ActionName = "DeleteAddonGroup" });
            }
        }

        //Set tool menu for Addon Group product list grid view.
        private void SetAddonGroupProductListToolMenu(ProductDetailsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AddonGroupProductDeletePopup',this)", ControllerName = "AddonGroup", ActionName = "DeleteAddonGroupProducts" });
            }
        }

        #endregion Private Method
    }
}