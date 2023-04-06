using CaptchaMvc.HtmlHelpers;
using System;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.Resources;

namespace Znode.WebStore.Core.Extensions
{
    public class CaptchaAuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        //
        // Summary:
        //     Gets or sets the AttributeCode.
        //
        // Returns:
        //     The AttributeCode.
        public string AttributeCode
        { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string globalAttributeCode = !string.IsNullOrEmpty(AttributeCode) ? AttributeCode : WebStoreConstants.CaptchaRequired;
            if (Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, globalAttributeCode, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue))
                filterContext.Controller.IsCaptchaValid(WebStore_Resources.ErrorCaptchaCode);
        }
    }
}
