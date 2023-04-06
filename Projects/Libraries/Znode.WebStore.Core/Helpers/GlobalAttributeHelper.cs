using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public static class GlobalAttributeHelper
    {
        //returns the value of store level global attribute to show price and inventory to logged-in users only  
        public static bool IsShowPriceAndInventoryToLoggedInUsersOnly()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, WebStoreConstants.LoginToSeePricingAndInventory, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        //returns the value of store level global attribute to show inventory of default and 'all warehouses combined' associated to current store. 
        public static bool IsShowAllLocationsInventory()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, "DisplayAllWarehousesStock", StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        //returns the value of store level global attribute to check Cloudflare is Enable or disable. 
        public static bool IsCloudflareEnabled()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, "IsCloudflareEnabled", StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        //gets the value of user level global attribute to show message set for user by Admin.  
        public static string GetUserMessage()
        {
            UserViewModel userViewModel = SessionHelper.GetDataFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            return userViewModel?.UserGlobalAttributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, "UserMessage", StringComparison.InvariantCultureIgnoreCase))?.AttributeValue;
        }

        //returns the value of store level global attribute to check placing Quote Request is Enable or disable for store. 
        public static bool IsQuoteRequestEnabled()
          => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, "EnableQuoteRequest", StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        //returns the value of store level global attribute which signifies the number of days in which quote will get expire
        public static int GetQuoteExpireDays()
        => Convert.ToInt32(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, "QuoteExpireInDays", StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        //returns the value of store level global attribute to show captcha required to log-in for user.
        public static bool IsCaptchaRequiredForLogin()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, WebStoreConstants.CaptchaRequiredForLogin, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        //returns the value of store level global attribute to check order return request is enable or disable for store .
        public static bool EnableReturnRequest()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, WebStoreConstants.EnableReturnRequest, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        // Returns the value of store level global attribute inventory stock notification.
        public static bool IsEnableInventoryStockNotification()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, WebStoreConstants.EnableInventoryStockNotification, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);

        //Returns the value of store level global attribute to check TradeCentric is enable or disable. Returns 'true' only if the tradecentric user's session is active.
        public static bool IsTradeCentricUserActive()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, WebStoreConstants.EnableTradeCentric, StringComparison.InvariantCultureIgnoreCase) && HelperUtility.IsNotNull(SessionHelper.GetDataFromSession<TradeCentricUserModel>(WebStoreConstants.TradeCentricSessionKey)))?.AttributeValue);

        //Returns the value of store level global attribute to check TradeCentric is enable or disable.
        public static bool IsEnableTradeCentric()
            => Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, WebStoreConstants.EnableTradeCentric, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);
    }
}
