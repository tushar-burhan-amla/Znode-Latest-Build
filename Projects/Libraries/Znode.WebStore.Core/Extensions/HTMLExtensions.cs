using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.WebStore.ViewModels;
namespace System.Web.Mvc.Html
{
    public static class HTMLExtensions
    {
        private const string ConfigButtonPartialViewPath = "~/Views/Shared/_ConfigButton.cshtml";
        private const string WidgetPlaceHolderPartialViewPath = "~/Views/Shared/_WidgetPlaceHolder.cshtml";

        private static ThemedViewEngine engine = new ThemedViewEngine();

        //Render Message by message key.
        public static MvcHtmlString RenderMessage(this HtmlHelper htmlHelper, string key)
        => MvcHtmlString.Create(HttpUtility.HtmlDecode(Helper.GetMessage(key, (string)HttpContext.Current.Request.RequestContext.RouteData.Values["controller"])));


        //Render Message by message key.
        public static MvcHtmlString RenderBlock(this HtmlHelper htmlHelper, string content)
        => MvcHtmlString.Create(HttpUtility.HtmlDecode(content));

        //Render Message by message key.
        public static MvcHtmlString RenderBlockEncoded(this HtmlHelper htmlHelper, string content)
        => MvcHtmlString.Create(HttpUtility.HtmlEncode(content));

        //Too many ajax calls from page increases load on server and also, it decreases the possible advantage of donut caching
        //But if donut caching is off, in that case, having everything rendered in first call, slows down the first impression of the page
        //So this method balances both the conditions.
        //If donut cache is enabled, it will use "WidgetPartial()" method, otherwise it will call "WidgetPartialAjax()" method
        public static MvcHtmlString WidgetPartialAuto(this HtmlHelper htmlHelper, string widgetCode, string displayName, string widgetKey, string typeOfMapping, int mappingId = 0, Dictionary<string, object> properties = null)
        {
            if (PortalAgent.CurrentPortal.IsFullPageCacheActive)
            {
                return WidgetPartial(htmlHelper, widgetCode, displayName, widgetKey, typeOfMapping, mappingId, properties);
            }
            return WidgetPartialAjax(htmlHelper, widgetCode, displayName, widgetKey, typeOfMapping, mappingId, properties);
        }


        //Created this overload method to pass template name in widgets like container. If template name exists then it will render template html
        public static MvcHtmlString WidgetPartialAuto(this HtmlHelper htmlHelper, string widgetCode, string displayName, string widgetKey, string typeOfMapping, string partialViewName, int mappingId = 0, Dictionary<string, object> properties = null)
        {
            if (PortalAgent.CurrentPortal.IsFullPageCacheActive)
            {
                return WidgetPartial(htmlHelper, widgetCode, displayName, widgetKey, typeOfMapping, partialViewName, mappingId, properties);
            }
            return WidgetPartialAjax(htmlHelper, widgetCode, displayName, widgetKey, typeOfMapping, partialViewName, mappingId, properties);
        }

        //Get Widgets control. It renders the widget on server-side and provides the data/html
        public static MvcHtmlString WidgetPartial(this HtmlHelper htmlHelper, string widgetCode, string displayName, string widgetKey, string typeOfMapping, int mappingId = 0, Dictionary<string, object> properties = null)
        {
            ControllerContext controllerContext = htmlHelper.ViewContext.Controller.ControllerContext;
            ViewEngineResult viewResult = engine.FindPartialView(controllerContext, $"_Widget{widgetCode}", true);
            using (WidgetParameter prm = new WidgetParameter())
            {
                prm.WidgetCode = widgetCode;
                prm.WidgetKey = widgetKey;
                prm.TypeOfMapping = typeOfMapping;
                prm.CMSMappingId = mappingId;
                prm.DisplayName = displayName;
                prm.properties = properties;
                return GetWidget(htmlHelper, controllerContext, viewResult, prm);
            }
        }

