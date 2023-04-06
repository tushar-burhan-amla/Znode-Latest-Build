using Newtonsoft.Json;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.WebStore
{
    public class SEOUrlRouteData : Route,ISEOUrlRouteData
    {

        #region constructor
        /// <summary>
        /// Initializes a new instance, using the specified URL pattern and handler class.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public SEOUrlRouteData(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }
        /// <summary>
        /// Initializes a new instance, using the specified URL pattern, handler class and default parameter values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public SEOUrlRouteData(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns modified information about the route.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData routeData = base.GetRouteData(httpContext);
            if (Equals(routeData, null) || Equals(routeData.Values.ContainsKey("slug"), false))
            {
                return null;
            }
            string currentSlug = routeData.Values["slug"].ToString();

            //For product edit.
            string parentOmsSavedCartLineItemId;
            int value;
            ProductEditRouteValue(httpContext, out parentOmsSavedCartLineItemId, out value);

            //if currentSlug found
            if (!string.IsNullOrEmpty(currentSlug))
            {
                string[] SEOSlugs = ZnodeWebstoreSettings.SEOSlugToSkip.Split(',');

                if (SEOSlugs.Contains(currentSlug.Split('/')[0].ToLower()) || currentSlug.Contains("no-image.png"))
                    routeData = null;
                else
                {
                    IWebstoreHelper helper = GetService<IWebstoreHelper>();
                    try
                    {
                        //Get the Seo Url Details.
                        SEOUrlViewModel model = helper.GetSeoUrlDetail(currentSlug);

                        //Log Urls calling GetSeoUrlDetail helper
                        ZnodeLogging.LogMessage($"GetSeoUrlDetail - SEO Slug is : {currentSlug}", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

                        if (!string.IsNullOrEmpty(model.Name))
                        {
                            if (model.IsActive)
                            {
                                switch (model.Name?.ToLower())
                                {
                                    case "product":
                                        if (value > 0)
                                        {
                                            routeData.Values["controller"] = "Product";
                                            routeData.Values["action"] = "Details";
                                            routeData.Values["id"] = model.SEOId;
                                            routeData.Values[parentOmsSavedCartLineItemId] = value;
                                            routeData.Values["seo"] = model.SeoCode;
                                        }
                                        else
                                        {
                                            routeData.Values["controller"] = "Product";
                                            routeData.Values["action"] = "Details";
                                            routeData.Values["id"] = model.SEOId;
                                            routeData.Values["seo"] = model.SeoCode;
                                        }
                                        break;
                                    case "category":
                                        routeData.Values["controller"] = "Category";
                                        routeData.Values["action"] = "Index";
                                        routeData.Values["category"] = model.CategoryName;
                                        routeData.Values["categoryId"] = model.SEOId;                                       
                                        routeData.Values["seodata"]=JsonConvert.SerializeObject(model);
                                        break;
                                    case "content page":
                                        routeData.Values["controller"] = "ContentPage";
                                        routeData.Values["action"] = "ContentPage";
                                        routeData.Values["contentPageId"] = model.SEOId;
                                        break;
                                    case "brand":
                                        routeData.Values["controller"] = "Brand";
                                        routeData.Values["action"] = "Index";
                                        routeData.Values["brand"] = model.BrandName;
                                        routeData.Values["brandId"] = model.SEOId;
                                        break;
                                    case "blognews":
                                        routeData.Values["controller"] = "BlogNews";
                                        routeData.Values["action"] = "Index";
                                        routeData.Values["blogNewsId"] = model.SEOId;
                                        break;
                                    default:
                                        routeData.Values["controller"] = "Home";
                                        routeData.Values["action"] = "Index";
                                        break;

                                }

                            }
                            else
                            {
                                routeData.Values["controller"] = "ErrorPage";
                                routeData.Values["action"] = "PageNotFound";
                            }
                        }
                        else
                            routeData = null;
                    }
                    catch
                    {
                        routeData = null;
                    }
                }
            }
            else
            {
                routeData.Values["controller"] = "Home";
                routeData.Values["action"] = "Index";
            }
            return routeData;
        }

        //Extract the parameter value (ParentOmsSavedCartLineItemId) in case of product edit mode.
        private static void ProductEditRouteValue(HttpContextBase httpContext, out string parentOmsSavedCartLineItemId, out int value)
        {
            parentOmsSavedCartLineItemId = httpContext?.Request?.QueryString?.AllKeys?.Where(x => x == "parentOmsSavedCartLineItemId")?.FirstOrDefault();
            value = 0;
            if (!string.IsNullOrEmpty(parentOmsSavedCartLineItemId))
                int.TryParse(httpContext.Request.QueryString[parentOmsSavedCartLineItemId].ToString(), out value);
        }
        #endregion

    }
}