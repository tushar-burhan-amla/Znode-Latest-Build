using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore.Controllers
{
    public class ContentPageController : BaseController
    {
        #region Private Readonly members
        private readonly IWidgetDataAgent _widgetDataAgent;

        #endregion Private Readonly members

        #region Public Constructor
        public ContentPageController(IWidgetDataAgent widgetDataAgent)
        {

            _widgetDataAgent = widgetDataAgent;

        }
        //Get content page data.
        #endregion
        public virtual ActionResult ContentPage(int contentPageId, bool fromSearch = false)
        {
            ContentPageListViewModel contentPages = _widgetDataAgent.GetContentPages(new WidgetParameter());

            ContentPageViewModel contentPage = contentPages?.ContentPageList?.FirstOrDefault(x => x.ContentPageId == contentPageId);

            if (HelperUtility.IsNull(contentPage))
                contentPage = contentPages?.ContentPageList?.FirstOrDefault(x => x.ContentPageId == contentPageId && x.LocaleId == Convert.ToInt32(DefaultSettingHelper.DefaultLocale));
            string contentPageTemplateName = string.Empty;

            ViewBag.contentPageId = contentPageId;
            if (HelperUtility.IsNotNull(contentPage))
            {
                contentPageTemplateName = contentPage.FileName;

                //Set Properties for SEO data.
                ViewBag.TemplateName = contentPageTemplateName;

                ViewBag.Title = contentPage.SEOTitle;
                ViewBag.Description = contentPage.SEODescription;
                ViewBag.Keywords = contentPage.SEOKeywords;
                ViewBag.RobotTag = string.IsNullOrEmpty(contentPage.RobotTag) || contentPage?.RobotTag?.ToLower() == "none" ? string.Empty : contentPage?.RobotTag.Replace("_", ",");
                ViewBag.Canonicalurl = contentPage.CanonicalURL;
            }

            return View("ContentPage");
        }

        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "pageId;publishState;localeId;profileId;accountId;catalogId")]
        public ActionResult ContentPageContent(string template, int pageId, string publishState = "PRODUCTION", int localeId = 0, int profileId = 0, int accountId = 0, int catalogId = 0)
        {
            ViewBag.contentPageId = pageId;
            return View(template, pageId);
        }
    }
}
