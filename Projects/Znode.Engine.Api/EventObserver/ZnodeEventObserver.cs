using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Engine.Services;
using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Observer;
namespace Znode.Engine.Api
{
    public partial class ZnodeEventObserver
    {
        #region Private Variables 
        Connector<IEnumerable<PublishedProductEntityModel>> productToken;
        Connector<List<int>> categoryToken;
        Connector<List<CategoryPublishEventModel>> categoryPublishToken;
        Connector<DefaultGlobalConfigListModel> cacheKey;
        Connector<PortalModel> webstorePortalKey;
        readonly EventAggregator eventAggregator;
        Connector<ZnodePortalCountry> myZnodePortalCountryToken;
        Connector<ZnodePromotionCoupon> myZnodePromotionCouponToken;
        Connector<PriceSKUModel> myPriceSKUModelToken;
        Connector<InventorySKUModel> myInventorySKUModelToken;
        Connector<PromotionModel> myPromotionModelToken;
        Connector<string> myWebstorePageToken;
        Connector<RecommendationSettingModel> PortalRecommendation;
        Connector<CMSTypeMappingModel> myContentPagePreviewModelToken;
        Connector<CloudflarePurgeModel> clearCloudflareCache;
        Connector<SearchKeywordsRedirectModel> searchKeywordsRedirectToken;
        Connector<PortalApprovalModel> portalApprovalDetailsToken;
        protected readonly IUserService userService;
        #endregion