        //Created this overload method which binds the View name in WidgetParameter model
        public static MvcHtmlString WidgetPartial(this HtmlHelper htmlHelper, string widgetCode, string displayName, string widgetKey, string typeOfMapping, string partialViewName, int mappingId = 0, Dictionary<string, object> properties = null)
        {
            ControllerContext controllerContext = htmlHelper.ViewContext.Controller.ControllerContext;
            ViewEngineResult viewResult = engine.FindPartialView(controllerContext, $"_Widget{widgetCode}", true);
            using (WidgetParameter prm = new WidgetParameter())
            {
                prm.WidgetCode = widgetCode;
                prm.WidgetKey = widgetKey;
                prm.TypeOfMapping = typeOfMapping;
                prm.CMSMappingId = mappingId;
                prm.DisplayName = displayName;
                prm.properties = properties;
                prm.PartialViewName = partialViewName;
                return GetWidget(htmlHelper, controllerContext, viewResult, prm);
            }
        }
        private static MvcHtmlString GetWidget(HtmlHelper htmlHelper, ControllerContext controllerContext, ViewEngineResult viewResult, WidgetParameter prm)
        {
            if (!Equals(prm, null) && prm.CMSMode
              && prm.TypeOfMapping.ToLower().Equals(ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower()))
                return ConfigButton(htmlHelper, ConfigButtonPartialViewPath, prm).Concat(GetPartial(htmlHelper, viewResult, controllerContext, prm));
            else
                return GetPartial(htmlHelper, viewResult, controllerContext, prm);
        }

        //Get Widgets control in a non-blocking manner with Ajax. 
        public static MvcHtmlString WidgetPartialAjax(this HtmlHelper htmlHelper, string widgetCode, string displayName, string widgetKey, string typeOfMapping, int mappingId = 0, Dictionary<string, object> properties = null)
        {
            using (WidgetParameter prm = new WidgetParameter())
            {
                prm.WidgetCode = widgetCode;
                prm.WidgetKey = widgetKey;
                prm.TypeOfMapping = typeOfMapping;
                prm.CMSMappingId = mappingId;
                prm.DisplayName = displayName;
                prm.properties = properties;

                return PartialExtensions.Partial(htmlHelper, WidgetPlaceHolderPartialViewPath, prm);
            }
        }


        //Created this Overload to bind template view name.Get Widgets control in a non-blocking manner with Ajax. 
        public static MvcHtmlString WidgetPartialAjax(this HtmlHelper htmlHelper, string widgetCode, string displayName, string widgetKey, string typeOfMapping, string partialViewName, int mappingId = 0, Dictionary<string, object> properties = null)
        {
            using (WidgetParameter prm = new WidgetParameter())
            {
                prm.WidgetCode = widgetCode;
                prm.WidgetKey = widgetKey;
                prm.TypeOfMapping = typeOfMapping;
                prm.CMSMappingId = mappingId;
                prm.DisplayName = displayName;
                prm.properties = properties;
                prm.PartialViewName = partialViewName;
                return PartialExtensions.Partial(htmlHelper, WidgetPlaceHolderPartialViewPath, prm);
            }
        }

        public static MvcHtmlString Concat(this MvcHtmlString first, MvcHtmlString htmlString) => MvcHtmlString.Create(first.ToString() + string.Concat(htmlString.ToString()));

        //Get Partial view of widgets.
        private static MvcHtmlString GetPartial(HtmlHelper htmlHelper, ViewEngineResult viewResult, ControllerContext controllerContext, object data = null)
        => BuildPartial(htmlHelper, controllerContext, viewResult, data);

        //Get Partial view of widgets.
        private static MvcHtmlString GetPartial(HtmlHelper htmlHelper, string partialViewName, object data = null)
        {
            ControllerContext controllerContext = htmlHelper.ViewContext.Controller.ControllerContext;
            ViewEngineResult viewResult = engine.FindPartialView(controllerContext, partialViewName, true);
            return BuildPartial(htmlHelper, controllerContext, viewResult, data);
        }

