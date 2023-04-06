using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    /// <summary>
    /// Base class for ZnodeCartPromotionManager, ZnodePricePromotionManager, and ZnodeProductPromotionManager.
    /// </summary>
    public class ZnodePromotionManager : ZnodeBusinessBase
    {
        #region Private Variables
        #endregion

        /// <summary>
        /// Notify those that care when we dispose.
        /// </summary>
        public event System.EventHandler Disposed;

        #region Constructor
        public ZnodePromotionManager()
        {

        }

        #endregion    

        // Gets the list of active cart promotions from the application cache.
        public List<PromotionModel> CartPromotionCache
        {
            get
            {
                List<PromotionModel> promotions = HttpRuntime.Cache["CartPromotionCache"] as List<PromotionModel>;
                if (HelperUtility.IsNull(promotions) || promotions?.Count <= 0)
                {
                    CacheActiveCartPromotions();
                }
                else
                {
                    promotions = HttpRuntime.Cache["CartPromotionCache"] as List<PromotionModel>;
                }
                return promotions ?? new List<PromotionModel>();
            }
        }

        // Gets the list of active price promotions from the application cache.
        public List<PromotionModel> PricePromotionCache
        {
            get
            {
                List<PromotionModel> promotions = HttpRuntime.Cache["PricePromotionCache"] as List<PromotionModel>;
                if (HelperUtility.IsNull(promotions) || promotions?.Count <= 0)
                {
                    CacheActivePricePromotions();
                }
                else
                {
                    promotions = HttpRuntime.Cache["PricePromotionCache"] as List<PromotionModel>;
                }
                return promotions ?? new List<PromotionModel>();
            }
        }

        // Gets the list of active product promotions from the application cache.
        public List<PromotionModel> ProductPromotionCache
        {
            get
            {
                List<PromotionModel> promotions = HttpRuntime.Cache["ProductPromotionCache"] as List<PromotionModel>;

                if (HelperUtility.IsNull(promotions) || promotions?.Count <= 0)
                {
                    CacheActiveProductPromotions();
                }
                else
                {
                    promotions = HttpRuntime.Cache["ProductPromotionCache"] as List<PromotionModel>;
                }

                return promotions ?? new List<PromotionModel>();
            }
        }

        // Gets the coupon from a promotion.
        public ZnodeCoupon GetCoupon(ZnodePromotionCoupon coupon)
              => HelperUtility.IsNull(coupon) ? new ZnodeCoupon() : new ZnodeCoupon
              {
                  RequiredBrandMinimumQuantity = coupon.InitialQuantity,
                  RequiredCatalogMinimumQuantity = coupon.InitialQuantity,
                  RequiredCategoryMinimumQuantity = coupon.InitialQuantity,
                  RequiredProductMinimumQuantity = coupon.InitialQuantity,
                  CouponId = coupon.PromotionCouponId,
                  CouponCode = coupon.Code,
                  CouponQuantityAvailable = coupon.AvailableQuantity,
              };

        //Caches all active promotions in the application cache.
        public static void CacheActivePromotions()
        {
            // NOTE: For performance reasons, we split the active promotions into three caches
            // because we don't need to verify cart promotions on product/category pages.
            CacheActiveCartPromotions();
            CacheActivePricePromotions();
            CacheActiveProductPromotions();
        }

        // Caches all active cart promotions in the application cache.
        public static void CacheActiveCartPromotions()
        {
            List<PromotionModel> cartPromotions = HttpRuntime.Cache["CartPromotionCache"] as List<PromotionModel>;
            if (HelperUtility.IsNull(cartPromotions) || cartPromotions?.Count <= 0)
            {
                cartPromotions = GetPromotionsByType("CART");
                ZnodeCacheDependencyManager.Insert("CartPromotionCache", cartPromotions, "ZnodePromotion");
            }
        }

        //Caches all active price promotions in the application cache.
        public static void CacheActivePricePromotions()
        {
            List<PromotionModel> cartPromotions = HttpRuntime.Cache["PricePromotionCache"] as List<PromotionModel>;
            if (HelperUtility.IsNull(cartPromotions) || cartPromotions?.Count <= 0)
            {
                cartPromotions = GetPromotionsByType("PRICE");
                ZnodeCacheDependencyManager.Insert("PricePromotionCache", cartPromotions, "ZnodePromotion");
            }
        }

        //Caches all active product promotions in the application cache.
        public static void CacheActiveProductPromotions()
        {
            List<PromotionModel> cartPromotions = HttpRuntime.Cache["ProductPromotionCache"] as List<PromotionModel>;
            if (HelperUtility.IsNull(HttpRuntime.Cache["ProductPromotionCache"]) || cartPromotions.Count <= 0)
            {
                cartPromotions = GetPromotionsByType("PRODUCT");
                ZnodeCacheDependencyManager.Insert("ProductPromotionCache", cartPromotions, "ZnodePromotion");
            }
        }
    
        // Caches all available promotion types in the application cache.
        [Obsolete("This method is not in use now, as removed caching for promotion types")]
        public static void CacheAvailablePromotionTypes()
        {
            List<IZnodePromotionsType> promoTypes = HttpRuntime.Cache["PromotionTypesCache"] as List<IZnodePromotionsType>;

            //Check if any promotion types available in cache or not.
            if (HelperUtility.IsNull(promoTypes) || promoTypes?.Count <= 0)
            {
                promoTypes = GetAvailablePromotionTypes();
                HttpRuntime.Cache["PromotionTypesCache"] = promoTypes.ToList();
            }
        }

        // Gets all available promotion types from the application cache.
        public static List<IZnodePromotionsType> GetAvailablePromotionTypes()
        {
            Container container = new Container();

                container.Configure(scanner => scanner.Scan(x =>
                {
                    x.AssembliesFromApplicationBaseDirectory();
                    x.AddAllTypesOf<IZnodePromotionsType>();
                }));

            // Only cache promotion types that have a ClassName and Name; this helps avoid showing base classes in some of the dropdown lists
            List<IZnodePromotionsType>  promoTypes = container.GetAllInstances<IZnodePromotionsType>().Where(x => !string.IsNullOrEmpty(x.ClassName) && !string.IsNullOrEmpty(x.Name))?.ToList();

            if (HelperUtility.IsNotNull(promoTypes))
                promoTypes.Sort((promoTypeA, promoTypeB) => string.CompareOrdinal(promoTypeA.Name, promoTypeB.Name));
            else
                promoTypes = new List<IZnodePromotionsType>();

            return promoTypes;
        }

        //Create and return instance for promotion classes
        public T GetPromotionTypeInstance<T>(PromotionModel promotionModel) where T : class
        {
            if (HelperUtility.IsNotNull(promotionModel))
            {
                string className = promotionModel.GetClassName();

                return GetPromotionTypeInstance<T>(className);
            }
            else return null;
        }

        public T GetPromotionTypeInstance<T>(string className) where T : class
        {
            if (!string.IsNullOrEmpty(className))
               return (T)GetKeyedService<IZnodePromotionsType>(className);
            else
               return null;
        }

        // Clean up. Nothing here though.
        public void Dispose(List<PromotionModel> promotions)
        {
            this.Dispose(true);
            GC.SuppressFinalize(promotions);
        }

        // Clean up.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                EventHandler handler = Disposed;
                if (!Equals(handler, null))
                    handler(this, EventArgs.Empty);
            }
        }

        #region Private Method

        // Gets the list of all promotions in the database.
        private static List<PromotionModel> AllPromotions
        {
            get
            {
                List<PromotionModel> list = HttpRuntime.Cache["AllPromotionCache"] as List<PromotionModel>;

                if (HelperUtility.IsNotNull(list) && list.Count > 0)
                {
                    return list;
                }
                IZnodePromotionHelper promotionHelper = GetService<IZnodePromotionHelper>();
                list = promotionHelper?.GetAllPromotions();
                return list ?? new List<PromotionModel>();
            }
        }

        //to get promotion by promotionsType
        private static List<PromotionModel> GetPromotionsByType(string promotionsType)
        {
            return GetPromotionsByType(promotionsType, AllPromotions);
        }

        //to get promotion by promotionsType
        private static List<PromotionModel> GetPromotionsByType(string promotionsType, List<PromotionModel> allPromotions)
        {
            List<PromotionModel> promotions = allPromotions?.Where(promo => (DateTime.Today.Date.AddDays(1) >= promo.StartDate && DateTime.Today.Date <= promo.EndDate)
                                                                            && promo.PromotionType.ClassType.Equals(promotionsType, StringComparison.OrdinalIgnoreCase)
                                                                            && promo.PromotionType.IsActive)
                                                           .OrderBy(x => x.DisplayOrder)
                                                           .ToList();

            return promotions ?? new List<PromotionModel>();
        }

        internal static List<PromotionModel> GetPromotionsByType(string promotionsType, string promotionName, List<PromotionModel> allPromotions, string orderBy = "QuantityMinimum")
        {
            List<PromotionModel> promotions = allPromotions.Where(promo => (promo.PromotionType.ClassType.Equals(promotionsType, StringComparison.OrdinalIgnoreCase)
                                                                            && promo.PromotionType.ClassName.Equals(promotionName)))
                                                           .OrderByDescending(x => typeof(PromotionModel).GetProperty(orderBy).GetValue(x, null))
                                                           .ToList();

            return promotions ?? new List<PromotionModel>();
        }

        protected int GetHeaderPortalId()
        {
            const string headerCartPortalId = "Znode-Cart-PortalId";
            int portalId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerCartPortalId], out portalId);
            return portalId > 0 ? portalId : ZnodeConfigManager.SiteConfig.PortalId;
        }
        #endregion
    }
}