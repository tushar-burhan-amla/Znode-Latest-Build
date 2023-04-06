using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Promotions
{
    public class ZnodePromotionHelper : IZnodePromotionHelper
    {
        #region Protected Variables
        protected IZnodeRepository<ZnodePromotionType> _promotionTypeRepository;
        protected IZnodeRepository<ZnodePromotion> _promotionRepository;
        protected IZnodeRepository<ZnodePromotionCoupon> _promotionCouponRepository;
        #endregion

        #region Constructor
        public ZnodePromotionHelper()
        {
            _promotionTypeRepository = new ZnodeRepository<ZnodePromotionType>();
            _promotionRepository = new ZnodeRepository<ZnodePromotion>();
            _promotionCouponRepository = new ZnodeRepository<ZnodePromotionCoupon>();
        }
        #endregion

        #region Public Method

        //to get all promotion type
        public virtual List<PromotionTypeModel> GetPromotionType()
        {
            List<ZnodePromotionType> allPromotions = _promotionTypeRepository.GetEntityList("").ToList();
            List<PromotionTypeModel> promotionType = new List<PromotionTypeModel>();
            allPromotions.ForEach(item => { promotionType.Add(new PromotionTypeModel { ClassName = item.ClassName, ClassType = item.ClassType, IsActive = item.IsActive, Name = item.Name, Description = item.Description, PromotionTypeId = item.PromotionTypeId }); });
            return promotionType;
        }

        //to get all promotions 
        public virtual List<PromotionModel> GetAllPromotions()
        {
            List<PromotionModel> list = HttpRuntime.Cache["AllPromotionCache"] as List<PromotionModel>;

            if (HelperUtility.IsNotNull(list) && list.Count > 0)
                return list;

            list = GetPromotions();
            ZnodeCacheDependencyManager.Insert("AllPromotionCache", list, "ZnodePromotion");
            return list ?? new List<PromotionModel>();
        }

        //Get promotions list which are active in current date.
        public virtual List<PromotionModel> GetActivePromotions(int? portalId, int? profileId, List<PromotionModel> promotionList)
        {
            return promotionList.Where(promo => (DateTime.Today.Date >= promo.StartDate && DateTime.Today.Date <= promo.EndDate)
                                                                                   && (promo.PortalId == portalId || promo.PortalId == null)
                                                                                   && (promo.ProfileId == profileId || promo.ProfileId == null)
                                                                                  && promo.PromotionType.IsActive).ToList();         
        }

        protected virtual List<PromotionModel> GetPromotions()
        {
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

            DataSet dataSet = objStoredProc.GetSPResultInDataSet("Znode_GetAllPromotionsList");
            List<PromotionModel> promotions = dataSet?.Tables[0]?.AsEnumerable()
              .Select(dataRow => new PromotionModel
              {
                  PromotionId = dataRow.Field<int>("PromotionId"),
                  PromoCode = dataRow.Field<string>("PromoCode"),
                  Name = dataRow.Field<string>("Name"),
                  Description = dataRow.Field<string>("Description"),
                  PromotionTypeId = dataRow.Field<int?>("PromotionTypeId"),
                  Discount = dataRow.Field<decimal?>("Discount"),
                  StartDate = dataRow.Field<DateTime?>("StartDate"),
                  EndDate = dataRow.Field<DateTime?>("EndDate"),
                  OrderMinimum = dataRow.Field<decimal?>("OrderMinimum"),
                  QuantityMinimum = dataRow.Field<decimal?>("QuantityMinimum"),
                  IsCouponRequired = dataRow.Field<bool?>("IsCouponRequired"),
                  DisplayOrder = dataRow.Field<int?>("DisplayOrder"),
                  IsUnique = dataRow.Field<bool>("IsUnique"),
                  PortalId = dataRow.Field<int?>("PortalId"),
                  ProfileId = dataRow.Field<int?>("ProfileId"),
                  PromotionProductQuantity = dataRow.Field<decimal?>("PromotionProductQuantity"),
                  ReferralPublishProductId = dataRow.Field<int?>("ReferralPublishProductId"),
                  PromotionMessage = dataRow.Field<string>("PromotionMessage"),
                  IsAllowedWithOtherCoupons = dataRow.Field<bool>("IsAllowedWithOtherCoupons"),
                  PromotionType = new PromotionTypeModel()
                  {
                      ClassName = dataRow.Field<string>("ClassName"),
                      ClassType = dataRow.Field<string>("ClassType"),
                      IsActive = dataRow.Field<bool>("IsActive"),
                      Name = dataRow.Field<string>("Name"),
                      Description = dataRow.Field<string>("Description1"),
                      PromotionTypeId = dataRow.Field<int>("PromotionTypeId")
                  }
              })?.ToList();
         
             return promotions ?? new List<PromotionModel>();
        }

        public virtual List<PromotionModel> GetOrderPromotions(int? omsOrderId, int? portalId, int? profileId, string[] newAppliedCoupons)
        {
            string newCoupons = string.Join(",", newAppliedCoupons);
            IZnodeViewRepository<PromotionModel> objStoredProc = new ZnodeViewRepository<PromotionModel>();
            objStoredProc.SetParameter("@OrderId", omsOrderId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Couponcode", newCoupons, ParameterDirection.Input, DbType.String);
            var orderPromotionList = objStoredProc.ExecuteStoredProcedureList("Znode_OrderPromotionCoupons @OrderId, @Couponcode");

            List<PromotionModel> promotions = new List<PromotionModel>();
            foreach (PromotionModel model in orderPromotionList)
            {
                if (IsValidPromotion(model, portalId, profileId))
                {
                    model.PromotionType = new PromotionTypeModel
                    {
                        ClassName = model.ClassName,
                        ClassType = model.ClassType,
                        IsActive = model.IsActive,
                        Name = model.Name,
                        Description = model.Description,
                        PromotionTypeId = model.PromotionTypeId.GetValueOrDefault(),
                    };
                    promotions.Add(model);
                }
            }
            return promotions ?? new List<PromotionModel>();
        }

        public virtual bool IsValidPromotion(PromotionModel model, int? portalId, int? profileId)
        {
            bool isValidPromo = true;
            if (!model.IsUsedInOrder)
            {
                if (DateTime.Today.Date.AddDays(1) >= model.StartDate && DateTime.Today.Date <= model.EndDate && (model.PortalId == portalId || model.PortalId == null) && (model.ProfileId == profileId || model.ProfileId == null))
                    isValidPromo = true;
                else
                    isValidPromo = false;
            }
            return isValidPromo;
        }

        // Get profile model on basis of user id from cache.
        public virtual ProfileModel GetProfileCache()
        {
            int portalId = HelperUtility.GetPortalId();
            //Get the list of profile from cache which are associated to customer.
            string cachename = GetLoginUserId() > 0 ? $"ProfileCache_{ GetLoginUserId() }_{portalId}" : $"ProfileCache_{portalId}";
            List<ProfileModel> profileList = (List<ProfileModel>)HttpContext.Current.Cache[cachename];

            //Null check for profile list and return default profile.
            if (profileList?.Count > 0)
                return profileList.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault() ?? null;

            return null;
        }

        // Get Catalog Id By Product Id.
        public virtual List<CatalogModel> GetCatalogByProduct(int productId)
        {
           List<int> allCatalog = GetCatalogs(productId);
            List<CatalogModel> catalogs = new List<CatalogModel>();
            allCatalog.ForEach(item => { catalogs.Add(new CatalogModel { PimCatalogId = item }); });
            return catalogs?.GroupBy(p => new { p.PimCatalogId }).Select(g => g.First())?.ToList() ?? new List<CatalogModel>();
        }

        // Get Category By Product Id.
        public virtual List<CategoryModel> GetCategoryByProduct(int productId)
        {
            List<int> allCategories = GetCategories(productId);
            List<CategoryModel> categories = new List<CategoryModel>();
            allCategories.ForEach(item => { categories.Add(new CategoryModel { PimCategoryId = item }); });
            return categories?.GroupBy(p => new { p.PimCategoryId }).Select(g => g.First())?.ToList() ?? new List<CategoryModel>();
        }

        // Get  Promotion Catalogs promotionId.
        public virtual List<CatalogModel> GetPromotionCatalogs(int promotionId)
        {
            IZnodeRepository<ZnodePromotionCatalog> _promotionCatalogsRepository = new ZnodeRepository<ZnodePromotionCatalog>();
            List<ZnodePromotionCatalog> allPromotionCatalog = _promotionCatalogsRepository.Table.Where(x => x.PromotionId == promotionId).ToList();
            List<CatalogModel> catalogs = new List<CatalogModel>();
            allPromotionCatalog.ForEach(item => { catalogs.Add(new CatalogModel { PimCatalogId = item.PublishCatalogId.GetValueOrDefault() }); });
            return catalogs?.GroupBy(p => new { p.PimCatalogId }).Select(g => g.First())?.ToList() ?? new List<CatalogModel>();
        }

        // Get Promotion Category by promotionId.
        public virtual List<CategoryModel> GetPromotionCategory(int promotionId)
        {
            IZnodeRepository<ZnodePromotionCategory> _promotionCategoryRepository = new ZnodeRepository<ZnodePromotionCategory>();
            List<ZnodePromotionCategory> allPromotionCategory = _promotionCategoryRepository.Table.Where(x => x.PromotionId == promotionId).ToList();
            List<CategoryModel> categories = new List<CategoryModel>();
            allPromotionCategory.ForEach(item => { categories.Add(new CategoryModel { PimCategoryId = item.PublishCategoryId.GetValueOrDefault() }); });
            return categories?.GroupBy(p => new { p.PimCategoryId }).Select(g => g.First())?.ToList() ?? new List<CategoryModel>();
        }

        // Get Promotion products by promotionId.
        public virtual List<ProductModel> GetPromotionProducts(int promotionId)
        {
            IZnodeRepository<ZnodePromotionProduct> _promotionProductRepository = new ZnodeRepository<ZnodePromotionProduct>();
            List<ZnodePromotionProduct> allPromotionProduct = _promotionProductRepository.Table.Where(x => x.PromotionId == promotionId).ToList();
            List<ProductModel> products = new List<ProductModel>();
            allPromotionProduct.ForEach(item => { products.Add(new ProductModel { ProductId = item.PublishProductId.GetValueOrDefault() }); });
            return products?.GroupBy(p => new { p.ProductId }).Select(g => g.First())?.ToList() ?? new List<ProductModel>();
        }

        // Get Promotion products by promotionId.
        public virtual List<BrandModel> GetPromotionBrands(int promotionId)
        {
            IZnodeRepository<ZnodePromotionBrand> _promotionBrandRepository = new ZnodeRepository<ZnodePromotionBrand>();
            List<ZnodePromotionBrand> allPromotionBrand = _promotionBrandRepository.Table.Where(x => x.PromotionId == promotionId).ToList();
            List<BrandModel> brands = new List<BrandModel>();
            if (allPromotionBrand?.Count > 0)
            {
                allPromotionBrand.ForEach(item => { brands.Add(new BrandModel { BrandCode = item.BrandCode }); });
            }
            return brands?.GroupBy(p => new { p.BrandCode }).Select(g => g.First())?.ToList() ?? new List<BrandModel>();
        }

        // Get Promotion products by promotionId.
        public virtual List<ShippingModel> GetPromotionShipping(int promotionId)
        {
            IZnodeRepository<ZnodePromotionShipping> _promotionShippingRepository = new ZnodeRepository<ZnodePromotionShipping>();
            List<ZnodePromotionShipping> allPromotion = _promotionShippingRepository.Table.Where(x => x.PromotionId == promotionId).ToList();
            List<ShippingModel> shipping = new List<ShippingModel>();
            allPromotion.ForEach(item => { shipping.Add(new ShippingModel { ShippingId = item.ShippingId.GetValueOrDefault() }); });
            return shipping?.GroupBy(p => new { p.ShippingId }).Select(g => g.First())?.ToList() ?? new List<ShippingModel>();
        }

        // Get promotion coupons by promotionId.
        public virtual List<CouponModel> GetPromotionCoupons(int promotionId, string couponCodes = "")
        {
            List<ZnodePromotionCoupon> promotionCoupons = new List<ZnodePromotionCoupon>();
            if (!string.IsNullOrEmpty(couponCodes))
            {
                string[] couponList = couponCodes.ToLower().Split(',');
                promotionCoupons = _promotionCouponRepository.Table.Where(x => x.PromotionId == promotionId && couponList.Contains(x.Code.ToLower())).ToList();
            }
            else
            {
                promotionCoupons = _promotionCouponRepository.Table.Where(x => x.PromotionId == promotionId).ToList();
            }

            List<CouponModel> coupons = new List<CouponModel>();
            if (promotionCoupons?.Count > 0)
            {
                promotionCoupons.ForEach(item =>
                {
                    coupons.Add(new CouponModel
                    {
                        Code = item.Code,
                        InitialQuantity = item.InitialQuantity,
                        AvailableQuantity = item.AvailableQuantity,
                        IsActive = item.IsActive
                    });
                });
            }
            return coupons ?? new List<CouponModel>();
        }

        //Get all promotions by coupon code
        public virtual PromotionModel GetCouponsPromotion(List<PromotionModel> promotionsFromCache, string couponCode, int? currentPortalId, int? currentProfileId, int? orderId = null)
        {
            PromotionModel promotionsWithCoupon = new PromotionModel();

            //to add promotions having active coupons exist in shopping cart
            promotionsWithCoupon = (from _promo in promotionsFromCache.ToList()
                                    join _coupon in _promotionCouponRepository.Table on _promo.PromotionId equals _coupon.PromotionId
                                    where (couponCode.ToLower() == _coupon.Code.ToLower()
                                    && (_promo.ProfileId == currentProfileId || _promo.ProfileId == null)
                                    && (_promo.PortalId == currentPortalId || _promo.PortalId == null)
                                   && _coupon.IsActive)
                                    select new PromotionModel
                                    {
                                        PromotionId = _promo.PromotionId,
                                        PromoCode = _promo.PromoCode,
                                        Name = _promo.Name,
                                        Description = _promo.Description,
                                        PromotionTypeId = _promo.PromotionTypeId,
                                        Discount = _promo.Discount,
                                        StartDate = _promo.StartDate,
                                        EndDate = _promo.EndDate,
                                        OrderMinimum = _promo.OrderMinimum,
                                        QuantityMinimum = _promo.QuantityMinimum,
                                        IsCouponRequired = _promo.IsCouponRequired,
                                        DisplayOrder = _promo.DisplayOrder,
                                        IsUnique = _promo.IsUnique,
                                        PortalId = _promo.PortalId,
                                        ProfileId = _promo.ProfileId,
                                        PromotionProductQuantity = _promo.PromotionProductQuantity,
                                        ReferralPublishProductId = _promo.ReferralPublishProductId,
                                        PromotionMessage = _promo.PromotionMessage,
                                        PromotionType = _promo.PromotionType,
                                        IsAllowedWithOtherCoupons = _promo.IsAllowedWithOtherCoupons
                                    }).FirstOrDefault();

            return promotionsWithCoupon;
        }

        //to check is multiple coupons allows or not
        public virtual bool AllowsMultipleCoupon(string couponCode, int? currentPortalId, int? currentProfileId)
        {
            bool isAllowsMultipleCoupon = false;

            List<PromotionModel> promotionsWithCoupons = (from _promo in _promotionRepository.Table
                                                          join _coupon in _promotionCouponRepository.Table
                                                          on _promo.PromotionId equals _coupon.PromotionId
                                                          where (_coupon.Code == couponCode)
                                                           && (_promo.ProfileId == currentProfileId || _promo.ProfileId == null)
                                                          && (_promo.PortalId == currentPortalId || _promo.PortalId == null)
                                                          select new PromotionModel
                                                          {
                                                              PromotionId = _promo.PromotionId,
                                                              PromoCode = _promo.PromoCode,
                                                              IsAllowedWithOtherCoupons = _promo.IsAllowedWithOtherCoupons
                                                          }).ToList();

            foreach (PromotionModel promotion in promotionsWithCoupons)
            {
                isAllowsMultipleCoupon = promotion.IsAllowedWithOtherCoupons;
                if (isAllowsMultipleCoupon)
                    break;
            }
            return isAllowsMultipleCoupon;
        }

        //to set promotion brand wise sku quantity in list model to calculate each item promotional quantity 
        public virtual List<PromotionCartItemQuantity> SetPromotionBrandSKUQuantity(List<BrandModel> promotionsBrand, ZnodeShoppingCart shoppingCart)
        {
            List<PromotionCartItemQuantity> brandSkus = new List<PromotionCartItemQuantity>();
            foreach (BrandModel brand in promotionsBrand)
            {
                foreach (ZnodeShoppingCartItem cartItem in shoppingCart.ShoppingCartItems)
                {
                    //to add simple product
                    if (string.Equals(brand.BrandCode, cartItem.Product.BrandCode, StringComparison.OrdinalIgnoreCase) &&
                        cartItem.Product.ZNodeGroupProductCollection.Count == 0 &&
                        cartItem.Product.ZNodeConfigurableProductCollection.Count == 0)
                    {
                        AddPromotionSKUQuantity(new PromotionCartItemQuantity { Brand = brand.BrandCode, SKU = cartItem.Product.SKU, Quantity = cartItem.Quantity }, brandSkus);
                    }
                    else if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)//to add configurable product
                    {
                        if (string.Equals(brand.BrandCode, cartItem.Product.BrandCode, StringComparison.OrdinalIgnoreCase))
                            AddPromotionSKUQuantity(new PromotionCartItemQuantity { Brand = cartItem.Product.BrandCode, SKU = cartItem.Product.SKU, Quantity = 0 }, brandSkus);

                        foreach (ZnodeProductBaseEntity configurable in cartItem.Product.ZNodeConfigurableProductCollection)
                        {
                            if (string.Equals(brand.BrandCode, configurable.BrandCode, StringComparison.OrdinalIgnoreCase))
                                AddPromotionSKUQuantity(new PromotionCartItemQuantity { Brand = configurable.BrandCode, SKU = configurable.SKU, Quantity = cartItem.Quantity }, brandSkus);
                        }
                    }
                    else if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)//to add group product
                    {
                        decimal groupCartQuantity = 0;
                        foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                        {
                            groupCartQuantity += group.SelectedQuantity;
                        }
                        if (string.Equals(brand.BrandCode, cartItem.Product.BrandCode, StringComparison.OrdinalIgnoreCase))
                            AddPromotionSKUQuantity(new PromotionCartItemQuantity { Brand = cartItem.Product.BrandCode, SKU = cartItem.Product.SKU, Quantity = groupCartQuantity }, brandSkus);
                    }
                }
            }
            return brandSkus;
        }

        //to set promotion brand wise sku quantity in list model to calculate each item promotional quantity 
        public virtual List<PromotionCartItemQuantity> SetPromotionCategorySKUQuantity(List<CategoryModel> promotionCategories, ZnodeShoppingCart shoppingCart, out decimal quantity)
        {
            List<PromotionCartItemQuantity> categorySkus = new List<PromotionCartItemQuantity>();
            quantity = 0m;
            foreach (ZnodeShoppingCartItem cartItem in shoppingCart.ShoppingCartItems)
            {
                bool isCategoryApplied = false;
                foreach (CategoryModel promo in promotionCategories)
                {
                    //simple product
                    if (cartItem.Product.ZNodeGroupProductCollection.Count == 0 && cartItem.Product.ZNodeConfigurableProductCollection.Count == 0)
                    {
                        isCategoryApplied = AddProductCategory(cartItem.Product.SKU, cartItem.Product.ProductID, cartItem.Quantity, promo.PimCategoryId, categorySkus);
                        if (isCategoryApplied)
                            quantity += cartItem.Quantity;
                    }
                    else if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)//to add configurable product
                    {
                        foreach (ZnodeProductBaseEntity configurable in cartItem.Product.ZNodeConfigurableProductCollection)
                        {
                            if (cartItem.ParentProductId > 0)
                            {
                                isCategoryApplied = AddProductCategory(cartItem.Product.SKU, cartItem.ParentProductId, cartItem.Quantity, promo.PimCategoryId, categorySkus);
                                if (isCategoryApplied)
                                    quantity += cartItem.Quantity;
                            }
                            else
                            {

                                isCategoryApplied = AddProductCategory(cartItem.Product.SKU, configurable.ProductID, cartItem.Quantity, promo.PimCategoryId, categorySkus);
                                if (isCategoryApplied)
                                    quantity += cartItem.Quantity;
                            }
                        }
                    }
                    else if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)//to add group product
                    {
                        decimal groupCartQuantity = 0;
                        foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                        {
                            groupCartQuantity += group.SelectedQuantity;
                        }
                        isCategoryApplied = AddProductCategory(cartItem.Product.SKU, cartItem.Product.ProductID, groupCartQuantity, promo.PimCategoryId, categorySkus);
                        if (isCategoryApplied)
                        {
                            quantity += groupCartQuantity;
                        }
                    }
                    if (isCategoryApplied)
                        break;
                }
            }
            return categorySkus;
        }

        //to check applied coupon is used in exiting order by OrderId
        public virtual bool IsExistingOrderCoupon(int orderId, string couponCode)
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@OrderId", orderId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Couponcode", couponCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_CouponExistInOrder @OrderId, @Couponcode, @Status OUT", 2, out status);

            if (result.FirstOrDefault().Id.Equals(0))
                return true;
            else
                return result.FirstOrDefault().Status.Value;
        }

        /// <summary>
        /// Check if the promotion is applicable
        /// </summary>
        /// <param name="promotionList">List of all the applicable promotions.</param>
        /// <param name="promoCode">Promotion to be applied</param>
        /// <param name="minValue">Minimum order amount/quantity</param>
        /// <param name="isOrderAmountBasedPromotion">Is order promotion [Default: false]</param>
        /// <returns>If the promotion is applicable or not</returns>

        public virtual bool IsApplicablePromotion(List<PromotionModel> promotionList, string promoCode, decimal minValue, bool isOrderAmountBasedPromotion = false, bool isCoupon = false)
        {
            PromotionModel applicablePromotion = promotionList?.FirstOrDefault(o => o.PromoCode.Equals(promoCode) &&
                                                (isOrderAmountBasedPromotion ? o.OrderMinimum <= minValue : o.QuantityMinimum <= minValue) && o.IsCouponRequired == isCoupon);

            return HelperUtility.IsNotNull(applicablePromotion);
        }

        /// <summary>
        /// Allow Promotion with coupons
        /// </summary>
        /// <param name="promotionList">list of promotions</param>
        public virtual List<PromotionModel> IsAllowWithPromotionsAndCoupons(List<PromotionModel> promotionList)
        {
            string[] allowedPromotionList = DefaultGlobalConfigHelper.AllowedPromotions?.Split(',');
            if (allowedPromotionList?.Count() > 0)
            {
                foreach (string promotion in allowedPromotionList)
                {
                    promotionList.ForEach(m =>
                    {
                        if (m.PromotionType.ClassName.Equals(promotion, StringComparison.OrdinalIgnoreCase))
                            m.IsAllowWithOtherPromotionsAndCoupons = true;
                    });
                }
            }
            return promotionList;
        }


        /// <summary>
        /// Check if the promotion is applicable
        /// </summary>
        /// <param name="promotionList">List of all the applicable promotions.</param>
        /// <param name="promoCode">Promotion to be applied</param>
        /// <param name="minQuantity">Minimum quantity</param>
        /// <param name="orderAmount">Order amount</param>
        /// <returns>Returns, if the promotion is applicable or not</returns>
        public virtual bool IsApplicablePromotion(List<PromotionModel> promotionList, string promoCode, decimal minQuantity, decimal orderAmount, bool isCoupon = false)
        {
            PromotionModel applicablePromotion = new PromotionModel();
            applicablePromotion = promotionList.FirstOrDefault(p => p.OrderMinimum <= orderAmount && p.QuantityMinimum <= minQuantity &&
                                                               p.PromoCode.Equals(promoCode) && p.IsCouponRequired == isCoupon);


            return HelperUtility.IsNotNull(applicablePromotion);
        }

        /// <summary>
        /// This method will find and return most suitable promotion from the applicable promotion list
        /// </summary>
        /// <param name="applicablePromoList"></param>
        /// <param name="minValue"></param>
        /// <param name="isCoupon"></param>
        /// <returns>Promotion list</returns>
        public virtual List<PromotionModel> GetMostApplicablePromoList(List<PromotionModel> applicablePromoList, decimal minValue, bool isCoupon)
        {
            IEnumerable<PromotionModel> Promotionlist = applicablePromoList.Where(x => x.OrderMinimum <= minValue && x.IsCouponRequired == isCoupon);
            decimal maxValue = Promotionlist.Max(y => y.OrderMinimum).GetValueOrDefault();
            Promotionlist = Promotionlist?.Where(x => x.OrderMinimum == maxValue);

            return Promotionlist?.ToList();
        }


        public virtual bool IsApplicablePromotion(List<PromotionModel> promotionList, string promoCode, decimal minOrderValue, decimal minQuantityValue, PromoApplicabilityCriteria applicabilityCriteria, bool isCoupon = false)
        {
            PromotionModel applicablePromotion;
            List<PromotionModel> applicablePromotions = promotionList.Where(p => p.PromoCode == promoCode && p.IsCouponRequired == isCoupon).ToList();

            switch (applicabilityCriteria)
            {
                case PromoApplicabilityCriteria.OrderAmount:
                    applicablePromotion = applicablePromotions.FirstOrDefault(w => w.OrderMinimum <= minOrderValue);
                    break;

                case PromoApplicabilityCriteria.Quantity:
                    applicablePromotion = applicablePromotions.FirstOrDefault(w => w.QuantityMinimum <= minQuantityValue);
                    break;

                case PromoApplicabilityCriteria.Both:
                    applicablePromotion = applicablePromotions.FirstOrDefault(w => w.OrderMinimum <= minOrderValue &&
                                                                                   w.QuantityMinimum <= minQuantityValue);
                    break;
                default:
                    applicablePromotion = null;
                    break;
            }

            return HelperUtility.IsNotNull(applicablePromotion);
        }

        public virtual List<PromotionModel> GetActiveShippingPromotions(int? portalId, int? profileId)
        {
            List<PromotionModel> promotions = (from _promo in _promotionRepository.Table
                                               join _promoType in _promotionTypeRepository.Table on
                                              _promo.PromotionTypeId equals _promoType.PromotionTypeId
                                               where (_promoType.ClassName == ZnodeConstant.PercentOffShipping ||
                                               _promoType.ClassName == ZnodeConstant.PercentOffShippingWithCarrier ||
                                               _promoType.ClassName == ZnodeConstant.AmountOffShipping ||
                                                _promoType.ClassName == ZnodeConstant.AmountOffShippingWithCarrier) && _promo.IsCouponRequired == false
                                               orderby _promo.DisplayOrder
                                               select new PromotionModel
                                               {
                                                   PromotionId = _promo.PromotionId,
                                                   PromoCode = _promo.PromoCode,
                                                   Name = _promo.Name,
                                                   Description = _promo.Description,
                                                   PromotionTypeId = _promo.PromotionTypeId,
                                                   Discount = _promo.Discount,
                                                   StartDate = _promo.StartDate,
                                                   EndDate = _promo.EndDate,
                                                   OrderMinimum = _promo.OrderMinimum,
                                                   QuantityMinimum = _promo.QuantityMinimum,
                                                   IsCouponRequired = _promo.IsCouponRequired,
                                                   DisplayOrder = _promo.DisplayOrder,
                                                   IsUnique = _promo.IsUnique,
                                                   PortalId = _promo.PortalId,
                                                   ProfileId = _promo.ProfileId,
                                                   PromotionProductQuantity = _promo.PromotionProductQuantity,
                                                   ReferralPublishProductId = _promo.ReferralPublishProductId,
                                                   PromotionMessage = _promo.PromotionMessage,
                                                   IsAllowedWithOtherCoupons = _promo.IsAllowedWithOtherCoupons,
                                                   PromotionType = new PromotionTypeModel
                                                   {
                                                       ClassName = _promoType.ClassName,
                                                       ClassType = _promoType.ClassType,
                                                       IsActive = _promoType.IsActive,
                                                       Name = _promoType.Name,
                                                       Description = _promoType.Description,
                                                       PromotionTypeId = _promoType.PromotionTypeId
                                                   }
                                               })?.ToList();

            return promotions ?? new List<PromotionModel>();
        }

        // This method check for Promotions With Exceptions in case of IsAllowWithOtherPromotionsAndCoupons is disable.
        public virtual bool IsAllowWithOtherPromotionsAndCoupons(string promotionClassName, bool globalIsAllowWithOtherPromotionsAndCoupons, int appliedCouponCount, bool isPendingOrder = false)
        {
            bool isAllowWithOtherPromotionsAndCoupons = globalIsAllowWithOtherPromotionsAndCoupons;
            if (globalIsAllowWithOtherPromotionsAndCoupons == false)
            {
                // To handle if not applied any coupons at that time Promotions will get applied.
                if (isPendingOrder || appliedCouponCount == 0)
                {
                    isAllowWithOtherPromotionsAndCoupons = true;
                }
                else if (!string.IsNullOrEmpty(promotionClassName))
                {
                    string[] allowedPromotionList = DefaultGlobalConfigHelper.AllowedPromotions?.Split(',').Select(x => x.Trim()).ToArray();
                    if (HelperUtility.IsNotNull(allowedPromotionList))
                        isAllowWithOtherPromotionsAndCoupons = allowedPromotionList.Contains(promotionClassName);
                }
            }
            return isAllowWithOtherPromotionsAndCoupons;
        }
        #endregion

        #region Protected Method

        //to get login userid from header
        protected virtual int GetLoginUserId()
        {
            const string headerUserId = "Znode-UserId";
            int userId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerUserId], out userId);
            return userId;
        }

        //to get all Brands
        protected virtual List<BrandModel> GetAllBrands()
        {
            IZnodeRepository<ZnodePimAttribute> _attributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            IZnodeRepository<ZnodePimAttributeDefaultValue> _attributeDefaultValueRepository = new ZnodeRepository<ZnodePimAttributeDefaultValue>();

            List<BrandModel> allBrandList = (from attribute in _attributeRepository.Table
                                             join attributeValue in _attributeDefaultValueRepository.Table on attribute.PimAttributeId equals attributeValue.PimAttributeId
                                             where (attribute.AttributeCode.ToLower() == ZnodeConstant.Brand.ToLower())
                                             select new BrandModel
                                             {
                                                 BrandCode = attributeValue.AttributeDefaultValueCode
                                             }).ToList();

            return allBrandList ?? new List<BrandModel>();
        }

        //to add unique promotion based on sku in list
        protected virtual void AddPromotionSKUQuantity(PromotionCartItemQuantity cartItem, List<PromotionCartItemQuantity> promotionSkus)
        {
            if (promotionSkus?.Count == 0)
            {
                promotionSkus.Add(cartItem);
            }
            else if (promotionSkus.Any(x => x.Brand == cartItem.Brand) && !string.IsNullOrEmpty(cartItem.Brand))
            {
                PromotionCartItemQuantity promo = promotionSkus.FirstOrDefault(x => x.Brand == cartItem.Brand);
                promo.Quantity += cartItem.Quantity;
                promo.SKU += "," + cartItem.SKU;
            }
            else if (promotionSkus.Any(x => x.Category == cartItem.Category) && !string.IsNullOrEmpty(cartItem.Category))
            {
                PromotionCartItemQuantity promo = promotionSkus.FirstOrDefault(x => x.Category == cartItem.Category);
                promo.Quantity += cartItem.Quantity;
                promo.SKU += "," + cartItem.SKU;
            }
            else
            {
                promotionSkus.Add(cartItem);
            }
        }

        protected virtual bool AddProductCategory(string sku, int productId, decimal quantity, int promoCategoryId, List<PromotionCartItemQuantity> categorySkus)
        {
            bool isApplied = false;
            List<CategoryModel> productCategories = GetCategoryByProduct(productId);
            if (productCategories.Where(o => o.PimCategoryId == promoCategoryId).Count() > 0)
            {
                AddPromotionSKUQuantity(new PromotionCartItemQuantity { Category = Convert.ToString(promoCategoryId), SKU = sku, Quantity = quantity }, categorySkus);
                isApplied = true;
            }
            return isApplied;
        }

        //Get Categories by product ID.
        protected virtual List<int> GetCategories(int productId)
        {
            ZnodeRepository<ZnodePimCategoryProduct> _pimCategoryProductRepository = new ZnodeRepository<ZnodePimCategoryProduct>();
            ZnodeRepository<ZnodePimCategoryHierarchy> _pimCategoryHierarchyRepository = new ZnodeRepository<ZnodePimCategoryHierarchy>();
            List<int> pimCategoryIds = (from _pimCategoryProduct in _pimCategoryProductRepository.Table
                                       join _pimCategoryHierarchy in _pimCategoryHierarchyRepository.Table on _pimCategoryProduct.PimCategoryId equals _pimCategoryHierarchy.PimCategoryId
                                       where _pimCategoryProduct.PimProductId == productId
                                       select  _pimCategoryHierarchy.PimCategoryHierarchyId).ToList();
            return pimCategoryIds;
        }

        //Get Catalogs by product ID.
        protected virtual List<int> GetCatalogs(int productId)
        {
            ZnodeRepository<ZnodePimCategoryProduct> _pimCategoryProductRepository = new ZnodeRepository<ZnodePimCategoryProduct>();
            ZnodeRepository<ZnodePimCategoryHierarchy> _pimCategoryHierarchyRepository = new ZnodeRepository<ZnodePimCategoryHierarchy>();
            List<int> pimCatalogIds = (from _pimCategoryProduct in _pimCategoryProductRepository.Table
                                       join _pimCategoryHierarchy in _pimCategoryHierarchyRepository.Table on _pimCategoryProduct.PimCategoryId equals _pimCategoryHierarchy.PimCategoryId
                                       where _pimCategoryProduct.PimProductId == productId
                                       select _pimCategoryHierarchy.PimCatalogId.Value).ToList();
            return pimCatalogIds;
        }

        #endregion
    }

    public enum PromoApplicabilityCriteria
    {
        OrderAmount,
        Quantity,
        Both
    }
}
