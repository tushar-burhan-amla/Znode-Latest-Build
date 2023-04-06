using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;

using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Controllers
{
    public class CategoryController : BaseController
    {
        #region Private Readonly members
        private readonly ICategoryAgent _categoryAgent;
        private readonly IWidgetDataAgent _widgetDataAgent;
        #endregion

        #region Public Constructor
        public CategoryController(ICategoryAgent categoryAgent, IWidgetDataAgent widgetDataAgent)
        {
            _categoryAgent = categoryAgent;
            _widgetDataAgent = widgetDataAgent;
        }
        #endregion
        //TO Do:Have To cache on Profile Basis.
        //Get Category list for header.
        // [DonutOutputCache(CacheProfile = "PortalCacheProfile")]
        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "profileId;portalId;localeId;publishState;accountId;catalogId")]
        public virtual ActionResult TopLevelList(int? profileId, int portalId = 0, int localeId = 0, string publishState = "PRODUCTION", int accountId = 0, int catalogId = 0)
            => PartialView("_HeaderNav", _categoryAgent.GetCategories());

        //Get Category list for header mega menu.      
        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "profileId;portalId;localeId;publishState;accountId;catalogId")]
        public virtual ActionResult GetMegaMenuList(int? profileId, int portalId = 0, int localeId = 0, string publishState = "PRODUCTION", int accountId = 0, int catalogId = 0)
            => PartialView("_HeaderNavigation", _categoryAgent.GetCategories());

        [ChildActionOnly]
        // To get category list for footer.        
        public virtual ActionResult FooterMenu(int? profileId, int portalId = 0, int localeId = 0, string publishState = "PRODUCTION", int accountId = 0)
            => PartialView("_HeaderNav", _categoryAgent.GetCategories());

        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because sitemap logic is move to sitemap controller." +
        " Please use sitemap controller for sitemap releated functions.")]
        public virtual JsonResult SiteMapList(int? pageSize, int? pageLength)
        {
            return Json(new { Result = _categoryAgent.GetSiteMapCategories(pageSize, pageLength) }, JsonRequestBehavior.AllowGet);
        }

        //Get Category list for header.
        public virtual ActionResult Index(string category, int categoryId = 0, bool? viewAll = false, bool fromSearch = false, string facetGroup = "", string seoData = "", string sort = "")
        {
            SEOUrlViewModel sEOUrlViewModel = JsonConvert.DeserializeObject<SEOUrlViewModel>(seoData);

            ViewBag.category = category;
            ViewBag.categoryId = categoryId;
            ViewBag.viewAll = viewAll;
            ViewBag.fromSearch = fromSearch;
            ViewBag.facetGroup = facetGroup;
            ViewBag.sortList = sort;

            TempData["Title"] = sEOUrlViewModel?.SEOTitle;
            TempData["Keywords"] = sEOUrlViewModel?.SEOKeywords;
            TempData["Description"] = sEOUrlViewModel?.SEODescription;
            TempData["CanonicalURL"] = sEOUrlViewModel?.CanonicalURL;
            TempData["RobotTag"] = string.IsNullOrEmpty(sEOUrlViewModel?.RobotTag) || sEOUrlViewModel?.RobotTag.ToLower() == "none" ? string.Empty : sEOUrlViewModel?.RobotTag?.Replace("_", ",");

            return View("ProductList");
        }

        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "categoryId;publishState;viewAll;facetGroup;pageSize;pageNumber;sort;profileId;localeId;accountId;catalogId;portalId")]
        public ActionResult CategoryContent(string category, int categoryId = 0, bool? viewAll = false, bool fromSearch = false, string publishState = "PRODUCTION", bool bindBreadcrumb = true, string facetGroup = "", string sort = "", int localeId = 0, int profileId = 0, int accountId = 0, int catalogId = 0, int portalId = 0)
        {
            if (viewAll.Value)
                SessionHelper.SaveDataInSession<bool>(WebStoreConstants.ViewAllMode, true);
            else
                SessionHelper.RemoveDataFromSession(WebStoreConstants.ViewAllMode);

            CategoryViewModel ViewModel = _categoryAgent.GetCategorySeoDetails(categoryId, bindBreadcrumb);

            if (bindBreadcrumb)
                _categoryAgent.GetBreadCrumb(ViewModel);

            if (!fromSearch)
                //Remove Facet from Session.
                _categoryAgent.RemoveFromSession(ZnodeConstant.FacetsSearchFields);

            if (HelperUtility.IsNotNull(ViewModel.SEODetails))
            {
                ViewBag.Title = ViewModel.SEODetails.SEOTitle;
                TempData["Title"] = ViewModel.SEODetails.SEOTitle;
                TempData["Keywords"] = ViewModel.SEODetails.SEOKeywords;
                TempData["Description"] = ViewModel.SEODetails.SEODescription;
                TempData["CanonicalURL"] = ViewModel.SEODetails.CanonicalURL;
                TempData["RobotTag"] = string.IsNullOrEmpty(ViewModel.SEODetails.RobotTag) || ViewModel.SEODetails.RobotTag.ToLower() == "none" ? string.Empty : ViewModel.SEODetails.RobotTag?.Replace("_", ",");
            }
            Dictionary<string, object> searchProperties = GetSearchProperties();
            ViewModel.ProductListViewModel = _widgetDataAgent.GetCategoryProducts(new WidgetParameter { CMSMappingId = categoryId, TypeOfMapping = ViewModel.CategoryName, LocaleId = PortalAgent.LocaleId, properties = searchProperties }
            );

            return PartialView("_ProductListContent", ViewModel);
        }

        public Dictionary<string, object> GetSearchProperties()
        {
            int pageSize;
            //Parse for page size value
            int.TryParse(Request.QueryString[WebStoreConstants.PageSize], out pageSize);

            if (pageSize == 0)
                pageSize = _widgetDataAgent.GetDefaultPageSize();

            string sortValue = Convert.ToString(Request.QueryString[WebStoreConstants.Sort]);

            if (string.IsNullOrEmpty(sortValue))
            {
                sortValue = _widgetDataAgent.GetPortalDefaultSortValue();
            }
            Dictionary<string, object> searchProperties = new Dictionary<string, object>();
            searchProperties.Add(WebStoreConstants.PageSize, Convert.ToString(pageSize));
            searchProperties.Add(WebStoreConstants.PageNumber, Request.QueryString[WebStoreConstants.PageNumber]);
            searchProperties.Add(WebStoreConstants.Sort, Convert.ToString(sortValue));
            return searchProperties;
        }

        public virtual ActionResult GetBreadCrumb(int categoryId)
        {
            string breadCrumb = _categoryAgent.GetBreadCrumb(categoryId);
            return Json(new
            {
                breadCrumb = breadCrumb ?? string.Empty
            }, JsonRequestBehavior.AllowGet);
        }
    }
}