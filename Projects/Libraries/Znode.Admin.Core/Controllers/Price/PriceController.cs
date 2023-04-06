using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using MvcSiteMapProvider;

namespace Znode.Engine.Admin.Controllers
{
    public class PriceController : BaseController
    {
        #region Private Variables
        private readonly IPriceAgent _priceAgent;
        private readonly IProductAgent _productAgent;
        private readonly IImportAgent _importAgent;
        private readonly string _CurrencyView = "_Currency";
        private readonly string _CultureView = "_Culture";
        #endregion

        #region Public Constructor
        public PriceController(IPriceAgent priceAgent, IProductAgent productAgent, IImportAgent importAgent)
        {
            _priceAgent = priceAgent;
            _productAgent = productAgent;
            _importAgent = importAgent;
        }
        #endregion

        #region Price
        //Get:Create Price.
        public virtual ActionResult Create()
        {
            PriceViewModel model = new PriceViewModel();
            _priceAgent.SetImportPricingDetails(model);
            model.FileTypes = HelperMethods.GetFileTypesForExport();
            return View(AdminConstants.CreateEdit, model);
        }

        //Post:Create Price.
        [HttpPost]
        public virtual ActionResult Create(PriceViewModel priceViewModel)
        {
            if (ModelState.IsValid)
            {
                priceViewModel = _priceAgent.Create(priceViewModel);
                //Check if any error occurred while creating pricing details.
                if (!priceViewModel.HasError && priceViewModel.ImportPriceList?.Count < 1)
                {
                    string message = HelperUtility.IsNotNull(priceViewModel.FilePath) ?
                        Admin_Resources.RecordCreationSuccessMessage + Admin_Resources.LinkViewImportLogs :
                        Admin_Resources.RecordCreationSuccessMessage;

                    SetNotificationMessage(GetSuccessNotificationMessage(message));
                    return RedirectToAction<PriceController>(x => x.Edit(priceViewModel.PriceListId));
                }
                else if (priceViewModel.ImportPriceList?.Count > 0)
                {
                    //Stored invalid Import pricing details list in tempdata to download.
                    priceViewModel.GridModel = FilterHelpers.GetDynamicGridModel(new FilterCollectionDataModel { Filters = new FilterCollection(), Page = 1, RecordPerPage = priceViewModel.ImportPriceList.Count }, priceViewModel?.ImportPriceList, GridListType.View_ImportPriceList.ToString(), string.Empty, null, true);
                    TempData[AdminConstants.DownloadImportErrorList] = priceViewModel.ImportPriceList;
                }
                SetNotificationMessage(GetErrorNotificationMessage(priceViewModel.ErrorMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

            priceViewModel.FileTypes = HelperMethods.GetFileTypesForExport();
            _priceAgent.SetImportPricingDetails(priceViewModel);
            return View(AdminConstants.CreateEdit, priceViewModel);
        }

        //Get:Currency.
        public virtual ActionResult GetCurrency(int currencyId)
         => PartialView(_CurrencyView, _priceAgent.GetCurrencyList(currencyId));

        //Get:Currency.
        public virtual ActionResult GetCulture(int cultureId, int currencyId)
         => PartialView(_CultureView, _priceAgent.GetCultureList(cultureId, currencyId));

        //Get:Edit Price.
        [HttpGet]
        public virtual ActionResult Edit(int priceListId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            PriceViewModel priceViewModel = _priceAgent.GetPrice(priceListId);
            if (HelperUtility.IsNotNull(priceViewModel))
            {
                priceViewModel.ImportPriceList = (List<ImportPriceViewModel>)TempData[AdminConstants.ImportErrorList];
                priceViewModel.GridModel = HelperUtility.IsNotNull(priceViewModel.ImportPriceList) ? FilterHelpers.GetDynamicGridModel(new FilterCollectionDataModel { Filters = new FilterCollection(), Page = 1, RecordPerPage = priceViewModel.ImportPriceList.Count }, priceViewModel?.ImportPriceList, GridListType.View_ImportPriceList.ToString(), string.Empty, null, true) : new GridModel();
                _priceAgent.SetImportPricingDetails(priceViewModel);
                priceViewModel.FileTypes = HelperMethods.GetFileTypesForExport();
            }
            return ActionView(AdminConstants.CreateEdit, priceViewModel);
        }

        //Post:Edit Price.
        [HttpPost]
        public virtual ActionResult Edit(PriceViewModel priceViewModel)
        {
            if (ModelState.IsValid)
            {
                PriceViewModel updatedModel = _priceAgent.Update(priceViewModel);
                //Check if any error occurred while updating pricing details.
                if (!updatedModel.HasError && updatedModel.ImportPriceList?.Count < 1)
                {
                    string message = HelperUtility.IsNotNull(priceViewModel.FilePath) ?
                        Admin_Resources.UpdateMessage + Admin_Resources.LinkViewImportLogs :
                        Admin_Resources.UpdateMessage;

                    SetNotificationMessage(GetSuccessNotificationMessage(message));
                    return RedirectToAction<PriceController>(x => x.Edit(priceViewModel.PriceListId));
                }
                else if (updatedModel.ImportPriceList?.Count > 0)
                {
                    //Stored invalid Import pricing details list in tempdata to download.
                    priceViewModel.GridModel = FilterHelpers.GetDynamicGridModel(new FilterCollectionDataModel { Filters = new FilterCollection(), Page = 1, RecordPerPage = updatedModel.ImportPriceList.Count }, updatedModel?.ImportPriceList, GridListType.View_ImportPriceList.ToString(), string.Empty, null, true);
                    TempData[AdminConstants.ImportErrorList] = updatedModel.ImportPriceList;
                    TempData[AdminConstants.DownloadImportErrorList] = updatedModel.ImportPriceList;
                }
                SetNotificationMessage(GetErrorNotificationMessage(updatedModel.ErrorMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<PriceController>(x => x.Edit(priceViewModel.PriceListId));
        }

        // Get Price list.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelPrice", Key = "Price", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePriceList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePriceList.ToString(), model);
            //Get Price list.
            PriceListViewModel priceList = _priceAgent.GetPriceList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model.
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.ZnodePriceList.ToString(), string.Empty, null, true, true, priceList?.GridModel?.FilterColumn?.ToolMenuList);
            priceList.GridModel.TotalRecordCount = priceList.TotalResults;
            return ActionView(AdminConstants.ListView, priceList);
        }

        //Delete Price.
        public virtual JsonResult Delete(string priceListId)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(priceListId))
            {
                status = _priceAgent.DeletePrice(priceListId, out errorMessage);

                if (status)
                    errorMessage = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Copy Price List.
        [HttpGet]
        public virtual ActionResult Copy(int priceListId)
        {
            PriceViewModel price = _priceAgent.GetPrice(priceListId);
            if (HelperUtility.IsNull(price))
                return RedirectToAction<PriceController>(x => x.List(null));

            price.ListCode = $"CopyOf{price.ListCode}";
            price.ListName = $"Copy Of {price.ListName}";
            return ActionView(price);
        }

        //Copy price list.
        [HttpPost]
        public virtual ActionResult Copy(PriceViewModel priceViewModel)
        {
            ActionResult action = GotoBackURL();
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                //If condition is true then it will redirect to price list.
                if (_priceAgent.CopyPrice(priceViewModel, out errorMessage))
                {
                    if (HelperUtility.IsNotNull(action))
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.CopyMessage));
                        return RedirectToAction<PriceController>(x => x.List(null));
                    }
                    else
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.CopyMessage));
                        return RedirectToAction<PriceController>(x => x.Copy(priceViewModel.PriceListId));
                    }
                }

