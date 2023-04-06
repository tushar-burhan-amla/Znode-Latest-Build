using GenericParsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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
    public class ProductAgent : BaseAgent, IProductAgent
    {
        #region Private Variables

        private readonly IPIMAttributeFamilyClient _attributeFamilyClient;
        private readonly IPIMAttributeClient _attributesClient;
        private readonly IProductsClient _productClient;
        private readonly ILocaleClient _localeClient;

        #endregion Private Variables

        #region Constructor

        public ProductAgent(IPIMAttributeClient pIMAttributeClient, IPIMAttributeFamilyClient pIMAttributeFamilyClient, IProductsClient productsClient, ILocaleClient localeClient)
        {
            _attributesClient = GetClient<IPIMAttributeClient>(pIMAttributeClient);
            _attributeFamilyClient = GetClient<IPIMAttributeFamilyClient>(pIMAttributeFamilyClient);
            _productClient = GetClient<IProductsClient>(productsClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
        }

        #endregion Constructor

        //Get attribute with group for creating new products.
        public virtual PIMFamilyDetailsViewModel GetAttributeFamilyDetails(int familyId = 0)
        => PIMAttributeFamilyViewModelMap.ToPIMFamilyDetailsViewModel(_productClient.GetProductFamilyDetails(new PIMFamilyModel { Id = 0, PIMAttributeFamilyId = familyId, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale), IsCategory = false }));

        //Get assigned personalized attribute
        public virtual PIMProductAttributeValuesListViewModel GetAssignedPersonalizedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(expands, null))
                expands = new ExpandCollection();
            expands.Add(ExpandKeys.Attributes);

            if (IsNull(filters))
                filters = new FilterCollection();

            filters.Add(new FilterTuple(ZnodePimAttributeValueLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            ZnodeLogging.LogMessage("Input parameters filters and expands of method GetAssignedPersonalizedAttributes: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { filters, expands });
            PIMProductAttributeValuesListModel pimAttributeValueListModel = _productClient.GetAssignedPersonalizedAttributes(expands, filters);
            PIMProductAttributeValuesListViewModel personalizedAttributes = !Equals(pimAttributeValueListModel, null) ? pimAttributeValueListModel.ToViewModel<PIMProductAttributeValuesListViewModel>() : new PIMProductAttributeValuesListViewModel();

            List<string> distinctAttributeCodes = personalizedAttributes.ProductAttributeValues.Select(e => e.AttributeCode + e.PimAttributeFamilyId).Distinct().ToList();
            personalizedAttributes.ProductAttributeValues = PIMAttributeFamilyViewModelMap.GetAttributeControls(personalizedAttributes.ProductAttributeValues, distinctAttributeCodes);
            ZnodeLogging.LogMessage("ProductAttributeValues list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, personalizedAttributes?.ProductAttributeValues);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return personalizedAttributes;
        }

        //Get un-assigned personalized attributes
        public virtual List<BaseDropDownList> GetUnAssignedPersonalizedAttributes(int productId, ExpandCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(filters, null))
                filters = new FilterCollection();

            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));

            ZnodeLogging.LogMessage("Input parameter filters of method GetUnassignedPersonalizedAttributes: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { filters});

            PIMAttributeListModel pimAttributeAttributeList = _productClient.GetUnassignedPersonalizedAttributes(null, filters);

            filters.Add(new FilterTuple(ZnodePimAttributeValueEnum.PimProductId.ToString(), FilterOperators.Equals, productId.ToString()));

            PIMProductAttributeValuesListModel assignedPersonalizedAttributes = _productClient.GetAssignedPersonalizedAttributes(null, filters);

            List<BaseDropDownList> unAssignedPersonalizedAttributes = new List<BaseDropDownList>();

            foreach (var attributes in pimAttributeAttributeList.Attributes)
            {
                bool isAttributeAssigned = assignedPersonalizedAttributes.ProductAttributeValues.Any(x => x.AttributeCode == attributes.AttributeCode);

                //Getting default selected locale's attribute name.
                unAssignedPersonalizedAttributes.Add(new BaseDropDownList { id = attributes.PimAttributeId.ToString(), name = attributes.Locales[0].AttributeName, IsChecked = isAttributeAssigned });
            }
            ZnodeLogging.LogMessage("unAssignedPersonalizedAttributes list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, unAssignedPersonalizedAttributes?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return unAssignedPersonalizedAttributes;
        }

        //Get assigned personalized attributes
        public virtual bool AssignPersonalizedAttributes(string attributeIds, int pimProductId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            FilterCollection filterLocale = new FilterCollection();
            filterLocale.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeIds) && pimProductId > 0)
            {
                try
                {
                    return !Equals(_productClient.AssignPersonalizedAttributes(ProductViewModelMap.ToAttributeValueListModel(attributeIds.Split(',').ToList(), pimProductId), filterLocale), null);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.InvalidData:
                            message = Admin_Resources.TextInvalidData;
                            return false;

                        default:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    message = Admin_Resources.TextInvalidData;
                    return false;
                }
            }
            return false;
        }

        //Get un-assigned personalized attributes
        public virtual bool UnassignPersonalizeAttributes(string attributeIds, int pimProductId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            errorMessage = string.Empty;

            if (!Equals(attributeIds, null) && pimProductId > 0)
            {
                try
                {
                    filters.Add(ZnodePimAttributeValueEnum.PimProductId.ToString(), FilterOperators.Equals, pimProductId.ToString());
                    ZnodeLogging.LogMessage("Input parameter filters of method GetAssignedPersonalizedAttributes: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new  { filters = filters });
                    var assignedPersonalizedAttributeIds = _productClient.GetAssignedPersonalizedAttributes(null, filters).ProductAttributeValues.Select(x => x.PimAttributeId.ToString());
                    return _productClient.UnassignPersonalizedAttributes(new ParameterModel { Ids = string.Join(",", assignedPersonalizedAttributeIds.Except(attributeIds.Split(','))) }, pimProductId);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    errorMessage = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    errorMessage = ex.Message;
                    return false;
                }
            }
            return false;
        }

        //Set filter for  Product id and Locale id
        public virtual void SetFilters(FilterCollection filters, int productId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = filters });
            if (IsNotNull(filters))
            {
                //Checking For PimProductId already Exists in Filters Or Not
                if (filters.Exists(x => x.Item1 == ZnodePimAttributeValueEnum.PimProductId.ToString()))
                {
                    //If PIMAttributeId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == ZnodePimAttributeValueEnum.PimProductId.ToString());

                    //Add New PimProductId Into filters
                    filters.Add(new FilterTuple(ZnodePimAttributeValueEnum.PimProductId.ToString(), FilterOperators.Equals, productId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodePimAttributeValueEnum.PimProductId.ToString(), FilterOperators.Equals, productId.ToString()));

                //Checking For LocaleId already Exists in Filters Or Not
                if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
                {
                    filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                    filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //Get product List
        public virtual ProductDetailsListViewModel GetProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int pimCatalogId = 0, string catalogName = null, int locale = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = locale != 0 ? locale : GetLocaleValue();
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));

            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            AddPIMCatalogIdInFilters(_filters, pimCatalogId);

            ZnodeLogging.LogMessage("Input parameters filters and sorts of method GetProducts: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = _filters, sorts = sorts });
            ProductDetailsListModel ProductList = _productClient.GetProducts(null, _filters, sorts, pageIndex, pageSize);

            //Get List view model from list model
            ProductDetailsListViewModel products = GetListViewModelFromListModel(ProductList);

            BindCatalogFilterValues(products, pimCatalogId, catalogName);

            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(products.Locale, localeId.ToString());

            SetListPagingData(products, ProductList);

            //Set tool menu for product list grid view.
            SetProductListToolMenu(products);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return products;
        }

        public virtual List<dynamic> GetExportProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int locale = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = locale != 0 ? locale : GetLocaleValue();
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            ZnodeLogging.LogMessage("Input parameters filters and sorts of method GetProducts: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            List<dynamic> xmlDataList = new List<dynamic>();
            int totalRecordCount = 0;
            // First time condition is not checked as totalRecordCount value is not present
            do
            {
                ProductDetailsListModel ProductList = _productClient.GetProducts(null, filters, sorts, pageIndex, pageSize);
                if(HelperUtility.IsNotNull(ProductList.XmlDataList))
                    xmlDataList.AddRange(ProductList.XmlDataList);
                // Set value of total records count
                totalRecordCount = Convert.ToInt32(ProductList.TotalResults);
                // Increase pageIndex to fetch next chunk data
                pageIndex++;
            }
            while (xmlDataList.Count < totalRecordCount);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return xmlDataList;
        }
        //Get List view model from list model
        public virtual ProductDetailsListViewModel GetListViewModelFromListModel(ProductDetailsListModel ProductList)
        {
            ProductDetailsListViewModel products = new ProductDetailsListViewModel()
            {
                ProductDetailList = ProductList?.ProductDetailList?.ToViewModel<ProductDetailsViewModel>().ToList(),
                Locale = PIMAttributeFamilyViewModelMap.ToLocaleListItem(ProductList.Locale),
                AttrubuteColumnName = ProductList?.AttributeColumnName,
                XmlDataList = ProductList?.XmlDataList ?? new List<dynamic>(),
                ProductDetailListDynamic = ProductList?.ProductDetailListDynamic,
                NewAttributeList = ProductList?.NewAttributeList
            };

            products.AttrubuteColumnName?.Remove(AdminConstants.ProductImage);
            products?.AttrubuteColumnName?.Remove(AdminConstants.Assortment);

            return products;
        }

        //Create Product
        public virtual ProductViewModel CreateProduct(BindDataModel bindDataModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                //Get the ids from bind data model.
                int? pimCatalogId = Equals(Convert.ToString(bindDataModel.GetValue("PimCatalogId")), "")
                                        ? (int?)null : Convert.ToInt32(bindDataModel.GetValue("PimCatalogId"));
                int? pimCategoryId = Equals(Convert.ToString(bindDataModel.GetValue("CategoryId")), "")
                                       ? (int?)null : Convert.ToInt32(bindDataModel.GetValue("CategoryId"));
                int? pimCategoryHierarchyId = Equals(Convert.ToString(bindDataModel.GetValue("PimCategoryHierarchyId")), "")
                                      ? (int?)null : Convert.ToInt32(bindDataModel.GetValue("PimCategoryHierarchyId"));
                //Remove unwanted attributes present on form collection
                RemoveNonAttributeKeys(bindDataModel);
                RemoveAttributeWithEmptyValue(bindDataModel);
                RemoveAttrAndMceEditorKeyWord(bindDataModel);
                ProductViewModel productViewModel = GetProductViewModel(bindDataModel);
                //Binds with model.
                productViewModel.PimCatalogId = pimCatalogId;
                productViewModel.PimCategoryId = pimCategoryId;
                productViewModel.PimCategoryHierarchyId = pimCategoryHierarchyId;
                ZnodeLogging.LogMessage("PimCatalogId, PimCategoryId and PimCategoryHierarchyId properties of productViewModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { PimCatalogId = productViewModel?.PimCatalogId, PimCategoryId = productViewModel?.PimCategoryId, PimCategoryHierarchyId = productViewModel?.PimCategoryHierarchyId });
                ProductModel productDataModel = _productClient.CreateProduct(ProductViewModelMap.ToDataModel(productViewModel));
                productDataModel.ActionMode = productViewModel.ProductId == 0 ? AdminConstants.Create : AdminConstants.Edit;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return productDataModel?.ToViewModel<ProductViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Attributes_Resources.ErrorAttributeAlreadyExists;
                        return new ProductViewModel { HasError = true };

                    case ErrorCodes.SKUAlreadyExist:
                        errorMessage = PIM_Resources.ErrorSKUAlreadyExists;
                        return new ProductViewModel { HasError = true };

                    default:
                        errorMessage = Admin_Resources.ErrorFailedToCreate;
                        return new ProductViewModel { HasError = true };
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return new ProductViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        //Get product details for edit
        public virtual PIMFamilyDetailsViewModel GetProduct(int productId, bool isCopy)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PIMFamilyDetailsViewModel productDetails = PIMAttributeFamilyViewModelMap.ToPIMFamilyDetailsViewModel(_productClient.GetProduct(new PIMGetProductModel { ProductId = productId, FamilyId = 0, IsCopy = isCopy, LocaleId = isCopy ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : GetLocaleValue() }));
            productDetails.CopyProductId = isCopy ? productId : 0;
            productDetails.ProductPublishId = isCopy ? null : productDetails.ProductPublishId;

            if (!isCopy)
            {
                if (string.IsNullOrEmpty(productDetails?.Attributes?.Where(x => x.AttributeCode == "SKU")?.FirstOrDefault()?.AttributeValue))
                    return new PIMFamilyDetailsViewModel();
            }
            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(productDetails.Locale, !isCopy ? GetLocaleValue().ToString() : productDetails.LocaleId.ToString());
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return productDetails;
        }

        //Get product attributes on family dropdown change.
        public virtual PIMFamilyDetailsViewModel GetProductAttributes(int pimProductId, int familyId)
        => PIMAttributeFamilyViewModelMap.ToPIMFamilyDetailsViewModel(_productClient.GetProduct(new PIMGetProductModel { ProductId = pimProductId, FamilyId = familyId, IsCopy = false, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale) }));

        //Get configure attribute on basis of family id and Product Id.
        public virtual PIMFamilyDetailsViewModel GetConfigureAttribute(int familyId, int productId)
            => PIMAttributeFamilyViewModelMap.ToPIMConfigureDetailsViewModel(_productClient.GetConfigureAttribute(new PIMFamilyModel { PIMAttributeFamilyId = familyId, Id = productId }));

        //Delete Products.
        public virtual bool DeleteProduct(string productId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                return _productClient.DeleteProduct(productId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Edit associated products . 
        public virtual bool EditAssignLinkProducts(LinkProductDetailViewModel linkProductDetailViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                LinkProductDetailModel result = _productClient.UpdateAssignLinkProducts(linkProductDetailViewModel.ToModel<LinkProductDetailModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }
        //Get similar type of combination for configure product.
        public virtual string GetSimilarCombination(int productId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            string combinationProduct = string.Empty;

            //Get list of product from session.
            List<dynamic> ProductList = GetFromSession<List<dynamic>>("ProductList");
            if (HelperUtility.IsNotNull(ProductList))
            {
                //Getcombination id for a selected product
                var combinationId = ProductList.Where(x => x.ProductId == productId).Select(x => x.CombinationId).FirstOrDefault();
                var combinationProductIds = ProductList.Where(x => x.CombinationId == combinationId && x.ProductId != productId).Select(x => x.ProductId).ToArray();
                combinationProduct = string.Join(",", combinationProductIds);
            }
            ZnodeLogging.LogMessage("parameter to be returned :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { combinationProduct = combinationProduct }); return combinationProduct;
        }

        //Get LocaleId
        public virtual int GetLocaleValue()
        {
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_productCulture"))
            {
                string cookieValue = CookieHelper.GetCookieValue<string>("_productCulture");
                localeId = string.IsNullOrEmpty(cookieValue) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : Convert.ToInt32(cookieValue);
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_productCulture", Convert.ToString(localeId));
            }
            ZnodeLogging.LogMessage("parameter to be returned :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,new { localeId = localeId });
            return localeId;
        }

        //Activate/Deactivate bulk Products.
        public virtual bool ActivateDeactivateProducts(string productIds, bool isActive)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                return _productClient.ActivateDeactivateProducts(productIds, isActive, Convert.ToInt32(DefaultSettingHelper.DefaultLocale));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #region Product Type

        //Get list of assigned products to parent product.
        public virtual ProductDetailsListViewModel GetAssociatedProducts(int parentProductId, int attributeId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            ProductDetailsListModel associatedProducts = _productClient.GetAssociatedProducts(parentProductId, attributeId, Expands, filters, sorts, pageIndex, pageSize);

            //Get List view model from list model
            ProductDetailsListViewModel products = GetListViewModelFromListModel(associatedProducts);

            SetListPagingData(products, associatedProducts);

            //Set tool menu for associated product list grid view.
            SetAssociatedProductListToolMenu(products);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return products;
        }

        //Assign product to parent product.
        public virtual bool AssociateProducts(string associatedProductIds, int parentProductId, int attributeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ProductTypeAssociationListViewModel associatedProductList = new ProductTypeAssociationListViewModel();
            if (!string.IsNullOrEmpty(associatedProductIds))
            {
                int[] selectedProductIds = associatedProductIds.Split(',').Select(int.Parse).ToArray();
                associatedProductList.AssociatedProducts = new List<ProductTypeAssociationViewModel>();
                foreach (int productId in selectedProductIds)
                {
                    ProductTypeAssociationViewModel associatedProduct = new ProductTypeAssociationViewModel { PimProductId = productId, PimParentProductId = parentProductId, PimAttributeId = attributeId };
                    associatedProductList.AssociatedProducts.Add(associatedProduct);
                }
            }
            associatedProductList = _productClient.AssociateProducts(associatedProductList.ToModel<ProductTypeAssociationListModel>()).ToViewModel<ProductTypeAssociationListViewModel>();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return associatedProductList?.AssociatedProducts?.Count > 0;
        }

        //Get Product list which are not assigned to parent product.
        public virtual bool UnassociatedProduct(string productTypeAssociationId) => _productClient.UnassociateProduct(new ParameterModel { Ids = productTypeAssociationId });

        //Get list of Products to be associated.
        public virtual ProductDetailsListViewModel GetProductsToBeAssociated(string productIds, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            productIds = string.IsNullOrEmpty(productIds) ? "0" : productIds;
            ProductDetailsListModel productList = _productClient.GetProductsToBeAssociated(productIds, null, filters, sorts, pageIndex, pageSize);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            //Get List view model from list model
            ProductDetailsListViewModel products = GetListViewModelFromListModel(productList);

            SetListPagingData(products, productList);
            //Set tool menu for associated product list grid view.
            SetAssociatedProductListToolMenu(products);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return products;
        }

        // Method for Get Configure Products To Be Associated or unassociated
        public virtual ProductDetailsListViewModel GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            associatedProductIds = !string.IsNullOrEmpty(associatedProductIds) ? associatedProductIds : "0";
            associatedAttributeIds = !string.IsNullOrEmpty(associatedAttributeIds) ? associatedAttributeIds : "0";
            ProductDetailsListModel productList = new ProductDetailsListModel();

            productList = _productClient.GetAssociatedUnAssociatedConfigureProducts(parentProductId, associatedProductIds, associatedAttributeIds, pimProductIdsIn, Expands, filters, sorts, pageIndex, pageSize);

            if (parentProductId == 0)
                foreach (var item in productList?.ProductDetailListDynamic)
                    item.PimProductTypeAssociationId = item.ProductId;

            //Get List view model from list model
            ProductDetailsListViewModel products = GetListViewModelFromListModel(productList);

            products.ParentProductId = parentProductId;
            products.AssociatedConfigureAttributeIds = associatedAttributeIds;
            products.AssociatedProductIds = associatedProductIds;

            SetListPagingData(products, productList);

            //Set the Tool Menus for Customer Notes List on Grid View.
            products.ProductType = ZnodeConstant.ConfigurableProduct;
            SetAssociatedProductListToolMenu(products);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return products;
        }

        #endregion Product Type

        #region Associated addons

        //Get associated addon group by parentproductId
        public virtual AddonGroupListViewModel GetAssociatedAddonGroup(int parentProductId, FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sortCollection, expands= expands });

            AddonGroupListViewModel addonsGroupDetails = new AddonGroupListViewModel { AddonGroups = ((_productClient.GetAssociatedAddonDetails(parentProductId, expands, filters, sortCollection)).AddonGroups.ToViewModel<AddonGroupViewModel>().ToList()) };
            foreach (var item in addonsGroupDetails?.AddonGroups)
                foreach (var itemPimAddOnProducts in item?.PimAddOnProducts)
                    itemPimAddOnProducts.RequiredTypeValue = (RequiredType)Enum.Parse(typeof(RequiredType), itemPimAddOnProducts.RequiredType);

            addonsGroupDetails.AddonGroups?.ForEach(addonGroup => addonGroup.DisplayType = EnumHelper<AddonType>.GetDisplayValue((AddonType)Enum.Parse(typeof(AddonType), addonGroup.DisplayType)));
            addonsGroupDetails.AddonGroups?.ForEach(item => { item.AssociatedChildProducts.ProductDetailList.ForEach(item1 => item1.IsDefaultList = GetBooleanList()); });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonsGroupDetails;
        }

        //Associate addon group by parentproductId.
        public virtual bool AssociateAddonGroups(int parentProductId, string ids)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            string[] associatedAddonGroupIds = null;
            if (!string.IsNullOrEmpty(ids))
                associatedAddonGroupIds = ids.Split(',');

            AddonProductListModel addonProductListModel = new AddonProductListModel();

            addonProductListModel.AddonProducts = new List<AddOnProductModel>();

            addonProductListModel.AddonProducts = associatedAddonGroupIds.Select(id => new AddOnProductModel { PimAddonGroupId = Convert.ToInt32(id), PimProductId = parentProductId, RequiredType = RequiredType.Required.ToString() }).ToList();

            addonProductListModel = _productClient.AssociateAddon(addonProductListModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonProductListModel?.AddonProducts?.Count > 0;
        }

        //Get unassociated addon groups by parentProductId
        public virtual AddonGroupListViewModel GetUnassociatedAddonGroups(int parentProductId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            AddonGroupListModel unassociatedAddonGroups = new AddonGroupListModel();

            filters.Add(ZnodePimCustomFieldLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts});

            unassociatedAddonGroups = _productClient.GetUnassociatedAddonGroups(parentProductId, Expands, filters, sorts, pageIndex, pageSize);

            AddonGroupListViewModel viewModel = unassociatedAddonGroups?.AddonGroups?.Count > 0 ? new AddonGroupListViewModel { AddonGroups = unassociatedAddonGroups.AddonGroups.ToViewModel<AddonGroupViewModel>().ToList() } : new AddonGroupListViewModel();
            viewModel.AddonGroups.ForEach(addonGroup => addonGroup.DisplayType = EnumHelper<AddonType>.GetDisplayValue((AddonType)Enum.Parse(typeof(AddonType), addonGroup.DisplayType)));
            SetListPagingData(viewModel, unassociatedAddonGroups);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //Delete addon products by addonProductId.
        public virtual bool DeleteAddonProduct(int addonProductId, int pimParentProductId = 0)
        {
            return _productClient.DeleteAssociatedAddons(new ParameterModel { Ids = addonProductId.ToString(), PimProductId = pimParentProductId });
        }

        //Associate addon product to parent product
        public virtual bool AssociateAddonProduct(int addonProductId, string productIds, int displayOrder, bool? isDefault, int pimProductId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            AddOnProductDetailListModel addonProductDetails = new AddOnProductDetailListModel();
            addonProductDetails.AddOnProductDetailList = new List<AddOnProductDetailModel>();

            addonProductDetails.AddOnProductDetailList = productIds?.Split(',').Select(productId => new AddOnProductDetailModel { PimAddOnProductId = addonProductId, PimChildProductId = Convert.ToInt32(productId), DisplayOrder = displayOrder, IsDefault = isDefault, PimProductId = pimProductId }).ToList();

            addonProductDetails = _productClient.CreateAddonProductDetail(addonProductDetails);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonProductDetails.AddOnProductDetailList?.Count > 0;
        }

        //Update Addon Display Order
        public virtual AddonProductDetailViewModel UpdateAddonDisplayOrder(int pimAddonProductDetailId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            AddonProductDetailViewModel addonProductDetails = JsonConvert.DeserializeObject<AddonProductDetailViewModel[]>(data)[0];

            addonProductDetails = new AddonProductDetailViewModel { PimAddOnProductDetailId = pimAddonProductDetailId, PimAddOnProductId = addonProductDetails.PimAddOnProductId, PimChildProductId = addonProductDetails.PimChildProductId, DisplayOrder = Convert.ToInt32(addonProductDetails.AddOnDisplayOrder), IsDefault = addonProductDetails.IsDefault };

            return _productClient.UpdateAddonDisplayOrder(addonProductDetails?.ToModel<AddOnProductDetailModel>())?.ToViewModel<AddonProductDetailViewModel>();
        }

        //Delete associated addon product by addonProductDetailId.
        public virtual bool DeleteAddonProductDetail(string addonProductDetailId, int pimProductId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (!string.IsNullOrEmpty(addonProductDetailId))
            {
                return _productClient.DeleteAddonProductDetails(new ParameterModel { Ids = addonProductDetailId, PimProductId = pimProductId });
            }
            return false;
        }

        //Update addon product association
        public virtual AddOnProductViewModel UpdateProductAddonAssociation(AddOnProductViewModel addonProductViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            AddOnProductViewModel addonProduct = new AddOnProductViewModel();
            addonProductViewModel.RequiredType = addonProductViewModel.RequiredTypeValue.ToString();
            if (IsNotNull(addonProductViewModel))
            {
                AddOnProductModel addonProductModel = _productClient.UpdateProductAddonAssociation(addonProductViewModel.ToModel<AddOnProductModel>());
                if (IsNotNull(addonProductModel))
                    addonProduct = addonProductModel.ToViewModel<AddOnProductViewModel>();
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonProduct;
        }

        #endregion Associated addons

        #region Assign Link Products

        //Get list of assigned link products.
        public virtual ProductDetailsListViewModel GetAssignedLinkProducts(int parentProductId, int attributeId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Check if the session has same attributeId in the session
            if (GetFromSession<int>("linkAttributeId") != attributeId)
            {
                RemoveInSession(DynamicGridConstants.listTypeSessionKey);
                filters.Clear();
            }

            SaveInSession<int>("linkAttributeId", attributeId);

            if (filters.Exists(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString());
            }
            filters.Add(ZnodePimCustomFieldLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString());

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });

            ProductDetailsListModel assignedLinkProducts = _productClient.GetAssignedLinkProducts(parentProductId, attributeId, Expands, filters, sortCollection, pageIndex, recordPerPage);

            assignedLinkProducts?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            //Get List view model from list model
            ProductDetailsListViewModel products = GetListViewModelFromListModel(assignedLinkProducts);

            SetListPagingData(products, assignedLinkProducts);

            //Set tool menu for assigned linked product list grid view.
            SetAssignedLinkedProductListToolMenu(products, attributeId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return products;
        }

        //Assign product to parent product as link product.
        public virtual bool AssignLinkProducts(int parentProductId, int attributeId, string productIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { parentProductId = parentProductId, attributeId = attributeId, productIds = productIds });

            LinkProductDetailListViewModel linkProductDetailList = new LinkProductDetailListViewModel();
            foreach (string productId in productIds.Split(','))
            {
                LinkProductDetailViewModel linkProductDetail = new LinkProductDetailViewModel { PimParentProductId = parentProductId, PimAttributeId = attributeId, PimProductId = Convert.ToInt32(productId) };
                linkProductDetailList.LinkProducts.Add(linkProductDetail);
            }

            linkProductDetailList.LinkProducts = _productClient.AssignLinkProducts(new LinkProductDetailListModel { LinkProductDetailList = linkProductDetailList.LinkProducts.ToModel<LinkProductDetailModel>().ToList() }).LinkProductDetailList.ToViewModel<LinkProductDetailViewModel>().ToList();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return linkProductDetailList?.LinkProducts?.Count > 0;
        }

        //Removes relation of child product from parent product as link products.
        public virtual bool UnassignLinkProducts(string linkProductDetailId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(linkProductDetailId))
            {
                try
                {
                    return _productClient.UnassignLinkProducts(new ParameterModel { Ids = linkProductDetailId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    errorMessage = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        #endregion Assign Link Products

        #region Custom Field

        #region public virtual Methods

        //Create new Custom field to Product
        public virtual CustomFieldViewModel AddCustomField(CustomFieldViewModel customFieldViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                CustomFieldModel customFieldModel = _productClient.AddCustomField(customFieldViewModel.ToModel<CustomFieldModel>());
                return !Equals(customFieldModel, null) ? customFieldModel.ToViewModel<CustomFieldViewModel>() : new CustomFieldViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return GetErrorViewModel(customFieldViewModel, PIM_Resources.ErrorCustomCodeAlreadyExist);

                    default:
                        return GetErrorViewModel(customFieldViewModel, ex.Message);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return GetErrorViewModel(customFieldViewModel, ex.Message);
            }
        }

        //Get error view model for custom field
        public virtual CustomFieldViewModel GetErrorViewModel(CustomFieldViewModel customFieldViewModel, string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            CustomFieldViewModel errorViewModel = new CustomFieldViewModel();
            errorViewModel.HasError = true;
            errorViewModel.ErrorMessage = message;
            LocaleListViewModel localeList = GetLocalList();

            //Assigning locale name from locale list to custom field locales.
            customFieldViewModel.CustomFieldLocales = customFieldViewModel.CustomFieldLocales.Join(localeList.Locales, customFieldLocale => customFieldLocale.LocaleId, locale => locale.LocaleId, (customFieldLocale, locale) => { customFieldLocale.LocaleName = locale.Name; return customFieldLocale; }).ToList();
            errorViewModel.CustomFieldLocales = customFieldViewModel.CustomFieldLocales;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return errorViewModel;
        }

        //Get locale list for addon list page.
        public virtual LocaleListViewModel GetLocalList()
        {
            LocaleListModel locales = _localeClient.GetLocaleList(null, new FilterCollection { new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, "true") }, null);
            return locales?.Locales?.Count > 0 ? new LocaleListViewModel { Locales = locales.Locales.ToViewModel<LocaleViewModel>().ToList() }
                : new LocaleListViewModel();
        }

        //Get data for edit custom field.
        public virtual CustomFieldListViewModel GetCustomFields(int productId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //checks if the filter collection null
            if (Equals(filters, null))
                filters = new FilterCollection();

            if (productId > 0)
            {
                if (!filters.Exists(x => x.Item1 == ZnodePimCustomFieldEnum.PimProductId.ToString()))
                    filters.Add(new FilterTuple(ZnodePimCustomFieldEnum.PimProductId.ToString(), FilterOperators.Equals, productId.ToString()));
            }

            if (filters.Exists(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString());
            }
            filters.Add(ZnodePimCustomFieldLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString());

            if (Equals(expands, null))
                expands = new ExpandCollection();
            expands.Add(ZnodePimCustomFieldEnum.ZnodePimCustomFieldLocales.ToString());

            ZnodeLogging.LogMessage("Input parameters of method GetCustomFields:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection, expands = expands });

            CustomFieldListModel list = _productClient.GetCustomFields(expands, filters, sortCollection, pageIndex, recordPerPage);
            CustomFieldListViewModel listViewModel = new CustomFieldListViewModel { CustomFields = list?.CustomFields?.ToViewModel<CustomFieldViewModel>().ToList() };
            SetListPagingData(listViewModel, list);

            //Set tool menu for custom field list grid view.
            SetCustomFieldListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return list?.CustomFields?.Count > 0 ? listViewModel
                : new CustomFieldListViewModel();
        }

        //Get data for edit custom field.
        public virtual CustomFieldViewModel GetCustomField(int customFieldId, ExpandCollection expands = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(expands, null))
                expands = new ExpandCollection();
            expands.Add(ZnodePimCustomFieldEnum.ZnodePimCustomFieldLocales.ToString());

            if (customFieldId > 0)
            {
                ZnodeLogging.LogMessage("Input parameters of method GetCustomField:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { customFieldId = customFieldId, expands = expands });

                CustomFieldViewModel customField = _productClient.GetCustomField(customFieldId, expands).ToViewModel<CustomFieldViewModel>();
                customField.Locales = GetLocalList();
                if (customField?.Locales?.Locales?.Count > 0)
                    BindCustomFieldLocaleswithLocales(customField);
                else
                    BindNewLocales(customField);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return customField;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return new CustomFieldViewModel();
        }

        //Update custom field
        public virtual CustomFieldViewModel UpdateCustomField(CustomFieldViewModel customFieldViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            customFieldViewModel.CustomFieldLocales.RemoveAll(x => string.IsNullOrEmpty(x.CustomKey) || string.IsNullOrEmpty(x.CustomKeyValue));
            CustomFieldModel customFieldModel = _productClient.UpdateCustomField(customFieldViewModel.ToModel<CustomFieldModel>());
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return !Equals(customFieldModel, null) ? customFieldModel.ToViewModel<CustomFieldViewModel>() : new CustomFieldViewModel();
        }

        //Delete custom field
        public virtual bool DeleteCustomField(string customFieldId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _productClient.DeleteCustomField(customFieldId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        public virtual CustomFieldViewModel GetLocales()
           => BindNewLocales(new CustomFieldViewModel { Locales = GetLocalList() });

        #endregion public virtual Methods

        #region Private Methods

        private void BindCustomFieldLocaleswithLocales(CustomFieldViewModel customField)
        {
            if (customField.Locales.Locales.Count() > customField?.CustomFieldLocales?.Count())
            {
                int customFieldCustomLocales = customField.Locales.Locales.Count() - customField.CustomFieldLocales.Count();
                for (int count = 0; count < customFieldCustomLocales; count++)
                {
                    customField.CustomFieldLocales.Add(new CustomFieldLocaleModel());
                }
            }
            foreach (var locale in customField?.Locales?.Locales)
            {
                foreach (var customFieldLocale in customField?.CustomFieldLocales)
                {
                    if (Equals(locale.LocaleId, Convert.ToInt32(customFieldLocale.LocaleId)) && locale.IsDefault)
                        customFieldLocale.IsDefault = true;

                    if (Equals(locale.LocaleId, Convert.ToInt32(customFieldLocale.LocaleId)) && Equals(customFieldLocale.LocaleName, null))
                        customFieldLocale.LocaleName = locale.Name;

                    if (Equals(Convert.ToInt32(customFieldLocale.LocaleId), 0) && customField.CustomFieldLocales.Where(x => Convert.ToInt32(x.LocaleId) == locale.LocaleId).ToList().Count() == 0)
                    {
                        customFieldLocale.LocaleId = locale.LocaleId;
                        customFieldLocale.LocaleName = locale.Name;
                    }
                }
            }
        }

        private CustomFieldViewModel BindNewLocales(CustomFieldViewModel customField)
        {
            customField.CustomFieldLocales = Equals(customField.CustomFieldLocales, null) ? new List<CustomFieldLocaleModel>() : customField.CustomFieldLocales;
            foreach (LocaleViewModel locale in customField?.Locales?.Locales)
            {
                customField.CustomFieldLocales.Add(new CustomFieldLocaleModel()
                {
                    LocaleId = locale.LocaleId,
                    LocaleName = locale.Name,
                    IsDefault = locale.IsDefault
                });
            }
            return customField;
        }

        //Get attribute Data
        private ProductViewModel GetProductViewModel(BindDataModel model)
        {
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.ProductFamily = Convert.ToInt32(model.GetValue("ddlfamily"));
            productViewModel.ProductType = Convert.ToInt32(model.GetValue("ProductType"));
            productViewModel.ProductId = Convert.ToInt32(model.GetValue("ProductId"));
            productViewModel.ConfigureAttributeIds = model.GetValue("ConfigureAttributeIds") != null ? model.GetValue("ConfigureAttributeIds").ToString() : "0";
            productViewModel.ConfigureFamilyIds = model.GetValue("ConfigureFamilyIds") != null ? model.GetValue("ConfigureFamilyIds").ToString() : "0";
            productViewModel.AssociatedProducts = model.GetValue("AssociatedProductIds") != null ? Convert.ToString(model.GetValue("AssociatedProductIds")) : "0";
            productViewModel.LocaleId = productViewModel.ProductId > 0 ? GetLocaleValue() : Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            productViewModel.CopyProductId = Convert.ToInt32(model.GetValue("CopyProductId"));

            model.ControlsData?.ToList().ForEach(item =>
            {
                //Appended keys with property name {AttributeCode}[0]_{PimAttributeId}[1]_{PimAttributeDefaultValueId}[2]_{PimAttributeValueId}[3]_{PimAttributeFamilyId}[4].
                List<object> itemList = new List<object>();
                itemList.AddRange(item.Key.Split('_'));
                if (itemList.Count() >= 5)
                {
                    productViewModel.ProductAttributeList.Add(new ProductAttributeViewModel
                    {
                        ProductAttributeId = Convert.ToInt32(itemList[1]),
                        ProductAttributeCode = itemList[0].ToString(),
                        ProductAttributeFamilyId = Convert.ToInt32(itemList[4]),
                        ProductAttributeValue = item.Value.ToString().Trim(),
                        ProductAttributeDefaultValueId = Convert.ToInt32(itemList[2]),
                        ProductAttributeValueId = Convert.ToInt32(itemList[3]),
                        LocaleId = productViewModel.LocaleId,
                        ProductId = productViewModel.ProductId,
                        AssociatedProducts = productViewModel.AssociatedProducts,
                        ConfigureAttributeIds = productViewModel.ConfigureAttributeIds,
                        ConfigureFamilyIds = productViewModel.ConfigureFamilyIds,
                    });
                }
            });

            return productViewModel;
        }

        #endregion Private Methods

        #endregion Custom Field


        //Get un-associated product list for Group/Bundle product.
        public virtual ProductDetailsListViewModel GetUnassociatedProducts(int parentProductId, string associatedProductIds, int addonProductId, int listType, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));

            ProductDetailsListModel unassociatedProducts = new ProductDetailsListModel();

            associatedProductIds = !string.IsNullOrEmpty(associatedProductIds) ? associatedProductIds : "0";
            switch ((UnAssociatedProductListType)listType)
            {
                case UnAssociatedProductListType.Addon:
                    if (Equals(filters, null))
                        filters = new FilterCollection();
                    if (!filters.Any(x => x.FilterName == ZnodePimAddOnProductEnum.PimProductId.ToString()))
                        filters.Add(ZnodePimAddOnProductEnum.PimProductId.ToString(), FilterOperators.Equals, parentProductId.ToString());
                    unassociatedProducts = _productClient.GetUnassociatedAddonProducts(addonProductId, Expands, filters, sorts, pageIndex, pageSize);
                    break;

                case UnAssociatedProductListType.AssociatedProducts:
                    unassociatedProducts = _productClient.GetUnassociatedProducts(parentProductId, associatedProductIds, Expands, filters, sorts, pageIndex, pageSize);
                    break;

                case UnAssociatedProductListType.Link:
                    unassociatedProducts = _productClient.GetUnassignedLinkProducts(parentProductId, Convert.ToInt32(associatedProductIds), Expands, filters, sorts, pageIndex, pageSize);
                    break;
            }

            //Get List view model from list model
            ProductDetailsListViewModel products = GetListViewModelFromListModel(unassociatedProducts);

            SetListPagingData(products, unassociatedProducts);
            SetViewModelProperties(parentProductId, associatedProductIds, addonProductId, listType, products);

            return products;
        }

        public virtual ProductTypeAssociationViewModel GetAssociatedProduct(int pimProductTypeAssociationId)
         => _productClient.GetAssociatedProduct(pimProductTypeAssociationId)?.ToViewModel<ProductTypeAssociationViewModel>();

        public virtual bool UpdateAssociatedProduct(ProductTypeAssociationViewModel productTypeAssociationViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                _productClient.UpdateAssociatedProduct(productTypeAssociationViewModel.ToModel<ProductTypeAssociationModel>());
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #region Publish Product

        //Publish product.
        public virtual bool PublishProduct(string productIds, string revisionType, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            errorMessage = PIM_Resources.ErrorPublished;
            try
            {
                return Convert.ToBoolean(_productClient.PublishProduct(new ParameterModel { Ids = productIds, RevisionType = revisionType })?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = PIM_Resources.ProductPublishError;
                        return false;
                    case ErrorCodes.NotPermitted:
                        errorMessage = (!string.IsNullOrEmpty(ex.ErrorMessage)) ? ex.ErrorMessage : PIM_Resources.ErrorPublished;
                        return false;
                    case ErrorCodes.ProductPublishError:
                        errorMessage = (!string.IsNullOrEmpty(ex.ErrorMessage)) ? ex.ErrorMessage : PIM_Resources.ErrorPublished;
                        return false;

                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion Publish Product

        #region Product SKU list for Autocomplete feature / Price/Inventory.

        public virtual PIMProductAttributeValuesListViewModel GetSkuProductListBySKU(string attributeValue, FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            FilterTuple isDownloadableFilter = null;
            if (IsNotNull(model?.Filters))
            {
                isDownloadableFilter = model.Filters.FirstOrDefault(x => string.Equals(x.FilterName, ZnodeConstant.IsDownloadable, StringComparison.CurrentCultureIgnoreCase));

                //Adding single quote to isdownloadable filter value. 
                UpdateIsDownloadableFilter(isDownloadableFilter, model, "\'" + isDownloadableFilter?.Item3 + "\'");

                //Removing LocaleId already Exists in Filters.
                model.Filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                model.Filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetLocaleValue().ToString()));
            }
            ZnodeLogging.LogMessage("Input parameters of method GetProductSKUsByAttributeCode:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { attributeValue = attributeValue, Expands = model?.Expands, Filters = model?.Filters, SortCollection = model?.SortCollection });

            //Product SKU list on the basis of attribute code and its value.
            PIMProductAttributeValuesListModel list = _productClient.GetProductSKUsByAttributeCode(attributeValue, model?.Expands, model?.Filters, model?.SortCollection, model?.Page, model?.RecordPerPage);

            //Removing single quote from isDownloadable filter value.
            UpdateIsDownloadableFilter(isDownloadableFilter, model, isDownloadableFilter?.Item3);
            PIMProductAttributeValuesListViewModel listViewModel = new PIMProductAttributeValuesListViewModel { ProductAttributeValues = list?.ProductAttributeValues?.ToViewModel<PIMProductAttributeValuesViewModel>().ToList() };

            SetListPagingData(listViewModel, list);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return list?.ProductAttributeValues?.Count > 0 ? listViewModel : new PIMProductAttributeValuesListViewModel { ProductAttributeValues = new List<PIMProductAttributeValuesViewModel>() };
        }

        //Update the value of isDownloadable filter if exists.
        private void UpdateIsDownloadableFilter(FilterTuple isDownloadableFilter, FilterCollectionDataModel model, string value)
        {
            if (IsNotNull(isDownloadableFilter))
            {

                model.Filters.RemoveAll(x => string.Equals(x.Item1, ZnodeConstant.IsDownloadable, StringComparison.CurrentCultureIgnoreCase));

                model.Filters.Add(new FilterTuple(isDownloadableFilter.Item1, isDownloadableFilter.Item2, value));
            }
        }
        #endregion Product SKU list for Autocomplete feature / Price/Inventory.

        #region Product Update Import
        /// <summary>
        /// get product update file sample file content.
        /// </summary>
        /// <returns>return file content</returns>
        public virtual string getFileContent()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                return ZnodeStorageManager.ReadTextStorage(ZnodeAdminSettings.ProductUpdateSampleCSVPath);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return string.Empty;
            }
        }

        //This method process the product update data from the file for import.
        public virtual bool ImportProductUpdateData(ImportViewModel importViewModel, out string statusMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            string fileName = importViewModel?.FileName;
            ZnodeLogging.LogMessage("FileName property of importViewModel:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { FileName = importViewModel?.FileName });

            statusMessage = string.Empty;
            try
            {
                importViewModel.FileName = fileName;
                importViewModel.ImportType = AdminConstants.ProductUpdate;
                bool isSuccess;

                //Check for empty csv file .If empty then give empty csv message else go for import.
                CheckCSVStatus(importViewModel, out statusMessage, fileName, out isSuccess);

                RemoveTemporaryFiles(fileName);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return isSuccess;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Warning);
                //Remove the file if any error comes in.
                RemoveTemporaryFiles(fileName);
                statusMessage = string.IsNullOrEmpty(statusMessage) ? Admin_Resources.ImportProcessFailed + Admin_Resources.LinkViewImportLogs : statusMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                statusMessage = Admin_Resources.ImportProcessFailed + Admin_Resources.LinkViewImportLogs;
                return false;
            }
        }

        //Check for empty csv file .If empty then give empty csv message else go for import.
        private void CheckCSVStatus(ImportViewModel importviewModel, out string statusMessage, string fileName, out bool isSuccess)
        {
            DataTable dt = new DataTable();
            int genericParserImportMaxBufferSize = ZnodeAdminSettings.GenericParserImportMaxBufferSize;
            ZnodeLogging.LogMessage("Value of GenericParserImportMaxBufferSize:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { GenericParserImportMaxBufferSize = genericParserImportMaxBufferSize });
            try
            {
                using (GenericParserAdapter parser = new GenericParserAdapter(fileName))
                {
                    parser.ColumnDelimiter = ZnodeConstant.ColumnDelimiter;
                    parser.FirstRowHasHeader = true;
                    parser.MaxBufferSize = genericParserImportMaxBufferSize;
                    dt = parser.GetDataTable();
                }

                string[] columnNames = (from dc in dt.Columns.Cast<DataColumn>()
                                        select dc.ColumnName).ToArray();

                foreach (string name in columnNames)
                {
                    if (name.Contains(' '))
                    {
                        statusMessage = Admin_Resources.OnlyAlphanumericCharactersAllowedInHeaders;
                        throw new ZnodeException(ErrorCodes.InvalidCSV, Admin_Resources.OnlyAlphanumericCharactersAllowedInHeaders);
                    }
                }

                isSuccess = false;
                if (IsNotNull(dt) && dt.Rows.Count > 0)
                {
                    if (_productClient.ImportProductUpdateData(ImportViewModelMap.ToModel(importviewModel)))
                    {
                        isSuccess = true;
                        statusMessage = Admin_Resources.UpdateProcessInitiated + Admin_Resources.LinkViewImportLogs;
                    }
                    else
                        statusMessage = Admin_Resources.ImportProcessFailed + Admin_Resources.LinkViewImportLogs;
                }
                else
                    statusMessage = Admin_Resources.MessageEmptyCSV;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Something went wrong. Error message:- {ex.Message}", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                isSuccess = false;
                statusMessage = Admin_Resources.ImportProcessFailed;
            }
        }

        #endregion

        private static void SetViewModelProperties(int parentProductId, string associatedProductIds, int addonProductId, int listType, ProductDetailsListViewModel unassociatedViewProducts)
        {
            unassociatedViewProducts.ParentProductId = parentProductId;
            unassociatedViewProducts.AssociatedProductIds = associatedProductIds;
            unassociatedViewProducts.AddonProductId = addonProductId;
            unassociatedViewProducts.ListType = listType;
        }

        //Set tool menu for product list grid view.
        private void SetProductListToolMenu(ProductDetailsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeleteProductPopup')", ControllerName = "Products", ActionName = "Delete" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "Products.prototype.IsActiveProducts('true')", ControllerName = "Products", ActionName = "ActivateDeactivateProducts" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "Products.prototype.IsActiveProducts('false')", ControllerName = "Products", ActionName = "ActivateDeactivateProducts" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextBulkUpdates, JSFunctionName = "Products.prototype.GetDialogUpdateProduct('false')", ControllerName = "Products", ActionName = "UpdateProducts" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.CSV,
                    ControllerName = "Export",
                    ActionName = "Export",
                    Value = "2",
                    Type = "Product",
                    JSFunctionName = "Products.prototype.Export(event)"
                });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.Excel,
                    ControllerName = "Export",
                    ActionName = "Export",
                    Value = "1",
                    Type = "Product",
                    JSFunctionName = "Products.prototype.Export(event)"
                });

            }
        }

        //Set tool menu for assigned linked product list grid view.
        private void SetAssignedLinkedProductListToolMenu(ProductDetailsListViewModel model, int attributeId)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('LinkDeletePopupId" + attributeId + "',this)", ControllerName = "Products", ActionName = "UnAssignLinkProducts" });
            }
        }

        //Set tool menu for associated product list grid view.
        protected virtual void SetAssociatedProductListToolMenu(ProductDetailsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedProductDeletePopup',this)", ControllerName = "Products", ActionName = "UnassociateProducts" });
                
                //if product is an configurable then add/remove option in tools dropdown.
                if (string.Equals(model.ProductType, ZnodeConstant.ConfigurableProduct, StringComparison.InvariantCultureIgnoreCase))
                     model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.SetDefaultVariantValue, JSFunctionName = "Products.prototype.UpdateDefaultProduct()", ControllerName = "Products", ActionName = "UpdateAssociatedProducts" });
            }
        }

        //Set tool menu for custom field list grid view.
        private void SetCustomFieldListToolMenu(CustomFieldListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Products", ActionName = "DeleteCustomField" });
            }
        }

        //This method will delete the temporary files from the server.
        private void RemoveTemporaryFiles(string fileName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(AdminConstants.ImportFolderPath));

            //Delete file from the directory.
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.FullName.Equals(fileName))
                {
                    file.Delete();
                    break;
                }
            }
        }

        //Add PIM Catalog id in filter collection
        private void AddPIMCatalogIdInFilters(FilterCollection filters, int pimCatalogId)
        {
            if (pimCatalogId > 0 || pimCatalogId == -1)
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.CatalogId, StringComparison.InvariantCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.CatalogId.ToString(), FilterOperators.Equals, pimCatalogId.ToString()));
            }
        }

        //Map catalog filter values in view model
        private void BindCatalogFilterValues(ProductDetailsListViewModel productsDetailsList, int pimCatalogId, string catalogName)
        {
            productsDetailsList.PimCatalogName = string.IsNullOrEmpty(catalogName) ? Admin_Resources.LabelAllProducts : catalogName;
            productsDetailsList.PimCatalogId = pimCatalogId;
        }
    }
}