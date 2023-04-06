using System.IO;
using System.Linq;
using System.Web.Mvc.Ajax;

using Znode.Engine.Admin.Helpers;

namespace System.Web.Mvc
{
    public static class HtmlHelpersExtension
    {
        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes = null)
        {
            var repID = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
        }
        public static MvcHtmlString AuthorizedRawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes = null)
        {
            if (IsLinkAuthorized(controllerName, actionName))
            {
                var repID = Guid.NewGuid().ToString();
                var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
                return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString RawActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes = null)
        {
            var repID = Guid.NewGuid().ToString();
            var lnk = Html.LinkExtensions.ActionLink(htmlHelper, repID, actionName, controllerName, routeValues, htmlAttributes);
            return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
        }

        public static MvcHtmlString AuthorizedRawActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes = null)
        {
            if (IsLinkAuthorized(controllerName, actionName))
            {
                var repID = Guid.NewGuid().ToString();
                var lnk = HttpContext.Current.Server.UrlDecode(Html.LinkExtensions.ActionLink(htmlHelper, repID, actionName, controllerName, routeValues, htmlAttributes).ToString());
                return MvcHtmlString.Create(lnk.Replace(repID, linkText));
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString PrototypeRawActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string cssClassName, string clickElement)
            => MvcHtmlString.Create($"<a class='{ cssClassName }' onclick='{ clickElement }'>{ linkText}</a>");

        public static MvcHtmlString AuthorizedPrototypeRawActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string cssClassName, string clickElement)
        {
            if (IsLinkAuthorized(controllerName, actionName))
            {
                return MvcHtmlString.Create($"<a class='{ cssClassName }' onclick='{ clickElement }'>{ linkText}</a>");
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString AuthorizedPrototypeRawActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object htmlAttributes = null)
        {
            if (IsLinkAuthorized(controllerName, actionName))
            {
                var attrbituesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

                string strHtmlAttributes = String.Join(" ",
                    attrbituesDictionary.Select(
                        item => $"{item.Key}=\"{htmlHelper.Encode(item.Value)}\""));

                return MvcHtmlString.Create($"<a {strHtmlAttributes}>{ linkText}</a>");
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString WidgetPartial(this HtmlHelper htmlHelper, string widgetCode, string displayName, string mappingKey, string partialViewName, int mappingId = 0)
        {
            ControllerContext controllerContext = htmlHelper.ViewContext.Controller.ControllerContext;
            ViewDataDictionary viewData = htmlHelper.ViewData;
            TempDataDictionary tempData = htmlHelper.ViewContext.TempData;

            using (StringWriter stringWriter = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);

                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, stringWriter);

                viewResult.View.Render(viewContext, stringWriter);

                return MvcHtmlString.Create(stringWriter.GetStringBuilder().ToString());
            }
        }

        public static MvcHtmlString GlobalAttributeActionLink(this HtmlHelper htmlHelper, string linkText, int mappingKey, string mappingtype, object htmlAttributes = null)
        {
            string controllerName = "GlobalAttribute";
            string actionName = "GetEntityAttributeDetails";
            object routeValues = new { @entityId = mappingKey, @entityType = mappingtype };
            if (IsLinkAuthorized(controllerName, actionName))
            {
                var repID = Guid.NewGuid().ToString();
                var lnk = HttpContext.Current.Server.UrlDecode(Html.LinkExtensions.ActionLink(htmlHelper, repID, actionName, controllerName, routeValues, htmlAttributes).ToString());
                return MvcHtmlString.Create(lnk.Replace(repID, linkText));
            }
            return MvcHtmlString.Empty;
        }


        //Check whether the user has rights of access the provided controller & Action.
        private static bool IsLinkAuthorized(string controllerName, string actionName)
        {
            if (SessionProxyHelper.IsAdminUser())
                return true;

            string permissionKey = $"{ controllerName}/{actionName}";
            var lstPermissions = SessionProxyHelper.GetUserPermission();
            return Equals(lstPermissions, null) ? false : lstPermissions.FindIndex(w => w.RequestUrlTemplate.Equals(permissionKey)) != -1;

        }

        public static MvcHtmlString Video(this HtmlHelper htmlHelper, string url)
        => MvcHtmlString.Create(HttpUtility.HtmlDecode(HelperMethods.GetVideoTag(url)));

        public static MvcHtmlString Audio(this HtmlHelper htmlHelper, string url)
       => MvcHtmlString.Create(HttpUtility.HtmlDecode(HelperMethods.GetAudioTag(url)));

        public static MvcHtmlString RenderBlock(this HtmlHelper htmlHelper, string content)
       => MvcHtmlString.Create(HttpUtility.HtmlDecode(content));

        //Render Message by message key.
        public static MvcHtmlString RenderBlockEncoded(this HtmlHelper htmlHelper, string content)
        => MvcHtmlString.Create(HttpUtility.HtmlEncode(content));

        //Renders date time range picker html.
        public static MvcHtmlString DateTimeRangePicker(this HtmlHelper htmlHelper, object model)
        => RenderRazorViewToString(htmlHelper, "~/Views/Shared/Controls/_DateTimeRangePicker.cshtml", model);

        //Converts partial view result to MvcHtmlString.
        public static MvcHtmlString RenderRazorViewToString(this HtmlHelper htmlHelper, string partialViewName, object model)
        {
            ControllerContext controllerContext = htmlHelper.ViewContext.Controller.ControllerContext;
            ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);
            ViewDataDictionary viewData = new ViewDataDictionary(model);
            TempDataDictionary tempData = htmlHelper.ViewContext.TempData;
            using (StringWriter stringWriter = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, stringWriter);

                viewResult.View.Render(viewContext, stringWriter);

                return MvcHtmlString.Create(stringWriter.GetStringBuilder().ToString());
            }
        }

        //Check whether the user has rights of access the provided controller & Action.
        public static bool IsAuthorized(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            if (SessionProxyHelper.IsAdminUser())
                return true;

            string permissionKey = $"{ controllerName}/{actionName}";
            var lstPermissions = SessionProxyHelper.GetUserPermission();
            return Equals(lstPermissions, null) ? false : lstPermissions.FindIndex(w => w.RequestUrlTemplate.Equals(permissionKey)) != -1;
        }
    }
}