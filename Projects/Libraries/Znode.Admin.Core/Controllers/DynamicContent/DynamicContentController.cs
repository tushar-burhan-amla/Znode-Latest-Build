using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.ViewModels;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class DynamicContentController : BaseController
    {
        #region Private Variables
        private readonly IDynamicContentAgent _dynamicContentAgent;
        #endregion

        #region Public Constructor
        public DynamicContentController(IDynamicContentAgent dynamicContentAgent)
        {
            _dynamicContentAgent = dynamicContentAgent;
        }
        #endregion

        [HttpGet]
        public virtual JsonResult GetEditorFormats(int portalId)
        {
            EditorFormatListViewModel editorFormatslist = _dynamicContentAgent.GetEditorFormats(portalId);
            if (IsNotNull(editorFormatslist))
                return Json(editorFormatslist, JsonRequestBehavior.AllowGet);
            else
                return Json(new EditorFormatListViewModel(), JsonRequestBehavior.AllowGet);
        }
    }
}
