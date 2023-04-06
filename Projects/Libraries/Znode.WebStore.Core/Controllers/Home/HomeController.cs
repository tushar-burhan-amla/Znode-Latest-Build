using DevTrends.MvcDonutCaching;

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Controllers
{
    public class HomeController : BaseController
    {
        #region Private Readonly members

        private readonly IPortalAgent _portalAgent;
        private readonly IStoreLocatorAgent _storeLocatorAgent;
        private readonly IUserAgent _userAgent;
        private readonly IWidgetDataAgent _widgetDataAgent;
        private readonly IBlogNewsAgent _blogNewsAgent;
        private readonly ICartAgent _cartAgent;

        public static string News = "News";
        public static string Blogs = "Blog";
        public static string NewsCount = "NewsCount";
        public static string BlogsCount = "BlogsCount";

        #endregion Private Readonly members

        #region Public Constructor

        public HomeController(IPortalAgent portalAgent, IStoreLocatorAgent storeLocatorAgent, IUserAgent userAgent, IWidgetDataAgent widgetDataAgent, IBlogNewsAgent blogNewsAgent, ICartAgent cartAgent)
        {
            _portalAgent = portalAgent;
            _storeLocatorAgent = storeLocatorAgent;
            _userAgent = userAgent;
            _widgetDataAgent = widgetDataAgent;
            _blogNewsAgent = blogNewsAgent;
            _cartAgent = cartAgent;
        }

        #endregion Public Constructor

        #region Private Constants

        private const string contentPage = "ContentPage";
        private const string storeLocator = "StoreLocator";
        private const string RobotsTxt = "RobotsTxt";

        #endregion Private Constants

        public virtual ActionResult Index()
        {
            ViewBag.Description = PortalAgent.CurrentPortal?.WebsiteDescription;
            string affiliate_Id = "affiliateId";
            if (!string.IsNullOrEmpty(Request.QueryString[affiliate_Id]))
                _userAgent.SetAffiliateId(Request.QueryString[affiliate_Id]);
#if DEBUG

            return (!Convert.ToBoolean(ZnodeWebstoreSettings.DisablePortalSelection)
                        && HelperUtility.IsNull(SessionHelper.GetDataFromSession<object>("PortalId")))
                    ? RedirectToAction<DevController>(o => o.PortalSelection())
                    : View("Home");
#else
                return View("Home");
#endif
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View("About");
        }

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View("Contact");
        }

        //Get Store Locator radius list.
        [HttpGet]
        public virtual ActionResult StoreLocator()
        {
            StoreLocatorViewModel model = new StoreLocatorViewModel();
            model.RadiusList = _storeLocatorAgent.GetDistanceList();
            _storeLocatorAgent.GetPortalList(model);
            return View(storeLocator, model);
        }

        // Get Web Store Locator list from Postalcode,State Name and City Name.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult StoreLocator(StoreLocatorViewModel storeLocatorViewModel)
        {
            storeLocatorViewModel.RadiusList = _storeLocatorAgent.GetDistanceList();

            _storeLocatorAgent.GetPortalList(storeLocatorViewModel);

            return View(storeLocator, storeLocatorViewModel);
        }

        //Change locale value of portal.
        public virtual ActionResult ChangeLocale(string LocaleId)
        {
            _portalAgent.ChangeLocale(LocaleId);
            string url = GetUrlWithoutFacetQuery();
            return Redirect(url);
        }

        //Sign Up For News Letter.
        [AllowAnonymous]
        public virtual JsonResult SignUpForNewsLetter(string emailId)
        {
            bool status = false;
            string message = string.Empty;
            if (!string.IsNullOrEmpty(emailId))
            {
                status = _userAgent.SignUpForNewsLetter(new NewsLetterSignUpViewModel() { Email = emailId }, out message);

                message = (!string.IsNullOrEmpty(message))
                    ? status ? WebStore_Resources.NewsLetterSignUpSuccess : message
                    : status ? WebStore_Resources.NewsLetterSignUpSuccess : WebStore_Resources.NewsLetterSignUpError;
            }
            return Json(new { sucess = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Handles all application level errors.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ErrorHandler(Exception exception)
        {
            HttpException httpexception = exception as HttpException;

            if (HelperUtility.IsNotNull(httpexception))
            {
                int httpCode = httpexception.GetHttpCode();

                switch (httpCode)
                {
                    case 404:
                        {
                            ViewBag.ErrorMessage = WebStore_Resources.HttpCode_401_AccessDeniedMsg;
                            break;
                        }
                    case 401:
                        {
                            ViewBag.ErrorMessage = WebStore_Resources.HttpCode_401_AccessDeniedMsg;
                            break;
                        }
                    default:
                        {
                            if (exception is HttpRequestValidationException)
                                ViewBag.ErrorMessage = WebStore_Resources.HttpCode_500_RequestValidationErrorMsg;
                            else
                                ViewBag.ErrorMessage = WebStore_Resources.HttpCode_500_InternalServerErrorMsg;
                            break;
                        }
                }
            }
            else
                ViewBag.ErrorMessage = WebStore_Resources.GenericErrorMessage;
            return View("ElmahError");
        }

        //Remove Facet group and Facet key from url.
        private string GetUrlWithoutFacetQuery()
        {
            var uri = Request.UrlReferrer;
            // this gets all the query string key value pairs as a collection
            NameValueCollection newQueryString = HttpUtility.ParseQueryString(uri.Query);
            // this removes the Facet keys if they exist
            newQueryString.Remove("FacetValue");
            newQueryString.Remove("FacetGroup");

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            string url = newQueryString.Count > 0
                        ? $"{pagePathWithoutQueryString}?{newQueryString}"
                        : pagePathWithoutQueryString;
            return url;
        }

        [Route("robots.txt", Name = "GetRobotsText")]
        [ZnodePageCache(Duration = 86400, Location = OutputCacheLocation.Server, VaryByParam = "", VaryByCustom = WebStoreConstants.RobotsTxtPortalIdFullPageCacheKey)]
        [HttpGet]
        public virtual ContentResult GetRobotsTxt1()
          => Content(_portalAgent.GetRobotsTxt().RobotsTxtContent, WebStoreConstants.TextPlain, Encoding.UTF8);

        [HttpGet]
        public virtual ActionResult GetBarcodeScanner()
        {
            BarcodeReaderViewModel barcode = _portalAgent.GetBarcodeScannerDetail();
            return View("_BarcodeScanner", barcode);
        }

        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "portalId;publishState;localeId;profileId;accountId;catalogId")]
        public PartialViewResult HomeContent(int portalId = 0, string publishState = "PRODUCTION", int localeId = 0, int profileId = 0, int accountId = 0, int catalogId = 0) => PartialView("_HomeContent");

        [ChildActionOnly]
        [ZnodeOutputCache(Duration = 3600, VaryByParam = "portalId;publishState;localeId;profileId;accountId;catalogId")]
        public PartialViewResult FooterContent(int portalId = 0, string publishState = "PRODUCTION", int localeId = 0, int profileId = 0, int accountId = 0, int catalogId = 0) => PartialView("_FooterContent");

        //Get footer.
        public PartialViewResult Footer() => PartialView("_Footer");

        //Get offer banner slider.
        public PartialViewResult OfferBanner(int cMSMappingId, string displayName, string typeOfMapping, string widgetCode, string widgetKey)
        {
            return PartialView("_OfferBanner", _widgetDataAgent.GetSlider(new WidgetParameter { CMSMappingId = cMSMappingId, DisplayName = displayName, TypeOfMapping = typeOfMapping, WidgetCode = widgetCode, WidgetKey = widgetKey, LocaleId = PortalAgent.LocaleId }));
        }

        //Get widget category list
        public PartialViewResult WidgetCategoryList(int cMSMappingId, string displayName, string typeOfMapping, string widgetCode, string widgetKey)
        {
            return PartialView("_Category", _widgetDataAgent.GetCategories(new WidgetParameter { CMSMappingId = cMSMappingId, DisplayName = displayName, TypeOfMapping = typeOfMapping, WidgetCode = widgetCode, WidgetKey = widgetKey, LocaleId = PortalAgent.LocaleId }));
        }

        //Get widget brand list
        public virtual PartialViewResult WidgetBrandList(int cMSMappingId, string displayName, string typeOfMapping, string widgetCode, string widgetKey)
        {
            return PartialView("_Brand", _widgetDataAgent.GetBrands(new WidgetParameter { CMSMappingId = cMSMappingId, DisplayName = displayName, TypeOfMapping = typeOfMapping, WidgetCode = widgetCode, WidgetKey = widgetKey, LocaleId = PortalAgent.LocaleId }));
        }

        //Get Cart Count for Cache
        [AllowAnonymous]
        public virtual decimal GetCartCount() => _cartAgent.GetCartCount();

        //Get mediaid to Download files of any format by mediaid
        public virtual ActionResult DownloadMediaById(int mediaId)
        {
            Api.Models.MediaDetailModel mediaInfo = GetService<IMediaManagerAgent>().GetMediaDetailsById(mediaId);
            return Json(new { status = mediaInfo }, JsonRequestBehavior.AllowGet);
        }

        //Get guid to Download files of any format 
        public virtual ActionResult DownloadMediaByGuid(string mediaGuid)
        {
            Api.Models.MediaDetailModel mediaInfo = GetService<IMediaManagerAgent>().GetMediaDetailsByGuid(mediaGuid);
            return Json(new { status = mediaInfo }, JsonRequestBehavior.AllowGet);
        }
    }
}