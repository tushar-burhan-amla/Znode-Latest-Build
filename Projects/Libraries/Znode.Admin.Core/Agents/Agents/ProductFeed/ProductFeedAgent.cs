using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class ProductFeedAgent : BaseAgent, IProductFeedAgent
    {
        #region Private Variables
        private readonly IStoreAgent _storeAgent;
        private readonly IProductFeedClient _productFeedClient;
        private readonly ILocaleAgent _localeAgent;
        #endregion

        #region Constructor

        public ProductFeedAgent(IProductFeedClient productFeedClient)
        {
            _storeAgent = new StoreAgent(GetClient<PortalClient>(), GetClient<EcommerceCatalogClient>(), GetClient<ThemeClient>(), GetClient<DomainClient>(), GetClient<PriceClient>(), GetClient<OrderStateClient>(),
                GetClient<ProductReviewStateClient>(), GetClient<PortalProfileClient>(), GetClient<WarehouseClient>(), GetClient<CSSClient>(), GetClient<ManageMessageClient>(), GetClient<ContentPageClient>(), GetClient<TaxClassClient>(),
                GetClient<PaymentClient>(), GetClient<ShippingClient>(), GetClient<PortalCountryClient>(), GetClient<TagManagerClient>(), GetClient<GeneralSettingClient>());
            _productFeedClient = GetClient<IProductFeedClient>(productFeedClient);
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
        }

        #endregion

        #region Product Feed

        //Get product feed master data.
        public virtual ProductFeedViewModel GetProductFeedMasterDetails()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ProductFeedViewModel viewModel = new ProductFeedViewModel();
            ProductFeedModel model = _productFeedClient.GetProductFeedMasterDetails();
            if (HelperUtility.IsNotNull(model))
            {
                viewModel = model.ToViewModel<ProductFeedViewModel>();
                BindProductFeedDropDown(model, viewModel);
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //This method will generate the Product feed in xml format.
        public virtual ProductFeedViewModel CreateProductFeed(ProductFeedViewModel model, string domainName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                model.SuccessXMLGenerationMessage = domainName;
                ProductFeedModel feedModel = _productFeedClient.CreateProductFeed(ProductFeedViewModelMap.ToModel(model));
                return feedModel?.ToViewModel<ProductFeedViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (ProductFeedViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ProductFeedGenerationErrorMessage);
            }
        }

        //Check whether the File Name already exists.
        public virtual bool CheckFileNameExist(string fileName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeProductFeedEnum.FileName.ToString(), FilterOperators.Is, fileName.Trim()));
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters,expands and sorts:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { filters });
            //Get the File Name List based on the File name filter.
            ProductFeedListModel productFeedList = _productFeedClient.GetProductFeedList(null, filters, null, null, null);
            if (IsNotNull(productFeedList) && IsNotNull(productFeedList.ProductFeeds))
            {
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return productFeedList.ProductFeeds.FindIndex(x => x.FileName == fileName) != -1;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }


        //Delete product feed and Sitemap url from Robots.txt.
        public virtual bool DeleteProductFeed(string productFeedId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                return _productFeedClient.DeleteProductFeed(new ParameterModel { Ids = productFeedId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Update the product feed.
        public virtual ProductFeedViewModel UpdateProductFeed(ProductFeedViewModel productFeedViewModel, string domainName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                productFeedViewModel.SuccessXMLGenerationMessage = domainName;

                ProductFeedModel feedModel = _productFeedClient.UpdateProductFeed(ProductFeedViewModelMap.ToModel(productFeedViewModel));
                return feedModel?.ToViewModel<ProductFeedViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (ProductFeedViewModel)GetViewModelWithErrorMessage(productFeedViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Get product feed list.
        public virtual ProductFeedListViewModel GetProductFeedList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters,expands and sorts:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { filters, expands, sorts });
            ProductFeedListModel productFeedList = _productFeedClient.GetProductFeedList(expands, filters, sorts, pageIndex, pageSize);

            ProductFeedListViewModel listViewModel = new ProductFeedListViewModel { ProductFeeds = productFeedList?.ProductFeeds?.ToViewModel<ProductFeedViewModel>()?.ToList() };

            SetListPagingData(listViewModel, productFeedList);

            listViewModel.Locale = _localeAgent.GetLocalesList();
            //Set tool option menus for product feed grid.
            SetProductFeedListToolMenu(listViewModel);

            if (listViewModel?.ProductFeeds?.Count > 0)
            {

                ZnodeLogging.LogMessage("ProductFeeds list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, listViewModel?.ProductFeeds?.Count());
                foreach (ProductFeedViewModel item in listViewModel.ProductFeeds)
                {
                    item.ConnectorTouchPoints = "ProductFeed_" + item.ProductFeedId + "_" + item.FileName;
                    item.SchedulerCallFor = ZnodeConstant.ProductFeed;
                }
            }

            return listViewModel?.ProductFeeds?.Count > 0 ? listViewModel : new ProductFeedListViewModel() { ProductFeeds = new List<ProductFeedViewModel>(), Locale = _localeAgent.GetLocalesList() };
        }

        //Get product feed by Id.
        public virtual ProductFeedViewModel GetProductFeedById(int productFeedId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ProductFeedModel productFeedModel = _productFeedClient.GetProductFeed(productFeedId, SetExpand());
            if (HelperUtility.IsNotNull(productFeedModel))
            {
                ProductFeedViewModel viewModel = SetProductFeedDetails(productFeedModel.ToViewModel<ProductFeedViewModel>());
                return viewModel;
            }
            return null;
        }

        //Set product feed data with existing product feed details.
        public virtual ProductFeedViewModel SetProductFeedDetails(ProductFeedViewModel productFeedViewModel)
        => MapProductFeedDropdownList(productFeedViewModel, GetProductFeedMasterDetails());

        //Generate product feed link.
        public virtual ProductFeedViewModel GenerateProductFeedLink(int productFeedId, string domainName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ProductFeedModel productFeedModel = _productFeedClient.GetProductFeed(productFeedId, SetExpand());
            if (HelperUtility.IsNotNull(productFeedModel))
            {
                productFeedModel.SuccessXMLGenerationMessage = domainName;
                productFeedModel.ModifiedDate = DateTime.Now;
                ProductFeedModel feedModel = _productFeedClient.UpdateProductFeed(productFeedModel);
                return feedModel?.ToViewModel<ProductFeedViewModel>();
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }

        //Check if the file name combination already exists.
        public virtual bool FileNameCombinationAlreadyExist(int localeId, string fileName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                return _productFeedClient.FileNameCombinationAlreadyExist(localeId, fileName);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return false;
            }
        }
            #endregion

            #region Private Methods
            //This method  will return the frequency list for which feed has to be genrated.
            private List<SelectListItem> GetFrequencyList() => ProductFeedViewModelMap.ToFrequencyListItems();

        //This method will return the Date modified list for which feed has to be generated.

        //This method will return the list for the XML Priority to be set in the generated feed.

        //This method will return the list of type of the feed to be generated
        private List<SelectListItem> GetXMLSiteMapList(List<ProductFeedTypeModel> ProductFeedTypeList) => ProductFeedViewModelMap.ToXMLSiteMapListItems(ProductFeedTypeList);

        //This method will return the list for which feed is to be generated
        private List<SelectListItem> GetXMLSiteMapTypeList(List<ProductFeedSiteMapTypeModel> productFeedSiteMapTypeList) => ProductFeedViewModelMap.ToXMLSiteMapTypeListItems(productFeedSiteMapTypeList);

        //Bind product feed master data in dropdown.
        private void BindProductFeedDropDown(ProductFeedModel model, ProductFeedViewModel viewModel)
        {
            viewModel.XMLSiteMapList = GetXMLSiteMapList(model.ProductFeedTypeList);
            viewModel.XMLSiteMapTypeList = GetXMLSiteMapTypeList(model.ProductFeedSiteMapTypeList);
            viewModel.StoreList = _storeAgent.GetStoreList();
            viewModel.Locale = _localeAgent.GetLocalesList();
        }

        //Set tool option menus for product feed grid.
        private void SetProductFeedListToolMenu(ProductFeedListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ProductFeedDeletePopup')", ControllerName = "ProductFeed", ActionName = "Delete" });
            }
        }

        //Set Expand For product feed.
        private static ExpandCollection SetExpand()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeProductFeedEnum.ZnodeProductFeedSiteMapType.ToString());
            expands.Add(ZnodeProductFeedEnum.ZnodeProductFeedType.ToString());
            expands.Add(ZnodeProductFeedEnum.ZnodePortal.ToString());
            return expands;
        }

        //Map product feed dropdown list.
        private static ProductFeedViewModel MapProductFeedDropdownList(ProductFeedViewModel productFeedViewModel, ProductFeedViewModel viewModel)
        {
            if (HelperUtility.IsNotNull(productFeedViewModel) && HelperUtility.IsNotNull(viewModel))
            {
                productFeedViewModel.XMLSiteMapList = viewModel.XMLSiteMapList;
                productFeedViewModel.XMLSiteMapTypeList = viewModel.XMLSiteMapTypeList;
                productFeedViewModel.StoreList = viewModel.StoreList;
                productFeedViewModel.Locale = viewModel.Locale;
            }
            return productFeedViewModel;
        }
        #endregion
    }
}