        #region Constructor
        public ZnodeEventObserver(EventAggregator eve)
        {
            eventAggregator = eve;
            userService = ZnodeDependencyResolver.GetService<IUserService>();
            eve.Attach<IEnumerable<PublishedProductEntityModel>>(this.OnPublishProduct); //1
            eve.Attach<List<int>>(this.OnPublishCategory);//1
            eve.Attach<List<CategoryPublishEventModel>>(this.OnPublishCategory);//1
            eve.Attach<Dictionary<string, string>>(this.OnLoggingConfigurationUpdate);
            eve.Attach<PortalModel>(this.OnPortalUpdate);
            eve.Attach<ZnodePortalCountry>(this.OnChangeCountryAssociation);
            eve.Attach<ZnodePromotionCoupon>(this.OnCreatePromotion);

            eve.Attach<PriceSKUModel>(this.OnUpdatePrice);
            eve.Attach<InventorySKUModel>(this.OnUpdateInventory);
            eve.Attach<PromotionModel>(this.OnUpdatePromotion);
            eve.Attach<string>(this.OnWebstorePageChange);
            eve.Attach<RecommendationSettingModel>(this.OnUpdateProductRecommendations);
            eve.Attach<CMSTypeMappingModel>(this.OnPublishContentPage);
            eve.Attach<CloudflarePurgeModel>(this.OnPublishStore);
            eve.Attach<SearchKeywordsRedirectModel>(this.OnKeywordsRedirectListChange);
            eve.Attach<PortalApprovalModel>(this.OnPortalApprovalDetailsChange);

            eve.Attach<UserModel>(this.OnNewUserCreateFromWebstore, EventConstant.OnNewCustomerAccountFromWebstore);
            eve.Attach<UserModel>(this.OnNewUserCreateFromAdmin, EventConstant.OnNewCustomerAccountFromAdmin);
            eve.Attach<UserModel>(this.OnAccountVerificationSuccessful, EventConstant.OnAccountVerificationSuccessful);
            eve.Attach<UserModel>(this.OnAccountVerificationRequestInProgress, EventConstant.OnAccountVerificationRequestInProgress);
            eve.Attach<UserModel>(this.OnRegistrationAttemptUsingExistingUsername, EventConstant.OnRegistrationAttemptUsingExistingUsername);
            eve.Attach<UserModel>(this.OnResetPassword, EventConstant.OnResetPassword);
            eve.Attach<OrderModel>(this.OnShippingReceipt, EventConstant.OnOrderShipped);
            eve.Attach<OrderModel>(this.OnCancelledOrderReceipt, EventConstant.OnOrderCancelled);
            eve.Attach<OrderModel>(this.OnNotificationForLowInventory, EventConstant.OnNotificationForLowInventory);
            eve.Attach<OrderModel>(this.OnOrderPlaced, EventConstant.OnOrderPlaced);
            eve.Attach<RMAReturnModel>(this.OnReturnStatusNotificationForCustomer, EventConstant.OnReturnStatusNotificationForCustomer);
            eve.Attach<RMAReturnModel>(this.OnRefundProcessedNotificationForCustomer, EventConstant.OnRefundProcessedNotificationForCustomer);
            eve.Attach<RMAReturnModel>(this.OnReturnRequestNotificationForCustomer, EventConstant.OnReturnRequestNotificationForCustomer);
            eve.Attach<UserModel>(this.OnCustomerAccountActivation, EventConstant.OnCustomerAccountActivation);
            eve.Attach<WebStoreCaseRequestModel>(this.OnCustomerFeedbackNotification, EventConstant.OnCustomerFeedbackNotification);
            eve.Attach<WebStoreCaseRequestModel>(this.OnServiceRequestMessage, EventConstant.OnServiceRequestMessage);
            eve.Attach<WebStoreCaseRequestModel>(this.OnContactUs, EventConstant.OnContactUs);
            eve.Attach<ReferralCommissionModel>(this.OnTrackingLinks, EventConstant.OnTrackingLinks);
            eve.Attach<OrderModel>(this.OnProductKeyOrderReceipt, EventConstant.OnProductKeyOrderReceipt); 
            eve.Attach<OrderModel>(this.OnQuoteConvertedToOrder, EventConstant.OnQuoteConvertedToOrder); 
            eve.Attach<UserModel>(this.OnBillingAccountNumberAdded, EventConstant.OnBillingAccountNumberAdded); 
            eve.Attach<UserModel>(this.OnConvertCustomerToAdministrator, EventConstant.OnConvertCustomerToAdministrator);
            eve.Attach<OrderModel>(this.OnRemainingVoucherBalance, EventConstant.OnRemainingVoucherBalance);
            eve.Attach<GiftCardModel>(this.OnVoucherExpirationReminder, EventConstant.OnVoucherExpirationReminder); 
            eve.Attach<GiftCardModel>(this.OnIssueVoucher, EventConstant.OnIssueVoucher);  
            eve.Attach<AccountQuoteModel>(this.OnPendingOrderApproved, EventConstant.OnPendingOrderApproved); 
            eve.Attach<AccountQuoteModel>(this.OnPendingOrderRejected, EventConstant.OnPendingOrderRejected);
            eve.Attach<OrderModel>(this.OnResendOrderReceipt, EventConstant.OnResendOrderReceipt); 
            eve.Attach<ShoppingCartModel>(this.OnPendingOrderStatusNotification, EventConstant.OnPendingOrderStatusNotification); 
            eve.Attach<AccountQuoteModel>(this.OnPendingOrderStatusNotification, EventConstant.OnPendingOrderStatusNotification); 
            eve.Attach<QuoteResponseModel>(this.OnQuoteRequestAcknowledgementToUser, EventConstant.OnQuoteRequestAcknowledgementToUser);
            //SEO => Product - >Content -> Category -
        }
        #endregion

        

        private void SendOrderConfirmationSMS(OrderModel userModel)
        {

        }

        private void SendOrderConfirmationSMS(RMAReturnModel userModel)
        {

        }

        /// <summary>
        /// Delete Webstore cache on updating pricing in product.
        /// </summary>
        /// <param name="model"></param>
        private void OnUpdatePrice(PriceSKUModel model)
        {
            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnUpdatePrice() firing.",
                PortalIds = new int[] { model.PortalId },
                Key = CachedKeys.OnUpdatePrice
            });

