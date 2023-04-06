using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        #region Private Variables
        private readonly IDashboardAgent _dashboardAgent;
        private readonly IProductAgent _productAgent;
        private readonly ICategoryAgent _categoryAgent;
        #endregion

        #region Constructor
        public DashboardController(IDashboardAgent dashboardAgent, IProductAgent productAgent, ICategoryAgent categoryAgent)
        {
            _dashboardAgent = dashboardAgent;
            _productAgent = productAgent;
            _categoryAgent = categoryAgent;
        }
        #endregion

        #region Public Methods       
        public virtual ActionResult Dashboard()
        {
            DashboardViewModel dashboard = new DashboardViewModel();
            _dashboardAgent.GetAccountAndStoreList(dashboard);
            dashboard.PortalId = Convert.ToInt32(dashboard.Portal?.FirstOrDefault().Value);
            dashboard.AccountId = Convert.ToInt32(dashboard.Account?.FirstOrDefault().Value);
            dashboard.AccountName= dashboard.Account?.FirstOrDefault().Text;
            dashboard.PortalName = dashboard.Portal?.FirstOrDefault().Text;
            return View(dashboard);
        }

        public virtual ActionResult Setup() => View();

        //Gets dashboard top brands list
        public virtual async Task<ActionResult> GetDashboardTopBrands(int portalId = 0) =>
            PartialView("_dashboardTopBrands", await _dashboardAgent.GetDashboardTopBrands(portalId));

        //Gets dashboard top products list
        public virtual async Task<ActionResult> GetDashboardTopProducts(int portalId = 0) =>
            PartialView("_dashboardTopProducts", await _dashboardAgent.GetDashboardTopProducts(portalId));

        //Gets dashboard top searches list
        public virtual async Task<ActionResult> GetDashboardTopSearches(int portalId = 0) =>
            PartialView("_dashboardTopSearches", await _dashboardAgent.GetDashboardTopSearches(portalId));

        //Gets dashboard total orders, total sales, total new customers, total average orders
        public virtual async Task<ActionResult> GetDashboardSalesDetails(int portalId = 0)
        {
            DashboardListViewModel dashboardListViewModel = new DashboardListViewModel();
            if (Request.IsAjaxRequest())
            {
                dashboardListViewModel = await _dashboardAgent.GetDashboardSalesDetails(portalId);
                string salesDetailsView = RenderRazorViewToString("_dashboardSalesDetails", dashboardListViewModel);
                string topSearchView = RenderRazorViewToString("_dashboardTopSearches", dashboardListViewModel);
                string topProductsView = RenderRazorViewToString("_dashboardTopProducts", dashboardListViewModel);
                string topBrandsView = RenderRazorViewToString("_dashboardTopBrands", dashboardListViewModel);
                return Json(new { html = salesDetailsView, TopSearch = topSearchView, TopProduct = topProductsView, TopBrand = topBrandsView }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                dashboardListViewModel = await _dashboardAgent.GetDashboardSalesCountDetails(portalId);
            }

            //returns the dashboard top products list
            return PartialView("_dashboardSalesDetails", dashboardListViewModel);
        }

        //Gets dashboard top categories list
        public virtual async Task<ActionResult> GetDashboardTopCategories() =>
            PartialView("_dashboardTopCategories", await _dashboardAgent.GetDashboardTopCategories());

        //Gets dashboard low inventory product count
        public virtual async Task<ActionResult> GetDashboardLowInventoryProductCount(int portalId = 0)
        {
            DashboardListViewModel dahboardInventory = await _dashboardAgent.GetDashboardLowInventoryProductCount(portalId);
            if (Request.IsAjaxRequest())
            {
                string lowInventoryCountView = RenderRazorViewToString("_dashboardLowInventoryProductCount", dahboardInventory);
                return Json(new { html = lowInventoryCountView }, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_dashboardLowInventoryProductCount", dahboardInventory);

        }

        //Global search filter posted here.
        public virtual ActionResult GlobalSearch(string GlobalSearchType, string searchValue)
        {
            try
            {
                if (searchValue.IndexOf("'") >= 0)
                    searchValue = searchValue.Replace("'", "''");

                switch (GlobalSearchType)
                {
                    case DynamicGridConstants.ProductFilter:
                        SetGlobalSearchFilterForProduct(GridListType.View_ManageProductList, searchValue);
                        return RedirectToAction("List", "Products", new { Area = "PIM" });
                    case DynamicGridConstants.OrderFilter:
                        SetGlobalSearchFilter(GridListType.ZnodeOrder, searchValue);
                        return RedirectToAction("List", "Order");
                    case DynamicGridConstants.CatalogFilter:
                        SetGlobalSearchFilter(GridListType.ZnodePimCatalog, searchValue);
                        return RedirectToAction("CatalogList", "Catalog", new { Area = "PIM" });
                    case DynamicGridConstants.CategoryFilter:
                        SetGlobalSearchFilterForCategory(GridListType.View_PimCategoryDetail, searchValue);
                        return RedirectToAction("List", "Category", new { Area = "PIM" });
                    case DynamicGridConstants.UserFilter:
                        SetGlobalSearchFilter(GridListType.ZnodeCustomerAccount, searchValue);
                        return RedirectToAction("CustomersList", "Customer");
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return RedirectToAction<DashboardController>(x => x.Dashboard());
            }
            return RedirectToAction<DashboardController>(x => x.Dashboard());
        }


        public virtual async Task<ActionResult> GetDashboardQuotes(int portalId = 0, int accountId = 0) =>
            PartialView("_dashboardQuotes", await _dashboardAgent.GetDashboardQuotes(portalId, accountId));

        public virtual async Task<ActionResult> GetDashboardOrders(int portalId = 0, int accountId = 0) =>
            PartialView("_dashboardOrders", await _dashboardAgent.GetDashboardOrders(portalId, accountId));

        public virtual async Task<ActionResult> GetDashboardReturns(int portalId = 0, int accountId = 0) =>
            PartialView("_dashboardReturns", await _dashboardAgent.GetDashboardReturns(portalId, accountId));

        public virtual async Task<ActionResult> GetDashboardSaleDetails(int portalId = 0, int accountId = 0) =>
            PartialView("_dashboardSaleDetails", await _dashboardAgent.GetDashboardSaleDetails(portalId, accountId));
        public virtual async Task<ActionResult> GetDashboardTopAccounts(int portalId = 0, int accountId = 0) =>
            PartialView("_dashboardTopAccounts", await _dashboardAgent.GetDashboardTopAccounts(portalId, accountId));

        //Gets dashboard total orders, total sales, count of quotes, count of returns
        public virtual async Task<ActionResult> GetDashboardDetails(int portalId = 0, int accountId = 0)
        {
            DashboardItemsListViewModel dashboardListViewModel = await _dashboardAgent.GetDashboardDetails(portalId, accountId);

            string quotesDetailsView = RenderRazorViewToString("_dashboardQuotes", dashboardListViewModel);
            string ordersView = RenderRazorViewToString("_dashboardOrders", dashboardListViewModel);
            string returnsView = RenderRazorViewToString("_dashboardReturns", dashboardListViewModel);
            string topAccountView = RenderRazorViewToString("_dashboardTopAccounts", dashboardListViewModel);
            string salesDetailsView = RenderRazorViewToString("_dashboardSaleDetails", dashboardListViewModel);
            if (Request.IsAjaxRequest())
            {
                return Json(new { quotes = quotesDetailsView, orders = ordersView, returns = returnsView, topaccounts = topAccountView, sales = salesDetailsView }, JsonRequestBehavior.AllowGet);
            }

            //returns the dashboard top products list
            return PartialView("_dashboardSaleDetails", dashboardListViewModel);
        }
        #endregion

        #region Private Methods

        //Set the global filter.
        private void SetGlobalSearchFilter(GridListType listName, string searchValue)
        {
            if (IsSearchValueEmpty(searchValue))
                return;
            var filterColumnList = FilterHelpers.GetDynamicGridModel(new FilterCollectionDataModel(), new List<Object>(), listName.ToString(), string.Empty, null, true, true, null);
            SetFilter(searchValue, filterColumnList.FilterColumn);
        }

        //Set the global filter for product.
        private void SetGlobalSearchFilterForProduct(GridListType listName, string searchValue)
        {
            if (IsSearchValueEmpty(searchValue))
                return;

            ProductDetailsListViewModel productList = _productAgent.GetProductList(new FilterCollection(), null, 1, HelperMethods.GridPagingStartValue);
            //Get the grid model.
            productList.GridModel = FilterHelpers.GetDynamicGridModel(new FilterCollectionDataModel(), IsNull(productList?.XmlDataList) ? new List<dynamic>() : productList.XmlDataList, GridListType.View_ManageProductList.ToString(), string.Empty, null, true, true, productList?.GridModel?.FilterColumn?.ToolMenuList, AttrColumn(productList.AttrubuteColumnName));

            SetFilter(searchValue, productList.GridModel.FilterColumn);
        }

        //Set the global filter for category.
        private void SetGlobalSearchFilterForCategory(GridListType listName, string searchValue)
        {
            if (IsSearchValueEmpty(searchValue))
                return;

            CategoryListViewModel categoryList = _categoryAgent.GetCategoryList(null, null, null, null, 2);

            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(new FilterCollectionDataModel(), IsNull(categoryList?.XmlDataList) ? new List<dynamic>() : categoryList.XmlDataList, GridListType.View_PimCategoryDetail.ToString(), string.Empty, null, true, true, categoryList?.GridModel?.FilterColumn?.ToolMenuList, AttrColumn(categoryList.AttrubuteColumnName));

            SetFilter(searchValue, categoryList.GridModel.FilterColumn);
        }

        //Set the global filter value in temp data.
        private void SetFilter(string searchValue, Models.FilterColumnListModel filterColumnList)
        {
            List<string> _searchableColumn = filterColumnList?.FilterColumnList?.FindAll(x => x.IsAllowSearch == "y" & string.Equals(x.DataType, "string", StringComparison.InvariantCultureIgnoreCase))?.Select(x => x.ColumnName)?.ToList();
            _searchableColumn?.RemoveAll(x => x.Contains("|"));

            if (_searchableColumn?.Count() > 0)
            {
                string searchableColumnName = _searchableColumn.Count() == 1 ? string.Join("|", _searchableColumn.ToArray()) + "|" : string.Join("|", _searchableColumn.ToArray());

                FilterTuple tuple = new FilterTuple(searchableColumnName, FilterOperators.Contains, searchValue);
                TempData[DynamicGridConstants.GlobalSearchFilter] = tuple;
            }
        }

        //Return true if search value is empty or null.
        private bool IsSearchValueEmpty(string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                TempData[DynamicGridConstants.GlobalSearchFilter] = null;
                return true;
            }
            return false;
        }

        #endregion
    }
}