                //If code already exists or condition is false then it will remain on the same page.
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(errorMessage));
                    return RedirectToAction<PriceController>(x => x.Copy(priceViewModel.PriceListId));
                }
            }
            return RedirectToAction<PriceController>(x => x.Copy(priceViewModel.PriceListId));
        }
        #endregion

        #region SKU Price
        //Create SKU Price.
        [HttpGet]
        public virtual ActionResult AddSKUPrice(int priceListId, string listName)
        {
            PriceSKUDataViewModel priceSKUDataViewModel = new PriceSKUDataViewModel() { PriceSKU = new PriceSKUViewModel() { PriceListId = priceListId } };
            priceSKUDataViewModel.baseDropDownList= _priceAgent.GetCustomColumnList();
            priceSKUDataViewModel.ListName = listName;
            return View(priceSKUDataViewModel);
        }

        //Create SKU Price.
        [HttpPost]
        public virtual ActionResult AddSKUPrice(PriceSKUDataViewModel priceSKUDataViewModel)
        {
            if (ModelState.IsValid)
            {
                priceSKUDataViewModel = _priceAgent.AddSKUPrice(priceSKUDataViewModel);
                priceSKUDataViewModel.baseDropDownList = _priceAgent.GetCustomColumnList();
                if (!priceSKUDataViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<PriceController>(x => x.EditSKUPrice(Convert.ToInt32(priceSKUDataViewModel.PriceSKU.PriceId), priceSKUDataViewModel.PriceSKU.ProductId));
                }
                else if (priceSKUDataViewModel.PriceSKU.PriceId > 0)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(priceSKUDataViewModel.ErrorMessage));
                    return RedirectToAction<PriceController>(x => x.EditSKUPrice(priceSKUDataViewModel.PriceSKU.PriceId.GetValueOrDefault(), priceSKUDataViewModel.PriceSKU.ProductId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(priceSKUDataViewModel.ErrorMessage));
            return View("AddSKUPrice", priceSKUDataViewModel);
        }

        //Get Price SKU list.
        public virtual ActionResult PriceSKUList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int priceListId, string listName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePrice.ToString(), model);
            _priceAgent.SetFilters(model?.Filters, priceListId);
            PriceSKUListViewModel priceSKUList = _priceAgent.GetSKUPriceList(priceListId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            priceSKUList.PriceListId = priceListId;
            priceSKUList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceSKUList?.PriceSKUList, GridListType.ZnodePrice.ToString(), string.Empty, null, true, true, priceSKUList?.GridModel?.FilterColumn?.ToolMenuList);
            priceSKUList.GridModel.TotalRecordCount = priceSKUList.TotalResults;

            return ActionView(priceSKUList);
        }

        //Edit SKU Price.
        [HttpGet]
        public virtual ActionResult EditSKUPrice(int priceId, int pimProductId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            PriceSKUDataViewModel priceSKUDataViewModel = new PriceSKUDataViewModel();
            priceSKUDataViewModel.PriceSKU = _priceAgent.GetSKUPrice(priceId);
            priceSKUDataViewModel.PriceSKU.ProductId = pimProductId;
            priceSKUDataViewModel.PriceTier = new PriceTierViewModel { PriceListId = priceSKUDataViewModel?.PriceSKU?.PriceListId };
            priceSKUDataViewModel.PriceListId = priceSKUDataViewModel?.PriceSKU?.PriceListId;
            return ActionView(AdminConstants.AddEditSKUPrice, priceSKUDataViewModel);
        }

        //Edit SKU Price.
        [HttpPost]
        public virtual ActionResult EditSKUPrice(PriceSKUDataViewModel model)
        {
            PriceSKUDataViewModel priceSKU = _priceAgent.UpdateSKUPrice(model);
            if (!priceSKU.HasError)
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                return RedirectToAction<PriceController>(x => x.EditSKUPrice(model.PriceId.GetValueOrDefault(), model.PriceSKU.ProductId));
            }
            return RedirectToAction<PriceController>(x => x.EditSKUPrice(model.PriceId.GetValueOrDefault(), model.PriceSKU.ProductId));
        }

        //Get price by sku.
        public virtual JsonResult GetPriceBySku(string pimProductId, string sku, string productType)
           => Json(new { data = _priceAgent.GetPriceBySku(pimProductId, sku, productType) }, JsonRequestBehavior.AllowGet);


        //Delete SKU Price.
        public virtual JsonResult DeleteSKUPrice(string priceId, int priceListId, int pimProductId = 0)
        {
            string message = string.Empty;
            if (priceListId > 0 && !string.IsNullOrEmpty(priceId))
            {
                bool status = _priceAgent.DeleteSKUPrice(priceId, priceListId, out message, pimProductId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get Product SKU list.
        public virtual ActionResult GetProductSKUList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int priceListId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SKUListForPrice.ToString(), model);
            _priceAgent.SetFilters(model?.Filters, priceListId);
            PIMProductAttributeValuesListViewModel skuList = _productAgent.GetSkuProductListBySKU(AdminConstants.SKU, model);
            skuList.PriceListId = priceListId;
            skuList.GridModel = FilterHelpers.GetDynamicGridModel(model, skuList?.ProductAttributeValues, GridListType.SKUListForPrice.ToString(), string.Empty, null, true);
            skuList.GridModel.TotalRecordCount = skuList.TotalResults;
            return ActionView(AdminConstants.ProductSKUListView, skuList);
        }
        #endregion

        #region Tier Price
        //Get Tier Price list.
        public virtual ActionResult PriceTierList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int priceListId, string sku, int pageNumber = 1)
        {
            _priceAgent.SetFiltersForSKUAndPriceList(model?.Filters, priceListId, sku);
            PriceSKUDataViewModel tierPriceList = new PriceSKUDataViewModel();
            model.Page = pageNumber;
            tierPriceList.PriceTierList = _priceAgent.GetTierPriceList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage).TierPriceList;
            tierPriceList.baseDropDownList=_priceAgent.GetCustomColumnList();
            tierPriceList.PriceListId = priceListId;
            tierPriceList.PriceSKU = new PriceSKUViewModel { SKU = sku };
            tierPriceList.PageNumber= pageNumber;
            if (pageNumber > 1) {
                string tierPriceView = RenderRazorViewToString("~/Areas/PIM/Views/Products/_TierPriceRow.cshtml", tierPriceList);
                return Json(new
                {
                    html = tierPriceView
                }, JsonRequestBehavior.AllowGet);
            }
            return PartialView("~/Areas/PIM/Views/Products/AddTierPrice.cshtml", tierPriceList);
        }

        //Add Tier Price.
        [HttpPost]
        public virtual JsonResult AddTierPrice(PriceSKUDataViewModel model)
        {
            string message = string.Empty;
            bool status = _priceAgent.AddTierPrice(model, out message);
            return Json(new { status = status, message = status ? Admin_Resources.SaveMessage : message }, JsonRequestBehavior.AllowGet);
        }

        //Delete Tier Price.
        public virtual ActionResult DeleteTierPrice(int priceTierId)
        {
            int priceId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[ZnodePriceEnum.PriceId.ToString()]);
            if (priceTierId > 0)
            {
                bool status = _priceAgent.DeleteTierPrice(priceTierId);
                SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.DeleteMessage) : GetErrorNotificationMessage(Admin_Resources.DeleteErrorMessage));
            }
            return RedirectToAction<PriceController>(x => x.EditSKUPrice(priceId, 0));
        }
        #endregion

        #region Associate Store
        //Get Un-Associated store list.
        public virtual ActionResult GetUnAssociatedStoreList(int priceListId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePortal.ToString(), model);
            _priceAgent.SetFilters(model.Filters, priceListId);
            StoreListViewModel storeList = _priceAgent.GetUnAssociatedStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            storeList.PriceListId = priceListId;
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList?.StoreList, GridListType.ZnodePortal.ToString(), string.Empty, null, true);
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;
            return ActionView(storeList);
        }

        //Get Associated store list.
        public virtual ActionResult GetAssociatedStoreList(int priceListId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string listName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePriceListPortal.ToString(), model);
            _priceAgent.SetFilters(model.Filters, priceListId);
            PricePortalListViewModel pricePortalList = _priceAgent.GetAssociatedStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            pricePortalList.PriceListId = priceListId;
            pricePortalList.ListName = string.IsNullOrEmpty(listName) ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[AdminConstants.ListName] : listName;
            pricePortalList.GridModel = FilterHelpers.GetDynamicGridModel(model, pricePortalList?.PricePortals, GridListType.ZnodePriceListPortal.ToString(), string.Empty, null, true, true, pricePortalList?.GridModel?.FilterColumn?.ToolMenuList);
            pricePortalList.GridModel.TotalRecordCount = pricePortalList.TotalResults;

            return ActionView(pricePortalList);
        }

        //Associated to store to price list.
        public virtual JsonResult AssociateStore(int priceListId, string storeIds)
        {
            string message = string.Empty;

            SetNotificationMessage(_priceAgent.AssociateStore(priceListId, storeIds, out message)
               ? GetSuccessNotificationMessage(Admin_Resources.StorePriceSuccessMessage)
               : GetErrorNotificationMessage(Admin_Resources.ErrorAssociateStore));
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        //Remove associated store.
        public virtual JsonResult RemoveAssociatedStores(string priceListPortalId, int priceListId)
        {
            if (priceListId > 0 && !string.IsNullOrEmpty(priceListPortalId))
            {
                bool status = _priceAgent.RemoveAssociatedStores(priceListPortalId, priceListId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Get: Edit associated Store's precedence value.
        [HttpGet]
        public virtual ActionResult EditAssociatedStoresPrecedence(int priceListPortalId)
           => ActionView(_priceAgent.GetAssociatedStorePrecedence(priceListPortalId));

        //Post: Edit associated Store's precedence value.
        [HttpPost]
        public virtual ActionResult EditAssociatedStoresPrecedence(PricePortalViewModel model)
        {
            if (ModelState.IsValid)
                model = _priceAgent.UpdateAssociatedStorePrecedence(model);
            else
                model.HasError = true;
            return Json(new
            {
                Message = (HelperUtility.IsNull(model) || model.HasError) ? Admin_Resources.UpdateErrorMessage : Admin_Resources.UpdateMessage
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Associate Customer
        //Get Associated customer list.
        public virtual ActionResult GetAssociatedCustomerList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int priceListId, string listName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.PriceListAccount.ToString(), model);
            _priceAgent.SetFilters(model?.Filters, priceListId);
            PriceUserListViewModel priceAccountList = _priceAgent.GetAssociatedCustomerList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            priceAccountList.PriceListId = priceListId;
            priceAccountList.ListName = string.IsNullOrEmpty(listName) ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[AdminConstants.ListName] : listName;
            priceAccountList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceAccountList?.PriceUserList, GridListType.PriceListAccount.ToString(), string.Empty, null, true, true, priceAccountList?.GridModel?.FilterColumn?.ToolMenuList);
            priceAccountList.GridModel.TotalRecordCount = priceAccountList.TotalResults;

            return ActionView(priceAccountList);
        }

        //Get Un-Associated customer list.
        public virtual ActionResult GetUnAssociatedCustomerList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int priceListId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedCustomers.ToString(), model);
            _priceAgent.SetFilters(model?.Filters, priceListId);
            PriceUserListViewModel customerList = _priceAgent.GetUnAssociatedCustomerList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            customerList.PriceListId = priceListId;
            customerList.GridModel = FilterHelpers.GetDynamicGridModel(model, customerList?.PriceUserList, GridListType.UnAssociatedCustomers.ToString(), string.Empty, null, true);
            customerList.GridModel.TotalRecordCount = customerList.TotalResults;
            return ActionView(customerList);
        }

        //Associate customer to price list.
        public virtual JsonResult AssociateCustomer(int priceListId, string customerIds)
        {
            string message = string.Empty;
            SetNotificationMessage(_priceAgent.AssociateCustomer(priceListId, customerIds, out message)
                        ? GetSuccessNotificationMessage(Admin_Resources.CustomerPriceSuccessMessage)
                        : GetErrorNotificationMessage(Admin_Resources.ErrorAssociateCustomer));
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        //Delete associated customer.
        public virtual JsonResult DeleteAssociatedCustomer(string priceListUserId, int priceListId)
        {
            string message = string.Empty;
            if (priceListId > 0 && !string.IsNullOrEmpty(priceListUserId))
            {
                bool status = _priceAgent.DeleteAssociatedCustomer(priceListUserId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(Admin_Resources.DeleteErrorMessage, JsonRequestBehavior.AllowGet);
        }

        //Get: Edit associated Customer's precedence value.
        [HttpGet]
        public virtual ActionResult EditAssociatedCustomerPrecedence(int priceListUserId)
         => ActionView(_priceAgent.GetAssociatedCustomerPrecedence(priceListUserId));

        //Post: Edit associated Customer's precedence value.
        [HttpPost]
        public virtual ActionResult EditAssociatedCustomerPrecedence(PriceUserViewModel model)
        {
            if (ModelState.IsValid)
                model = _priceAgent.UpdateAssociatedCustomerPrecedence(model);
            else
                model.HasError = true;
            return Json(new
            {
                Message = (HelperUtility.IsNull(model) || model.HasError) ? Admin_Resources.UpdateErrorMessage : Admin_Resources.UpdateMessage
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Associate Account
        //Get Associated account list.
        public virtual ActionResult GetAssociatedAccountList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int priceListId, string listName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AccountPriceList.ToString(), model);
            _priceAgent.SetFilters(model?.Filters, priceListId);
            PriceAccountListViewModel priceAccountList = _priceAgent.GetAssociatedAccountList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            priceAccountList.PriceListId = priceListId;
            priceAccountList.ListName = string.IsNullOrEmpty(listName) ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[AdminConstants.ListName] : listName;
            priceAccountList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceAccountList?.PriceAccountList, GridListType.AccountPriceList.ToString(), string.Empty, null, true, true, priceAccountList?.GridModel?.FilterColumn?.ToolMenuList);
            priceAccountList.GridModel.TotalRecordCount = priceAccountList.TotalResults;

            return ActionView(priceAccountList);
        }

        //Get Un-Associated account list.
        public virtual ActionResult GetUnAssociatedAccountList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int priceListId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedAccounts.ToString(), model);
            _priceAgent.SetFilters(model?.Filters, priceListId);
            PriceAccountListViewModel accountList = _priceAgent.GetUnAssociatedAccountList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            accountList.PriceListId = priceListId;
            accountList.GridModel = FilterHelpers.GetDynamicGridModel(model, accountList?.PriceAccountList, GridListType.UnassociatedAccounts.ToString(), string.Empty, null, true);
            accountList.GridModel.TotalRecordCount = accountList.TotalResults;
            return ActionView(accountList);
        }

        //Associate account to price list.
        public virtual JsonResult AssociateAccount(int priceListId, string customerIds)
        {
            string message = string.Empty;
            SetNotificationMessage(_priceAgent.AssociateAccount(priceListId, customerIds, out message)
                        ? GetSuccessNotificationMessage(Admin_Resources.AccountPriceSuccessMessage)
                        : GetErrorNotificationMessage(Admin_Resources.ErrorAssociateAccount));
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        //Delete Associated Account.
        public virtual JsonResult DeleteAssociatedAccount(string priceListAccountId, int priceListId)
        {
            string message = string.Empty;
            if (priceListId > 0 && !string.IsNullOrEmpty(priceListAccountId))
            {
                bool status = _priceAgent.RemoveAssociatedAccounts(priceListAccountId, priceListId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(Admin_Resources.DeleteErrorMessage, JsonRequestBehavior.AllowGet);
        }

        //Get: Edit associated Account's precedence value.
        [HttpGet]
        public virtual ActionResult EditAssociatedAccountPrecedence(int priceListAccountId)
         => ActionView(_priceAgent.GetAssociatedAccountPrecedence(priceListAccountId));

        //Post: Edit associated Account's precedence value.
        [HttpPost]
        public virtual ActionResult EditAssociatedAccountPrecedence(PriceAccountViewModel model)
        {
            if (ModelState.IsValid)
                model = _priceAgent.UpdateAssociatedAccountPrecedence(model);
            else
                model.HasError = true;
            return Json(new
            {
                Message = (HelperUtility.IsNull(model) || model.HasError) ? Admin_Resources.UpdateErrorMessage : Admin_Resources.UpdateMessage
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Associate Profile
        //Get Un-Associated profile list.
        public virtual ActionResult GetUnAssociatedProfileList(int priceListId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedProfiles.ToString(), model);
            _priceAgent.SetFilters(model.Filters, priceListId);
            ProfileListViewModel profileList = _priceAgent.GetUnAssociatedProfileList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            profileList.PriceListId = priceListId;
            profileList.GridModel = FilterHelpers.GetDynamicGridModel(model, profileList?.List, GridListType.UnAssociatedProfiles.ToString(), string.Empty, null, true);
            profileList.GridModel.TotalRecordCount = profileList.TotalResults;
            return ActionView(profileList);
        }

        //Get Associated Profile list.
        public virtual ActionResult GetAssociatedProfileList(int priceListId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string listName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePriceListProfile.ToString(), model);
            _priceAgent.SetFilters(model.Filters, priceListId);
            PriceProfileListViewModel priceProfileList = _priceAgent.GetAssociatedProfileList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            priceProfileList.PriceListId = priceListId;
            priceProfileList.ListName = string.IsNullOrEmpty(listName) ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[AdminConstants.ListName] : listName;
            priceProfileList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceProfileList?.PriceProfiles, GridListType.ZnodePriceListProfile.ToString(), string.Empty, null, true, true, priceProfileList?.GridModel?.FilterColumn?.ToolMenuList);
            priceProfileList.GridModel.TotalRecordCount = priceProfileList.TotalResults;

            return ActionView(priceProfileList);
        }

        //Associate profile to price list.
        public virtual JsonResult AssociateProfile(int priceListId, string profileIds)
        {
            string message = string.Empty;

            SetNotificationMessage(_priceAgent.AssociateProfile(priceListId, profileIds, out message)
                       ? GetSuccessNotificationMessage(Admin_Resources.ProfilePriceSuccessMessage)
                       : GetErrorNotificationMessage(Admin_Resources.ErrorAssociateProfile));

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        //Remove associated profiles from price list.
        public virtual JsonResult RemoveAssociatedProfiles(string priceListProfileId, int priceListId)
        {
            if (priceListId > 0 && !string.IsNullOrEmpty(priceListProfileId))
            {
                bool status = _priceAgent.RemoveAssociatedProfiles(priceListProfileId, priceListId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Get: Edit associated Profile's precedence value.
        [HttpGet]
        public virtual ActionResult EditAssociatedProfilePrecedence(int priceListProfileId)
         => ActionView(_priceAgent.GetAssociatedProfilePrecedence(priceListProfileId));

        //Post: Edit associated Profile's precedence value.
        [HttpPost]
        public virtual ActionResult EditAssociatedProfilePrecedence(PriceProfileViewModel model)
        {
            if (ModelState.IsValid)
                model = _priceAgent.UpdateAssociatedProfilePrecedence(model);
            else
                model.HasError = true;
            return Json(new
            {
                Message = (HelperUtility.IsNull(model) || model.HasError) ? Admin_Resources.UpdateErrorMessage : Admin_Resources.UpdateMessage
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Import Export Price Data

        //This method is used to display price data in grid before saving into database.
        [HttpPost]
        public virtual ActionResult PreviewImportPrice(HttpPostedFileBase file)
        {
            List<ImportPriceViewModel> ImportPriceList = _priceAgent.PreviewImportData(file);
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(new FilterCollectionDataModel { Filters = new FilterCollection(), Page = 1, RecordPerPage = Convert.ToInt32(ImportPriceList?.Count) }, ImportPriceList?.Count > 0 ? ImportPriceList : new List<ImportPriceViewModel>(), GridListType.ZnodePreviewImportPrice.ToString(), string.Empty, null, true);
            return PartialView(AdminConstants.PreviewImportPriceView, gridModel);
        }

        //This method is used to download price data in excel format.
        public virtual ActionResult ExportPriceData(string priceListIds, string listCode)
        {
            if (!string.IsNullOrEmpty(priceListIds))
            {
                List<ExportPriceViewModel> exportPriceData = _priceAgent.ExportPriceData(priceListIds);
                if (exportPriceData?.Count > 0)
                {
                    DownloadHelper downloadHelper = new DownloadHelper();
                    downloadHelper.ExportDownload(exportPriceData, Convert.ToInt32(FileTypes.CSV).ToString(), Response, null, $"{Server.HtmlDecode(listCode)}{AdminConstants.CSV}");
                }
                else
                    SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.ExportNoRecordFound, NotificationType.error));
            }
            else
                SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.ExportNoRecordFound, NotificationType.error));

            return RedirectToAction<PriceController>(x => x.List(null));
        }

        //This method is used to download price data which was not inserted while importing.
        public virtual ActionResult DownLoadInValidImportPriceData(int priceListId)
        {
            List<ImportPriceViewModel> invalidImportData = (List<ImportPriceViewModel>)TempData[AdminConstants.DownloadImportErrorList];
            if (invalidImportData?.Count > 0)
            {
                DownloadHelper downloadHelper = new DownloadHelper();
                downloadHelper.ExportDownload(invalidImportData, Convert.ToInt32(FileTypes.CSV).ToString(), Response, null, $"Error{invalidImportData.FirstOrDefault().PriceListCode}{AdminConstants.CSV}");
            }
            return RedirectToAction<PriceController>(x => x.Edit(priceListId));
        }

        #endregion

        //Download Template.        
        public virtual ActionResult DownloadTemplate(int downloadImportHeadId, string downloadImportName, int downloadImportFamilyId, int downloadImportPromotionTypeId = 0)
        {
            _importAgent.DownloadTemplate(downloadImportHeadId, downloadImportName, downloadImportFamilyId, downloadImportPromotionTypeId, Response);
            if (Request.IsAjaxRequest())
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            else
                return new EmptyResult();
        }
    }
}