        //Find and build partial view for widgets render.
        private static MvcHtmlString BuildPartial(HtmlHelper htmlHelper, ControllerContext controllerContext, ViewEngineResult viewResult, object data = null)
        {
            ViewDataDictionary viewData = new ViewDataDictionary(data);
            TempDataDictionary tempData = htmlHelper.ViewContext.TempData;
            using (StringWriter stringWriter = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, stringWriter);

                viewResult.View.Render(viewContext, stringWriter);

                return MvcHtmlString.Create(stringWriter.GetStringBuilder().ToString());
            }
        }

        public static MvcHtmlString ConfigButton(HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
                => GetPartial(htmlHelper, partialViewName, widgetparameter);

        //Slider control widgets.
        public static MvcHtmlString SliderControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
            => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetSlider(widgetparameter));

        //Text control Widget.
        public static MvcHtmlString TextControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetContent(widgetparameter));

        public static MvcHtmlString SearchWidgetControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        {
            Dictionary<string, object> searchProperties = new Dictionary<string, object>();
            HttpRequest currentRequest = HttpContext.Current.Request;
            searchProperties.Add(WebStoreConstants.PageSize, currentRequest.QueryString[WebStoreConstants.PageSize]);
            searchProperties.Add(WebStoreConstants.PageNumber, currentRequest.QueryString[WebStoreConstants.PageNumber]);
            searchProperties.Add(WebStoreConstants.Sort, currentRequest.QueryString[WebStoreConstants.Sort]);
            widgetparameter.properties = searchProperties;

            return GetPartial(htmlHelper, partialViewName, WidgetHelper.GetSearchWidgetData(widgetparameter));
        }


        //Search control Widget.
        public static MvcHtmlString SearchControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, new AutoComplete());

        //Layout header control Widget.
        public static MvcHtmlString HeaderControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //layout footer control Widget.
        public static MvcHtmlString FooterControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Top level cart count control Widget.
        public static MvcHtmlString CartControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Top level category menu control Widget.
        public static MvcHtmlString TopLevelNavigationControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //offer banner control Widget.
        public static MvcHtmlString OfferBannerControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetSlider(widgetparameter));

        //Home page special control Widget.
        public static MvcHtmlString HomePageSpecialControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter, string type = null)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Footer Help section control Widget.
        public static MvcHtmlString HelpsControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetContentPages(widgetparameter));

        //Footer store info control Widget.
        public static MvcHtmlString StoreInfoControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //footer customer support control Widget.
        public static MvcHtmlString CustomerSupportControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
       => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Landing page Product grid control Widget.
        public static MvcHtmlString ProductGridControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetCategoryProducts(widgetparameter));

        //Recently viewed product control widget.
        public static MvcHtmlString RecentlyViewProductControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
       => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Sub category list control Widget.
        public static MvcHtmlString SubCategoryListControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Facets list control Widget.
        public static MvcHtmlString FacetsListControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Advertisement banner control Widget.
        public static MvcHtmlString AdvertisementControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
       => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Locale list control Widget.
        public static MvcHtmlString LocalesControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
       => GetPartial(htmlHelper, partialViewName, htmlHelper.ViewContext.ViewData);

        //Product List Widget Control.
        public static MvcHtmlString ProductControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
       => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetProducts(widgetparameter));

        //Recommended product list control for home page.
        public static MvcHtmlString HomeRecommendationControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, widgetparameter);

        //Recommended product list control for PDP page.
        public static MvcHtmlString PDPRecommendationControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, widgetparameter);

        //Recommended product list control for cart page.
        public static MvcHtmlString CartRecommendationControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, widgetparameter);

        //Media Widget Control
        public static MvcHtmlString MediaControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
            => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetMedia(widgetparameter));

        //Link Widget Control.
        public static MvcHtmlString LinkControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetLinkWidget(widgetparameter));

        //Category List Widget Control.
        public static MvcHtmlString CategoryControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetCategories(widgetparameter));

        //Brand List Widget Control.
        public static MvcHtmlString BrandControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetBrands(widgetparameter));

        //Category grid control Widget.
        public static MvcHtmlString CategoryGridControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetSubCategories(widgetparameter));

        //Facets control for widget.
        public static MvcHtmlString FacetsControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetFacetList(widgetparameter));

        //Quick View control Widget.
        public static MvcHtmlString QuickViewControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetProductQuickView(widgetparameter));

        //Quick Order control Widget.
        public static MvcHtmlString QuickOrderControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, new AutoComplete() { Properties = widgetparameter.properties });

        //Quick Order pad control Widget.
        public static MvcHtmlString QuickOrderPadControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, new AutoComplete());

        [Obsolete("Google Tag Manager is now installed using a different approach. Obsolete since date: 15th March 2018")]
        //meta tag.
        public static MvcHtmlString TagManager(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetTagManager(widgetparameter));


        public static MvcHtmlString AddToCartLink(this HtmlHelper html, string text, string action, Dictionary<string, string> routeValues, string cssClassName, string clickElement = null, string disable = "")
        {
            if (HelperUtility.IsNotNull(routeValues))
            {
                string productId = string.Empty;
                routeValues.TryGetValue("ProductId", out productId);
                //Build a form tag.
                TagBuilder tbForm = new TagBuilder("form");
                tbForm.MergeAttribute("method", "POST");
                tbForm.MergeAttribute("action", action);
                tbForm.MergeAttribute("id", $"Form_{productId}");

                //Build hidden fields.
                List<string> inputs = new List<string>();
                if (HelperUtility.IsNotNull(routeValues))
                {
                    foreach (var key in routeValues.Keys)
                    {
                        string inputFormat = @"<input type='hidden' id='dynamic-" + key.ToLower() + "' name='{0}' value='{1}' />";

                        string input = string.Format(inputFormat, key, html.Encode(routeValues[key]));
                        inputs.Add(input);
                    }
                }

                //build a button.
                string submitBtn = $"<button type='submit' id ='button-addtocart_{productId}' onclick ='return  {clickElement}' {disable} class='{cssClassName}' value='{text}'>{text}</button>";
                inputs.Add(submitBtn);
                inputs.Add(html.AntiForgeryToken().ToString());

                string innerHtml = string.Join("\n", inputs.ToArray());
                tbForm.InnerHtml = innerHtml;

                // return self closing
                return new MvcHtmlString(tbForm.ToString());
            }
            return null;
        }

        //Add to cart ajax control.
        public static MvcHtmlString AddToCartAjaxRequest(this HtmlHelper html, string text, string action, Dictionary<string, string> routeValues, string cssClassName, string clickElement = null, string disable = "", string successElement = null)
        {
            if (HelperUtility.IsNotNull(routeValues))
            {
                if (Helper.IsEnhancedEcommerceTrackingEnabled())
                {
                    successElement = string.Concat("GoogleAnalytics.prototype.SendProductAddToCarts(this, event, '" + @Helper.GetPortalCurrency() + "');", successElement);
                }
                string productId = string.Empty;
                routeValues.TryGetValue("ProductId", out productId);
                //Build a form tag.
                TagBuilder tbForm = new TagBuilder("form");
                tbForm.MergeAttribute("method", "POST");
                tbForm.MergeAttribute("action", action);
                tbForm.MergeAttribute("id", $"Form_{productId}");
                tbForm.MergeAttribute("data-ajax", "true");
                tbForm.MergeAttribute(" data-ajax-begin", "ZnodeBase.prototype.ShowLoader();");
                tbForm.MergeAttribute("data-ajax-success", successElement);
                tbForm.MergeAttribute("data-ajax-error", "ZnodeBase.prototype.HideLoader();");

                //Build hidden fields.
                List<string> inputs = new List<string>();
                if (HelperUtility.IsNotNull(routeValues))
                {
                    foreach (var key in routeValues.Keys)
                    {
                        string inputFormat = @"<input type='hidden' id='dynamic-" + key.ToLower() + "' name='{0}' value='{1}' />";

                        string input = string.Format(inputFormat, key, html.Encode(routeValues[key]));
                        inputs.Add(input);
                    }
                }

                //build a button.
                string submitBtn = $"<button data-test-selector='btnAddToCart' type='submit' id ='button-addtocart_{productId}' onclick ='return  {clickElement}' {disable} class='{cssClassName}' value='{text}'><i class='zf-cart'></i>{text}</button>";
                inputs.Add(submitBtn);
                inputs.Add(html.AntiForgeryToken().ToString());

                string innerHtml = string.Join("\n", inputs.ToArray());
                tbForm.InnerHtml = innerHtml;

                // return self closing
                return new MvcHtmlString(tbForm.ToString());
            }
            return null;
        }

        //Get cart item count.
        public static MvcHtmlString CartCount(this HtmlHelper html, MvcHtmlString innerHtml)
        {
            //Get cart item count.
            string cartItemCount = $"<a href='/cart'>{innerHtml}<i class='f-size zf-cart header-toolbar-cart'></i><span class='cartcount'>{Helper.GetRoundOffQuantity(WidgetHelper.GetCartCount())}</span></a>";
            return new MvcHtmlString(cartItemCount);
        }

        public static MvcHtmlString Video(this HtmlHelper htmlHelper, string url)
               => MvcHtmlString.Create(HttpUtility.HtmlDecode(MediaPaths.GetVideoTag(url)));

        //Form Widget Control.
        public static MvcHtmlString FormWidgetControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        => GetPartial(htmlHelper, partialViewName, WidgetHelper.GetFormConfiguration(widgetparameter));

        //Available total balance for ECertificate.
        public static MvcHtmlString ECertTotalBalanceControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        {
            var eCertTotalBalanceWidgetData = WidgetHelper.GetECertTotalBalance(widgetparameter);

            return (HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity.IsAuthenticated &&
                    HelperUtility.IsNotNull(eCertTotalBalanceWidgetData))
                   //Only authorized users can see ECert widget
                   ? GetPartial(htmlHelper, partialViewName, eCertTotalBalanceWidgetData)
                   : MvcHtmlString.Empty;
        }

        /// <summary>
        /// Method to render Captcha for where IsCaptchaRequired global attribute is set true on store level.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="partialViewName"></param>
        /// <returns></returns>
        public static MvcHtmlString ZnodeCaptcha(this HtmlHelper htmlHelper, string partialViewName = "")
            => RenderCaptcha(htmlHelper, WebStoreConstants.CaptchaRequired, partialViewName);

        /// <summary>
        /// Method to render Captcha according to global attribute is set true on store level for captcha if captcha is for login page or any other. 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="attributeCode"></param>
        /// <param name="partialViewName"></param>
        /// <returns></returns>
        public static MvcHtmlString ZnodeCaptcha(this HtmlHelper htmlHelper, string attributeCode, string partialViewName = "")
                => RenderCaptcha(htmlHelper, attributeCode, partialViewName);

        /// <summary>
        /// Method to render case-sensitive Captcha according to global attribute value of IsCaptchaRequired attribute.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="partialViewName"></param>
        /// <returns></returns>
        public static MvcHtmlString ZnodeCaptchaWithCaseSensitive(this HtmlHelper htmlHelper, string partialViewName = "")
        {
            return RenderAndValidateCaptcha(htmlHelper, WebStoreConstants.CaptchaRequired, partialViewName);
        }


        /// <summary>
        /// Method to render case-sensitive Captcha according to global attribute value set for captcha attribute.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="attributeCode">Global attribute code</param>
        /// <param name="partialViewName">Partial view name of captcha</param>
        /// <returns></returns>
        public static MvcHtmlString ZnodeCaptchaWithCaseSensitive(this HtmlHelper htmlHelper, string attributeCode, string partialViewName = "")
        {
            return RenderAndValidateCaptcha(htmlHelper, attributeCode, partialViewName);
        }

        private static MvcHtmlString RenderAndValidateCaptcha(HtmlHelper htmlHelper, string attributeCode, string partialViewName)
        {
            if (Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, attributeCode, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue))
            {
                ValidateCaptchaCase();
                return GetPartial(htmlHelper, !string.IsNullOrEmpty(partialViewName) ? partialViewName : $"~/Views/Themes/{PortalAgent.CurrentPortal.Theme}/Views/Shared/_Captcha.cshtml", new object());
            }
            return null;
        }

        //Method to validate the captcha.
        public static void ValidateCaptchaCase()
        {
            //Logic to validate the captcha case.
            var captchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            captchaManager.PlainCaptchaPairFactory = length =>
            {
                string randomText = RandomText.Generate(captchaManager.CharactersFactory(), length);
                bool ignoreCase = false;
                return new KeyValuePair<string, ICaptchaValue>(Guid.NewGuid().ToString("N"),
                    new StringCaptchaValue(randomText, randomText, ignoreCase));
            };
        }

        //Render captcha according to attributeCode
        private static MvcHtmlString RenderCaptcha(HtmlHelper htmlHelper, string attributeCode, string partialViewName)
        {
            return Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, attributeCode, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue)
              ? GetPartial(htmlHelper, !string.IsNullOrEmpty(partialViewName) ? partialViewName : $"~/Views/Themes/{PortalAgent.CurrentPortal.Theme}/Views/Shared/_Captcha.cshtml", new object()) : null;
        }

        //Render content container based on the container Key.
        public static MvcHtmlString RenderContainer(this HtmlHelper htmlHelper, string containerKey)
         => GetPartial(htmlHelper, string.Empty, Helper.GetContentContainer(containerKey));

        //Render content container based on the container Key.
        public static MvcHtmlString RenderContainer(this HtmlHelper htmlHelper, string containerKey, string partialViewName)
        {
            if (!string.IsNullOrEmpty(containerKey))
                return GetPartial(htmlHelper, partialViewName, Helper.GetContentContainer(containerKey));
            else
                return null;
        }

        //Slider control widgets.
        public static MvcHtmlString ContainerControl(this HtmlHelper htmlHelper, string partialViewName, WidgetParameter widgetparameter)
        {
            if (!string.IsNullOrEmpty(partialViewName))
                return RenderContainer(htmlHelper, WidgetHelper.GetContainer(widgetparameter), widgetparameter.PartialViewName);
           return GetPartial(htmlHelper, partialViewName, WidgetHelper.GetContainer(widgetparameter));
        }

        //Get media details by media Id.
        public static MvcHtmlString DownloadMediaByMediaId(this HtmlHelper htmlHelper, int mediaId, string partialViewName = "_DownloadMediaButton")
        {
            if (mediaId > 0)
            {
                MediaViewModel mediaViewModel = new MediaViewModel();
                mediaViewModel.MediaId = mediaId;
                return GetPartial(htmlHelper, partialViewName, mediaViewModel);
            }
            else
                return null;
        }

        //Get media details by media Guid.
        public static MvcHtmlString DownloadMediaByMediaGuid(this HtmlHelper htmlHelper, string mediaGuid, string partialViewName = "_DownloadMediaButton")
        {
            mediaGuid = Znode.Engine.WebStore.Helper.GetGuidByMediaUrl(mediaGuid);
            if (!string.IsNullOrEmpty(mediaGuid))
            {
                MediaViewModel mediaViewModel = new MediaViewModel();
                mediaViewModel.MediaPath = mediaGuid;
                return GetPartial(htmlHelper, partialViewName, mediaViewModel);
            }
            else
                return null;
        }
    }
}