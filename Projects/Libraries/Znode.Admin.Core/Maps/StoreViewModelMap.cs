using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Helpers;
using System;
namespace Znode.Engine.Admin.Maps
{
    public static class StoreViewModelMap
    {
        //Map PortalModel to StoreViewModel.
        public static StoreViewModel ToViewModel(PortalModel portalModel)
        {
            if (!Equals(portalModel, null))
            {
                StoreViewModel model = portalModel.ToViewModel<StoreViewModel>();
                model.AvailableStoreFeatureList = ToStoreFeatureListViewModel(portalModel.AvailablePortalFeatures?.ToList());
                model.SelectedStoreFeatureList = ToStoreFeatureListViewModel(portalModel.SelectedPortalFeatures?.ToList());
                model.PersistentCartEnabled = HelperUtility.IsNotNull(portalModel.AvailablePortalFeatures) ? (bool)portalModel.AvailablePortalFeatures.Where(x => x.PortalFeatureName == HelperUtility.StoreFeature.Persistent_Cart.ToString())?.FirstOrDefault()?.PortalFeatureValue : false;
                return model;

            }
            return null;
        }

        //Convert Enumerable list of theme model to select lit items.
        public static List<SelectListItem> ToThemeList(IEnumerable<ThemeModel> themeList)
        {
            List<SelectListItem> themeItems = new List<SelectListItem>();
            if (themeList?.Count() > 0)
                themeItems = (from item in themeList
                              orderby item.Name ascending
                              select new SelectListItem
                              {
                                  Text = !item.IsParentTheme ? string.Format("{0} ({1})", item.Name, item.ParentThemeName) : item.Name,
                                  Value = item.CMSThemeId.ToString()
                              }).ToList();
            return themeItems;
        }

        //Convert Enumerable list of theme model to select lit items.
        public static List<SelectListItem> ToCSSList(IEnumerable<CSSModel> cssList)
        {
            List<SelectListItem> cssItems = new List<SelectListItem>();
            if (cssList?.Count() > 0)
                cssItems = (from item in cssList
                            orderby item.CSSName ascending
                            select new SelectListItem
                            {
                                Text = item.CSSName,
                                Value = item.CMSThemeCSSId.ToString()
                            }).ToList();
            return cssItems;
        }

        //Convert Enumerable list of theme model to select lit items.
        public static List<SelectListItem> ToPublishCatalogList(IEnumerable<PublishCatalogModel> catalogList)
        {
            List<SelectListItem> catalogItems = new List<SelectListItem>();
            if (catalogList?.Count() > 0)
                catalogItems = (from item in catalogList
                                orderby item.CatalogName ascending
                                select new SelectListItem
                                {
                                    Text = item.CatalogName,
                                    Value = item.PublishCatalogId.ToString()
                                }).ToList();

            return catalogItems;
        }

        //Convert Enumerable list of Order state model to select lit items.
        public static List<SelectListItem> ToOrderStateList(IEnumerable<OrderStateModel> orderStateList, bool IsAllStatusRequired = false)
        {
            List<SelectListItem> orderStateItems = new List<SelectListItem>();
            if (orderStateList?.Count() > 0)
                orderStateItems = (from item in orderStateList
                                   orderby item.Description ascending
                                   select new SelectListItem
                                   {
                                       Text = item.Description,
                                       Value = item.OrderStateId.ToString()
                                   }).ToList();


            int index = orderStateItems.FindIndex(x => string.Equals(x.Text, AdminConstants.PendingApproval, StringComparison.CurrentCultureIgnoreCase)
                                               || string.Equals(x.Text, "pending", StringComparison.CurrentCultureIgnoreCase));
            if (index != -1 && !IsAllStatusRequired)
                orderStateItems.RemoveAt(index);
            return orderStateItems;
        }

        //Convert Enumerable list of Product review state model to select lit items.
        public static List<SelectListItem> ToProductReviewStateList(IEnumerable<ProductReviewStateModel> productReviewStateList)
        {
            List<SelectListItem> productReviewStateItems = new List<SelectListItem>();
            if (productReviewStateList?.Count() > 0)
                productReviewStateItems = (from item in productReviewStateList
                                           orderby item.ReviewStateName ascending
                                           select new SelectListItem
                                           {
                                               Text = item.ReviewStateName,
                                               Value = item.ReviewStateID.ToString()
                                           }).ToList();
            return productReviewStateItems;
        }

        //Map Portal feature model to store feature view model.
        public static StoreFeatureViewModel ToStoreFeatureViewModel(PortalFeatureModel portalFeatureModel)
        {
            if (Equals(portalFeatureModel, null))
                return null;

            return new StoreFeatureViewModel
            {
                StoreFeatureId = portalFeatureModel.PortalFeatureId,
                StoreFeatureName = portalFeatureModel.PortalFeatureName
            };
        }

        //Map list of portal feature model to store feature view model.
        public static List<StoreFeatureViewModel> ToStoreFeatureListViewModel(List<PortalFeatureModel> portalFeatureListModel)
        {
            List<StoreFeatureViewModel> storeFeature = new List<StoreFeatureViewModel>();

            if (portalFeatureListModel?.Count > 0)
                foreach (PortalFeatureModel portalFeature in portalFeatureListModel)
                    storeFeature.Add(ToStoreFeatureViewModel(portalFeature));

            return storeFeature;

        }

        public static DefaultGlobalConfigListModel ToGlobalConfigurationListModel(DefaultGlobalConfigViewModel viewModel)
        {
            DefaultGlobalConfigListModel listModel = new DefaultGlobalConfigListModel();

            if (!string.IsNullOrEmpty(viewModel?.LocaleId))
                foreach (string Id in viewModel.LocaleId.Split(','))
                    listModel.DefaultGlobalConfigs.Add(new DefaultGlobalConfigModel { LocaleId = Id, Action = viewModel.Action, PortalId = viewModel.PortalId });

            return listModel;
        }
    }
}