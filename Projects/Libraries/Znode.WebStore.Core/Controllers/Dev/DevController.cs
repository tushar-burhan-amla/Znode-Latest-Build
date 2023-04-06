using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Controllers
{
    public class DevController : BaseController
    {
        #region Private Readonly members

        private readonly IPortalAgent _portalAgent;

        #endregion Private Readonly members

        #region Public Constructor

        public DevController(IPortalAgent portalAgent)
        {
            _portalAgent = portalAgent;
        }

        #endregion Public Constructor


#if DEBUG
        public virtual ActionResult PortalSelection()
        {
                TempData["PortalList"] = _portalAgent.GetDevPortalSelectList();
                HttpContext.Cache.Remove(HttpContext.Request.Url.Authority);
                return View("PortalSelection");
        }


        [HttpPost]
        public virtual ActionResult PortalSelection(string portalId)
        {
            HttpContext.Cache.Remove(HttpContext.Request.Url.Authority);
            SessionHelper.SaveDataInSession<object>("PortalId", portalId);
            if (PortalAgent.SetCurrentPortalInCache()?.PortalId.ToString() == portalId)
                return RedirectToAction<HomeController>(o => o.Index());
            else
            {
                TempData["PortalList"] = _portalAgent.GetDevPortalSelectList();
                return View("PortalSelection");
            }
        }
#endif
    }
}
