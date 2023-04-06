using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Utilities = Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class PromotionService : BaseService, IPromotionService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePromotion> _promotionRepository;
        private readonly IZnodeRepository<ZnodePromotionCoupon> _couponRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodePromotionProduct> _promotionProductRepository;
        private readonly IZnodeRepository<ZnodePromotionCategory> _promotionCategoryRepository;
        private readonly IZnodeRepository<ZnodePromotionCatalog> _promotionCatalogRepository;
        private readonly IZnodeRepository<ZnodePromotionBrand> _promotionBrandRepository;
        private readonly IZnodeRepository<ZnodeBrandDetail> _brandDetailsRepository;
        private readonly IZnodeRepository<ZnodePublishProductEntity> _publishProductEntityRepository;
        private readonly IZnodeRepository<ZnodePromotionShipping> _promotionShippingRepository;

        #endregion

        #region Constructor
        public PromotionService()
        {
            _promotionRepository = new ZnodeRepository<ZnodePromotion>();
            _couponRepository = new ZnodeRepository<ZnodePromotionCoupon>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _promotionProductRepository = new ZnodeRepository<ZnodePromotionProduct>();
            _promotionCategoryRepository = new ZnodeRepository<ZnodePromotionCategory>();
            _promotionCatalogRepository = new ZnodeRepository<ZnodePromotionCatalog>();
            _promotionBrandRepository = new ZnodeRepository<ZnodePromotionBrand>();
            _brandDetailsRepository = new ZnodeRepository<ZnodeBrandDetail>();
            _publishProductEntityRepository = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);
            _promotionShippingRepository = new ZnodeRepository<ZnodePromotionShipping>();
        }
        #endregion

        #region Public Methods

        #region Promotion

        //Get promotion by promotion id.
        public virtual PromotionModel GetPromotion(int promotionId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            if (promotionId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ErrorPromotionIdlessThanOne);

            ZnodeLogging.LogMessage("Input parameter promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { promotionId });

            //get expands
            var navigationProperties = GetExpands(expands);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePromotionEnum.PromotionId.ToString(), FilterOperators.Equals, promotionId.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodePromotion promotion = _promotionRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties);
            PromotionModel promotionModel = new PromotionModel();
            if (IsNotNull(promotion))
            {
                promotionModel = promotion?.ToModel<PromotionModel, ZnodePromotion>();

                SetAssociationToPromotion(promotionModel, promotionId);

                if (!string.IsNullOrEmpty(promotionModel.AssociatedProductIds))
                    promotionModel.ProductName = GetProductName(promotionModel.AssociatedProductIds.Split(',')[0]);

                if (promotionModel.ReferralPublishProductId > 0)
                    promotionModel.ReferralProductName = GetProductName(Convert.ToString(promotionModel.ReferralPublishProductId));

                if (promotionModel.PortalId > 0)
                {
                    promotionModel.PortalAllowsMultipleCoupon = ZnodeDependencyResolver.GetService<IZnodeOrderHelper>().GetPortalFeatureValue(promotionModel.PortalId.GetValueOrDefault(), HelperUtility.StoreFeature.Allow_multiple_coupons);
                    promotionModel.StoreName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == promotionModel.PortalId)?.StoreName;
                }
                promotionModel.ProfileIds = GetPromotionProfileIds(promotionId);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return promotionModel;
        }

        //Get paged promotion list
        public virtual PromotionListModel GetPromotionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);

            //Bind portalId for promotion made on all Store.
            BindPortalIds(filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //get expands
            var navigationProperties = GetExpands(expands);

            IList<ZnodePromotion> promotionList = _promotionRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, navigationProperties, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("promotionList count", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { promotionList?.Count });

            PromotionListModel listModel = new PromotionListModel();
            listModel.PromotionList = promotionList?.Count > 0 ? promotionList.ToModel<PromotionModel>().ToList() : new List<PromotionModel>();
            //Get the Portal Details List.
            var portalList = _portalRepository.GetEntityList("");

            listModel.PromotionList.ForEach(z => z.StoreName = (IsNull(portalList.Where(a => a.PortalId == z.PortalId).Select(p => p.StoreName).FirstOrDefault())) ? "All Stores" : portalList.Where(a => a.PortalId == z.PortalId).Select(p => p.StoreName).FirstOrDefault());

            listModel?.PromotionList?.ForEach(item => item.PromotionTypeName = promotionList.Where(p => p?.ZnodePromotionType?.PromotionTypeId == item.PromotionTypeId).Select(p => p?.ZnodePromotionType.Name).FirstOrDefault());

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return listModel;

        }

        //Create new promotion
        public virtual PromotionModel CreatePromotion(PromotionModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            if (IsAlreadyExistsPromotion(model.PromoCode))
            {
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorPromotionExists);
            }
            foreach (var coupon in model?.CouponList?.CouponList)
            {
                if (model.IsUnique && string.IsNullOrEmpty(coupon.Code))
                {
                    throw new ZnodeException(ErrorCodes.CreationFailed, Admin_Resources.ErrorPromotionCreate);
                }
                if (IsAlreadyExistsCouponCode(coupon.Code))
                {
                    throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.AlreadyExistsCouponCode);
                }
            }

            //Replace default value by null
            ZnodeLogging.LogMessage("Executing SetDefaultValues.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            SetDefaultValues(model);

            // set the minimum quantity null
            if (model.QuantityMinimum == 0)
                model.QuantityMinimum = null;
            if (model.IsUnique)
                model?.CouponList?.CouponList?.ForEach(x => x.AvailableQuantity = 1);
            string promotionXML = HelperUtility.ToXML(model);

            //SP call to Insert/Update promotion.
            IZnodeViewRepository<PromotionModel> objStoredProc = new ZnodeViewRepository<PromotionModel>();
            objStoredProc.SetParameter("PromotionXML", promotionXML, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            PromotionModel promotion = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdatePromotions @PromotionXML, @UserId ,@Status OUT", 2, out status)?.FirstOrDefault();

            model.PromotionId = promotion.PromotionId;
            ZnodeLogging.LogMessage("Create promotion with PromotionId.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, model?.PromotionId);

            var clearCacheInitializer = new ZnodeEventNotifier<ZnodePromotionCoupon>(new ZnodePromotionCoupon());

            ZnodeLogging.LogMessage(IsNotNull(model) ? Admin_Resources.SuccessPromotionCreate :Admin_Resources.ErrorPromotionCreate, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return model;

        }

        //Save association against promotion
        public virtual bool SaveAssociation(int promotionId, PromotionModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters promotionId.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionId);

            bool isSavePromotions = false;
            if (!string.IsNullOrEmpty(model.AssociatedCatelogIds) && model.AssociatedCatelogIds != "0")
            {
                List<ZnodePromotionCatalog> promotionCatalog = new List<ZnodePromotionCatalog>();
                var associatedIds = model.AssociatedCatelogIds.Split(',').ToList();
                ZnodeLogging.LogMessage("associatedIds: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, associatedIds);

                foreach (var item in associatedIds)
                    promotionCatalog.Add(new ZnodePromotionCatalog() { PromotionId = promotionId, PublishCatalogId = Convert.ToInt32(item) });

                var createAssociation = _promotionCatalogRepository.Insert(promotionCatalog);
                ZnodeLogging.LogMessage("Insert promotionCatalog with promotionCatalog count : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionCatalog?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            else if (!string.IsNullOrEmpty(model.AssociatedCategoryIds) && model.AssociatedCategoryIds != "0")
            {
                List<ZnodePromotionCategory> promotionCategory = new List<ZnodePromotionCategory>();
                var associatedIds = model.AssociatedCategoryIds.Split(',').ToList();
                ZnodeLogging.LogMessage("associatedIds: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, associatedIds);

                foreach (var item in associatedIds)
                    promotionCategory.Add(new ZnodePromotionCategory() { PromotionId = promotionId, PublishCategoryId = Convert.ToInt32(item) });
                var createAssociation = _promotionCategoryRepository.Insert(promotionCategory);
                ZnodeLogging.LogMessage("Insert promotionCategory with promotionCategory count : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionCategory?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            else if (!string.IsNullOrEmpty(model.AssociatedProductIds) && model.AssociatedProductIds != "0")
            {
                if ((model.PromotionTypeName == ZnodeConstant.AmountOfDisplayedProductPrice || model.PromotionTypeName == ZnodeConstant.PercentOfDisplayedProductPrice) &&
                    Equals(model.PortalId, null))
                    model.AssociatedProductIds = GetAllCatalogProductIds(model.AssociatedProductIds);

                List<ZnodePromotionProduct> promotionProduct = new List<ZnodePromotionProduct>();
                var associatedIds = model.AssociatedProductIds.Split(',').ToList();
                ZnodeLogging.LogMessage("associatedIds: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, associatedIds);

                foreach (var item in associatedIds)
                    promotionProduct.Add(new ZnodePromotionProduct() { PromotionId = promotionId, PublishProductId = Convert.ToInt32(item) });
                var createAssociation = _promotionProductRepository.Insert(promotionProduct);
                ZnodeLogging.LogMessage("Insert promotionProduct with promotionProduct count : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionProduct?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            else if (!string.IsNullOrEmpty(model.AssociatedBrandIds) && model.AssociatedBrandIds != "0")
            {
                List<ZnodePromotionBrand> promotionBrand = new List<ZnodePromotionBrand>();
                var associatedIds = model.AssociatedBrandIds.Split(',').ToList();
                ZnodeLogging.LogMessage("associatedIds: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, associatedIds);

                foreach (var item in associatedIds)
                {
                    int brandId = Convert.ToInt32(item);
                    string brandCode = _brandDetailsRepository.Table.Where(x => x.BrandId == brandId).Select(x => x.BrandCode).FirstOrDefault();
                    promotionBrand.Add(new ZnodePromotionBrand() { PromotionId = promotionId, BrandId = brandId, BrandCode = brandCode });
                }
                var createAssociation = _promotionBrandRepository.Insert(promotionBrand);
                ZnodeLogging.LogMessage("Insert promotionBrand with promotionBrand count : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionBrand?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            else if (!string.IsNullOrEmpty(model.AssociatedShippingIds) && model.AssociatedShippingIds != "0")
            {
                List<ZnodePromotionShipping> promotionshipping = new List<ZnodePromotionShipping>();
                var associatedIds = model.AssociatedShippingIds.Split(',').ToList();
                ZnodeLogging.LogMessage("associatedIds: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, associatedIds);

                foreach (var item in associatedIds)
                    promotionshipping.Add(new ZnodePromotionShipping() { PromotionId = promotionId, ShippingId = Convert.ToInt32(item) });
                var createAssociation = _promotionShippingRepository.Insert(promotionshipping);
                ZnodeLogging.LogMessage("Insert promotionshipping with promotionshipping count : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionshipping?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            else
                isSavePromotions = true;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return isSavePromotions;
        }

        public virtual string GetAllCatalogProductIds(string AssociatedProductIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters AssociatedProductIds.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, AssociatedProductIds);

            var arrayAssociatedProductIds = AssociatedProductIds.Split(',').ToList();
            List<int> productIdsForallCatelog = new List<int>();
            foreach (var item in arrayAssociatedProductIds)
            {
                int associatedProdId = Convert.ToInt32(item);
                int LocaleID = GetDefaultLocaleId();
                var publishedProductIds = (from attr in _publishProductEntityRepository.Table
                                           join locale in _publishProductEntityRepository.Table
                                                on new { attr.SKU, attr.LocaleId } equals new { locale.SKU, locale.LocaleId }
                                           where attr.ZnodeProductId == associatedProdId && attr.LocaleId == LocaleID
                                           select (int)locale.ZnodeProductId
                                           ).ToList();
                productIdsForallCatelog.AddRange(publishedProductIds);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return string.Join(",", productIdsForallCatelog.ToArray());
        }

        //Update promotion
        public virtual bool UpdatePromotion(PromotionModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PromotionModel with PromotionId.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, model?.PromotionId);

            bool isUpdatePromotions = false;

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorPromotionModelNull);
            if (model.PromotionId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorPromotionIdlessThanOne);

            if (model?.PromotionId > 0)
            {
                SetDefaultValues(model);

                // set the minimum quantity null
                if (model.QuantityMinimum == 0)
                    model.QuantityMinimum = null;

                string promotionXML = HelperUtility.ToXML(model);

                //SP call to Insert/update Promotion.
                IZnodeViewRepository<PromotionModel> objStoredProc = new ZnodeViewRepository<PromotionModel>();
                objStoredProc.SetParameter("PromotionXML", promotionXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                PromotionModel promotion = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdatePromotions @PromotionXML, @UserId ,@Status OUT", 2, out status)?.FirstOrDefault();

                isUpdatePromotions = status.Equals(0) ? false : true;

         
                if (!isUpdatePromotions)
                    throw new ZnodeException(ErrorCodes.InternalItemNotUpdated, Admin_Resources.ErrorPromotionUpdate);
            }
            //Clear webStore Cache on success update.
            if (isUpdatePromotions)
            {
                model.PortalId = PortalId;
                var clearCachePromotionWebstore = new ZnodeEventNotifier<PromotionModel>(model);
            }
            var clearCacheInitializer = new ZnodeEventNotifier<ZnodePromotionCoupon>(new ZnodePromotionCoupon());

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return isUpdatePromotions;
        }

        //Delete promotion by promotionId.
        public virtual bool DeletePromotion(ParameterModel promotionId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("promotionId to be deleted.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionId?.Ids);

            //Checks promotion id.
            if (string.IsNullOrEmpty(promotionId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorPromotionIdNull);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePromotionEnum.PromotionId.ToString(), promotionId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_DeletePromotion @PromotionId,  @Status OUT", 1, out status);

            //SP will return status as 1 if promotion as well as all its associated items deleted successfully.
            if (Equals(status, 1))
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                var clearCacheInitializer = new ZnodeEventNotifier<ZnodePromotionCoupon>(new ZnodePromotionCoupon());
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeletePromotionDueToAssociation);
            }

        }

        //Get Published Categories
        public virtual CategoryListModel GetPublishedCategoryList(ParameterModel filterIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            string localeId = DefaultGlobalConfigSettingHelper.Locale;

            FilterCollection filters = new FilterCollection();
            if(!string.IsNullOrEmpty(filterIds.Ids))
                filters.Add("ZnodeCatalogId", FilterOperators.In, filterIds.Ids);
            filters.Add("LocaleId", FilterOperators.Equals, localeId);

            List<ZnodePublishCategoryEntity> categoryList =  GetService<IPublishedCategoryDataService>().GetPublishedCategoryList(new PageListModel(filters, null, null));

            CategoryListModel categoryListModel = new CategoryListModel { Categories = categoryList.ToModel<CategoryModel>().ToList() };

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return categoryListModel;
        }

        //Get Published Products
        public virtual ProductDetailsListModel GetPublishedProductList(ParameterModel filterIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            SetLocaleFilterIfNotPresent(ref filters);

            string localeId = filters?.Where(x => x.FilterName.ToLower() == Utilities.FilterKeys.LocaleId)?.FirstOrDefault().FilterValue;
            filters?.RemoveAll(x => x.FilterName == Utilities.FilterKeys.LocaleId);

            List<ZnodePublishProductEntity> productList = GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(GetFilters(filterIds, localeId), null, null));
            ZnodeLogging.LogMessage("productList count", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, productList?.Count);

            ProductDetailsListModel productListModel = new ProductDetailsListModel { ProductDetailList = productList.ToModel<ProductDetailsModel>().ToList() };

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return productListModel;
        }

        //Get filters for Published products
        protected virtual FilterCollection GetFilters(ParameterModel filterIds, string localeId)
        {
            int? catalogVersionId = GetCatalogVersionId();
            FilterCollection filters = new FilterCollection();
            if(!string.IsNullOrEmpty(filterIds.Ids))
                filters.Add("ZnodeCategoryIds", FilterOperators.In, filterIds.Ids);
            filters.Add("LocaleId", FilterOperators.Equals, localeId);
            if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.Value.ToString());

            return filters;
        }

        //Get promotion attributes with discount id.
        public virtual PIMFamilyDetailsModel GetPromotionAttribute(string discountName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            //Get all attributes associated with default family and familyId
            IZnodeViewRepository<PIMProductAttributeValuesModel> pimAttributeValues = new ZnodeViewRepository<PIMProductAttributeValuesModel>();

            pimAttributeValues.SetParameter("@DiscountTypeName", discountName, ParameterDirection.Input, DbType.String);
            var pimAttributes = pimAttributeValues.ExecuteStoredProcedureList("Znode_GetPromotionAttributeValues @DiscountTypeName");

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return !Equals(pimAttributes, null) ? new PIMFamilyDetailsModel
            {
                Attributes = pimAttributes.ToList(),
            }
                : null;
        }

        //Associate catalog to already created promotion.
        public virtual bool AssociateCatalogToPromotion(AssociatedParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            bool isSavePromotions = false;
            if (!string.IsNullOrEmpty(model.AssociateIds))
            {
                List<ZnodePromotionCatalog> promotionCatalog = new List<ZnodePromotionCatalog>();
                var associatedIds = model.AssociateIds.Split(',').ToList();
                ZnodeLogging.LogMessage("associatedIds :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, associatedIds);

                foreach (var item in associatedIds)
                {
                    promotionCatalog.Add(new ZnodePromotionCatalog() { PromotionId = model.PromotionId, PublishCatalogId = Convert.ToInt32(item) });
                }
                var createAssociation = _promotionCatalogRepository.Insert(promotionCatalog);
                ZnodeLogging.LogMessage("Insert promotionCatalog with promotionCatalog count ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionCatalog?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return isSavePromotions;
        }

        //Associate category to already created promotion.
        public virtual bool AssociateCategoryToPromotion(AssociatedParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            bool isSavePromotions = false;
            if (!string.IsNullOrEmpty(model.AssociateIds))
            {
                List<ZnodePromotionCategory> promotionCategory = new List<ZnodePromotionCategory>();
                var associatedIds = model.AssociateIds.Split(',').ToList();
                ZnodeLogging.LogMessage("associatedIds :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, associatedIds);

                foreach (var item in associatedIds)
                {
                    promotionCategory.Add(new ZnodePromotionCategory() { PromotionId = model.PromotionId, PublishCategoryId = Convert.ToInt32(item) });
                }
                var createAssociation = _promotionCategoryRepository.Insert(promotionCategory);
                ZnodeLogging.LogMessage("Insert promotionCategory with promotionCategory count ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionCategory?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return isSavePromotions;
        }

        //Associate products to already created promotion..
        public virtual bool AssociateProductToPromotion(AssociatedParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            bool isSavePromotions = false;
            if (!string.IsNullOrEmpty(model.AssociateIds))
            {
                if ((model.DiscountTypeName == ZnodeConstant.AmountOfDisplayedProductPrice || model.DiscountTypeName == ZnodeConstant.PercentOfDisplayedProductPrice) &&
                    Equals(model.PortalId, null))
                    model.AssociateIds = GetAllCatalogProductIds(model.AssociateIds);

                List<ZnodePromotionProduct> promotionProduct = new List<ZnodePromotionProduct>();
                var associatedIds = model.AssociateIds.Split(',').ToList();
                foreach (var item in associatedIds)
                {
                    promotionProduct.Add(new ZnodePromotionProduct() { PromotionId = model.PromotionId, PublishProductId = Convert.ToInt32(item) });
                }
                var createAssociation = _promotionProductRepository.Insert(promotionProduct);
                ZnodeLogging.LogMessage("Insert promotionProduct with promotionProduct count ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, promotionProduct?.Count);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return isSavePromotions;
        }

        //Get list of associated or UnAssociated Product  on the basis of isAssociatedProduct.
        public virtual PublishProductListModel GetAssociatedUnAssociatedProductList(int portalId, string productIds, int promotionId, bool isAssociatedProduct, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        => isAssociatedProduct ? GetAssociatedProductList(portalId, productIds, promotionId, expands, filters, sorts, page) : GetUnAssociatedProductList(portalId, productIds, promotionId, expands, filters, sorts, page);

        //Get list of associated Ptoducts from promotion id.
        public virtual PublishProductListModel GetAssociatedProductList(int portalId, string productIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId, productIds, promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose,new object[] { portalId, productIds, promotionId });

            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName == Utilities.FilterKeys.LocaleId)?.FilterValue, out localeId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PromotionId.ToLower());
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.ZnodeCatalogId.ToLower());
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.LocaleId.ToLower());
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.RevisionType);
            if (promotionId > 0)
            {
                //Get associated Product ids seperated by comma.
                var AssociatedProductIds = string.Join(",", _promotionProductRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishProductId).ToArray());
                AssociatedProductIds = !string.IsNullOrEmpty(AssociatedProductIds) ? AssociatedProductIds : "0";
                filters.Add(Utilities.FilterKeys.PublishProductId, FilterOperators.In, AssociatedProductIds);
            }
            else
                filters.Add(Utilities.FilterKeys.PublishProductId, FilterOperators.In, productIds);

            ReplaceSortKeysForProduct(ref sorts, promotionId);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection());
            IZnodeViewRepository<PublishProductModel> objStoredProc = new ZnodeViewRepository<PublishProductModel>();
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PromotionId", promotionId, ParameterDirection.Input, DbType.Int32);
            var productList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPromotionPublishProduct @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PromotionId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("productList count ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, productList?.Count);

            PublishProductListModel listModel = new PublishProductListModel();
            listModel.PublishProducts = productList?.Count > 0 ? productList.ToList() : new List<PublishProductModel>();
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Get list of un-associated products from mango
        public virtual PublishProductListModel GetUnAssociatedProductList(int portalId, string productIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId, productIds, promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId, productIds, promotionId });
            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName == Utilities.FilterKeys.LocaleId)?.FilterValue, out localeId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PromotionId.ToLower());
            //Get filter value
            string filterValue = filters.FirstOrDefault(x => x.FilterName.ToLower() == Utilities.FilterKeys.AttributeValuesForPromotion.ToString().ToLower() && x.FilterOperator == FilterOperators.In)?.FilterValue;

            if (!string.IsNullOrEmpty(filterValue))
            {
                //Remove Attribute Values For Promotion Filters with IN operator from filters list
                filters.RemoveAll(x => x.FilterName.ToLower() == Utilities.FilterKeys.AttributeValuesForPromotion.ToString().ToLower() && x.FilterOperator == FilterOperators.In);

                //Add Attribute Values For Promotion Filters
                filters.Add(Utilities.FilterKeys.AttributeValuesForPromotion, FilterOperators.In, filterValue.Replace('_', ','));
            }
            if (promotionId > 0)
            {
                //Get associated Product ids seperated by comma.
                var AssociatedProductIds = string.Join(",", _promotionProductRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishProductId).ToArray());
                ZnodeLogging.LogMessage("AssociatedProductIds count", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { AssociatedProductIds});

                AssociatedProductIds = !string.IsNullOrEmpty(AssociatedProductIds) ? AssociatedProductIds : "0";
                filters.Add(Utilities.FilterKeys.ZnodeProductId, FilterOperators.NotIn, AssociatedProductIds);
            }
            else
                filters.Add(Utilities.FilterKeys.ZnodeProductId, FilterOperators.In, productIds);

            //Get catalog id from filter.
            int catalogId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);
            //get catalog current version id by catalog id.
            int? currentCatalogVersionId = GetCatalogVersionId(catalogId, localeId, portalId);
            ZnodeLogging.LogMessage("currentCatalogVersionId returned from GetCatalogVersionId", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { currentCatalogVersionId });

            if (catalogId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(currentCatalogVersionId));
            else
            {
                string versionIds = filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType) ? GetCatalogAllVersionIds(localeId) : GetCatalogAllVersionIds();
                if(!string.IsNullOrEmpty(versionIds))
                    filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, versionIds);
            }
            if (filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == Utilities.FilterKeys.RevisionType);

            //Replace filter keys with published filter keys
            ReplaceProductFilterKeys(ref filters);
            promotionId = 0;
            ReplaceSortKeysForProduct(ref sorts, promotionId);

            SetProductIndexFilter(filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //get publish products                      
            List<ZnodePublishProductEntity> products = GetService<IPublishedProductDataService>().GetPublishProductsPageList(pageListModel, out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("products count", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { products?.Count});

            PublishProductListModel publishProductListModel = new PublishProductListModel() { PublishProducts = products.ToModel<PublishProductModel>()?.ToList() };

            IPublishProductHelper publishProductHelper = GetService<IPublishProductHelper>();
            if (publishProductListModel?.PublishProducts?.Count > 0)
                //get expands associated to Product
                publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProductListModel, localeId, 0, currentCatalogVersionId.Value);

            SetProductDetailsForList(portalId, publishProductListModel);

            //Map pagination parameters
            publishProductListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return publishProductListModel;
        }
        //Get list of associated or UnAssociated Category  on the basis of isAssociatedCategory.
        public virtual PublishCategoryListModel GetAssociatedUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId, bool isAssociatedCategory, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        => isAssociatedCategory ? GetAssociatedCategoryList(portalId, categoryIds, promotionId, expands, filters, sorts, page) : GetUnAssociatedCategoryList(portalId, categoryIds, promotionId, expands, filters, sorts, page);

        //Get associated category list
        public virtual PublishCategoryListModel GetUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId,categoryIds,promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId, categoryIds,promotionId });

            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName == Utilities.FilterKeys.LocaleId)?.FilterValue, out localeId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PromotionId.ToLower());

            //Get catalog id from filter.
            int catalogId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);

            SetCategoryIndexFilter(filters);

            if (promotionId > 0)
            {
                //Get associated catelog ids seperated by comma.
                var AssociatedCategoryIds = string.Join(",", _promotionCategoryRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishCategoryId).ToArray());
                filters.Add(Utilities.FilterKeys.ZnodeCategoryId, FilterOperators.NotIn, !string.IsNullOrEmpty(AssociatedCategoryIds) ? AssociatedCategoryIds : "0");
            }
            else
                filters.Add(Utilities.FilterKeys.ZnodeCategoryId, FilterOperators.In, !string.IsNullOrEmpty(categoryIds) ? categoryIds : "0");

            if (catalogId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId, localeId)));
            else
            {
                string versionIds = filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType) ? GetCatalogAllVersionIds(localeId) : GetCatalogAllVersionIds();
                if (!string.IsNullOrEmpty(versionIds))
                    filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, versionIds);
            }
            if (filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == Utilities.FilterKeys.RevisionType);
            //Replace filter keys with published filter keys
            ReplaceCategoryFilterKeys(ref filters);
            ReplaceSortKeysForCategory(ref sorts);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //get publish categories   
            List<ZnodePublishCategoryEntity> categories = GetService<IPublishedCategoryDataService>().GetPublishCategoryPageList(pageListModel, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("publish categories count  ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { categories?.Count });

            PublishCategoryListModel publishCategoryListModel = new PublishCategoryListModel() { PublishCategories = categories.ToModel<PublishCategoryModel>()?.ToList() };

            //get products associated to categories from expands
            GetDataFromExpands(expands, publishCategoryListModel, localeId);

            //Map pagination parameters
            publishCategoryListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return publishCategoryListModel;
        }


        //Get list of associated Category from promotion id.
        public virtual PublishCategoryListModel GetAssociatedCategoryList(int portalId, string categoryIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId,categoryIds,promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId, categoryIds, promotionId });

            //Get associated Category ids seperated by comma.
            var AssociatedCategoryIds = string.Join(",", _promotionCategoryRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishCategoryId).ToArray());
            AssociatedCategoryIds = !string.IsNullOrEmpty(AssociatedCategoryIds) ? AssociatedCategoryIds : "0";
            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName == Utilities.FilterKeys.LocaleId)?.FilterValue, out localeId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PromotionId.ToLower());
            //Add associated list to ZnodeCategoryId filter.
            filters.Add(Utilities.FilterKeys.ZnodeCategoryId, FilterOperators.In, AssociatedCategoryIds);

            //Get catalog id from filter.
            int catalogId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);

            if (catalogId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId)));
            else
            {
                string versionIds = filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType) ? GetCatalogAllVersionIds(localeId) : GetCatalogAllVersionIds();
                if (!string.IsNullOrEmpty(versionIds))
                    filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, versionIds);
            }
            if (filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == Utilities.FilterKeys.RevisionType);
            //Replace filter keys with published filter keys
            ReplaceCategoryFilterKeys(ref filters);
            ReplaceSortKeysForCategory(ref sorts);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //get publish categories   
            List<ZnodePublishCategoryEntity> categories = GetService<IPublishedCategoryDataService>().GetPublishCategoryPageList(pageListModel, out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("publish categories count  ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { categories?.Count });

            PublishCategoryListModel publishCategoryListModel = new PublishCategoryListModel() { PublishCategories = categories?.ToModel<PublishCategoryModel>()?.ToList() };

            publishCategoryListModel?.PublishCategories?.Where(c => c.PromotionId == 0).ToList().ForEach(cc => cc.PromotionId = promotionId);

            //get products associated to categories from expands
            GetDataFromExpands(expands, publishCategoryListModel, localeId);

            //Map pagination parameters
            publishCategoryListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return publishCategoryListModel;
        }

        //Get list of associated or UnAssociated Catalog  on the basis of isAssociatedCatalog.
        public virtual PublishCatalogListModel GetAssociatedUnAssociatedCatalogList(int portalId, string catalogIds, int promotionId, bool isAssociatedCatalog, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        => isAssociatedCatalog ? GetAssociatedCatalogList(portalId, catalogIds, promotionId, expands, filters, sorts, page) : GetUnAssociatedCatalogList(portalId, catalogIds, promotionId, expands, filters, sorts, page);

        //Get list of un-associated catelogs from mango
        public virtual PublishCatalogListModel GetUnAssociatedCatalogList(int portalId, string catalogIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId, catalogIds, promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId, catalogIds, promotionId });

            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName == Utilities.FilterKeys.LocaleId)?.FilterValue, out localeId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.LocaleId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PromotionId.ToLower());
            if (promotionId > 0)
            {
                //Get associated catelog ids seperated by comma.
                var AssociatedCatelogIds = string.Join(",", _promotionCatalogRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishCatalogId).ToArray());
                filters.Add(Utilities.FilterKeys.ZnodeCatalogId, FilterOperators.NotIn, !string.IsNullOrEmpty(AssociatedCatelogIds) ? AssociatedCatelogIds : "0");
            }
            else
                filters.Add(Utilities.FilterKeys.ZnodeCatalogId, FilterOperators.In, !string.IsNullOrEmpty(catalogIds) ? catalogIds : "0");

            //Replace filter keys with published filter keys
            ReplaceCatalogFilterKeys(ref filters);
            ReplaceSortKeysForCatalog(ref sorts);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //get publish categories
            List<ZnodePublishCatalogEntity> catalogs = GetService<IPublishedCatalogDataService>().GetPublishedCatalogPagedList(pageListModel)?.GroupBy(x => x.ZnodeCatalogId, (key, group) => group.First())?.ToList();

            ZnodeLogging.LogMessage("publish categories count  ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { catalogs?.Count });

            //map catalog entity to catalog model
            PublishCatalogListModel publishCatalogs = new PublishCatalogListModel() { PublishCatalogs = catalogs?.ToModel<PublishCatalogModel>()?.ToList() };

            //get products,categories associated to catalogs from expands
            GetDataFromExpands(expands, publishCatalogs, localeId);

            if(IsNotNull(catalogs))
                pageListModel.TotalRowCount = catalogs.Count;
            //Map pagination parameters
            publishCatalogs.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishCatalogs;
        }

        //Get list of associated catalogs from promotion id.
        public virtual PublishCatalogListModel GetAssociatedCatalogList(int portalId, string catalogIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId, catalogIds, promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId, catalogIds, promotionId });

            //Get associated catalog ids seperated by comma.
            var AssociatedCatalogIds = "";
            if (promotionId > 0)
            {
                AssociatedCatalogIds = string.Join(",", _promotionCatalogRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishCatalogId).ToArray());
                AssociatedCatalogIds = !string.IsNullOrEmpty(AssociatedCatalogIds) ? AssociatedCatalogIds : "0";
            }
            else
                AssociatedCatalogIds = catalogIds;


            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName == Utilities.FilterKeys.LocaleId)?.FilterValue, out localeId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.LocaleId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PromotionId.ToLower());
            //Add associated list to ZnodeCatalogId filter.
            filters.Add(Utilities.FilterKeys.ZnodeCatalogId, FilterOperators.In, !string.IsNullOrEmpty(AssociatedCatalogIds) ? AssociatedCatalogIds : "0");

            string versionIds = GetAssociatedCatalogVersionId(AssociatedCatalogIds);

            //Bind version id filter to avoid duplicate catalogs fetched while the associated catalogs are publishing.
            if(!string.IsNullOrEmpty(versionIds))
                filters.Add(Utilities.FilterKeys.VersionId, FilterOperators.In, versionIds);

            //Replace filter keys with published filter keys
            ReplaceCatalogFilterKeys(ref filters);
            ReplaceSortKeysForCatalog(ref sorts);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });


            //get published categories 
            List<ZnodePublishCatalogEntity> catalogs = GetService<IPublishedCatalogDataService>().GetPublishedCatalogPagedList(pageListModel)?.GroupBy(x => x.ZnodeCatalogId, (key, group) => group.First())?.ToList();

            ZnodeLogging.LogMessage("publish categories count   ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { catalogs?.Count });

            //map catalog entity to catalog model
            PublishCatalogListModel publishCatalogs = new PublishCatalogListModel() { PublishCatalogs = catalogs?.ToModel<PublishCatalogModel>()?.ToList() };
            publishCatalogs?.PublishCatalogs?.Where(c => c.PromotionId == 0).ToList().ForEach(cc => cc.PromotionId = promotionId);
            if(IsNotNull(catalogs))
                pageListModel.TotalRowCount = catalogs.Count;
            //Map pagination parameters
            publishCatalogs.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return publishCatalogs;
        }

        //Removes a product type association entry from promotion.
        public virtual bool UnAssociateProduct(ParameterModel publishProductIds, int promotionId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters publishProductIds, promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { publishProductIds, promotionId });

            if (string.IsNullOrEmpty(publishProductIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAssociatedProductIdNull);

            FilterCollection filters = new FilterCollection();

            //Create tuple to generate where clause.             
            filters.Add(new FilterTuple(ZnodePromotionProductEnum.PublishProductId.ToString(), ProcedureFilterOperators.In, publishProductIds.Ids));
            filters.Add(new FilterTuple(ZnodePromotionProductEnum.PromotionId.ToString(), ProcedureFilterOperators.Equals, promotionId.ToString()));

            //Generating where clause to get account details.             
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return _promotionProductRepository.Delete(whereClauseModel.WhereClause);
        }

        //Removes a Category type association entry from promotion.
        public virtual bool UnAssociateCategory(ParameterModel publishCategoryIds, int promotionId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters publishCategoryIds, promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { publishCategoryIds, promotionId });

            if (string.IsNullOrEmpty(publishCategoryIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAssociatedCategoryIdNull);

            FilterCollection filters = new FilterCollection();

            //Create tuple to generate where clause.             
            filters.Add(new FilterTuple(ZnodePromotionCategoryEnum.PublishCategoryId.ToString(), ProcedureFilterOperators.In, publishCategoryIds.Ids));
            filters.Add(new FilterTuple(ZnodePromotionCategoryEnum.PromotionId.ToString(), ProcedureFilterOperators.Equals, promotionId.ToString()));

            //Generating where clause to get account details.             
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return _promotionCategoryRepository.Delete(whereClauseModel.WhereClause);
        }

        //Removes a Catalog type association entry from promotion.
        public virtual bool UnAssociateCatalog(ParameterModel publishCatalogIds, int promotionId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(publishCatalogIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAssociatedCatalogIdNull);

            FilterCollection filters = new FilterCollection();

            //Create tuple to generate where clause.             
            filters.Add(new FilterTuple(ZnodePromotionCatalogEnum.PublishCatalogId.ToString(), ProcedureFilterOperators.In, publishCatalogIds.Ids));
            filters.Add(new FilterTuple(ZnodePromotionCatalogEnum.PromotionId.ToString(), ProcedureFilterOperators.Equals, promotionId.ToString()));

            //Generating where clause to get account details.             
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return _promotionCatalogRepository.Delete(whereClauseModel.WhereClause);
        }
        #endregion

        #region Coupon

        public virtual CouponModel GetCoupon(FilterCollection filters)
        {
            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            //gets coupon as per filter passed
            return _couponRepository.GetEntity(whereClauseModel.WhereClause).ToModel<CouponModel>();
        }

        //Get Coupon List
        public virtual CouponListModel GetCouponList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetEntityList", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //maps the entity list to model
            IList<ZnodePromotionCoupon> couponList = _couponRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues);
            ZnodeLogging.LogMessage("couponList count ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { couponList.Count });

            CouponListModel listModel = new CouponListModel();

            listModel.CouponList = couponList?.Count > 0 ? couponList.ToModel<CouponModel>().ToList() : new List<CouponModel>();
            listModel.CouponList?.ForEach(x => { x.CouponApplied = x.AvailableQuantity == 0; });

            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            return listModel;
        }
        #endregion

        #region Brand
        //Associate Brand to already created promotion.
        //Get list of Brand.
        public virtual BrandListModel GetAssociatedUnAssociatedBrandList(string brandIds, int promotionId, bool isAssociatedBrand, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters brandIds, promotionId", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { brandIds, promotionId });

            //Get locale Id and check if brand associated to product or not.
            int localeId = Convert.ToInt32(filters.Find(x => x.FilterName.Equals(Utilities.FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.Item3);
            if (promotionId == 0)
            {
                if (filters.Exists(x => x.Item1 == ZnodePromotionBrandEnum.BrandId.ToString()))
                    filters.RemoveAll(x => x.Item1 == ZnodePromotionBrandEnum.BrandId.ToString());

                if (!isAssociatedBrand && !string.IsNullOrEmpty(brandIds))
                    filters.Add(new FilterTuple(ZnodePromotionBrandEnum.BrandId.ToString(), FilterOperators.NotIn, brandIds.ToString()));
                else if (isAssociatedBrand && !string.IsNullOrEmpty(brandIds))
                    filters.Add(new FilterTuple(ZnodePromotionBrandEnum.BrandId.ToString(), FilterOperators.In, brandIds.ToString()));
            }
            else if (!isAssociatedBrand && promotionId > 0)
            {
                brandIds = string.Join(",", _promotionBrandRepository.Table.Where(x => x.PromotionId == promotionId).Select(g => g.BrandId).ToArray());
                brandIds = !string.IsNullOrEmpty(brandIds) ? brandIds : "0";
                filters.Add(new FilterTuple(ZnodePromotionBrandEnum.BrandId.ToString(), FilterOperators.NotIn, brandIds.ToString()));
            }
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<BrandModel> objStoredProc = new ZnodeViewRepository<BrandModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", promotionId == 0 ? false : isAssociatedBrand, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@PromotionId", promotionId, ParameterDirection.Input, DbType.Int32);

            IList<BrandModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetBrandDetailsLocale @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@IsAssociated,@PromotionId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("list count ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { list.Count });

            BrandListModel listModel = new BrandListModel { Brands = list?.ToList() };

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return listModel;
        }

        public virtual bool AssociateBrandToPromotion(AssociatedParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            bool isSavePromotions = false;
            if (!string.IsNullOrEmpty(model.AssociateIds))
            {
                List<ZnodePromotionBrand> promotionBrand = new List<ZnodePromotionBrand>();
                string[] associatedIds = model.AssociateIds.Split(',');

                foreach (string item in associatedIds)
                {
                    int brandId = Convert.ToInt32(item);
                    string brandCode = _brandDetailsRepository.Table.Where(x => x.BrandId == brandId).Select(g => g.BrandCode).FirstOrDefault();
                    promotionBrand.Add(new ZnodePromotionBrand() { PromotionId = model.PromotionId, BrandId = brandId, BrandCode = brandCode });
                }

                var createAssociation = _promotionBrandRepository.Insert(promotionBrand);

                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return isSavePromotions;
        }

        //Removes a Brand type association entry from promotion.
        public virtual bool UnAssociateBrand(ParameterModel brandIds, int promotionId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters brandIds, promotionId", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { brandIds, promotionId });

            if (string.IsNullOrEmpty(brandIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAssociatedBrandIdNull);

            FilterCollection filters = new FilterCollection();

            //Create tuple to generate where clause.             
            filters.Add(new FilterTuple(ZnodePromotionBrandEnum.BrandId.ToString(), ProcedureFilterOperators.In, brandIds.Ids));
            filters.Add(new FilterTuple(ZnodePromotionProductEnum.PromotionId.ToString(), ProcedureFilterOperators.Equals, promotionId.ToString()));
            //Generating where clause to get account details.             
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return _promotionBrandRepository.Delete(whereClauseModel.WhereClause);
        }
        #endregion

        #region Shipping
        //Associate Shipping to already created promotion.
        //Get list of Shipping.
        public virtual ShippingListModel GetAssociatedUnAssociatedShippingList(int portalId, string ShippingIds, int promotionId, bool isAssociatedShipping, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId, ShippingIds, promotionId", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId, ShippingIds, promotionId });

            if (promotionId == 0)
            {
                if (filters.Exists(x => x.Item1 == ZnodePromotionShippingEnum.ShippingId.ToString()))
                    filters.RemoveAll(x => x.Item1 == ZnodePromotionShippingEnum.ShippingId.ToString());

                if (!isAssociatedShipping && !string.IsNullOrEmpty(ShippingIds))
                    filters.Add(new FilterTuple(ZnodePromotionShippingEnum.ShippingId.ToString(), FilterOperators.NotIn, ShippingIds.ToString()));
                else if (isAssociatedShipping && !string.IsNullOrEmpty(ShippingIds))
                    filters.Add(new FilterTuple(ZnodePromotionShippingEnum.ShippingId.ToString(), FilterOperators.In, ShippingIds.ToString()));
            }
            else if (!isAssociatedShipping && promotionId > 0)
            {
                ShippingIds = string.Join(",", _promotionShippingRepository.Table.Where(x => x.PromotionId == promotionId).Select(g => g.ShippingId).ToArray());
                ShippingIds = !string.IsNullOrEmpty(ShippingIds) ? ShippingIds : "0";
                filters.Add(new FilterTuple(ZnodePromotionShippingEnum.ShippingId.ToString(), FilterOperators.NotIn, ShippingIds.ToString()));
            }
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<ShippingModel> objStoredProc = new ZnodeViewRepository<ShippingModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", isAssociatedShipping, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@PromotionId", promotionId, ParameterDirection.Input, DbType.Int32);

            IList<ShippingModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPromotionShippingDetails @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@PortalId,@IsAssociated,@PromotionId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("list count ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { list.Count });

            ShippingListModel listModel = new ShippingListModel { ShippingList = list?.ToList() };

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return listModel;
        }

        public virtual bool AssociateShippingToPromotion(AssociatedParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            bool isSavePromotions = false;
            if (!string.IsNullOrEmpty(model.AssociateIds))
            {
                List<ZnodePromotionShipping> promotionShipping = new List<ZnodePromotionShipping>();
                var associatedIds = model.AssociateIds.Split(',').ToList();
                foreach (var item in associatedIds)
                {
                    int ShippingId = Convert.ToInt32(item);
                    promotionShipping.Add(new ZnodePromotionShipping() { PromotionId = model.PromotionId, ShippingId = ShippingId });
                }
                var createAssociation = _promotionShippingRepository.Insert(promotionShipping);
                if (createAssociation.Any())
                    isSavePromotions = true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return isSavePromotions;
        }

        //Removes a Shipping type association entry from promotion.
        public virtual bool UnAssociateShipping(ParameterModel ShippingIds, int promotionId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters ShippingIds, promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { ShippingIds, promotionId });

            if (string.IsNullOrEmpty(ShippingIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAssociatedShippingIdNull);

            FilterCollection filters = new FilterCollection();

            //Create tuple to generate where clause.             
            filters.Add(new FilterTuple(ZnodePromotionShippingEnum.ShippingId.ToString(), ProcedureFilterOperators.In, ShippingIds.Ids));
            filters.Add(new FilterTuple(ZnodePromotionProductEnum.PromotionId.ToString(), ProcedureFilterOperators.Equals, promotionId.ToString()));
            //Generating where clause to get account details.             
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return _promotionShippingRepository.Delete(whereClauseModel.WhereClause);
        }
        #endregion
        #endregion

        #region Private Method

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodePromotionEnum.ZnodePromotionType.ToString().ToLower())) SetExpands(ZnodePromotionEnum.ZnodePromotionType.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Bind version id filter to avoid duplicate catalogs fetched while the associated catalogs are publishing.
        private string GetAssociatedCatalogVersionId(string AssociatedCatalogIds)
        {
            List<int> catalogIds = AssociatedCatalogIds.Split(',').Select(x => int.Parse(x)).ToList();
            List<int> versionList = GetService<IPublishedPortalDataService>().GetVersionEntity(catalogIds)?.Select(x => x.VersionId)?.ToList();
            return string.Join(",", versionList?.Select(n => n.ToString()).ToArray());
        }

        //get products associated to categories from expands
        private void GetDataFromExpands(NameValueCollection expands, PublishCategoryListModel publishCategories, int localeId)
        {
            if (publishCategories?.PublishCategories?.Count > 0 && (expands?.HasKeys() ?? false))
            {
                foreach (string key in expands.Keys)
                {
                    if (string.Equals(key, ExpandKeys.Product, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish products associated with category
                        List<PublishProductModel> products = GetPublishProducts(publishCategories.PublishCategories.Select(x => x.PublishCategoryId), localeId);

                        //map products to associated categories
                        publishCategories.PublishCategories.ForEach(
                            x => x.products = products.Where(s => s.ZnodeCategoryIds.Equals(x.PublishCategoryId))?.ToList());
                    }
                }
            }
        }

        //get products,categories associated to catalogs from expands
        private void GetDataFromExpands(NameValueCollection expands, PublishCatalogListModel publishCatalogs, int localeId)
        {
            if (publishCatalogs?.PublishCatalogs?.Count > 0 && (expands?.HasKeys() ?? false))
            {
                foreach (string key in expands.Keys)
                {
                    if (string.Equals(key, ExpandKeys.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish categories associated with category
                        List<PublishCategoryModel> categories = GetPublishCategories(publishCatalogs.PublishCatalogs.Select(s => s.PublishCatalogId), localeId);

                        //map categories to catalog
                        publishCatalogs.PublishCatalogs.ForEach(
                            x => x.PublishCategories = categories.Where(s => s.ZnodeCatalogId == x.PublishCatalogId)?.ToList());
                    }

                    if (string.Equals(key, ExpandKeys.Product, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish products associated with catalog
                        List<PublishProductModel> products = GetPublishProducts(publishCatalogs.PublishCatalogs.Select(s => s.PublishCatalogId), localeId);

                        //map products to catalog
                        publishCatalogs.PublishCatalogs.ForEach(
                           x => x.PublishProducts = products.Where(s => s.ZnodeCatalogId == x.PublishCatalogId)?.ToList());
                    }
                }
            }
        }

        //get publish categories associated with catalog
        private List<PublishCategoryModel> GetPublishCategories(IEnumerable<int> catalogIds, int localeId)
        {
            FilterCollection filters = new FilterCollection();
            if (HelperUtility.IsNotNull(catalogIds))
                filters.Add("ZnodeCatalogId", FilterOperators.In, string.Join(",", catalogIds));
            if (localeId > 0)
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
            List<ZnodePublishCategoryEntity> categoryList = GetService<IPublishedCategoryDataService>().GetPublishedCategoryList(new PageListModel(filters, null, null));

            return categoryList?.ToModel<PublishCategoryModel>()?.ToList();
        }

        //get publish products associated with category
        private List<PublishProductModel> GetPublishProducts(IEnumerable<int> categoryIds, int? localeId)
        {
            FilterCollection filters = new FilterCollection();
            if (HelperUtility.IsNotNull(categoryIds))
                filters.Add("ZnodeCatalogId", FilterOperators.In, string.Join(",", categoryIds));
            if (localeId > 0)
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());

            return GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null))?.ToModel<PublishProductModel>()?.ToList();

        }

        private bool IsAlreadyExistsPromotion(string promocode)
            => _promotionRepository.Table.Any(a => a.PromoCode == promocode);

        private bool IsAlreadyExistsCouponCode(string coupon)
           => _couponRepository.Table.Any(a => a.Code == coupon);

        public virtual void SetAssociationToPromotion(PromotionModel promotionModel, int promotionId)
        {
            ZnodeLogging.LogMessage("Input parameter promotionId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { promotionId });

            promotionModel.AssociatedCatelogIds = string.Join(",", _promotionCatalogRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishCatalogId).ToArray());
            promotionModel.AssociatedCategoryIds = string.Join(",", _promotionCategoryRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishCategoryId).ToArray());
            promotionModel.AssociatedProductIds = string.Join(",", _promotionProductRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.PublishProductId).ToArray());
            promotionModel.AssociatedBrandIds = string.Join(",", _promotionBrandRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.BrandId).ToArray());
            promotionModel.AssociatedShippingIds = string.Join(",", _promotionShippingRepository.Table.Where(x => x.PromotionId == promotionId).Select(x => x.ShippingId).ToArray());
        }

        private void SetDefaultValues(PromotionModel promotionModel)
        {
            promotionModel.PortalId = promotionModel.PortalId == 0 ? null : promotionModel.PortalId;
            promotionModel.ProfileId = promotionModel.ProfileId == 0 ? null : promotionModel.ProfileId;
            promotionModel.BrandCode = promotionModel.BrandCode == "0" ? string.Empty : promotionModel.BrandCode;
        }

        //Replace Product Filter Keys
        private void ReplaceProductFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.LocaleId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.LocaleId, Utilities.FilterKeys.PublishedLocaleId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodeCatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodeCatalogId.ToLower(), Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.Name, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.Name.ToLower(), Utilities.FilterKeys.Name); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ItemName.ToLower(), Utilities.FilterKeys.Name); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.Sku, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.Sku, Utilities.FilterKeys.SKU); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CatalogId.ToLower(), Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodeCategoryId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodeCategoryId.ToLower(), Utilities.FilterKeys.ZnodeCategoryIds); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodeCategoryIds, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodeCategoryIds.ToLower(), Utilities.FilterKeys.ZnodeCategoryIds); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodeProductId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodeProductId.ToLower(), Utilities.FilterKeys.ZnodeProductId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.AttributeCodeForPromotion, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.AttributeCodeForPromotion.ToLower(), Utilities.FilterKeys.AttributeCodeForPromotion); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.AttributeValuesForPromotion, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.AttributeValuesForPromotion.ToLower(), Utilities.FilterKeys.AttributeValuesForPromotion); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ProductType, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ProductType.ToLower(), Utilities.FilterKeys.AttributeValueForProductType); }
            }
            ReplaceFilterKeysForOr(ref filters);
        }
        //Replace Category Filter Keys
        private void ReplaceCategoryFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.LocaleId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.LocaleId, Utilities.FilterKeys.PublishedLocaleId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CatalogId, Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CategoryId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CategoryId, Utilities.FilterKeys.ZnodeCategoryId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CategoryName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CategoryName, Utilities.FilterKeys.Name); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ItemName, Utilities.FilterKeys.Name); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ParentCategoryIds, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ParentCategoryIds, Utilities.FilterKeys.ZnodeParentCategoryIds); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodecatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodecatalogId, Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, WebStoreEnum.ProfileIds.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, WebStoreEnum.ProfileIds.ToString().ToLower(), WebStoreEnum.ProfileIds.ToString()); }
                if (string.Equals(tuple.Item1, WebStoreEnum.IsActive.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, WebStoreEnum.IsActive.ToString().ToLower(), WebStoreEnum.IsActive.ToString()); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
            }
            ReplaceFilterKeysForOr(ref filters);
        }

        //Replace Filter Keys
        private void ReplaceFilterKeysForOr(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (tuple.Item1.Contains("|"))
                {
                    List<string> newValues = new List<string>();
                    foreach (var item in tuple.Item1.Split('|'))
                    {
                        if (string.Equals(item, Utilities.FilterKeys.CategoryName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Name); }
                        else if (string.Equals(item, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Name); }
                        else if (string.Equals(item, Utilities.FilterKeys.Sku, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.SKU); }
                        else if (string.Equals(item, Utilities.FilterKeys.Name, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Name); }
                        else if (string.Equals(item, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.PublishedCatalogName); }
                        else if (string.Equals(item, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Name); }
                        else if (string.Equals(item, Utilities.FilterKeys.ProductType, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Attributes); }
                        else newValues.Add(item);
                    }
                    ReplaceFilterKeyName(ref filters, tuple.Item1, string.Join("|", newValues));
                }
            }
        }

        //Replace Catalog Filter Keys
        private void ReplaceCatalogFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CatalogId, Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodecatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodecatalogId, Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.RevisionType.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.RevisionType.ToLower(), Utilities.FilterKeys.revisiontype); }
            }
            ReplaceFilterKeysForOr(ref filters);
        }
        //Replace sort Keys
        private void ReplaceSortKeysForProduct(ref NameValueCollection sorts, int promotionId)
        {
            foreach (string key in sorts.Keys)
            {
                if (promotionId == 0)
                {
                    if (string.Equals(key, Utilities.FilterKeys.Name.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.Name.ToLower(), Utilities.FilterKeys.Name); }
                    if (string.Equals(key, Utilities.FilterKeys.PublishProductId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishProductId.ToLower(), Utilities.FilterKeys.ZnodeProductId); }
                }
                else
                {
                    if (string.Equals(key, Utilities.FilterKeys.Name.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.Name.ToLower(), Utilities.FilterKeys.Name); }
                    if (string.Equals(key, Utilities.FilterKeys.PublishProductId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishProductId.ToLower(), Utilities.FilterKeys.PublishProductId); }
                }

                if (string.Equals(key, Utilities.FilterKeys.Sku, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.Sku, Utilities.FilterKeys.SKU); }
                if (string.Equals(key, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.ItemName, Utilities.FilterKeys.Name); }
                if (string.Equals(key, Utilities.FilterKeys.ItemId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.ItemId, Utilities.FilterKeys.ZnodeProductId); }
                if (string.Equals(key, Utilities.FilterKeys.PublishedCatalogName.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
            }
        }

        //Replace sort Keys for Catalog
        private void ReplaceSortKeysForCatalog(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, Utilities.FilterKeys.PublishCatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishCatalogId.ToLower(), Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(key, Utilities.FilterKeys.PublishedCatalogName.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
            }
        }

        //Replace sort Keys for Category
        private void ReplaceSortKeysForCategory(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, Utilities.FilterKeys.PublishCategoryId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishCategoryId.ToLower(), Utilities.FilterKeys.ZnodeCategoryId); }
                if (string.Equals(key, Utilities.FilterKeys.PublishCatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishCatalogId.ToLower(), Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(key, Utilities.FilterKeys.PublishedCatalogName.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
                if (string.Equals(key, Utilities.FilterKeys.CategoryName, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.CategoryName, Utilities.FilterKeys.Name); }
            }
        }
        // Delete coupon.
        private bool DeleteCoupons(PromotionModel model)
        {
            FilterCollection deleteCouponfilters = new FilterCollection();
            deleteCouponfilters.Add(new FilterTuple(ZnodePromotionCouponEnum.PromotionId.ToString(), ProcedureFilterOperators.Equals, model.PromotionId.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(deleteCouponfilters.ToFilterDataCollection());
            return _couponRepository.Delete(whereClauseModel.WhereClause);
        }

        //Update single coupon
        private bool UpdateCouponDetails(PromotionModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            bool isCouponUpdated = false;
            model.CouponList?.CouponList.ForEach(x => { x.PromotionId = model.PromotionId; x.AvailableQuantity = model.IsUnique ? 1 : x.AvailableQuantity; });
            CouponModel couponModel = model?.CouponList?.CouponList?.FirstOrDefault();

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePromotionEnum.PromotionId.ToString(), FilterOperators.Equals, model.PromotionId.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodePromotionCoupon promotionCoupon = _couponRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
            if (IsNotNull(promotionCoupon) && IsNotNull(couponModel))
            {
                couponModel.PromotionCouponId = promotionCoupon.PromotionCouponId;
                ZnodeLogging.LogMessage("PromotionCouponId.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, couponModel.PromotionCouponId);

                isCouponUpdated = _couponRepository.Update(couponModel.ToEntity<ZnodePromotionCoupon>());
                ZnodeLogging.LogMessage(isCouponUpdated ? String.Format(Admin_Resources.SuccessPromotionUpdate,model.PromotionId) : Admin_Resources.ErrorPromotionUpdate, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            }
            else
            {
                isCouponUpdated = IsNotNull(_couponRepository.Insert(couponModel.ToEntity<ZnodePromotionCoupon>()));
                ZnodeLogging.LogMessage(isCouponUpdated ?String.Format(Admin_Resources.SuccessInsertPromotion, model.PromotionId) : Admin_Resources.ErrorInsertPromotion, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return isCouponUpdated;
        }

        //Get Product name by product id.
        private string GetProductName(string productId)
        {
            int znodeProductId = Convert.ToInt32(productId);
            int localeId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);
            return _publishProductEntityRepository.Table.FirstOrDefault(x => x.ZnodeProductId == znodeProductId && x.LocaleId == localeId)?.Name ;
        }

        //Bind portalId for promotion made on all Store.
        private static void BindPortalIds(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            if (!filters.Exists(x => x.FilterValue.ToString().Contains(Utilities.FilterKeys.Null)))
            {
                string PortalIds = string.Concat(filters.Where(x => x.Item1.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault(), ',', Utilities.FilterKeys.Null);
                filters.RemoveAll(x => x.FilterName.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower());
                filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.In, PortalIds));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

        }

        //Get Image Path For Category and set stored based In Stock, Out Of Stock, Back Order Message for List.
        protected virtual void SetProductDetailsForList(int portalId, PublishProductListModel publishProductListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId });

            string ImageName = string.Empty;
            IImageHelper image = GetService<IImageHelper>();

            //Get image path for products.
            publishProductListModel?.PublishProducts.ForEach(
                x =>
                {
                    ImageName = x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                    x.ImageSmallPath = image.GetImageHttpPathSmall(ImageName);
                    x.ImageMediumPath = image.GetImageHttpPathMedium(ImageName);
                    x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(ImageName);
                    x.ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(ImageName);
                    x.ProductType = x.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductType)?.SelectValues.FirstOrDefault()?.Value;
                    x.IsActive = Convert.ToBoolean(x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.IsActive)?.FirstOrDefault()?.AttributeValues);
                });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

        }
        #endregion

        #region Protected Methods
        protected virtual List<int?> GetPromotionProfileIds(int promotionId)
        {
            IZnodeRepository<ZnodePromotionProfileMapper> _promotionProfileRepository = new ZnodeRepository<ZnodePromotionProfileMapper>();
            return _promotionProfileRepository.Table.Where(x => x.PromotionId == promotionId)?.Select(x => x.ProfileId)?.ToList();
        }
        #endregion
    }

}
