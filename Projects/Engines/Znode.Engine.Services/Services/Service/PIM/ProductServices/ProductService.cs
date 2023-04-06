using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Xml.Linq;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin.Import;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class ProductService : BaseService, IProductService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePimProduct> _PimProduct;
        private readonly IZnodeRepository<ZnodePimLinkProductDetail> _linkDetailRepository;
        private readonly IZnodeRepository<ZnodePimProductTypeAssociation> _productTypeAssociationRepository;
        private IZnodeRepository<ZnodePimFamilyGroupMapper> _familyGroupMapperRepository;
        private IZnodeRepository<ZnodePimAttributeGroupMapper> _attributeGroupMapperRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _attributeRepository;
        private readonly IZnodeRepository<ZnodePimCustomFieldLocale> _customFieldLocaleRepository;
        private readonly IZnodeRepository<ZnodePimCustomField> _customFieldRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValue> _attributeValueRepository;
        private readonly IZnodeRepository<ZnodePimAddOnProduct> _addonProductRepository;
        private readonly IZnodeRepository<ZnodePimAddOnProductDetail> _addonProductDetailRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValueLocale> _attributeValueLocaleRepository;
        private readonly IZnodeRepository<ZnodePimAttributeLocale> _attributeLocaleRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValue> _pimAttributeDefaultValueRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValueLocale> _pimAttributeDefaultValueLocaleRepository;
        private readonly IZnodeRepository<ZnodeLocale> _localeRepository;
        private IZnodeRepository<ZnodeHighlight> _HighlightsRepository;
        private IZnodeRepository<ZnodeHighlightLocale> _highlightLocaleRepository;
        private IZnodeRepository<ZnodeMedia> _mediaRepository;
        private readonly IZnodeRepository<ZnodePimCategoryProduct> _pimCategoryProductRepository;
        private readonly IZnodeRepository<ZnodePimConfigureProductAttribute> _configureProductAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAddonGroupProduct> _addonGroupProduct;
        private readonly IZnodeRepository<ZnodePublishedXml> _publishedXmlRepository;
        private readonly IZnodeRepository<ZnodeImportSuccessLog> _importSuccessLog;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _seoDetailRepository;
        private readonly IZnodeRepository<ZnodePublishCatalogLog> _publishCatalogLogRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly ProductAssociationHelper productAssociationHelper;
        private readonly IZnodeRepository<ZnodeExportProcessLog> _znodeExportProcessLog;
        
        private readonly IImportHelper importHelper;

        private const string linkAttributes = "Link";
        private const string addOn = "AddOns";
        private const string personalization = "Personalization";
        private const string CustomFields = "CustomFields";
        private const string Downloadable = "Downloadable";
        private const string _skuText = "sku";
        private bool isPublishForGuid;
        #endregion Private Variables

        #region Public Constructor

        public ProductService()
        {
            _PimProduct = new ZnodeRepository<ZnodePimProduct>();
            _linkDetailRepository = new ZnodeRepository<ZnodePimLinkProductDetail>();
            _productTypeAssociationRepository = new ZnodeRepository<ZnodePimProductTypeAssociation>();
            _familyGroupMapperRepository = new ZnodeRepository<ZnodePimFamilyGroupMapper>();
            _attributeGroupMapperRepository = new ZnodeRepository<ZnodePimAttributeGroupMapper>();
            _attributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _attributeValueRepository = new ZnodeRepository<ZnodePimAttributeValue>();
            _customFieldLocaleRepository = new ZnodeRepository<ZnodePimCustomFieldLocale>();
            _customFieldRepository = new ZnodeRepository<ZnodePimCustomField>();
            _addonProductRepository = new ZnodeRepository<ZnodePimAddOnProduct>();
            _addonProductDetailRepository = new ZnodeRepository<ZnodePimAddOnProductDetail>();
            _attributeValueLocaleRepository = new ZnodeRepository<ZnodePimAttributeValueLocale>();
            _attributeLocaleRepository = new ZnodeRepository<ZnodePimAttributeLocale>();
            _localeRepository = new ZnodeRepository<ZnodeLocale>();
            _HighlightsRepository = new ZnodeRepository<ZnodeHighlight>();
            _highlightLocaleRepository = new ZnodeRepository<ZnodeHighlightLocale>();
            _mediaRepository = new ZnodeRepository<ZnodeMedia>();
            _pimAttributeDefaultValueRepository = new ZnodeRepository<ZnodePimAttributeDefaultValue>();
            _pimAttributeDefaultValueLocaleRepository = new ZnodeRepository<ZnodePimAttributeDefaultValueLocale>();
            _pimCategoryProductRepository = new ZnodeRepository<ZnodePimCategoryProduct>();
            _configureProductAttributeRepository = new ZnodeRepository<ZnodePimConfigureProductAttribute>();
            _addonGroupProduct = new ZnodeRepository<ZnodePimAddonGroupProduct>();
            _publishedXmlRepository = new ZnodeRepository<ZnodePublishedXml>();
            _importSuccessLog = new ZnodeRepository<ZnodeImportSuccessLog>();
            _seoDetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
            _publishCatalogLogRepository = new ZnodeRepository<ZnodePublishCatalogLog>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            importHelper = GetService<IImportHelper>();
            productAssociationHelper = new ProductAssociationHelper();
           _znodeExportProcessLog = new ZnodeRepository<ZnodeExportProcessLog>();
        }

        #endregion Public Constructor

        #region Products List

        //Get paged Products list
        public virtual ProductDetailsListModel GetProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Sets to true when need to exclude some products from list.
            bool isNotInFilter = false;

            string productIdsToExclude = GetProductIdsToExclude(filters, out isNotInFilter);
            ZnodeLogging.LogMessage("productIdsToExclude returned from GetProductIdsToExclude:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, productIdsToExclude);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetXmlProduct:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            var xmlList = GetXmlProduct(filters, pageListModel, productIdsToExclude, isNotInFilter);

            ProductDetailsListModel productList = new ProductDetailsListModel
            {
                Locale = GetActiveLocaleList(),
                AttributeColumnName = xmlList.AttributeColumnName,
                XmlDataList = xmlList.XmlDataList
            };
            productList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return productList;
        }

        //Filters product list according to parameters.
        public virtual ProductDetailsListModel GetProductList(ParameterModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorParameterModelNull);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetXmlProduct:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            var xmlList = GetXmlProduct(filters, pageListModel, model.Ids, false);

            ProductDetailsListModel productList = new ProductDetailsListModel
            {
                Locale = GetActiveLocaleList(),
                AttributeColumnName = xmlList.AttributeColumnName,
                XmlDataList = xmlList.XmlDataList
            };

            productList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return productList;
        }

        //Filters product list according to parameters.
        public virtual ProductDetailsListModel GetProductBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Sets to true when need to exclude some products from list.
            bool isNotInFilter = false;
            string productIdsToExclude = GetProductIdsToExclude(filters, out isNotInFilter);
            ZnodeLogging.LogMessage("productIdsToExclude returned from GetProductIdsToExclude", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, productIdsToExclude);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetXmlBrandProduct GetXmlBrandProduct:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            var xmlList = GetXmlBrandProduct(filters, pageListModel, productIdsToExclude, false);

            ProductDetailsListModel productList = new ProductDetailsListModel
            {
                Locale = GetActiveLocaleList(),
                AttributeColumnName = xmlList.AttributeColumnName,
                XmlDataList = xmlList.XmlDataList
            };

            productList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return productList;
        }

        //Not being used, same call also created in category service. Need to verify for it's removal
        //Get Product By Category
        public virtual CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters categoryId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,new object[] { categoryId});

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //Get associated ProductIds
            string associatedProductIds = string.Join(",", _pimCategoryProductRepository.Table?.Where(x => x.PimCategoryId == categoryId)?.Select(y => Equals(y.PimProductId, null) ? 0 : y.PimProductId)?.ToArray());
            associatedProductIds = associatedProductIds.Length > 0 ? associatedProductIds : "0";
            ZnodeLogging.LogMessage("associatedProductIds:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, associatedProductIds);


            ZnodeLogging.LogMessage("GetProductDetailsListModel method call with parameters pageListModel, associatedProductIds :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,new object[] { pageListModel?.ToDebugString(), associatedProductIds });
            ProductDetailsListModel productDetailsListModel = GetProductDetailsListModel(filters, pageListModel, associatedProductIds);

            CategoryProductListModel categoryProductListModel = new CategoryProductListModel { CategoryProducts = productDetailsListModel.ProductDetailList?.Count > 0 ? productDetailsListModel.ProductDetailList.ToEntity<CategoryProductModel>()?.ToList() : null };

            //Get PimCategoryProductId
            if (!associatedProducts && categoryProductListModel.CategoryProducts?.Count > 0)
                foreach (var item in categoryProductListModel.CategoryProducts)
                    item.PimCategoryProductId = _pimCategoryProductRepository.Table.Where(x => x.PimCategoryId == categoryId && x.PimProductId == item.PimProductId).Select(g => g.PimCategoryProductId).FirstOrDefault();

            categoryProductListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryProductListModel;
        }

        //Get list of un-associated simple product.
        public virtual ProductDetailsListModel GetUnassociatedProducts(int parentProductId, string alreadyAssociatedProductIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Method GetUnassociatedProducts - execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Method GetAssociatedUnAssociatedCategoryProducts - parentProductId, alreadyAssociatedProductIds  :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId, alreadyAssociatedProductIds });

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetProductDetailsListModel method:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //Get associated product ids.
            string associatedProductIds = string.Empty;
            if (parentProductId > 0)
            {
                associatedProductIds = string.Join(",", _productTypeAssociationRepository.Table?.Where(x => x.PimParentProductId == parentProductId)?.Select(y => Equals(y.PimProductId, null) ? 0 : y.PimProductId)?.ToArray());
                associatedProductIds = string.IsNullOrEmpty(associatedProductIds) ? parentProductId.ToString() : string.Concat(parentProductId, ',', associatedProductIds);
                ZnodeLogging.LogMessage("Associated product ids :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, associatedProductIds);

            }
            else
            {
                associatedProductIds = alreadyAssociatedProductIds;
                ZnodeLogging.LogMessage("Associated product ids :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, associatedProductIds);
            }

            //Get simple product list other than associated product to parent product.
            int localeId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
            filters.Add(View_ManageProductListEnum.ProductType.ToString(), ProcedureFilterOperators.Is, GetProductTypeByLocale(localeId));

            return GetProductDetailsListModel(filters, pageListModel, associatedProductIds);
        }

        // Gets list of list of products which are not associated as link products to the parent product.
        public virtual ProductDetailsListModel GetUnAssociatedLinkProducts(int parentProductId, int linkAttributeId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("parentProductId, linkAttributeId  :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId, linkAttributeId });

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetProductDetailsListModel method:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            string associatedProductIds = string.Empty;

            if (parentProductId > 0)
            {
                associatedProductIds = string.Join(",", _linkDetailRepository.Table?.Where(x => x.PimParentProductId == parentProductId && x.PimAttributeId == linkAttributeId)?.Select(y => Equals(y.PimProductId, null) ? 0 : y.PimProductId)?.ToArray());
                if (string.IsNullOrEmpty(associatedProductIds))
                {
                    associatedProductIds = parentProductId.ToString();
                }
                else if (!(associatedProductIds.Split(',').ToList().Contains(parentProductId.ToString())))
                {
                    associatedProductIds = string.Concat(parentProductId, ',', associatedProductIds);
                }                
            }
            ZnodeLogging.LogMessage("associatedProductIds :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, associatedProductIds);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return GetProductDetailsListModel(filters, pageListModel, associatedProductIds);
        }       

        //Get unassociated addon product by addonProductId.
        public virtual ProductDetailsListModel UnassociatedAddonProducts(int addonProductId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("addonProductId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonProductId);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetProductDetailsListModel method :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            int localeId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
            string associatedProductIds = string.Empty;
            string parentProductId = string.Empty;
            foreach (FilterTuple filter in filters)
            {
                parentProductId = filters.Where(x => x.FilterName == ZnodePimAddOnProductEnum.PimProductId.ToString().ToLower())?.Select(x => x.FilterValue).FirstOrDefault();
                break;
            }

            parentProductId = filters.Where(x => x.FilterName == ZnodePimAddOnProductEnum.PimProductId.ToString().ToLower())?.Select(x => x.FilterValue).FirstOrDefault();
            ZnodeLogging.LogMessage("parentProductId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, parentProductId);
            if (addonProductId > 0)
            {
                associatedProductIds = string.Join(",", _addonProductDetailRepository.Table?.Where(x => x.PimAddOnProductId == addonProductId)?.Select(y => Equals(y.PimChildProductId, null) ? 0 : y.PimChildProductId)?.ToArray());
                associatedProductIds = string.IsNullOrEmpty(associatedProductIds) ? parentProductId : string.Concat(associatedProductIds, "," + Convert.ToString(parentProductId));
            }

            //gets the where clause with filter Values.
            filters.Add(View_ManageProductListEnum.ProductType.ToString(), ProcedureFilterOperators.Is, GetProductTypeByLocale(localeId));

            filters.RemoveAll(x => x.FilterName == ZnodePimAddOnProductEnum.PimProductId.ToString().ToLower());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString().ToLower(), FilterOperators.Equals, localeId.ToString());
            return GetProductDetailsListModel(filters, pageListModel, associatedProductIds);
        }

        private ProductDetailsListModel GetProductDetailsListModel(FilterCollection filters, PageListModel pageListModel, string associatedProductIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            var xmlList = GetXmlProduct(filters, pageListModel, associatedProductIds, true);

            ProductDetailsListModel productList = new ProductDetailsListModel
            {
                Locale = GetActiveLocaleList(),
                AttributeColumnName = xmlList.AttributeColumnName,
                XmlDataList = xmlList.XmlDataList
            };

            productList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return productList;
        }

        #endregion Products List

        #region Product

        //Create/Update new product
        public virtual ProductModel CreateProduct(ProductModel model)
        {

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            var xmlData = HelperUtility.ToXML<List<ProductAttributeModel>>(model.ProductAttributeList);
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("ProductXml", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("CopyPimProductId", model.CopyProductId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> createResult = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdatePimProduct @ProductXml,@UserId, @status OUT, @CopyPimProductId", 1, out status);
            ZnodeLogging.LogMessage("createResult count : ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, createResult?.Count);

            if (createResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessSaveProduct, model.ProductAttributeList.Where(p => p.ProductAttributeCode == "SKU").FirstOrDefault().ProductAttributeCode, model.ProductAttributeList.Where(p => p.ProductAttributeCode == "SKU").FirstOrDefault().ProductAttributeValue), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                model.ProductId = createResult.FirstOrDefault().Id;

                ZnodeLogging.LogMessage("InsertCatalogData method call: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, null);
                //Inserts product data against catalog category.
                InsertProductDataForCatalogCategory(model);

                return model;
            }
            else
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorSaveProduct, model.ProductAttributeList[2].ProductAttributeCode, model.ProductAttributeList[2].ProductAttributeValue), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                if(!createResult.FirstOrDefault().Status.Value)
                        throw new ZnodeException(ErrorCodes.DuplicateProductKey,string.Format("{0} {1}", ZnodeConstant.Product, Admin_Resources.AlreadyExists),new Dictionary<string, string>() { { ZnodeConstant.PimProductId, createResult.FirstOrDefault().Id.ToString() } });
                return model;
            }
        }

        //Get product details by productId
        public virtual PIMFamilyDetailsModel GetProduct(PIMGetProductModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Get all attributes associated with ProductId and with familyId associated with ProductId
            IZnodeViewRepository<View_PimAttributeValues> pimAttributeValues = new ZnodeViewRepository<View_PimAttributeValues>();
            pimAttributeValues.SetParameter("ChangeFamilyId", model.FamilyId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValues.SetParameter("PimProductId", model.ProductId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValues.SetParameter(ZnodePimAttributeLocaleEnum.LocaleId.ToString(), model.LocaleId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValues.SetParameter("IsCopy", model.IsCopy, ParameterDirection.Input, DbType.Boolean);
            var pimAttributes = pimAttributeValues.ExecuteStoredProcedureList("Znode_GetPimProductAttributeValues @ChangeFamilyId, @PimProductId, @LocaleId,@IsCopy");
            ZnodeLogging.LogMessage("FamilyId and ProductId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { model?.FamilyId, model?.ProductId });

            if (model.FamilyId <= 0)
            {
                model.FamilyId = Convert.ToInt32(_PimProduct.Table.Where(x => x.PimProductId == model.ProductId).Select(x => x.PimAttributeFamilyId).FirstOrDefault());
            }
            bool isDownloadable = pimAttributes.Any(x => x.AttributeCode == "IsDownloadable" && x.AttributeValue == ZnodeConstant.TrueValue);
            ZnodeLogging.LogMessage("isDownloadable status: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, isDownloadable);

            //Get all groups associated with default family and familyId
            var pimAttributeGroups = (CategoryService.GetGroupsAssociatedWithFamily(model.FamilyId, false, model.LocaleId)).ToList();
            ZnodeLogging.LogMessage("all groups associated with family returned from GetGroupsAssociatedWithFamily: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimAttributeGroups?.Count});

            if (model.IsCopy)
            {
                //Remove additional group for product creation.
                pimAttributeGroups.RemoveAll(x => x.GroupType == linkAttributes);
                pimAttributeGroups.RemoveAll(x => x.GroupCode == addOn || x.GroupCode == personalization || x.GroupCode == CustomFields || x.GroupCode == Downloadable);
            }

            if (!isDownloadable)
                pimAttributeGroups?.RemoveAll(x => x.GroupCode == Downloadable);

            //Get All Pim category Families except the default family
            var pimAttributeFamilies = GetProductFamilies(false, model.LocaleId);
            ZnodeLogging.LogMessage(" All Pim category Families except the default family returned from pimAttributeFamilies : ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimAttributeFamilies?.Count });

            //Get All associated products
            string associatedProductIds = string.Join(",", _productTypeAssociationRepository.Table?.Where(x => x.PimParentProductId == model.ProductId)?.Select(y => Equals(y.PimProductId, null) ? 0 : y.PimProductId)?.ToArray());

            //Get All associated configure products attribute Ids
            string configureProductAttributeIds = GetAssociateProductAttributeIds(model.ProductId);
            ZnodeLogging.LogMessage(" All associated products ids and all associated configure products attribute Ids associatedProductIds, configureProductAttributeIds : ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { associatedProductIds, configureProductAttributeIds });

            return IsNotNull(pimAttributeGroups) && IsNotNull(pimAttributes) && IsNotNull(pimAttributeFamilies)
                ? MapToPIMFamilyDetailsModel(model.FamilyId, model.ProductId, pimAttributes, pimAttributeGroups, pimAttributeFamilies, associatedProductIds, configureProductAttributeIds) : null;
        }

        //Delete an exiting product
        public virtual bool DeleteProduct(ParameterModel productIds)
        {
            ZnodeLogging.LogMessage("Method DeleteProduct - execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("product ids to be deleted : ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { productIds?.Ids });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimProductId", productIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimProducts @PimProductId, @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { deleteResult?.Count });
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        // Get Product Attributes as per selected Family.
        public virtual PIMFamilyDetailsModel GetProductFamilyDetails(PIMFamilyModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Get all groups associated with default family and familyId
            IList<View_PimAttributeGroupbyFamily> AttributeGroups = CategoryService.GetGroupsAssociatedWithFamily(model.PIMAttributeFamilyId, model.IsCategory, model.LocaleId);
            var pimAttributeGroups = AttributeGroups.ToList();
            ZnodeLogging.LogMessage("All groups associated with default family and familyId count : ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimAttributeGroups?.Count });

            //Remove additional group for product creation.
            pimAttributeGroups.RemoveAll(x => x.GroupType == linkAttributes);
            pimAttributeGroups.RemoveAll(x => x.GroupCode == addOn || x.GroupCode == personalization || x.GroupCode == CustomFields || x.GroupCode == Downloadable);

            //Get all attributes associated with default family and familyId
            IZnodeViewRepository<View_PimAttributeValues> pimAttributeValues = new ZnodeViewRepository<View_PimAttributeValues>();
            pimAttributeValues.SetParameter(View_PimAttributeValuesEnum.PimAttributeFamilyId.ToString(), model.PIMAttributeFamilyId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValues.SetParameter(ZnodePimAttributeEnum.IsCategory.ToString(), model.IsCategory, ParameterDirection.Input, DbType.Boolean);
            pimAttributeValues.SetParameter(ZnodePimAttributeLocaleEnum.LocaleId.ToString(), model.LocaleId, ParameterDirection.Input, DbType.Int32);
            var pimAttributes = pimAttributeValues.ExecuteStoredProcedureList("Znode_GetPimAttributeValues @PimAttributeFamilyId, @IsCategory, @LocaleId");

            //Get all families except the default family
            var pimAttributeFamilies = GetProductFamilies(model.IsCategory, model.LocaleId);
            ZnodeLogging.LogMessage("pimAttributeFamilies returned from GetProductFamilies: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimAttributeFamilies });

            string associatedProductIds = string.Empty;
            string configureProductIds = string.Empty;
            if (model.Id > 0)
            {
                //Get All associated product ids.
                associatedProductIds = string.Join(",", _productTypeAssociationRepository.Table?.Where(x => x.PimParentProductId == model.Id)?.Select(y => Equals(y.PimProductId, null) ? 0 : y.PimProductId)?.ToArray());

                //Get All associated configure products ids.
                configureProductIds = string.Join(",", _configureProductAttributeRepository.Table?.Where(x => x.PimProductId == model.Id)?.Select(y => Equals(y.PimAttributeId, null) ? 0 : y.PimAttributeId)?.ToArray());

            }

            ZnodeLogging.LogMessage("associatedProductIds and configureProductIds: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { associatedProductIds, configureProductIds });

            return IsNotNull(pimAttributeGroups) && IsNotNull(pimAttributes) && IsNotNull(pimAttributeFamilies)
                 ? MapToPIMFamilyDetailsModel(model.PIMAttributeFamilyId, model.Id, pimAttributes, pimAttributeGroups, pimAttributeFamilies, associatedProductIds, configureProductIds) : null;
        }

        //Get list of configure attributes associated with ProductId and familyId
        public virtual PIMFamilyDetailsModel GetConfigureAttributes(PIMFamilyModel pimFamilyModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_GetConfigureAttributeDetail> pimAttributeValues = new ZnodeViewRepository<View_GetConfigureAttributeDetail>();
            pimAttributeValues.SetParameter("PimFamilyId", pimFamilyModel.PIMAttributeFamilyId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValues.SetParameter("PimProductId", pimFamilyModel.Id, ParameterDirection.Input, DbType.Int32);
            var pimAttributes = pimAttributeValues.ExecuteStoredProcedureList("Znode_GetConfigureAttributeDetail @PimFamilyId,@PimProductId");
            PIMFamilyDetailsModel pimFamilyDetailsModel = new PIMFamilyDetailsModel { Attributes = pimAttributes.ToModel<PIMProductAttributeValuesModel>().ToList(), };

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimFamilyDetailsModel;
        }

        //Get list of associated and unassociated product which contain configure attributes.
        public virtual ProductDetailsListModel GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters parentProductId,associatedProductIds :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId, associatedProductIds });

            //Get associated product Ids.
            associatedProductIds = parentProductId > 0 ? string.Join(",", _productTypeAssociationRepository.Table?.Where(x => x.PimParentProductId == parentProductId)?.Select(y => Equals(y.PimProductId, null) ? 0 : y.PimProductId)?.ToArray()) : associatedProductIds;
            associatedProductIds = !string.IsNullOrEmpty(associatedProductIds) ? associatedProductIds : "0";

            associatedAttributeIds = !string.IsNullOrEmpty(associatedAttributeIds) ? associatedAttributeIds : "0";
            associatedAttributeIds = parentProductId > 0 && associatedAttributeIds == "0" ? GetAssociateProductAttributeIds(parentProductId) : associatedAttributeIds;

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();

            int LocaleId = CategoryService.GetLocaleId(filters);
            string whereClause = GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            executeSpHelper.GetParameter("@WhereClause", whereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            executeSpHelper.GetParameter("@PimAttributeIds", associatedAttributeIds, ParameterDirection.Input, SqlDbType.NVarChar);
            executeSpHelper.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            executeSpHelper.GetParameter("@LocaleId", LocaleId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@PimProductId", associatedProductIds, ParameterDirection.Input, SqlDbType.VarChar);
            executeSpHelper.GetParameter("@IsProductNotIn", pimProductIdsIn, ParameterDirection.Input, SqlDbType.Bit);
            executeSpHelper.GetParameter("@RelatedProductId", parentProductId, ParameterDirection.Input, SqlDbType.Int);

            DataSet resultDataSet = executeSpHelper.GetSPResultInDataSet("Znode_ManageProductListByAttributes");

            ProductDetailsListModel list = new ProductDetailsListModel();
            if (IsNotNull(resultDataSet) && associatedAttributeIds != "0")
            {
                list.NewAttributeList = DataTableToList(resultDataSet.Tables[0]);
                list.ProductDetailListDynamic = DataTableToList(resultDataSet.Tables[1]);

                if (!string.IsNullOrEmpty(resultDataSet.Tables[2].Rows[0]["RowsCount"].ToString()))
                    pageListModel.TotalRowCount = Convert.ToInt32(resultDataSet.Tables[2].Rows[0]["RowsCount"].ToString());
            }
            list.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return list;
        }

        [Obsolete]
        //Unused method
        //Return List of product on basis of where clause
        public IList<View_ManageProductList> ProductList(FilterCollection filters, PageListModel pageListModel, string pimProductIds, bool pimProductIdsNotIn = false)
        {
            int localeId = CategoryService.GetLocaleId(filters);
            pimProductIdsNotIn = GetIsProductNotInValue(filters, pimProductIdsNotIn);
            bool isCallForAttribute = GetIsCallForAttribute(filters);
            string whereClause = GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            string attributeCode = GetAttributeCodes(filters);
            IZnodeViewRepository<View_ManageProductList> objStoredProc = new ZnodeViewRepository<View_ManageProductList>();
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PimProductId", pimProductIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@IsProductNotIn", pimProductIdsNotIn, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@IsCallForAttribute", isCallForAttribute, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@AttributeCode", attributeCode, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList("Znode_ManageProductList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PimProductId,@IsProductNotIn,@IsCallForAttribute,@AttributeCode", 4, out pageListModel.TotalRowCount);
        }

        [Obsolete]
        //Unused method
        //Return List of product on basis of where clause
        public ProductDetailsListModel ProductListXML(FilterCollection filters, PageListModel pageListModel, string pimProductIds, bool pimProductIdsNotIn = false)
        => GetXmlProduct(filters, pageListModel, pimProductIds, pimProductIdsNotIn);

        //Gets the attribute code form filters.
        protected string GetAttributeCodes(FilterCollection filters)
        {
            if (filters?.Count > 0)
            {
                string attributeCode = string.Join(",", filters.Select(x => x.FilterName).ToArray());
                if (!string.IsNullOrEmpty(attributeCode) && attributeCode.Contains("|"))
                    attributeCode = attributeCode.Replace('|', ',');
                return attributeCode;
            }
            return string.Empty;
        }
        
        //Return List of product on basis of where clause
        public virtual ProductDetailsListModel GetXmlProduct(FilterCollection filters, PageListModel pageListModel, string pimProductIdsNotIn, bool pimProductIdsIn = false)
        {
            DataSet ds = GetXmlProductsDataSet(filters, pageListModel, pimProductIdsNotIn, ref pimProductIdsIn);
            return Xmldataset(ds, out pageListModel.TotalRowCount);
        }

        public virtual DataSet GetXmlProductsDataSet(FilterCollection filters, PageListModel pageListModel, string pimProductIdsNotIn, ref bool pimProductIdsIn)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Updating the filter value to 'lk' for IsActive Flag
            filters = UpdateFiltersForIsActiveFlag(filters);
            int localeId = CategoryService.GetLocaleId(filters);
            pimProductIdsIn = GetIsProductNotInValue(filters, pimProductIdsIn);
            bool isCatalogFilter = false;
            //Get catalog filter values from FilterCollection.
            int pimCatalogId = ServiceHelper.GetCatalogFilterValues(filters, ref isCatalogFilter);

            bool isCallForAttribute = GetIsCallForAttribute(filters);

            string attributeCode = GetAttributeCodes(filters);
            //Get publish status value from  filter collection
            string publishStatus = GetPublishStatusFromFilterCollection(filters);
            string whereClause = GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause and attributeCode to set SP parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,new object[] { whereClause, attributeCode });

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

            objStoredProc.GetParameter("@WhereClause", whereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PimProductId", pimProductIdsNotIn, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@IsProductNotIn", pimProductIdsIn, ParameterDirection.Input, SqlDbType.Bit);
            objStoredProc.GetParameter("@IsCallForAttribute", isCallForAttribute, ParameterDirection.Input, SqlDbType.Bit);
            objStoredProc.GetParameter("@AttributeCode", attributeCode, ParameterDirection.Input, SqlDbType.VarChar);
            objStoredProc.GetParameter("@PimCatalogId", pimCatalogId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@IsCatalogFilter", isCatalogFilter, ParameterDirection.Input, SqlDbType.Bit);
            objStoredProc.GetParameter("@publishstatus", publishStatus, ParameterDirection.Input, SqlDbType.NVarChar);

            var ds = objStoredProc.GetSPResultInDataSet("Znode_ManageProductList_XML");
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return ds;
        }

        //Return List of product on basis of where clause
        protected virtual ProductDetailsListModel GetXmlBrandProduct(FilterCollection filters, PageListModel pageListModel, string pimProductIdsNotIn, bool pimProductIdsIn = false)
        {
            int localeId = CategoryService.GetLocaleId(filters);
            pimProductIdsIn = GetIsProductNotInValue(filters, pimProductIdsIn);
            bool isCallForAttribute = GetIsCallForAttribute(filters);
            string brandCode = GetBrandCode(filters);
            string whereClause = GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            string attributeCode = GetAttributeCodes(filters);
            ZnodeLogging.LogMessage("brandCode and attributeCode returned from GetBrandCode and GetAttributeCodes:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { brandCode, attributeCode });

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@WhereClause", whereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@BrandCode", brandCode, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@IsAssociated", isCallForAttribute, ParameterDirection.Input, SqlDbType.Bit);
            objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PimProductId", pimProductIdsNotIn, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@IsProductNotIn", pimProductIdsIn, ParameterDirection.Input, SqlDbType.Bit);
            objStoredProc.GetParameter("@IsCallForAttribute", isCallForAttribute, ParameterDirection.Input, SqlDbType.Bit);
            objStoredProc.GetParameter("@AttributeCode", attributeCode, ParameterDirection.Input, SqlDbType.VarChar);
            var ds = objStoredProc.GetSPResultInDataSet("Znode_ManageBrandProductList_XML");

            return Xmldataset(ds, out pageListModel.TotalRowCount);
        }

        //Generate XML where clause for SP filters.
        public static string GenerateXMLWhereClauseForSP(FilterDataCollection filters)
        {
            return DynamicClauseHelper.GenerateWhereClauseForSP(filters);
        }

        //Generate where clause for Publish Status Filter 
        protected virtual string GetPublishStatusFromFilterCollection(FilterCollection filters)
        {
            string publishStatus = string.Empty;
            if (filters.Any(x => string.Equals(x.FilterName, FilterKeys.PublishStatus, StringComparison.CurrentCultureIgnoreCase)))
            {
                publishStatus = DynamicClauseHelper.GenerateWhereClauseForPublishStatusFilter(filters.ToFilterDataCollection());
                filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.PublishStatus, StringComparison.InvariantCultureIgnoreCase));
            }
            return publishStatus;
        }
        
        //update filter collection for IsActive Flag (Changing the Filter Operator from 'eq' to 'lk')
        protected virtual FilterCollection UpdateFiltersForIsActiveFlag(FilterCollection filters)
        {
            if (filters.Any(x => string.Equals(x.FilterName, FilterKeys.IsActive, StringComparison.CurrentCultureIgnoreCase)))
            {
                FilterTuple filterTuple = filters.FirstOrDefault(x => string.Equals(x.FilterName, FilterKeys.IsActive, StringComparison.InvariantCultureIgnoreCase));
                string filterName = filterTuple.FilterName;
                string filterValue = filterTuple.FilterValue;

                //Update filter Operator from 'eq' to 'lk' if filtervalue is not null.
                if (!string.IsNullOrEmpty(filterValue))
                {
                    filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsActive, StringComparison.InvariantCultureIgnoreCase));
                    filters.Add(new FilterTuple(filterName, FilterOperators.Like, filterValue));
                }
            }
            return filters;
        }

        //Activate/Deactivate products in bulk
        public virtual bool ActivateDeactivateProducts(ActivateDeactivateProductsModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("ProductId", model.ProductIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("PimAttributeCode", "IsActive", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("LocaleId", model.LocaleId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("AttributeValue", model.IsActive.ToString().ToLower(), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            ZnodeLogging.LogMessage("ActivateDeactivateProductsModel with product id:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { model?.ProductIds });

            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateAttributeValue @ProductId,@PimAttributeCode,@LocaleId,@AttributeValue,@UserId, @Status OUT", 5, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessProductStatus, model.IsActive), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorProductStatus, model.IsActive) , ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Update Product Attribute Code Value.
        public virtual ProductAttributeCodeValueListModel UpdateProductAttributeValue(AttributeCodeValueModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (string.IsNullOrEmpty(model.SKU))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorSKUNull);

            if (string.IsNullOrEmpty(model.LocaleCode))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, PIM_Resources.ErrorLocaleEmpty);

            ZnodeLogging.LogMessage("AttributeCodeValueModel with SKU:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { model?.SKU });

            if (model.PIMAttributeCodeValueList?.Count > 0)
            {
                var xmlData = HelperUtility.ToXML<List<PIMAttributeCodeValueModel>>(model.PIMAttributeCodeValueList);

                IZnodeViewRepository<ProductAttributeCodeValueModel> objStoredProc = new ZnodeViewRepository<ProductAttributeCodeValueModel>();
                objStoredProc.SetParameter("SKU", model.SKU, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("LocaleCode", model.LocaleCode, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("AttributeCodeValues", xmlData, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;

                IList<ProductAttributeCodeValueModel> result = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateProductAttributeValue @SKU,@LocaleCode,@AttributeCodeValues,@UserId, @Status OUT", 4, out status);
                ProductAttributeCodeValueListModel listModel = new ProductAttributeCodeValueListModel();

                if (status == 1)
                {
                    listModel.Status = true;
                    ZnodeLogging.LogMessage(PIM_Resources.SuccessUpdateProductAttributeValue, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    if (model.IsPublish)
                        PublishProductBySKU(model.SKU);
                    return listModel;
                }
                else
                {
                    listModel.AttributeCodeValueList = result?.Count > 0 ? result.ToList() : null;
                    listModel.Status = false;
                    ZnodeLogging.LogMessage(PIM_Resources.ErrorUpdateProductAttributeValue, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return listModel;
                }
            }
            else
            {
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorNoAttributeCodeValue);
            }
        }

        #endregion Product

        #region Publish Product

        //Publish a Product using sku.
        public virtual void PublishProductBySKU(string sku)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            int productId = GetPimProductIdBySKU(sku);
            ZnodeLogging.LogMessage("productId returned from GetPimProductIdBySKU: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { productId });

            if (productId > 0)
            {
                ParameterModel parameterModel = new ParameterModel() { Ids = productId.ToString() };

                try
                {
                    PublishedModel publishedModel = Publish(parameterModel);
                    ZnodeLogging.LogMessage($"{publishedModel.ErrorMessage}");
                }
                catch (ZnodeException zer)
                {
                    ZnodeLogging.LogMessage(zer.ErrorMessage);
                }
            }
            else
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorPublishProduct, sku));
        }

        //Clear cache after product publish
        public virtual void ClearCacheAfterProductPublish(List<ProductAssociationPublishModel> publishProductModel)
        {
            if (HelperUtility.IsNull(publishProductModel))
            {
                return;
            }

            List<PublishedProductEntityModel> _productEntityForClearCache = new List<PublishedProductEntityModel>();

            foreach (ProductAssociationPublishModel associationPublishModel in publishProductModel)
            {
                _productEntityForClearCache.Add(new PublishedProductEntityModel { ZnodeCatalogId = associationPublishModel.PublishCatalogId, ZnodeProductId = associationPublishModel.PublishProductId, SeoUrl = associationPublishModel.SEOUrl });
            }

            var clearCacheInitializer = new ZnodeEventNotifier<IEnumerable<PublishedProductEntityModel>>(_productEntityForClearCache);
        }

        //Publish a single pim product using pim product id. 
        public virtual PublishedModel Publish(ParameterModel parameterModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("ParameterModel with ids: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parameterModel?.Ids });

            PublishProductDataService publishProductDataService = (PublishProductDataService)GetService<IPublishProductDataService>();

            int pimProductId = 0;
            bool status = false;
            bool isPublished = false;

            if (!string.IsNullOrEmpty(parameterModel?.Ids))
                int.TryParse(parameterModel?.Ids, out pimProductId);
                     
            //Bind revision type if revision type is empty, means product is been published in only Production mode. 
            string revisionType = string.IsNullOrEmpty(parameterModel?.RevisionType) ? "NONE" : parameterModel?.RevisionType;

            //Check whether any other catalog is in publish state or not to prevent DB deadlock. #Step 1
            if (publishProductDataService.IsCatalogPublishInProgress())
                throw new ZnodeException(ErrorCodes.ProductPublishError, PIM_Resources.ErrorPublishSingleProduct);

            //Check whether product is associated with a already published category. #Step 2
            if (!publishProductDataService.IsProductAssociateWithPublishCategory(Convert.ToInt32(pimProductId)))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ProductPublishError);

            if (publishProductDataService.IsExportPublishInProgress())
            {
               throw new ZnodeException(ErrorCodes.NotPermitted, PIM_Resources.ErrorPublishCatalog);
            }

            try
            {
                //Call master sp to perform the single product publish operation. #Step 3
                DataSet dataSet = publishProductDataService.ProcessSingleProductPublish(pimProductId, revisionType, out status);

                //Only perform if status is true, false means master sp task failed
                if (status)
                {
                    //Collect product data from datatable to use it for product elastic search index creation. #Step 4
                    List<ZnodePublishProductEntity> publishProductEntityList = new List<ZnodePublishProductEntity>();
                    publishProductEntityList = dataSet?.Tables[0]?.ToList<ZnodePublishProductEntity>();

                    //Perform elastic index creation operation of products. #Step 5
                    foreach (string revisiontype in publishProductDataService.GetRevisionTypesForElasticIndex(revisionType))
                        publishProductDataService.CreateProductElasticIndex(publishProductEntityList?.Where(x => x.RevisionType.Equals(revisiontype, StringComparison.CurrentCultureIgnoreCase)).ToList(), revisiontype);

                    //Collect product(s) data from datatable to remove product data from cache #Step 6
                    List<ProductAssociationPublishModel> productList = new List<ProductAssociationPublishModel>();
                    productList = dataSet?.Tables[1]?.ToList<ProductAssociationPublishModel>();

                    //Call update store procedure to update associated & linked products data
                    PublishedProductDataService publishedProductDataService = (PublishedProductDataService)GetService<IPublishedProductDataService>();
                    publishedProductDataService.UpdatePublishedProductAssociatedData();
                    
                    //Clear webstore and cloudflare cache after product publish #Step 7
                    ClearCacheAfterProductPublish(productList);

                    //Update flag value true if all operation of catalog publish execute successfully. #Step 8
                    isPublished = true;
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                //Return model with proper details. #Step 8
                return isPublished ? new PublishedModel { IsPublished = true, ErrorMessage = Admin_Resources.SuccessPublish }
                                     : new PublishedModel { IsPublished = false, ErrorMessage = Admin_Resources.ErrorPublished };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        #endregion Publish Product

        #region ProductPrivateMethod

        //Get All Pim category Families except the default family
        private List<PIMAttributeFamilyModel> GetProductFamilies(bool isCategory, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters localeId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { localeId });

            //Get all family with familyId
            IZnodeViewRepository<PIMAttributeFamilyModel> pimAttributeValues = new ZnodeViewRepository<PIMAttributeFamilyModel>();
            pimAttributeValues.SetParameter(ZnodePimAttributeEnum.IsCategory.ToString(), isCategory, ParameterDirection.Input, DbType.Boolean);
            pimAttributeValues.SetParameter(ZnodePimAttributeLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            var pimFamilies = pimAttributeValues.ExecuteStoredProcedureList("Znode_GetPimAttributeFamilyList @IsCategory, @LocaleId");
            ZnodeLogging.LogMessage("pimFamilies count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimFamilies?.Count });

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimFamilies.ToList();
        }

        private PIMFamilyDetailsModel MapToPIMFamilyDetailsModel(int familyId, int ProductId, IList<View_PimAttributeValues> pimAttributes, IList<View_PimAttributeGroupbyFamily> pimAttributeGroups,
                        List<PIMAttributeFamilyModel> pimAttributeFamilies, string associatedProductIds, string configureAttributeIds)
        => new PIMFamilyDetailsModel
        {
            PimAttributeFamilyId = familyId,
            Attributes = pimAttributes.ToModel<PIMProductAttributeValuesModel>().ToList(),
            Groups = pimAttributeGroups.ToModel<PIMAttributeGroupModel>().ToList(),
            Family = pimAttributeFamilies,
            Name = pimAttributes.Where(x => x?.AttributeCode == "ProductName")?.FirstOrDefault()?.AttributeValue,
            SKU = pimAttributes.Where(x => x?.AttributeCode == ZnodeConstant.ProductSKU)?.FirstOrDefault()?.AttributeValue,
            Id = ProductId,
            AssociatedProductIds = Equals(associatedProductIds, string.Empty) ? "0" : associatedProductIds,
            ConfigureAttributeIds = Equals(configureAttributeIds, string.Empty) ? "0" : configureAttributeIds,
            Locale = _localeRepository.Table.Where(x => x.IsActive).ToModel<LocaleModel>().ToList(),
            ProductPublishId = ProductId
        };

        #endregion ProductPrivateMethod

        #region ProductType

        //Get list of associated simple product to parent product.
        public virtual ProductDetailsListModel GetAssociatedProducts(int parentProductId, int attributeId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters parentProductId, attributeId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId, attributeId });

            int localeId = CategoryService.GetLocaleId(filters);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //gets the where clause with filter Values.
            string whereClause = GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause returned from GenerateXMLWhereClauseForSP", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClause });

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

            objStoredProc.GetParameter("@WhereClause", whereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@RelatedProductId", parentProductId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@IsAssociated", false, ParameterDirection.Input, SqlDbType.Bit);
            objStoredProc.GetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, SqlDbType.Int);
            objStoredProc.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            DataSet ds = objStoredProc.GetSPResultInDataSet("Znode_ManageProductTypeAssociationList");

            ProductDetailsListModel xmlList = Xmldataset(ds, out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return GetProductDetailListModelFromXML(pageListModel, xmlList);
        }

        //Associate product to parent product.
        public virtual bool AssociateProduct(ProductTypeAssociationListModel listModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (Equals(listModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            //Code that remove duplicated ids which was already associated.
            int parentProductId = Convert.ToInt32(listModel.AssociatedProducts.Select(a => a.PimParentProductId).FirstOrDefault());
            ZnodeLogging.LogMessage("parentProductId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId });

            var associatedProductList = _productTypeAssociationRepository.Table?.Where(x => x.PimParentProductId == parentProductId).Select(y => y.PimProductId);
            ZnodeLogging.LogMessage("parentProductId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId });

            listModel.AssociatedProducts = associatedProductList.Any() ? listModel.AssociatedProducts.Where(i => !associatedProductList.Contains(i.PimProductId)).ToList() : listModel.AssociatedProducts;

            if (listModel.AssociatedProducts.Count == 0)
                return true;

            listModel.AssociatedProducts = _productTypeAssociationRepository.Insert(listModel.AssociatedProducts.ToEntity<ZnodePimProductTypeAssociation, ProductTypeAssociationModel>().ToList()).ToModel<ProductTypeAssociationModel, ZnodePimProductTypeAssociation>().ToList();

            if (listModel?.AssociatedProducts?.Count() > 0)
            {
                productAssociationHelper.SaveProductAsDraft(parentProductId);
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessAssociateProductsToParentProduct, string.Join(",", listModel.AssociatedProducts.Select(a => a.PimProductId.ToString()).ToArray()), listModel.AssociatedProducts.Select(a => a.PimParentProductId).FirstOrDefault()), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorAssociateProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Remove associate product to parent product.
        public virtual bool UnassociateProduct(ParameterModel productTypeAssociationId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(productTypeAssociationId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAssociatedProductIdsNull);

            ZnodeLogging.LogMessage("Ids to be unassociated :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { productTypeAssociationId?.Ids });

            FilterCollection filters = new FilterCollection();

            //Create tuple to generate where clause.
            filters.Add(new FilterTuple(ZnodePimProductTypeAssociationEnum.PimProductTypeAssociationId.ToString(), ProcedureFilterOperators.In, productTypeAssociationId.Ids));

            //Generating where clause to get account details.
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel for delete :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel?.WhereClause });

            int? pimProductId = _productTypeAssociationRepository.GetEntityList(whereClauseModel.WhereClause).FirstOrDefault()?.PimParentProductId;
            productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(pimProductId));

            return _productTypeAssociationRepository.Delete(whereClauseModel.WhereClause);
        }

        public virtual ProductTypeAssociationModel GetAssociatedProduct(int PimProductTypeAssociationId)
              => PimProductTypeAssociationId > 0 ? _productTypeAssociationRepository.GetById(PimProductTypeAssociationId).ToModel<ProductTypeAssociationModel>() : new ProductTypeAssociationModel();

        public virtual bool UpdateAssociatedProduct(ProductTypeAssociationModel productTypeAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("ProductTypeAssociationModel with PimProductTypeAssociationId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { productTypeAssociationModel?.PimProductTypeAssociationId });

            bool status = false;
            if (IsNull(productTypeAssociationModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (productTypeAssociationModel?.PimProductTypeAssociationId > 0)
            {
                if (productTypeAssociationModel.IsDefault)
                    status = UpdateDefaultProduct(productTypeAssociationModel);
                else
                    status = _productTypeAssociationRepository.Update(productTypeAssociationModel.ToEntity<ZnodePimProductTypeAssociation>());
            }
            if(status)
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(productTypeAssociationModel?.PimParentProductId));

            ZnodeLogging.LogMessage(status ? PIM_Resources.SuccessUpdateAssociatedProduct : PIM_Resources.ErrorUpdateAssociatedProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return status;
        }

        #endregion ProductType

        #region Linked Products

        // Gets list of products which are associated as Add-ons to the parent product.
        public virtual ProductDetailsListModel GetAssociatedLinkProducts(int parentProductId, int linkAttributeId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters parentProductId,linkAttributeId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId, linkAttributeId });

            ProductDetailsListModel associatedLinkProducts = null;

            if (parentProductId > 0)
            {
                //Bind the Filter, sorts & Paging details.
                PageListModel pageListModel = new PageListModel(filters, sorts, page);
                ZnodeLogging.LogMessage("pageListModel to set SP parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

                int localeId = CategoryService.GetLocaleId(filters);
                string whereClauseModel = GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());

                ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

                //SP parameters
                objStoredProc.GetParameter("@WhereClause", whereClauseModel, ParameterDirection.Input, SqlDbType.NVarChar);
                objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
                objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
                objStoredProc.GetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
                objStoredProc.GetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, SqlDbType.Int);
                objStoredProc.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
                objStoredProc.GetParameter("@RelatedProductId", parentProductId, ParameterDirection.Input, SqlDbType.Int);
                objStoredProc.GetParameter("@PimAttributeId", linkAttributeId, ParameterDirection.Input, SqlDbType.Int);
                DataSet ds = objStoredProc.GetSPResultInDataSet("Znode_ManageLinkProductList");
                ProductDetailsListModel xmlList = Xmldataset(ds, out pageListModel.TotalRowCount);

                return GetProductDetailListModelFromXML(pageListModel, xmlList);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return associatedLinkProducts;
        }

        // Associate link product to parent product.
        public virtual bool AssignLinkProduct(LinkProductDetailListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            model.LinkProductDetailList = _linkDetailRepository.Insert(model.LinkProductDetailList.ToEntity<ZnodePimLinkProductDetail, LinkProductDetailModel>()).ToModel<LinkProductDetailModel, ZnodePimLinkProductDetail>().ToList();
            ZnodeLogging.LogMessage("Insert LinkProductDetailList with count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { model?.LinkProductDetailList });

            if (model?.LinkProductDetailList.Count > 0)
            {
                int? pimProductId = model?.LinkProductDetailList?.FirstOrDefault().PimParentProductId.GetValueOrDefault();
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(pimProductId));
                ZnodeLogging.LogMessage(PIM_Resources.SuccessAssignLinkProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorAssignLinkProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        // Un-associate link product to parent product.
        public virtual bool UnassignLinkProduct(ParameterModel linkProductDetailId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(linkProductDetailId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorLinkProductDetailIdNull);

            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(new FilterTuple(ZnodePimLinkProductDetailEnum.PimLinkProductDetailId.ToString(), ProcedureFilterOperators.In, linkProductDetailId.Ids));
            //gets the where clause with filter Values.
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            int? pimProductId = _linkDetailRepository.GetEntityList(whereClause.WhereClause).FirstOrDefault()?.PimParentProductId;
            productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(pimProductId));

            return _linkDetailRepository.Delete(whereClause.WhereClause);
        }

        //Update assign product display Order.
        public virtual bool UpdateAssignLinkProducts(LinkProductDetailModel linkProductDetailModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(linkProductDetailModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);
            bool isUpdated = false;


            ZnodePimLinkProductDetail pimLinkProductDetail = _linkDetailRepository.Table.Where(x => x.PimLinkProductDetailId == linkProductDetailModel.PimLinkProductDetailId)?.FirstOrDefault();

            //Assign value to DisplayOrder.
            if (IsNotNull(pimLinkProductDetail))
            {
                pimLinkProductDetail.ModifiedDate = DateTime.Now;
                pimLinkProductDetail.DisplayOrder = linkProductDetailModel.DisplayOrder;
                ZnodeLogging.LogMessage("PimLinkProductDetailId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pimLinkProductDetail?.PimLinkProductDetailId);
                isUpdated = _linkDetailRepository.Update(pimLinkProductDetail);
                if(isUpdated)
                    productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(pimLinkProductDetail?.PimParentProductId));

                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.UpdateMessage : Admin_Resources.ErrorFailedToUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            return isUpdated;
        }
        #endregion Linked Products

        #region PersonalizedAttributes

        //Get list of assigned Personalized attributes
        public virtual PIMProductAttributeValuesListModel GetAssignedPersonalizedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            string localeId = filters.Find(x => x.Item1 == "localeid")?.Item3;
            filters.RemoveAll(x => x.Item1 == "localeid");

            //Get all attributes associated with default family and familyId
            IZnodeViewRepository<View_PimPersonalisedAttributeValues> pimAttributeValuesViewRepository = new ZnodeViewRepository<View_PimPersonalisedAttributeValues>();
            int productId = Convert.ToInt32(filters.Where(x => x.FilterName == View_PimPersonalisedAttributeValuesEnum.PimProductId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
            ZnodeLogging.LogMessage("productId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { productId });

            FilterCollection attributeLocaleFilter = new FilterCollection();
            attributeLocaleFilter.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId);
            SetLocaleFilterIfNotPresent(ref attributeLocaleFilter);
            EntityWhereClauseModel whereClauseModelForAttributeLocales = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(attributeLocaleFilter.ToFilterDataCollection());

            pimAttributeValuesViewRepository.SetParameter(View_PimPersonalisedAttributeValuesEnum.PimProductId.ToString(), productId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValuesViewRepository.SetParameter("LocaleId", Convert.ToInt32(localeId), ParameterDirection.Input, DbType.Int32);

            var pimAttributes = pimAttributeValuesViewRepository.ExecuteStoredProcedureList("Znode_GetPimPersonalisedAttributeValues @PimProductId,@LocaleId");
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return new PIMProductAttributeValuesListModel { ProductAttributeValues = pimAttributes.ToModel<PIMProductAttributeValuesModel>().ToList() };
        }

        //Get list of un-assigned Personalized attributes
        public virtual PIMAttributeListModel GetUnassignedPersonalizedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            string localeId = filters.Find(x => x.Item1 == "localeid")?.Item3;
            filters.RemoveAll(x => x.Item1 == "localeid");
            //Get unassigned personalized attributes for a product.
            filters.Add(ZnodePimAttributeEnum.IsPersonalizable.ToString(), FilterOperators.Equals, "true");
            EntityWhereClauseModel whereClauseModelForAttributes = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            var unassignedPersonalizedAttributes = _attributeRepository.GetEntityList(whereClauseModelForAttributes.WhereClause).ToList();
            ZnodeLogging.LogMessage("unassignedPersonalizedAttributes count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { unassignedPersonalizedAttributes?.Count });

            IList<ZnodePimAttributeLocale> unassignedPersonalizedAttributesLocale = null;
            //Check count of unassignedPersonalizedAttributes. If greater than 0 then create locale filter.
            if (unassignedPersonalizedAttributes.Count > 0)
            {
                FilterCollection attributeLocaleFilter = new FilterCollection();
                attributeLocaleFilter.Add(ZnodePimAttributeLocaleEnum.PimAttributeId.ToString(), FilterOperators.In, string.Join(",", (unassignedPersonalizedAttributes.Select(x => x.PimAttributeId.ToString()).ToArray())));
                attributeLocaleFilter.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId);

                SetLocaleFilterIfNotPresent(ref attributeLocaleFilter);

                EntityWhereClauseModel whereClauseModelForAttributeLocales = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(attributeLocaleFilter.ToFilterDataCollection());

                unassignedPersonalizedAttributesLocale = _attributeLocaleRepository.GetEntityList(whereClauseModelForAttributeLocales.WhereClause).ToList();
            }
            PIMAttributeListModel attributes = new PIMAttributeListModel { Attributes = unassignedPersonalizedAttributes?.ToModel<PIMAttributeModel>().ToList() };
            attributes.Attributes.ForEach(x =>
            {
                string attributeLocale = unassignedPersonalizedAttributesLocale?.Where(y => x.PimAttributeId == y.PimAttributeId).Select(z => z.AttributeName).FirstOrDefault();

                x.Locales.Add(new PIMAttributeLocaleModel()
                {
                    AttributeName = IsNull(attributeLocale) ? GetAttributeDefaultLocale(x) : attributeLocale
                });
            });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return attributes;
        }

        //Associated personalized attribute to parent product.
        public virtual bool AssignPersonalizedAttributes(PIMAttributeValueListModel model, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            PIMAttributeValueListModel attributesToBeAssignedToProduct = new PIMAttributeValueListModel();

            if (model?.AttributeValues?.Count > 0)
            {
                filters.Add(new FilterTuple(View_PimPersonalisedAttributeValuesEnum.PimProductId.ToString().ToLower(), FilterOperators.Equals, model.AttributeValues[0].PimProductId.ToString()));
                //Get list of assigned personalized attributes.
                PIMProductAttributeValuesListModel assignedPersonalizedAttribute = GetAssignedPersonalizedAttributes(null, filters, null);

                attributesToBeAssignedToProduct.AttributeValues = model.AttributeValues.Where(lit => !assignedPersonalizedAttribute.ProductAttributeValues.Any(lit2 => lit.PimAttributeId.Equals(lit2.PimAttributeId))).ToList();

                IEnumerable<ZnodePimAttributeValue> assignPersonalizedAttributes = _attributeValueRepository.Insert(attributesToBeAssignedToProduct.AttributeValues.ToEntity<ZnodePimAttributeValue>().ToList());
                ZnodeLogging.LogMessage("Insert attributesToBeAssignedToProduct with assignPersonalizedAttributes:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { assignPersonalizedAttributes });

                AddPersonalizedAttributesInLocale(assignPersonalizedAttributes.ToModel<PIMProductAttributeValuesModel>().ToList());

                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(model.AttributeValues[0].PimProductId));

                if (assignPersonalizedAttributes.Any())
                    return true;
                else
                    return false;
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorInvalidData);

        }

        // To add personalized value local entry againt product.
        public virtual void AddPersonalizedAttributesInLocale(List<PIMProductAttributeValuesModel> assignPersonalizedAttributes)
        {
            List<PIMProductAttributeValuesLocalModel> productAttributeValueLocal = new List<PIMProductAttributeValuesLocalModel>();

            foreach (PIMProductAttributeValuesModel item in assignPersonalizedAttributes)
            {
                PIMProductAttributeValuesLocalModel personalizedAttributes =
                                             (from pimAttributeDefaultValue in _pimAttributeDefaultValueRepository.Table
                                              join pimAttributeDefaultValueLocal in _pimAttributeDefaultValueLocaleRepository.Table
                                              on pimAttributeDefaultValue.PimAttributeDefaultValueId equals pimAttributeDefaultValueLocal.PimAttributeDefaultValueId
                                              where pimAttributeDefaultValue.PimAttributeId == item.PimAttributeId
                                              select new PIMProductAttributeValuesLocalModel()
                                              {
                                                  PimAttributeValueId =item.PimAttributeValueId,
                                                  LocaleId =pimAttributeDefaultValueLocal.LocaleId,
                                                  AttributeValue = pimAttributeDefaultValueLocal.AttributeDefaultValue,
                                              }).FirstOrDefault();

                if (personalizedAttributes != null)
                    productAttributeValueLocal.Add(personalizedAttributes);
            }

            if (productAttributeValueLocal.Count > 0)
                _attributeValueLocaleRepository.Insert(productAttributeValueLocal?.ToEntity<ZnodePimAttributeValueLocale>());
        }

        //Un-assign personalized attribute to parent product.
        public virtual bool UnassignPersonalizedAttributes(ParameterModel parameters, int pimParentProductId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (pimParentProductId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorParentProductIdLessThanOne);

            if (Equals(parameters, null))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorParameterModelNull);

            ZnodeLogging.LogMessage("pimParentProductId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimParentProductId });

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeValueEnum.PimAttributeId.ToString(), ProcedureFilterOperators.In, parameters.Ids));
            filters.Add(new FilterTuple(ZnodePimAttributeValueEnum.PimProductId.ToString(), ProcedureFilterOperators.Equals, pimParentProductId.ToString()));

            //Delete locales
            bool isPimAttributeValueLocaleDeleted = true;
            string[] personalizedAttributeValues = _attributeValueRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause).Select(x => x.PimAttributeValueId.ToString()).ToArray();
            string personalizedAttributebuteValueIdParameters = string.Join(",", personalizedAttributeValues);

            if (!string.IsNullOrEmpty(personalizedAttributebuteValueIdParameters))
            {
                FilterCollection attributeValueLocaleFilters = new FilterCollection();

                attributeValueLocaleFilters.Add(new FilterTuple(ZnodePimAttributeValueLocaleEnum.PimAttributeValueId.ToString(), ProcedureFilterOperators.In, personalizedAttributebuteValueIdParameters));

                string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(attributeValueLocaleFilters.ToFilterDataCollection()).WhereClause;
                ZnodeLogging.LogMessage("whereClause for delete:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClause });

                if (_attributeValueLocaleRepository.GetEntityList(whereClause).Count > 0)
                {
                    isPimAttributeValueLocaleDeleted = _attributeValueLocaleRepository.Delete(whereClause);
                }
            }

            productAssociationHelper.SaveProductAsDraft(pimParentProductId);

            if (isPimAttributeValueLocaleDeleted)
                return _attributeValueRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            else
                return false;
        }

        #endregion PersonalizedAttributes

        #region Custom Field

        //Add custom field against parent product.
        public virtual CustomFieldModel AddCustomField(CustomFieldModel customFieldModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (Equals(customFieldModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsCustomCodeExist(customFieldModel.CustomCode, customFieldModel.ProductId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorCustomCodeExists);

            ZnodeLogging.LogMessage("CustomFieldModel with CustomCode and ProductId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { customFieldModel?.CustomCode, customFieldModel?.ProductId });


            ZnodePimCustomField newCustomField = _customFieldRepository.Insert(customFieldModel.ToEntity<ZnodePimCustomField>());

            if (newCustomField?.PimCustomFieldId > 0)
            {
                customFieldModel.CustomFieldId = newCustomField.PimCustomFieldId;
                CreateCustomFieldLocale(customFieldModel);
            }

            productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(customFieldModel?.ProductId));

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return customFieldModel;
        }

        //Get list of custom field against parent product
        public virtual CustomFieldListModel GetCustomFieldList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //Get Default Locale value from filters.
            int defaultLocale = Convert.ToInt32(filters.Where(x => x.FilterName.Equals(ZnodePimCustomFieldLocaleEnum.LocaleId.ToString().ToLower()))?.Select(y => y.FilterValue)?.FirstOrDefault());
            filters.RemoveAll(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString().ToLower());

            IZnodeViewRepository<CustomFieldModel> objStoredProc = new ZnodeViewRepository<CustomFieldModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(defaultLocale), ParameterDirection.Input, DbType.Int32);
            IList<CustomFieldModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetCustomFieldDetail @WhereClause, @Rows,@PageNo,@Order_By,@RowsCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { list?.Count });

            CustomFieldListModel customFieldListModel = new CustomFieldListModel { CustomFields = list?.ToList() };
            customFieldListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return customFieldListModel;
        }

        //Get data for custom field to edit
        public virtual CustomFieldModel GetCustomField(int customFieldId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("customFieldId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { customFieldId });

            if (customFieldId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePimCustomFieldEnum.PimCustomFieldId.ToString(), FilterOperators.Equals, customFieldId.ToString()));

                //gets the where clause with filter Values.
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                var navigationProperties = GetExpands(expands);

                return _customFieldRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties).ToModel<CustomFieldModel>();
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return new CustomFieldModel();
        }

        //Update custom field.
        public virtual bool UpdateCustomField(CustomFieldModel customFieldModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("customFieldModel with customFieldId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { customFieldModel?.CustomFieldId });

            bool status = false;
            if (Equals(customFieldModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (customFieldModel.CustomFieldId < 1 && customFieldModel.ProductId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, PIM_Resources.ErrorCustomFieldIdOrProductIdLessThanOne);

            List<ZnodePimCustomFieldLocale> customFieldLocales = new List<ZnodePimCustomFieldLocale>();

            //Update custom field locale.
            foreach (CustomFieldLocaleModel locale in customFieldModel?.CustomFieldLocales)
            {
                locale.CustomFieldId = customFieldModel.CustomFieldId;

                if (locale?.CustomFieldLocaleId == 0)
                    customFieldLocales.Add(locale.ToEntity<ZnodePimCustomFieldLocale>());
                else
                    status = _customFieldLocaleRepository.Update(locale.ToEntity<ZnodePimCustomFieldLocale>());
            }
            status = _customFieldRepository.Update(customFieldModel.ToEntity<ZnodePimCustomField>());
            IEnumerable<ZnodePimCustomFieldLocale> data = _customFieldLocaleRepository.Insert(customFieldLocales);

            status = data.All(x => x.PimCustomFieldLocaleId > 0);

            if (status)
            {
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(customFieldModel?.ProductId));
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessUpdateCustomField, customFieldModel.CustomCode), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            else
            {
                throw new ZnodeException(ErrorCodes.UpdateCustomFieldError, string.Format(PIM_Resources.ErrorUpdateCustomField, customFieldModel.CustomCode));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return status;
        }

        //Delete custom field from parent product.
        public virtual bool DeleteCustomField(ParameterModel customFieldId)
        {
            bool status = true;
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("customFieldId to be deleted: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { customFieldId?.Ids });

            if (customFieldId.Ids.Count() <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimCustomFieldEnum.PimCustomFieldId.ToString(), ProcedureFilterOperators.In, customFieldId.Ids.ToString()));

            int? pimProductId = _customFieldRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause)?.PimProductId.GetValueOrDefault();
            _customFieldLocaleRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            status = _customFieldRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

            if (status)
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(pimProductId));

            return status;
        }

        #endregion Custom Field

        #region Associate Add-ons

        //Associate addonGroup to parent product.
        public virtual bool AssociateAddon(AddonProductListModel addonProducts)
        {
            ZnodeLogging.LogMessage("Execution start.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(addonProducts))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonProductModelNull);

            if (IsNull(addonProducts.AddonProducts))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonProductsNull);

            addonProducts.AddonProducts = _addonProductRepository.Insert(addonProducts.AddonProducts.ToEntity<ZnodePimAddOnProduct, AddOnProductModel>().ToList()).ToModel<AddOnProductModel, ZnodePimAddOnProduct>().ToList();
            ZnodeLogging.LogMessage("Insert AddonProducts with AddonProducts count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonProducts?.AddonProducts?.Count() });

            CreateProductChildAddonProductAssociation(addonProducts);

            if (addonProducts?.AddonProducts?.Count() > 0)
            {
                int? pimProductId = addonProducts.AddonProducts.FirstOrDefault().PimProductId;
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(pimProductId));
                //Log message if products are successfully associated.
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessAssignAddonGroupToProduct, string.Join(",", addonProducts.AddonProducts.Select(a => a.PimAddonGroupId.ToString()).ToArray()), addonProducts.AddonProducts.Select(a => a.PimProductId).FirstOrDefault()), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                //Log message if products failed to be associated.
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorAssignAddonGroupToProduct, string.Join(",", addonProducts.AddonProducts.Select(a => a.PimAddonGroupId.ToString()).ToArray()), addonProducts.AddonProducts.Select(a => a.PimProductId).FirstOrDefault()), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Delete associated addon
        public virtual bool DeleteAssociatedAddons(ParameterModel addonProductIds)
        {
            ZnodeLogging.LogMessage("Execution start.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("addonProductIds to be deleted:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonProductIds?.Ids });

            if (string.IsNullOrEmpty(addonProductIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonProductIdNull);

            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(new FilterTuple(ZnodePimAddOnProductEnum.PimAddOnProductId.ToString(), ProcedureFilterOperators.In, addonProductIds.Ids));
            //gets the where clause with filter Values.
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

            //Delete associated child products in the addon first.
            bool isAddonProductDetailsDeleted = true;
            int addonProductId = Convert.ToInt32(addonProductIds.Ids);
            if (_addonProductDetailRepository.Table.Any(x => x.PimAddOnProductId == addonProductId))
            {
                isAddonProductDetailsDeleted = _addonProductDetailRepository.Delete(whereClause.WhereClause);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Then delete the associated addons.
            isAddonProductDetailsDeleted &= _addonProductRepository.Delete(whereClause.WhereClause);

            if (isAddonProductDetailsDeleted)
                productAssociationHelper.SaveProductAsDraft(addonProductIds.PimProductId);

            return isAddonProductDetailsDeleted;
        }

        //Get associated addonGroup list by productId.
        public virtual AddonGroupListModel GetAssociatedAddonDetails(int productId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution start.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("productId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { productId });

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetAddonGroupModel:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            int localeId = CategoryService.GetLocaleId(filters);
            AddonGroupDetailListModel associatedAddonProducts = null;

            IZnodeViewRepository<AddonGroupDetailModel> objStoredProc = new ZnodeViewRepository<AddonGroupDetailModel>();

            //SP parameters
            objStoredProc.SetParameter("@PimProductId", productId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);

            var list = objStoredProc.ExecuteStoredProcedureList("Znode_ManageProductAddonList @PimProductId,@LocaleId");

            associatedAddonProducts = new AddonGroupDetailListModel { AddonGroupDetails = list.ToList() };
            AddonGroupListModel addonGroups = GetAddonGroupModel(associatedAddonProducts, pageListModel.TotalRowCount, pageListModel.PagingStart, pageListModel.PagingLength);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return addonGroups;
        }

        //Create or associate child product to parent product using addonGroup.
        public virtual bool CreateAddonProductDetail(AddOnProductDetailListModel addonProductDetails)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(addonProductDetails))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonProductDetailModelNull);

            int? pimProductId = addonProductDetails.AddOnProductDetailList?.FirstOrDefault().PimProductId;
            addonProductDetails.AddOnProductDetailList = _addonProductDetailRepository.Insert(addonProductDetails.AddOnProductDetailList.ToEntity<ZnodePimAddOnProductDetail, AddOnProductDetailModel>()).ToModel<AddOnProductDetailModel, ZnodePimAddOnProductDetail>().ToList();
            ZnodeLogging.LogMessage("AddOnProductDetailList inserted with count :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonProductDetails.AddOnProductDetailList?.Count });

            if (addonProductDetails?.AddOnProductDetailList?.Count() > 0)
            {
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(pimProductId));
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessAssignProductsToAddonProduct, string.Join(",", addonProductDetails.AddOnProductDetailList.Select(a => a.PimChildProductId.ToString()).ToArray()), addonProductDetails.AddOnProductDetailList.Select(a => a.PimAddOnProductId).FirstOrDefault()), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorAssignProductsToAddonProduct, string.Join(",", addonProductDetails.AddOnProductDetailList.Select(a => a.PimChildProductId.ToString()).ToArray()), addonProductDetails.AddOnProductDetailList.Select(a => a.PimAddOnProductId).FirstOrDefault()), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Delete associated addon product.
        public virtual bool DeleteAddonProductDetails(ParameterModel addonProductDetailIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = true;

            if (string.IsNullOrEmpty(addonProductDetailIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonProductIdNull);
            ZnodeLogging.LogMessage("addonProductDetailIds to be deleted:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonProductDetailIds?.Ids });

            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(new FilterTuple(ZnodePimAddOnProductDetailEnum.PimAddOnProductDetailId.ToString(), ProcedureFilterOperators.In, addonProductDetailIds.Ids));
            //gets the where clause with filter Values.
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            status = _addonProductDetailRepository.Delete(whereClause.WhereClause);
            if(status)
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(addonProductDetailIds?.PimProductId));

            return status;
        }

        //Get unassociated addonGroup list.
        public virtual AddonGroupListModel GetUnassociatedAddonGroups(int parentProductId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { parentProductId });

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString()});

            AddonGroupListModel unassociatedAddonProducts = null;
            string associatedAddonGroupIds = string.Empty;
            int localeId = CategoryService.GetLocaleId(filters);

            if (parentProductId > 0)
                associatedAddonGroupIds = string.Join(",", _addonProductRepository.Table?.Where(x => x.PimProductId == parentProductId)?.Select(y => Equals(y.PimAddonGroupId, null) ? 0 : y.PimAddonGroupId)?.ToArray());

            if (!string.IsNullOrEmpty(associatedAddonGroupIds))
                filters.Add(View_GetPimAddonGroupsEnum.PimAddonGroupId.ToString(), ProcedureFilterOperators.NotIn, associatedAddonGroupIds);

            IZnodeViewRepository<View_GetPimAddonGroups> objStoredProc = new ZnodeViewRepository<View_GetPimAddonGroups>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.String);
            var list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimAddonGroups @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount);

            unassociatedAddonProducts = new AddonGroupListModel { AddonGroups = list.ToModel<AddonGroupModel>().ToList() };
            unassociatedAddonProducts.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return unassociatedAddonProducts;
        }

        //Update addon product association.
        public virtual bool UpdateProductAddonAssociation(AddOnProductModel addonProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(addonProductModel))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonProductModelNull);

            bool isAddonProductUpdated = _addonProductRepository.Update(addonProductModel.ToEntity<ZnodePimAddOnProduct>());

            if(isAddonProductUpdated)
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(addonProductModel?.PimProductId));

            ZnodeLogging.LogMessage(isAddonProductUpdated ? PIM_Resources.SuccessAssociateAddonProduct : PIM_Resources.ErrorAssociateAddonProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return isAddonProductUpdated;
        }

        //Update Product Addon Display Order
        public virtual bool UpdateAddonDisplayOrder(AddOnProductDetailModel addOnProductDetailModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(addOnProductDetailModel))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonProductModelNull);

            bool isAddonDisplayorderUpdated = _addonProductDetailRepository.Update(addOnProductDetailModel.ToEntity<ZnodePimAddOnProductDetail>());

            if (isAddonDisplayorderUpdated && (bool)addOnProductDetailModel.IsDefault)
            {
                var addonDetails = _addonProductDetailRepository.Table.Where(x => x.PimAddOnProductDetailId != addOnProductDetailModel.PimAddOnProductDetailId && x.PimChildProductId != addOnProductDetailModel.PimChildProductId && x.PimAddOnProductId == addOnProductDetailModel.PimAddOnProductId).ToList();
                addonDetails?.ForEach(item =>
                {
                    item.IsDefault = false;
                    _addonProductDetailRepository.Update(item);
                });
            }
            if(isAddonDisplayorderUpdated)
                productAssociationHelper.SaveProductAsDraft(_addonProductRepository.Table.Where(x => x.PimAddOnProductId == addOnProductDetailModel.PimAddOnProductId).FirstOrDefault().PimProductId);

            ZnodeLogging.LogMessage(isAddonDisplayorderUpdated ? PIM_Resources.SuccessUpdateAddonDisplayOrder : PIM_Resources.ErrorUpdateAddonDisplayOrder, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return isAddonDisplayorderUpdated;
        }

        #endregion Associate Add-ons

        #region Helper Methods

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (Equals(key, Constants.ExpandKeys.AttributeLocale))
                        SetExpands(ZnodePimAttributeEnum.ZnodePimAttributeLocales.ToString(), navigationProperties);
                    if (Equals(key, ZnodePimCustomFieldEnum.ZnodePimCustomFieldLocales.ToString().ToLower()))
                        SetExpands(ZnodePimCustomFieldEnum.ZnodePimCustomFieldLocales.ToString(), navigationProperties);
                    if (Equals(key, ZnodePimAddonGroupEnum.ZnodePimAddonGroupLocales.ToString().ToLower()))
                        SetExpands(ZnodePimAddonGroupEnum.ZnodePimAddonGroupLocales.ToString(), navigationProperties);
                    if (Equals(key, ZnodeConstant.Promotions)) SetExpands(ZnodeConstant.Promotions, navigationProperties);
                    if (Equals(key, ZnodeConstant.Inventory)) SetExpands(ZnodeConstant.Inventory, navigationProperties);
                    if (Equals(key, ZnodeConstant.ProductTemplate)) SetExpands(ZnodeConstant.ProductTemplate, navigationProperties);
                    if (Equals(key, ZnodeConstant.ProductReviews)) SetExpands(ZnodeConstant.ProductReviews, navigationProperties);
                    if (Equals(key, ZnodeConstant.Pricing)) SetExpands(ZnodeConstant.Pricing, navigationProperties);
                    if (Equals(key, ZnodeConstant.SEO)) SetExpands(ZnodeConstant.SEO, navigationProperties);
                    if (Equals(key, ZnodeConstant.AddOns)) SetExpands(ZnodeConstant.AddOns, navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Checks if custom code already exists or not.
        private bool IsCustomCodeExist(string customCode, int? pimProductId)
            => _customFieldRepository.Table.Any(x => x.CustomCode == customCode && x.PimProductId == pimProductId);

        //Create new custom field locale.
        private bool CreateCustomFieldLocale(CustomFieldModel customFieldModel)
        {
            GetLocaleByDefaultLocaleId(customFieldModel);
            customFieldModel.CustomFieldLocales.ForEach(x => x.CustomFieldId = customFieldModel.CustomFieldId);
            return !Equals(_customFieldLocaleRepository.Insert(customFieldModel.CustomFieldLocales.ToEntity<ZnodePimCustomFieldLocale>()), null);
        }

        //Get default locale
        private CustomFieldModel GetLocaleByDefaultLocaleId(CustomFieldModel customFieldModel)
        {
            int defaultLocaleId = GetDefaultLocaleId();
            ZnodeLogging.LogMessage("defaultLocaleId returned from GetDefaultLocaleId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { defaultLocaleId });

            if (customFieldModel.CustomFieldLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.CustomKey) && string.IsNullOrEmpty(x.CustomKeyValue)))
            {
                customFieldModel.CustomFieldLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.CustomKey = customFieldModel.CustomCode);
                customFieldModel.CustomFieldLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.CustomKeyValue = customFieldModel.CustomCode);
            }
            customFieldModel.CustomFieldLocales.RemoveAll(x => x.CustomKey == null && x.CustomKeyValue == null);

            return customFieldModel;
        }

        private AddonGroupListModel GetAddonGroupModel(AddonGroupDetailListModel models, int totalCount, int pagingStart, int pagingLength)
        {
            AddonGroupDetailListModel distinctAddonGroups = new AddonGroupDetailListModel();
            distinctAddonGroups.AddonGroupDetails = models.AddonGroupDetails.GroupBy(x => x.AddonGroupName).Select(g => g.First()).ToList();

            AddonGroupListModel addonGroupList = new AddonGroupListModel();
            if (IsNotNull(distinctAddonGroups))
            {
                foreach (AddonGroupDetailModel model in distinctAddonGroups.AddonGroupDetails)
                {
                    AddonGroupModel addonGroup = new AddonGroupModel();
                    addonGroup.AddonGroupName = model.AddonGroupName;
                    addonGroup.DisplayType = model.DisplayType;
                    addonGroup.PimAddonProductId = model.PimAddOnProductId.GetValueOrDefault();

                    addonGroup.PimAddOnProducts = new List<AddOnProductModel>() { new AddOnProductModel() };
                    addonGroup.PimAddOnProducts[0].DisplayOrder = model.DisplayOrder;
                    addonGroup.PimAddOnProducts[0].RequiredType = model.RequiredType;
                    addonGroup.PimAddonGroupId = model.PimAddonGroupId.GetValueOrDefault();

                    ProductDetailsListModel childProducts = new ProductDetailsListModel();

                    AddonGroupDetailListModel commanAddonProducts = new AddonGroupDetailListModel();
                    commanAddonProducts.AddonGroupDetails = models.AddonGroupDetails.Where(x => x.PimAddOnProductId == model.PimAddOnProductId && x.RelatedProductId > 0).Select(x => x).ToList();

                    foreach (var item in commanAddonProducts.AddonGroupDetails)
                    {
                        childProducts.ProductDetailList.Add(new ProductDetailsModel { PimAddonGroupId = item.PimAddonGroupId, Price = item.Price, Quantity = item.Quantity, SKU = item.SKU, Assortment = item.Assortment, ProductId = item.RelatedProductId.GetValueOrDefault(), ProductName = item.ProductName, PimAddOnProductDetailId = item.PimAddOnProductDetailId, AddOnDisplayOrder = item.AddOnDisplayOrder, PimAddOnProductId = item.PimAddOnProductId, PimChildProductId = item.ProductId, IsDefault = item.IsDefault });
                    }
                    addonGroup.AssociatedChildProducts = childProducts;
                    addonGroup.AssociatedChildProducts.TotalResults = totalCount;
                    addonGroup.AssociatedChildProducts.PageIndex = pagingStart;
                    addonGroup.AssociatedChildProducts.PageSize = pagingLength;
                    addonGroupList.AddonGroups.Add(addonGroup);
                }
            }
            addonGroupList.AddonGroups = addonGroupList.AddonGroups.OrderBy(x => x.PimAddonProductId).ToList();

            return addonGroupList;
        }

        #endregion Helper Methods

        #region Product SKU list for Autocomplete feature/ Price / Inventory.

        //Get product sku list by attribute value and attribute code.
        public virtual PIMProductAttributeValuesListModel GetProductSKUsByAttributeCode(string attributeValue, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetSKUListForPriceOrInventory:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            PIMProductAttributeValuesListModel pimProductAttributeValuesListModel;

            //Checks if filter contains priceListId or inventoryId to get Product SKU list for Price or Inventory.
            if (filters.Exists(x => x.FilterName == ZnodePriceEnum.PriceListId.ToString().ToLower()))
            {
                int localeId, priceListId;

                //Get locale id and pricelist id for sp input parameters.
                GetLocaleAndPriceListId(filters, out localeId, out priceListId);

                //Get Product SKU list for Price or Inventory.
                pimProductAttributeValuesListModel = GetSKUListForPriceOrInventory(filters, pageListModel, localeId, priceListId);
            }
            else
                pimProductAttributeValuesListModel = GetSKUListForAutoComplete(attributeValue);

            pimProductAttributeValuesListModel.BindPageListModel(pageListModel);
            return pimProductAttributeValuesListModel;
        }

        //Get ProductNotIn parameter value from filter otherwise set default
        public static bool GetIsProductNotInValue(FilterCollection filters, bool isProductNotIn)
        {
            //Checking For IsProductNotIn exists in Filters Or Not
            if (filters.Exists(x => x.Item1.Equals("IsProductNotIn", StringComparison.InvariantCultureIgnoreCase)))
            {
                isProductNotIn = Convert.ToBoolean(filters.Where(x => x.Item1.Equals("IsProductNotIn", StringComparison.InvariantCultureIgnoreCase))?.Select(x => x.FilterValue)?.FirstOrDefault());
                filters.RemoveAll(x => x.Item1.Equals("IsProductNotIn", StringComparison.InvariantCultureIgnoreCase));
            }
            return isProductNotIn;
        }

        //Get isCallForAttribute parameter value from filter for Brand/Vendor/ShippingRule/Highlights
        public static bool GetIsCallForAttribute(FilterCollection filters)
        {
            bool isCallForAttribute = false;
            //Checking For IsProductNotIn exists in Filters Or Not
            if (filters.Exists(x => x.Item1.Equals(ZnodeConstant.IsCallForAttribute, StringComparison.InvariantCultureIgnoreCase)))
            {
                isCallForAttribute = Convert.ToBoolean(filters.Where(x => x.Item1.Equals(ZnodeConstant.IsCallForAttribute, StringComparison.InvariantCultureIgnoreCase))?.Select(x => x.FilterValue)?.FirstOrDefault());
                filters.RemoveAll(x => x.Item1.Equals(ZnodeConstant.IsCallForAttribute, StringComparison.InvariantCultureIgnoreCase));
            }
            return isCallForAttribute;
        }

         #endregion Product SKU list for Autocomplete feature/ Price / Inventory.

        #region Product Update Import

        // This method will fetch the product update data from file and insert it into DB and then inserted data will be processed.
        public virtual int ProcessProductUpdateData(ImportModel importModel)
        {
            int userId = GetLoginUserId();
            ZnodeLogging.LogMessage("userId returned from GetLoginUserId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { userId });

            string importedGuid = string.Empty;
            int result = importHelper.ProcessProductUpdateData(importModel, userId, out importedGuid);
            if (importModel.IsAutoPublish && Convert.ToBoolean(result))
            {
                bool isProductPublish = false;
                try
                {
                    HttpContext httpContext = HttpContext.Current;
                    Action threadWorker = delegate ()
                    {
                        HttpContext.Current = httpContext;
                        try
                        {
                            isProductPublish = IsMassProductUpdatePublishStart(importedGuid);

                            if (isProductPublish)
                            {
                                //Get dataset of publish product to insert it into elastic search and clear cache # step 1
                                DataSet dataSet = null;
                                ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();

                                executeSpHelper.GetParameter("@ImportGUID", importedGuid, ParameterDirection.Input, SqlDbType.NVarChar);

                                dataSet = executeSpHelper.GetSPResultInDataSet("Znode_GetPublishProductRecordset");

                                if(dataSet?.Tables?.Count > 1 && dataSet?.Tables[0]?.Rows?.Count > 0 && dataSet?.Tables[1]?.Rows?.Count > 0)
                                {
                                    IPublishProductDataService publishProductDataService = GetService<IPublishProductDataService>();

                                    //Collect product data from datatable to use it for product elastic search index creation. #Step 2
                                    List<ZnodePublishProductEntity> publishProductEntityList = dataSet?.Tables[1]?.ToList<ZnodePublishProductEntity>();

                                    //Get revision type(s) for the elastic index process. #Step 3
                                    List<string> revisionTypes = GetRevisionTypesForElasticIndex();

                                    //Perform elastic index creation operation of products. #Step 4
                                    foreach (string revisiontype in revisionTypes)
                                        publishProductDataService.CreateProductElasticIndex(publishProductEntityList?.Where(x => x.RevisionType == revisiontype).ToList(), revisiontype);

                                    //Collect product(s) data from datatable to remove product data from cache #Step 5
                                    List<ProductAssociationPublishModel> productList = dataSet?.Tables[0]?.ToList<ProductAssociationPublishModel>();

                                    //Clear webstore and cloudflare cache after product publish #Step 6
                                    ClearCacheAfterProductPublish(productList);
                                }                                
                            }

                        }
                        catch (Exception ex)
                        {
                            ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                            throw ex;
                        }
                    };
                    AsyncCallback callBack = new AsyncCallback(AfterPublishSuccessCallBack);
                    threadWorker.BeginInvoke(callBack, null);
                    return 1;
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                    throw ex;
                }
            }
            else if (!importModel.IsAutoPublish && Convert.ToBoolean(result))
                return 1;

            return 0;
        }

        protected virtual void AfterPublishSuccessCallBack(IAsyncResult ar)
        {
            AsyncResult result = ar as AsyncResult;
        }

        //Fetch appropriate revision type data for elastic search based on given revision type
        protected virtual List<string> GetRevisionTypesForElasticIndex()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Check preview setting is enabled or disabled.
            bool isPreviewEnabled = IsWebstorePreviewEnabled();

            /*If isPreviewEnabled - True then return both revision type. i.e. preview and production.
            * else isPreviewEnabled - False then return only production revision type. */
            if (isPreviewEnabled)
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PREVIEW.ToString(), ZnodePublishStatesEnum.PRODUCTION.ToString() };
            }
            else
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return new List<string> { ZnodePublishStatesEnum.PRODUCTION.ToString() };
            }
        }
        #endregion


        //Get isCallForAttribute parameter value from filter for Brand/Vendor/ShippingRule/Highlights
        public static string GetBrandCode(FilterCollection filters)
        {
            string brandCode = string.Empty;
            //Checking For IsProductNotIn exists in Filters Or Not
            if (filters.Exists(x => x.Item1.Equals(ZnodeConstant.BrandCode, StringComparison.InvariantCultureIgnoreCase)))
            {
                brandCode = filters.Where(x => x.Item1.Equals(ZnodeConstant.BrandCode, StringComparison.InvariantCultureIgnoreCase))?.Select(x => x.FilterValue)?.FirstOrDefault();
                filters.RemoveAll(x => x.Item1.Equals(ZnodeConstant.BrandCode, StringComparison.InvariantCultureIgnoreCase));
            }
            return brandCode;
        }
        #region Private Method

          //Get ProductDetailsListModel from products xml from SP
        protected ProductDetailsListModel GetProductDetailListModelFromXML(PageListModel pageListModel, ProductDetailsListModel xmlList)
        {
            ProductDetailsListModel productList = new ProductDetailsListModel
            {
                Locale = GetActiveLocaleList(),
                AttributeColumnName = xmlList.AttributeColumnName,
                XmlDataList = xmlList.XmlDataList
            };

            productList.BindPageListModel(pageListModel);
            return productList;
        }

        //Get locale id and pricelist id for sp input parameters.
        private void GetLocaleAndPriceListId(FilterCollection filters, out int localeId, out int priceListId)
        {
            localeId = Convert.ToInt32(filters.Find(filterTuple => filterTuple.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower())?.Item3);
            priceListId = Convert.ToInt32(filters.Find(filterTuple => filterTuple.Item1 == ZnodePriceEnum.PriceListId.ToString().ToLower())?.Item3);

            filters.RemoveAll(x => x.FilterName == ZnodeLocaleEnum.LocaleId.ToString().ToLower() || x.FilterName == ZnodePriceEnum.PriceListId.ToString().ToLower());
        }

        //Get Product SKU list for Price if priceListId is greater than 0 else get list for Inventory.
        private PIMProductAttributeValuesListModel GetSKUListForPriceOrInventory(FilterCollection filters, PageListModel pageListModel, int localeId, int priceListId)
        {
            IZnodeViewRepository<PIMProductAttributeValuesModel> objStoredProcedure = new ZnodeViewRepository<PIMProductAttributeValuesModel>();
            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            objStoredProcedure.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProcedure.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProcedure.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProcedure.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProcedure.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProcedure.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.String);
            objStoredProcedure.SetParameter("@PriceListId", priceListId, ParameterDirection.Input, DbType.String);
            return new PIMProductAttributeValuesListModel() { ProductAttributeValues = objStoredProcedure.ExecuteStoredProcedureList("Znode_GetSkuListForInventoryAndPrice  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PriceListId", 4, out pageListModel.TotalRowCount)?.ToList() };
        }

        //Product SKU list for Autocomplete feature.
        private PIMProductAttributeValuesListModel GetSKUListForAutoComplete(string attributeValue)
        {
            ZnodeLogging.LogMessage("attributeValue:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { attributeValue });

            IZnodeViewRepository<View_GetListOfPimAttributeValues> objStoredProc = new ZnodeViewRepository<View_GetListOfPimAttributeValues>();

            //SP parameters
            objStoredProc.SetParameter("@AttributeCode", "SKU", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@AttributeValue", attributeValue, ParameterDirection.Input, DbType.String);

            IList<View_GetListOfPimAttributeValues> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetListOfPimAttributeValues  @AttributeCode,@AttributeValue");

            return new PIMProductAttributeValuesListModel { ProductAttributeValues = list?.Count > 0 ? list.ToModel<PIMProductAttributeValuesModel>()?.ToList() : null };
        }

        //Convert DataTable to List of dynamic type.
        protected virtual List<dynamic> DataTableToList(DataTable dataTable)
        {
            var result = new List<dynamic>();
            try
            {
                if (dataTable?.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var expandObject = (IDictionary<string, object>)new ExpandoObject();
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            expandObject.Add(column.ColumnName, row[column.ColumnName]);
                        }
                        result.Add(expandObject);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dataTable?.Dispose();
            }
            return result;
        }


        protected virtual string GetProductIdsToExclude(FilterCollection filters, out bool isNotInFilter)
        {
            isNotInFilter = false;
            //This condition execute only when Category list is required for Catalog category association.
            if (filters.Any(filterTuple => filterTuple.FilterName.Equals(Znode.Libraries.ECommerce.Utilities.FilterKeys.IsAssociatedProducts)))
            {
                isNotInFilter = true;
                IZnodeRepository<ZnodePimCategoryProduct> pimCategoryProduct = new ZnodeRepository<ZnodePimCategoryProduct>();
                IZnodeRepository<ZnodePimCategoryHierarchy> pimCategoryHierarchy = new ZnodeRepository<ZnodePimCategoryHierarchy>();

                //Get the catalog Id.
                int catalogId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodePimCatalogEnum.PimCatalogId.ToString().ToLower())?.FirstOrDefault()?.Item3);
                ZnodeLogging.LogMessage("catalogId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { catalogId });

                int categoryId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodePimCategoryEnum.PimCategoryId.ToString().ToLower())?.FirstOrDefault()?.Item3);
                ZnodeLogging.LogMessage("categoryId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { categoryId });

                List<int?> productIds = null;

                //If the category id is less than 1 then gets the product ids belong to category else get the products category belong to catalog.
                if (categoryId <= 0)
                    productIds = (from categoryProduct in pimCategoryProduct.Table 
                                  join categoryIds in 
                                  (from categoryHierarchy in pimCategoryHierarchy.Table
                                  where categoryHierarchy.PimCatalogId == catalogId
                                  select (int)categoryHierarchy.PimCategoryId).Distinct()
                                  on categoryProduct.PimCategoryId equals categoryIds
                                  select (int?)categoryProduct.PimProductId).ToList();
                else
                    productIds = (from categoryHierarchy in pimCategoryHierarchy.Table
                                  join categoryProduct in pimCategoryProduct.Table on categoryHierarchy.PimCategoryId equals categoryProduct.PimCategoryId
                                  where categoryHierarchy.PimCatalogId == catalogId && categoryHierarchy.PimCategoryId == categoryId
                                  select (int?)categoryProduct.PimProductId).ToList();

                //Get the distinct product ids from list.
                productIds = productIds.Select(x => x).Distinct().ToList();
                //Remove all the null entry.
                productIds?.RemoveAll(x => x == null);

                //Remove the filters.
                filters.Remove(filters.Where(x => x.FilterName == ZnodePimCatalogEnum.PimCatalogId.ToString().ToLower())?.FirstOrDefault());
                filters.Remove(filters.Where(x => x.FilterName == ZnodePimCategoryEnum.PimCategoryId.ToString().ToLower())?.FirstOrDefault());
                filters.Remove(filters.Where(x => x.FilterName == Znode.Libraries.ECommerce.Utilities.FilterKeys.IsAssociatedProducts.ToLower())?.FirstOrDefault());
                return string.Join(",", productIds);

            }
            return string.Empty;
        }

        //Inserts product data against catalog category.
        protected virtual  void InsertProductDataForCatalogCategory(ProductModel model)
        {
            ZnodeLogging.LogMessage("PimCatalogId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.PimCatalogId);

            if (model?.PimCatalogId > 0)
            {
                if (model?.ProductId > 0)
                {
                    ICategoryService _categoryService = GetService<ICategoryService>();
                    List<CategoryProductModel> categoryProductlistModel = new List<CategoryProductModel>();
                    categoryProductlistModel.Add(new CategoryProductModel { PimCategoryId = model?.PimCategoryId, PimProductId = model.ProductId, Status = true, DisplayOrder = 999 });
                    _categoryService.AssociateCategoryProduct(categoryProductlistModel);

                }
            }
        }
        
        //Get attribute default locale value if selected locale value not found.
        private string GetAttributeDefaultLocale(PIMAttributeModel pimAttributeModel)
        {
            FilterCollection attributeLocaleFilter = new FilterCollection();
            attributeLocaleFilter.Add(ZnodePimAttributeLocaleEnum.PimAttributeId.ToString(), FilterOperators.Equals, pimAttributeModel.PimAttributeId.ToString());
            attributeLocaleFilter.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, DefaultGlobalConfigSettingHelper.Locale);
            ZnodePimAttributeLocale pimAttributeLocale = _attributeLocaleRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(attributeLocaleFilter.ToFilterDataCollection()).WhereClause);
            return IsNull(pimAttributeLocale?.AttributeName) ? pimAttributeModel?.AttributeCode : pimAttributeLocale?.AttributeName;
        }

        //Get value of Product type from LocaleId
        private string GetProductTypeByLocale(int localeId)
        {
            ZnodeLogging.LogMessage("localeId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, localeId);

            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            return Convert.ToString(executeSpHelper.GetSPResultInObject("Znode_GetDefaultSimpleProductFilter"));
        }

        //Get value of  configure Product attribute
        protected virtual string GetAssociateProductAttributeIds(int productId)
        {
            string attributeId = string.Join(",", _configureProductAttributeRepository.Table?.Join(_attributeRepository.Table, p => p.PimAttributeId, q => q.PimAttributeId, (p, q) =>
                                new { q.PimAttributeId, p.PimProductId, q.IsConfigurable })?
                                    .Where(x => x.PimProductId == productId && x.IsConfigurable)?
                                    .Select(y => Equals(y.PimAttributeId, null) ? 0 : y.PimAttributeId)?.ToArray());
            return string.IsNullOrEmpty(attributeId) ? "0" : attributeId;
        }

        //Check whether product is associated with published category
        protected virtual bool IsProductAssociatedWithPublishedCategory(int productId)
            => (from pimCategoryProduct in new ZnodeRepository<ZnodePimCategoryProduct>().Table
                where pimCategoryProduct.PimProductId == productId
                select pimCategoryProduct.PimProductId).Any();

        protected virtual ProductDetailsListModel Xmldataset(DataSet ds, out int recordCount)
        {
            // out pageListModel.TotalRowCount
            recordCount = 0;
            if (!Equals(ds, null) && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var xml = Convert.ToString(ds.Tables[0]?.Rows[0]["ProductXML"]);

                if (!string.IsNullOrEmpty(xml))
                {
                    var columns = ds.Tables[1];

                    var _columnlist = GetDictionary(columns);

                    if (!string.IsNullOrEmpty(ds.Tables[2].Rows[0]["RowsCount"].ToString()))
                        recordCount = Convert.ToInt32(ds.Tables[2].Rows[0]["RowsCount"].ToString());

                    XDocument xmlDoc = XDocument.Parse(xml);

                    dynamic root = new ExpandoObject();

                    XmlToDynamic.Parse(root, xmlDoc.Elements().First());

                    var _list = new List<dynamic>();
                    if (!(root.MainProduct.Product is List<dynamic>))
                    {
                        var _product = root.MainProduct.Product;
                        _list.Add(root.MainProduct.Product);
                    }
                    else
                    {
                        _list = (List<dynamic>)root.MainProduct.Product;
                    }

                    return new ProductDetailsListModel { AttributeColumnName = _columnlist, XmlDataList = _list };
                }
            }
            return new ProductDetailsListModel();
        }

        private Dictionary<string, object> GetDictionary(DataTable dt)
        {
            return dt.AsEnumerable()
              .ToDictionary<DataRow, string, object>(row => row.Field<string>(0),
                                        row => row.Field<object>(1));
        }

        //Create product and child addon product association while associating a new addon group.
        private void CreateProductChildAddonProductAssociation(AddonProductListModel addonProducts)
        {
            AddOnProductDetailListModel addonProductDetailList = new AddOnProductDetailListModel();

            foreach (AddOnProductModel model in addonProducts.AddonProducts)
            {
                addonProductDetailList.AddOnProductDetailList = new List<AddOnProductDetailModel>();
                if (model?.PimAddonGroupId.GetValueOrDefault() > 0)
                {
                    List<int?> addonGroupProduct = _addonGroupProduct.Table.Where(x => x.PimAddonGroupId == model.PimAddonGroupId).Select(x => x.PimChildProductId).ToList();
                    ZnodeLogging.LogMessage("addonGroupProduct count", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonGroupProduct.Count);
                    foreach (int? childProductId in addonGroupProduct)
                    {
                        if (addonProducts.AddonProducts.FirstOrDefault().PimProductId != childProductId.GetValueOrDefault())
                        {
                            AddOnProductDetailModel addonProductDetail = new AddOnProductDetailModel()
                            {
                                PimAddOnProductId = model.PimAddOnProductId,
                                PimChildProductId = childProductId.GetValueOrDefault(),
                                DisplayOrder = 999,
                                IsDefault = false
                            };
                            addonProductDetailList.AddOnProductDetailList.Add(addonProductDetail);
                        }
                    }
                    ZnodeLogging.LogMessage("CreateAddonProductDetail method with parameter", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonProductDetailList);

                    if(addonProductDetailList.AddOnProductDetailList.Count > 0)
                        CreateAddonProductDetail(addonProductDetailList);
                }
            }
        }

        private bool IsMassProductUpdatePublishStart(string importedGuid)
        {
            while (!isPublishForGuid)
            {
                isPublishForGuid = _importSuccessLog.Table.Where(x => x.IsProductPublish == true && x.ImportedGuId == importedGuid).Select(x => x.IsProductPublish.Value).FirstOrDefault();
            }

            return isPublishForGuid;
        }

        //Get PimProductId by SKU
        private int GetPimProductIdBySKU(string sku)
        => _attributeValueLocaleRepository.Table.Where(l => l.AttributeValue == sku)
                                          .Join(_attributeRepository.Table.Where(o => o.AttributeCode.ToLower() == _skuText)
                                                                    .Join(_attributeValueRepository.Table,
                                                                          o => o.PimAttributeId,
                                                                          ob => ob.PimAttributeId,
                                                                          (o, ob) => new
                                                                          {
                                                                              ob.PimAttributeValueId,
                                                                              ob.PimProductId
                                                                          }),
                                           lc => lc.PimAttributeValueId,
                                           vl => vl.PimAttributeValueId,
                                          (lc, vl) => new
                                          {
                                              vl.PimProductId
                                          })?.FirstOrDefault()?.PimProductId ?? 0;

        #endregion Private Method

        #region Protected Method

        //Update Default Product and get status of update.
        protected virtual bool UpdateDefaultProduct(ProductTypeAssociationModel productTypeAssociationModel)
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimParentProductId", productTypeAssociationModel.PimParentProductId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PimProductId", productTypeAssociationModel.PimProductId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("IsDefault", productTypeAssociationModel.IsDefault, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("DisplayOrder", productTypeAssociationModel.DisplayOrder, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            int status = 0;

            IList<View_ReturnBoolean> updateResult = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateDefaultChildProductOfParent @PimParentProductId,@PimProductId,@IsDefault,@DisplayOrder,@Status OUT", 3, out status);

            return updateResult?.Count > 0 && updateResult?.FirstOrDefault()?.Id > 0 ? updateResult.FirstOrDefault().Status.Value : false;
        }

        protected virtual bool IsExportPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isExportPublishInProgress = _znodeExportProcessLog.Table.Any(x => x.Status == ZnodeConstant.ExportStatusInprogress || x.Status == ZnodeConstant.SearchIndexStartedStatus);


            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isExportPublishInProgress;
        }

        #endregion Protected Method
    }
}