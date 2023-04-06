using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Engine.WebStore.Controllers
{
    public class SearchController : BaseController
    {
        private readonly ISearchAgent _searchAgent;
        private readonly IWidgetDataAgent _widgetDataAgent;
        private readonly ICategoryAgent _categoryAgent;
     
        public SearchController(ISearchAgent searchAgent, IWidgetDataAgent widgetDataAgent, ICategoryAgent categoryAgent)
        {
            _searchAgent = searchAgent;
            _widgetDataAgent = widgetDataAgent;
            _categoryAgent = categoryAgent;
        }

        //Get products on the basis of paging.
        public virtual ActionResult ProductsPaging(string categoryName, int categoryId, int pageNum = 1)
        {
            Dictionary<string, object> searchProperties = new Dictionary<string, object>();
            searchProperties.Add(WebStoreConstants.PageSize, Request.QueryString[WebStoreConstants.PageSize]);
            searchProperties.Add(WebStoreConstants.PageNumber, Request.QueryString[WebStoreConstants.PageNumber]);
            searchProperties.Add(WebStoreConstants.Sort, Request.QueryString[WebStoreConstants.Sort]);

            return View("_ProductPaging", _widgetDataAgent.GetCategoryProducts(new WidgetParameter { CMSMappingId = categoryId, TypeOfMapping = categoryName, properties = searchProperties }, pageNum).Products);
        }

        //Index method.
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult Index(SearchRequestViewModel searchRequestModel, string category = "")
        {
            string redirectURl = string.Empty;
            _searchAgent.SetFilterData(searchRequestModel);
            if (!string.IsNullOrEmpty(searchRequestModel.SearchTerm) && searchRequestModel.CategoryId <= 0)
                redirectURl = _searchAgent.CheckURLExistForSearchTerm(searchRequestModel.SearchTerm);

            if (!string.IsNullOrEmpty(redirectURl))
                return Redirect(redirectURl);

            if (searchRequestModel.CategoryId > 0 & string.IsNullOrEmpty(searchRequestModel.SearchTerm))
            {
                bool? viewAll = SessionHelper.GetDataFromSession<bool?>(WebStoreConstants.ViewAllMode);
                return RedirectToAction("Index", "Category", new { category = searchRequestModel.Category, categoryId = searchRequestModel.CategoryId, viewAll = viewAll, fromSearch = true });
            }
            if (searchRequestModel.BrandId > 0)
            {
                bool? viewAll = SessionHelper.GetDataFromSession<bool?>(WebStoreConstants.ViewAllMode);
                return RedirectToAction("Index", "Brand", new { brand = searchRequestModel.BrandName, brandId = searchRequestModel.BrandId, fromSearch = true });
            }

            if (searchRequestModel.PageId > 0)
            {
                bool? viewAll = SessionHelper.GetDataFromSession<bool?>(WebStoreConstants.ViewAllMode);
                return RedirectToAction("ContentPage", "ContentPage", new { contentPageId = searchRequestModel.PageId, fromSearch = true });
            }
            if (!string.IsNullOrEmpty(category))
            {
                searchRequestModel.CategoryId = -1;
                searchRequestModel.SearchTerm = category;
            }

            Dictionary<string, object> searchProperties = GetSearchProperties(searchRequestModel.SearchTerm);
            searchRequestModel.ProductListViewModel = _widgetDataAgent.GetCategoryProducts(new WidgetParameter { CMSMappingId = searchRequestModel.CategoryId, LocaleId = PortalAgent.LocaleId, properties = searchProperties }, 1, 16, 0, true);

            return View("Search", searchRequestModel);
        }

        public virtual Dictionary<string, object> GetSearchProperties(string searchTerm)
        {
            int pageSize;
            //Parse for page size value
            int.TryParse(Request.QueryString[WebStoreConstants.PageSize], out pageSize);

            if (pageSize == 0)
                pageSize = _widgetDataAgent.GetDefaultPageSize();


            string sortValue = Convert.ToString(Request.QueryString[WebStoreConstants.Sort]);
            if(string.IsNullOrEmpty(sortValue))
            {
                // Get the default sort values
                sortValue = _widgetDataAgent.GetPortalDefaultSortValue();
            }
            Dictionary<string, object> searchProperties = new Dictionary<string, object>();
            searchProperties.Add(WebStoreConstants.PageSize, Convert.ToString(pageSize));
            searchProperties.Add(WebStoreConstants.PageNumber, Request.QueryString[WebStoreConstants.PageNumber]);
            searchProperties.Add(WebStoreConstants.Sort, Convert.ToString(sortValue));
            searchProperties.Add(WebStoreConstants.SearchTerm, searchTerm);
            searchProperties.Add(WebStoreConstants.UseSuggestion, Request.QueryString[WebStoreConstants.UseSuggestion]);
            return searchProperties;
        }

        //Get Seo Url details.
        //ToDo:
        public virtual ActionResult GetSeoUrlDetails(string seoUrl)
        {
            SEOUrlViewModel seoModel = _searchAgent.GetSeoUrlDetail("Electronics");
            return View();
        }

        [HttpGet]
        [RedirectFromLogin]
        public virtual JsonResult GetSuggestions(string query)
        {
            List<AutoComplete> result = _searchAgent.GetSuggestions(query, "", Url);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Get facets.
        public ActionResult GetFacets(int cMSMappingId, string displayName, string typeOfMapping, string widgetCode, string widgetKey, string properties)
            => PartialView("_FacetList", _widgetDataAgent.GetFacetList(new WidgetParameter { CMSMappingId = cMSMappingId, DisplayName = displayName, TypeOfMapping = HttpUtility.UrlDecode(typeOfMapping), WidgetCode = widgetCode, WidgetKey = widgetKey, LocaleId = PortalAgent.LocaleId, properties = GetProperties(properties) }, 1, -1));

        private Dictionary<string, object> GetProperties(string properties)
        {
            var res = new Dictionary<string, object>();

            int pagenumber = SessionHelper.GetDataFromSession<int>(WebStoreConstants.PageNumber);

            if (string.IsNullOrEmpty(properties)) return res;
            foreach (var item in properties.Split('_'))
            {
                if (string.IsNullOrEmpty(item)) continue;
                switch (item.Split('=')[0])
                {
                    case WebStoreConstants.PageSize:
                        res.Add(item.Split('=')[0], SessionHelper.GetDataFromSession<int>(WebStoreConstants.PageSize));
                        break;
                    case WebStoreConstants.Sort:
                        res.Add(item.Split('=')[0], SessionHelper.GetDataFromSession<int>(WebStoreConstants.Sort));
                        break;
                    case WebStoreConstants.PageNumber:
                        res.Add(item.Split('=')[0], pagenumber == 0 ? 1 : pagenumber);
                        break;
                    default:
                        res.Add(item.Split('=')[0], item.Split('=')[1]);
                        break;
                }
            }
            return res;
        }
        //Get sub categories.
        public ActionResult GetSubCategories(int cMSMappingId, string displayName, string typeOfMapping, string widgetCode, string widgetKey)
          => PartialView("_CategoryGrid", _widgetDataAgent.GetSubCategories(new WidgetParameter { CMSMappingId = cMSMappingId, DisplayName = displayName, TypeOfMapping = typeOfMapping, WidgetCode = widgetCode, WidgetKey = widgetKey, LocaleId = PortalAgent.LocaleId }));

        //Get CMS Pages base on keyword search
        public virtual ActionResult GetSearchContentPage(string searchTerm, int pageNumber = 1, int pageSize = 16)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Dictionary<string, object> searchProperties = GetCMSSearchProperties(searchTerm, pageNumber, pageSize);
                CMSPageListViewModel cmsSearchPages = _widgetDataAgent.GetSearchCMSPages(new WidgetParameter { properties = searchProperties });
                return PartialView("_ContentPage", cmsSearchPages);
            }
            return PartialView("_ContentPage", new CMSPageListViewModel());
        }

        public virtual Dictionary<string, object> GetCMSSearchProperties(string searchTerm, int pageNumber, int pageSize)
        {
            Dictionary<string, object> searchProperties = new Dictionary<string, object>();

            searchProperties.Add(WebStoreConstants.PageSize, Convert.ToString(pageSize));
            searchProperties.Add(WebStoreConstants.PageNumber, Convert.ToString(pageNumber));
            searchProperties.Add(WebStoreConstants.SearchTerm, searchTerm);

            return searchProperties;
        }
    }
}