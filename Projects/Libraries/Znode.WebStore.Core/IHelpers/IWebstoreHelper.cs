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

namespace Znode.Engine.WebStore
{
     public interface IWebstoreHelper
    {
        void Session_Start(object sender, EventArgs e);

        //Get user view model from session.
         UserViewModel GetUserViewModelFromSession();

        //Get seo url detail.
         SEOUrlViewModel GetSeoUrlDetail(string currentSlug);

        //Get Active 301 Redirects.
         UrlRedirectListModel GetActive301Redirects();

        //Get filter configuration XML.
         ApplicationSettingDataModel GetFilterConfigurationXML(string listName);

        //Get filter configuration XML setting.
         string GetFilterConfigurationXMLSetting(string listName);

        //Get current portal.
         PortalViewModel GetCurrentPortal(int cachePortalId = 0);

        //Get Login Providers.
         SocialModel GetLoginProviders();

         WidgetDataAgent WidgetDataAgent();

         IMessageAgent MessageAgent();

         ILocaleClient LocaleClient();

         IDefaultGlobalConfigClient DefaultGlobalConfigClient();

        /// <summary>
        /// Checks if the store/portal has ever been published.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns>True, if the store has been published at least once.</returns>
         bool HasPortalBeenPublishedBefore(int portalId);

        //Get billing account number.
         string GetBillingAccountNumber(int userId);

         ZnodePublishStatesEnum GetCurrentPublishState();

         void SaveDataInCookie(string key, string value, double time);

        //Gets the default value for assigning to InHandDate
        DateTime GetInHandDate();

        //Gets the Names and Descriptions of passed Enum Members and returns in ShippingConstraintsViewModel
        IList<ShippingConstraintsViewModel> GetEnumMembersNameAndDescription(Enum value);

        //Get Highlights list from Attributes
        List<HighlightsViewModel> GetHighlightListFromAttributes(List<AttributesViewModel> attributeModel, string sku, int publishProductId);
    }
}
