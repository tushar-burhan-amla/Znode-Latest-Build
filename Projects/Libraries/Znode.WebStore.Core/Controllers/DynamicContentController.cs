using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.Controllers;
using Znode.Libraries.ECommerce.Utilities;
using System.Web.SessionState;
using System.Web.UI;
using System;

namespace Znode.WebStore.Core.Controllers
{    
    public class DynamicContentController : BaseController
    {
        #region Public Constructor
        public DynamicContentController()
        {            
        }
        #endregion

        //Get published blog news data.
        [HttpGet]
        public virtual ActionResult Widget(string widgetCode, string displayName, string widgetKey, string typeOfMapping, string partialViewName, int mappingId = 0, string properties = null)
        {
            using (WidgetParameter prm = new WidgetParameter())
            {
                prm.WidgetCode = widgetCode;
                prm.WidgetKey = widgetKey;
                prm.TypeOfMapping = typeOfMapping;
                prm.CMSMappingId = mappingId;
                prm.DisplayName = displayName;
                prm.properties = string.IsNullOrEmpty(properties) ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(properties);
                prm.PartialViewName = partialViewName;
                return PartialView("~/Views/Shared/_WidgetAjax.cshtml", prm);
            }
        }

        [HttpGet]
        [ActionSessionState(SessionStateBehavior.ReadOnly)]
        [ZnodePageCache(Duration = 600, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "portalId")]
        [Obsolete("This action is no longer in use.")]
        public virtual ContentResult GetDynamicStyles(int portalId)
        {
            return Content(PortalAgent.CurrentPortal.DynamicStyle, "text/css");
        }
    }
}
