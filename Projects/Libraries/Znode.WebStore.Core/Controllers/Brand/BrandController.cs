using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Controllers
{
    public class BrandController : BaseController
    {
        #region Private Variables
        private readonly IBrandAgent _brandAgent;
        private readonly ICategoryAgent _categoryAgent;
        #endregion

        #region Public Constructor
        public BrandController(IBrandAgent brandAgent, ICategoryAgent categoryAgent)
        {
            _brandAgent = brandAgent;
            _categoryAgent = categoryAgent;
        }
        #endregion

        #region Public Methods.

        public virtual ActionResult List() => View(_brandAgent.BrandList());

        //Get Filtered brand list.
        public virtual ActionResult BrandList(string brandName)
        {
            List<BrandViewModel> list = _brandAgent.BrandList(brandName);
            return View("FilterBrandList", list);
        }

        //Get Brand Products And Facets.
        [HttpGet]
        public virtual ActionResult Index(string brand, int brandId = 0, bool fromSearch = false)
        {
            BrandViewModel brandData = _brandAgent.GetBrandDetails(brandId);

            _brandAgent.SetBreadCrumbHtml(brandData);

            //Set Properties for SEO data.
            ViewBag.Title = string.IsNullOrEmpty(brandData.SEOTitle) ? brandData.BrandName : brandData.SEOTitle;
            ViewBag.Description = brandData.SEODescription;
            ViewBag.Keywords = brandData.SEOKeywords;

            return View("BrandProducts", brandData);
        }

        #endregion
    }
}