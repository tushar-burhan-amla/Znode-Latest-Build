using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Xml;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.WebStore.Core.Agents;

namespace Znode.Engine.WebStore.Controllers
{
    public class SiteMapController : BaseController
    {
        #region Private Readonly members
        private readonly ISiteMapAgent _siteMapAgent;

        private const string Sitemap = "sitemap.xml";
        private const string GoogleProductFeed = "googleproductfeed.xml";
        private const string BingProductFeed = "bingproductfeed.xml";
        private const string XmlProductFeed = "xmlproductfeed.xml";
        private const string CategorySitemapFile = "categoryxmlsitemap_{chunk}.xml";
        private const string ProductSitemapFile = "productxmlsitemap_{chunk}.xml";
        private const string ContentSitemapFile = "contentpagesxmlsitemap_{chunk}.xml";
        private const string AllSitemapFile = "allxmlsitemap_{chunk}.xml";
        private const string GoogleProductFeedFile = "googleproductfeed_{chunk}_{localeId}.xml";
        private const string BingProductFeedFile = "bingproductfeed_{chunk}_{localeId}.xml";
        private const string XmlProductFeedFile = "xmlproductfeed_{chunk}_{localeId}.xml";
        #endregion

        #region Public Constructor
        public SiteMapController(ISiteMapAgent siteMapAgent)
        {
            _siteMapAgent = siteMapAgent;
        }
        #endregion

        // GET: SiteMap
        [HttpGet]
        public virtual ActionResult Index() => ActionView("SiteMap");

        /// <summary>
        /// This method used to get the categories of sitemap.
        /// </summary>
        /// <param name="pageSize">This parameter is use to set the page number.</param>
        /// <param name="pageLength">This parameter is use to set the length of the page.</param>
        /// <returns>Json Result is return.</returns>
        public virtual JsonResult SiteMapList(int? pageSize, int? pageLength)
        {
            return Json(new { Result = _siteMapAgent.GetSiteMapCategoryList(pageSize, pageLength) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method used to get the product of sitemap.
        /// </summary>
        /// <param name="pageSize">This parameter is use to set the page number.</param>
        /// <param name="pageLength">This parameter is use to set the length of the page.</param>
        /// <returns>Json Result is return.</returns>
        public virtual JsonResult GetPublishProduct(int? pageIndex, int? pageSize)
            => Json(new { result = _siteMapAgent.GetPublishProductList(pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);

        //This method is used to render the sitemap xml documents.
        [AllowAnonymous]
        [Route(CategorySitemapFile)]
        [Route(ProductSitemapFile)]
        [Route(ContentSitemapFile)]
        [Route(AllSitemapFile)]
        [Route(GoogleProductFeedFile)]
        [Route(BingProductFeedFile)]
        [Route(XmlProductFeedFile)]
        public ActionResult RenderSiteMap()
        {
            string RequestUrl = Request.Url.LocalPath;
            string xmlDocument = GetXmlFromAPI(RequestUrl);
            if (HelperUtility.IsNull(xmlDocument))
            {
                return View("ErrorPage");
            }
            return Content(xmlDocument, "text/xml");
        }

        //This method generates the virtual xml document containing the list of child Urls.
        [AllowAnonymous]
        [Route(Sitemap, Name = "RenderSiteMapIndexFile")]
        [Route(GoogleProductFeed)]
        [Route(BingProductFeed)]
        [Route(XmlProductFeed)]
        [HttpGet]
        public ActionResult RenderSiteMapIndexFile()
        {
            string xmlDocument = string.Empty;
            string actionName = ((System.Web.Routing.Route)ControllerContext.RouteData.Route).Url;
            string requestUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            List<ProductFeedModel> productFeedListModels = _siteMapAgent.GetProductFeedByPortalId(actionName, requestUrl, out xmlDocument);
            if (HelperUtility.IsNotNull(productFeedListModels) && productFeedListModels?.Count > 0)
            {
                return Content(xmlDocument, "text/xml");
            }
            else
            {
                return View("ErrorPage");
            }
        }

        // This method gets the XML document for webstore domains from API.
        private string GetXmlFromAPI(string requestUrl)
        {
            return _siteMapAgent.GetXmlFromAPIForWebstoreDomain(requestUrl);
        }
    }
}