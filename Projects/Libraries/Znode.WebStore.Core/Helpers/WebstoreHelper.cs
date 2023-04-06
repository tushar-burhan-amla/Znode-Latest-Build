using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.WebStore.Core.Helpers;
namespace Znode.Engine.WebStore
{
    public class WebstoreHelper : BaseAgent, IWebstoreHelper
    {
        public void Session_Start(object sender, EventArgs e)
        {
            //Check if enable cart persistent option is true.
            if (PortalAgent.CurrentPortal.PersistentCartEnabled)
            {
                //Get cart.
                ICartAgent _cartAgent = GetService<ICartAgent>();
                decimal cartcount = _cartAgent.GetCartCount();

                //If cart has any cart items then redirect it to cart page.
                if (cartcount > 0)
                {
                    SessionHelper.SaveDataInSession<bool>(WebStoreConstants.CartMerged, true);
                    HttpContext.Current.Response.Redirect("~/cart");
                }
            }
        }

        //Get user view model from session.
        public UserViewModel GetUserViewModelFromSession()
        => GetService<IUserAgent>()?.GetUserViewModelFromSession();

        //Get seo url detail.
        public SEOUrlViewModel GetSeoUrlDetail(string currentSlug)
        => GetService<ISearchAgent>()?.GetSeoUrlDetail(currentSlug);

        //Get Active 301 Redirects.
        public UrlRedirectListModel GetActive301Redirects()
        => GetService<IUrlRedirectAgent>()?.GetActive301Redirects();

        //Get filter configuration XML.
        public ApplicationSettingDataModel GetFilterConfigurationXML(string listName)
        => GetService<IApplicationSettingsAgent>()?.GetFilterConfigurationXML(listName);

        //Get filter configuration XML setting.
        public string GetFilterConfigurationXMLSetting(string listName)
        => GetService<IApplicationSettingsAgent>()?.GetFilterConfigurationXML(listName)?.Setting;

        //Get current portal.
        public PortalViewModel GetCurrentPortal(int cachePortalId = 0)
        => new PortalAgent(GetClient<WebStorePortalClient>(), GetClient<DomainClient>(), GetClient<PortalClient>())?.GetCurrentPortal(cachePortalId);

        //Get Login Providers.
        public SocialModel GetLoginProviders()
        => GetService<IUserAgent>()?.GetLoginProviders();

        public WidgetDataAgent WidgetDataAgent()
        => new WidgetDataAgent(GetClient<IWebStoreWidgetClient>(GetService<IWebStoreWidgetClient>()), GetClient<IPublishProductClient>(GetService<IPublishProductClient>()), GetClient<IPublishCategoryClient>(GetService<IPublishCategoryClient>()), GetClient<IBlogNewsClient>(GetService<IBlogNewsClient>()), GetClient<IContentPageClient>(GetService<IContentPageClient>()), GetClient<ISearchClient>(GetService<ISearchClient>()), GetClient<ICMSPageSearchClient>(GetService<ICMSPageSearchClient>()));

        public IMessageAgent MessageAgent()
        => new MessageAgent(GetClient<IWebStoreMessageClient>(GetService<IWebStoreMessageClient>()));

        public ILocaleClient LocaleClient()
            => GetClient<ILocaleClient>(GetService<ILocaleClient>());

        public IDefaultGlobalConfigClient DefaultGlobalConfigClient()
            => GetClient<IDefaultGlobalConfigClient>(GetService<IDefaultGlobalConfigClient>());

        /// <summary>
        /// Checks if the store/portal has ever been published.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns>True, if the store has been published at least once.</returns>
        public bool HasPortalBeenPublishedBefore(int portalId)
        {
            var portals = GetClient<PortalClient>().GetPortalPublishStatus(new FilterCollection() { new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()) }, new SortCollection(), null, null);

            return portals?.PublishPortalLogList?.Any() == true ? portals.PublishPortalLogList.Where(x => x.IsPortalPublished.HasValue && x.IsPortalPublished.Value).Any() : false;
        }

        //Get billing account number.
        public string GetBillingAccountNumber(int userId)
        {
            return SessionProxyHelper.GetBillingAccountNumber(userId);
        }

        public ZnodePublishStatesEnum GetCurrentPublishState()
        {
            return HelperUtility.IsNotNull(PortalAgent.CurrentPortal) ? PortalAgent.CurrentPortal.PublishState : 0;
        }

        public void SaveDataInCookie(string key, string value, double time)
        {
            CookieHelper.SetCookie(key, value, time * 60);
        }

        //Gets the default value for assigning to InHandDate
        public virtual DateTime GetInHandDate()
        {
            const int postDateDays = 30;
            return DateTime.Today.Date.AddDays(postDateDays);
        }

        //Gets the Names and Descriptions of passed Enum Members and returns in ShippingConstraintsViewModel
        public virtual IList<ShippingConstraintsViewModel> GetEnumMembersNameAndDescription(Enum value)
        {
            return HelperUtility.GetNamesAndDescriptionsFromEnum(value).Select(s => new ShippingConstraintsViewModel
                {
                    ShippingConstraintCode = s.Key,
                    Description = s.Value
                }).ToList();
        }
        
        //Get Highlights list from Attributes
        public virtual List<HighlightsViewModel> GetHighlightListFromAttributes(List<AttributesViewModel> attributeModel, string sku, int publishProductId)
        {
            List<HighlightsViewModel> highlightList = new List<HighlightsViewModel>();
            if (attributeModel.Where(x => x.AttributeCode.Equals(ZnodeConstant.Highlights.ToString(), StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                foreach (AttributesSelectValuesViewModel attributesSelectValuesViewModel in attributeModel.Where(x => x.AttributeCode.Equals(ZnodeConstant.Highlights.ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().SelectValues.OrderBy(x => x.DisplayOrder).ToList())
                {
                    HighlightsViewModel highlight = new HighlightsViewModel()
                    {
                        HighlightName = attributesSelectValuesViewModel.Value,
                        HighlightCode = attributesSelectValuesViewModel.Code,
                        MediaPath = ImageUrlHelper.GetImageWithThumbnailPath(string.IsNullOrEmpty(attributesSelectValuesViewModel.Path) ? "no-image.png" : attributesSelectValuesViewModel.Path),
                        PublishProductId = publishProductId,
                        SKU = sku,
                        DisplayOrder = attributesSelectValuesViewModel.DisplayOrder
                    };
                    highlightList.Add(highlight);
                }
                return highlightList;
            }
            return new List<HighlightsViewModel>();
        }
    }
}