            eventAggregator.Detach(myPriceSKUModelToken);
        }

        /// <summary>
        /// Delete Webstore cache on updating inventory in product.
        /// </summary>
        /// <param name="model"></param>
        private void OnUpdateInventory(InventorySKUModel model)
        {
            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnUpdateInventory() firing.",
                PortalIds = new int[] { model.PortalId },
                Key = CachedKeys.OnUpdateInventory
            });

            eventAggregator.Detach(myInventorySKUModelToken);
        }

        /// <summary>
        /// Delete Webstore cache on updating promotion in product.
        /// </summary>
        /// <param name="model"></param>
        private void OnUpdatePromotion(PromotionModel model)
        {
            int[] portalIds = model.PortalId.HasValue ? new int[] { model.PortalId.Value } : new int[0];
            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnUpdatePromotion() firing.",
                PortalIds = portalIds,
                Key = CachedKeys.OnUpdatePromotion
            });

            eventAggregator.Detach(myPromotionModelToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void OnCreatePromotion(ZnodePromotionCoupon model)
        {
            HttpRuntime.Cache.Remove(CachedKeys.AllPromotionCache);

            eventAggregator.Detach(myZnodePromotionCouponToken);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void OnPortalUpdate(PortalModel model)
        {
            ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPortalUpdate() firing.",
                RouteTemplateKeys = new[] { CachedKeys.webstoreportal }
            });

            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPortalUpdate() firing.",
                PortalIds = new int[] { model.PortalId },
                Key = CachedKeys.webstoreportal
            });

            eventAggregator.Detach(webstorePortalKey);
        }

        /// <summary>
        /// Clear webstore cache on product recommendation setting update event.
        /// </summary>
        /// <param name="recommendationSettingModel"></param>
        private void OnUpdateProductRecommendations(RecommendationSettingModel recommendationSettingModel)
        {
            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnUpdateProductRecommendations() firing.",
                PortalIds = new int[] { recommendationSettingModel.PortalId },
                Key = CachedKeys.webstoreportal
            });

            eventAggregator.Detach(PortalRecommendation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void OnPublishProduct(IEnumerable<PublishedProductEntityModel> model)
        {
            if (HelperUtility.IsNull(model))
            {
                return;
            }

            IEnumerable<int> catalogid = model.Select(x => x.ZnodeCatalogId).AsEnumerable().Distinct();
            int[] portalIds = GetPortalIds(catalogid);

            //Clear API cache 
            ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishProduct() firing.",
                RouteTemplateKeys = new[]
                {
                    CachedKeys.ProductListKey_,
                    CachedKeys.fulltextsearch,
                    CachedKeys.webstoreproduct,
                    CachedKeys.getlinkproductlist,
                    CachedKeys.geturlredirectlist
                }
            });
            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishProduct() firing.",
                PortalIds = portalIds,
                Key = CachedKeys.ProductListKey_
            });

            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishProduct() firing.",
                PortalIds = portalIds,
                Key = CachedKeys.geturlredirects
            });

            eventAggregator.Detach(productToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// This event is obsolete instead of this event please use new event which have parameter List<CategoryPublishEventModel>.
        [Obsolete]
        private void OnPublishCategory(List<int> model)
        {
            //Clear API cache 
            ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishCategory(List<int> model) firing.",
                RouteTemplateKeys = new[]
                {
                    CachedKeys.CategoryListKey_,
                    CachedKeys.categorydetails
                }
            });

            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishCategory(List<int> model) firing.",
                PortalIds = GetPortalIds(model),
                Key = CachedKeys.CategoryListKey_
            });

            eventAggregator.Detach(categoryToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void OnPublishCategory(List<CategoryPublishEventModel> model)
        {
            if (HelperUtility.IsNull(model))
            {
                return;
            }

            //Clear API cache 
            ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishCategory(List<int> model) firing.",
                RouteTemplateKeys = new[]
                {
                    CachedKeys.CategoryListKey_,
                    CachedKeys.categorydetails,
                    CachedKeys.geturlredirectlist
                }
            });

            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishCategory(List<int> model) firing.",
                PortalIds = GetPortalIds(model.Select(x => x.PortalId).ToList()),
                Key = CachedKeys.CategoryListKey_
            });

            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnPublishCategory(List<int> model) firing.",
                PortalIds = GetPortalIds(model.Select(x => x.PortalId).ToList()),
                Key = CachedKeys.geturlredirects
            });

            new CloudflareHelper().PurgeUrlContentsOnCategoryPublish(model);

            eventAggregator.Detach(categoryPublishToken);
        }

        private void OnLoggingConfigurationUpdate(Dictionary<string, string> model)
        {
            ClearCacheHelper.EnqueueEviction(new LoggingSettingsUpdateEvent()
            {
                Comment = "From ZnodeEventObserver.OnLoggingConfigurationUpdate() firing."
            });

            eventAggregator.Detach(cacheKey);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void OnChangeCountryAssociation(ZnodePortalCountry model)
        {
            ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
            {
                Comment = $"From ZnodeEventObserver.OnChangeCountryAssociation() firing.",
                PortalIds = new int[] { model.PortalId },
                Key = string.Concat(CachedKeys.CountriesList, Convert.ToString(model.PortalId))
            });

            eventAggregator.Detach(myZnodePortalCountryToken);
        }

        /// <summary>
        /// Delete Donut Webstore cache.
        /// </summary>
        /// <param name="model"></param>
        private void OnWebstorePageChange(string portalIds)
        {
            eventAggregator.Detach(myWebstorePageToken);
        }

        private int[] GetPortalIds(IEnumerable<int> catalogids)
        {
            //get portal id
            IZnodeRepository<ZnodePortalCatalog> portalCatalog = new ZnodeRepository<ZnodePortalCatalog>();

            var portal = (from p in portalCatalog.Table
                          where catalogids.Contains(p.PublishCatalogId)
                          select new
                          {
                              p.PortalId
                          }).ToArray();
            int[] terms = new int[portal.Count()];
            int i = 0;
            foreach (var item in portal)
            {
                terms[i] = Convert.ToInt32(item.PortalId);
                i++;
            }

            return terms;
        }

        private int[] GetAllPortalIds()
        {
            //get portal id
            IZnodeRepository<ZnodePortal> portal = new ZnodeRepository<ZnodePortal>();
            var portalids = (from p in portal.Table
                             select new
                             {
                                 p.PortalId
                             }).ToArray();
            int[] terms = new int[portalids.Count()];
            int i = 0;
            foreach (var item in portalids)
            {
                terms[i] = Convert.ToInt32(item.PortalId);
                i++;
            }

            return terms;
        }

        /// <summary>
        /// Publishes the Content page.
        /// </summary>
        /// <param name="model"></param>
        private void OnPublishContentPage(CMSTypeMappingModel model)
        {
            CMSPreviewHelper.PublishWidgetToPreview(model);

            eventAggregator.Detach(myContentPagePreviewModelToken);
        }

        /// <summary>
        /// Purge Cloudflare Cache.
        /// </summary>
        /// <param name="cloudflarePurgeModel"></param>
        private void OnPublishStore(CloudflarePurgeModel cloudflarePurgeModel)
        {
            if (HelperUtility.IsNull(cloudflarePurgeModel))
            {
                return;
            }

            new CloudflareHelper().PurgeEverythingByPortalId(string.Join(",", cloudflarePurgeModel.PortalId));

            eventAggregator.Detach(clearCloudflareCache);
        }

        /// <summary>
        /// To clear catalog keywords redirect list cache.
        /// </summary>
        /// <param name="searchKeywordsRedirect"></param>
        private void OnKeywordsRedirectListChange(SearchKeywordsRedirectModel searchKeywordsRedirect)
        {
            ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
            {
                Comment = "From publishing search keywords redirect model.",
                RouteTemplateKeys = new string[] { CachedKeys.getcatalogkeywordsredirectlist }
            });

            eventAggregator.Detach(searchKeywordsRedirectToken);
        }

        /// <summary>
        /// To clear portal approval details cache.
        /// </summary>
        /// <param name="portalApprovalDetails">Portal approval details.</param>
        private void OnPortalApprovalDetailsChange(PortalApprovalModel portalApprovalDetails)
        {
            ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
            {
                Comment = "From publishing portal approval model.",
                RouteTemplateKeys = new string[] { CachedKeys.GetPortalApprovalDetailsById }
            });

            eventAggregator.Detach(portalApprovalDetailsToken);
        }
    }
}