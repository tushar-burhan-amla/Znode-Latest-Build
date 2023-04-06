using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore
{
    public class ThemedViewEngine : RazorViewEngine
    {
        public ThemedViewEngine()
        {
            ViewLocationFormats = new[]
                        {
                            "~/Views/Themes/{2}/Views/{1}/{0}.cshtml",
                            "~/Views/Themes/{1}/{0}.cshtml",
                            "~/Views/Themes/{2}/DynamicGrid/{0}.cshtml",
                            "~/Views/Themes/{2}/Views/Shared/{0}.cshtml",
                            "~/Views/Themes/{2}/Views/Shared/PDP Template/{0}.cshtml",
                            "~/Views/Themes/Templates/{0}.cshtml",
                            "~/Views/Themes/{2}/Views/Product/{0}.cshtml",
                            "~/Views/Themes/{2}/Views/Category/{0}.cshtml",
        };

            PartialViewLocationFormats =
                new[]
                    {
                        "~/Views/Themes/{2}/Views/Shared/{0}.cshtml",
                        "~/Views/Themes/{2}/Views/Product/{0}.cshtml",
                         "~/Views/Themes/{2}/Views/Category/{0}.cshtml",
                         "~/Views/Themes/{2}/Views/Checkout/{0}.cshtml",
                         "~/Views/Themes/{2}/Views/User/{0}.cshtml",
                        "~/Views/Themes/{2}/Views/Shared/Widget/{0}.cshtml",
                        "~/Views/Themes/{2}/Views/Shared/Widget/WidgetPartial/{0}.cshtml",
                         "~/Views/Themes/{2}/Views/Shared/PDP Template/{0}.cshtml",
                        "~/Views/Themes/{2}/Views/{1}/{0}.cshtml",
                        "~/Views/Themes/{2}/DynamicGrid/{0}.cshtml",
                    };
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            try
            {
                return File.Exists(controllerContext.HttpContext.Server.MapPath(virtualPath));
            }
            catch (HttpException exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                if (exception.GetHttpCode() != 0x194)
                {
                    throw;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            string[] strArray;
            string[] strArray2;

            if (Equals(controllerContext, null))
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException("View Name must be specified.", "viewName");
            }

            string themeName = GetThemeToUse();

            string parentThemeName = GetParentThemeToUse();

            string requiredString = controllerContext.RouteData.GetRequiredString("controller");

            string viewPath = GetPath(controllerContext, ViewLocationFormats, "ViewLocationFormats", viewName, themeName, parentThemeName, requiredString, "View", useCache, out strArray);
            string masterPath = GetPath(controllerContext, MasterLocationFormats, "MasterLocationFormats", masterName, themeName, parentThemeName, requiredString, "Master", useCache, out strArray2);

            if (!string.IsNullOrEmpty(viewPath) && (!string.IsNullOrEmpty(masterPath) || string.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
            }
            if (!Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(WebStoreConstants.ThemeFolderPath), themeName)) && string.IsNullOrEmpty(viewPath))
                throw new FileNotFoundException ("ThemeMissing", themeName);

            strArray2 = new string[0];
            return new ViewEngineResult(strArray.Union(strArray2));
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            string[] strArray;
            if (Equals(controllerContext, null))
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException("Partial View Name must be specified.", "partialViewName");
            }

            string themeName = GetThemeToUse();
            string parentThemeName = GetParentThemeToUse();

            string requiredString = controllerContext.RouteData.GetRequiredString("controller");
            string partialViewPath = GetPath(controllerContext, PartialViewLocationFormats, "PartialViewLocationFormats", partialViewName, themeName, parentThemeName, requiredString, "Partial", useCache, out strArray);
            return string.IsNullOrEmpty(partialViewPath) ? new ViewEngineResult(strArray) : new ViewEngineResult(CreatePartialView(controllerContext, partialViewPath), this);
        }

        private static string GetThemeToUse()
            => PortalAgent.CurrentPortal.Theme ?? "Default";

        private static string GetParentThemeToUse()
            => PortalAgent.CurrentPortal.ParentTheme;

        private static readonly string[] _emptyLocations;

        private string GetPath(ControllerContext controllerContext, string[] locations, string locationsPropertyName, string name, string themeName, string parentThemeName, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
        {
            searchedLocations = _emptyLocations;
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            if (Equals(locations, null) || (locations.Length.Equals(0)))
            {
                throw new InvalidOperationException("locations must not be null or empty.");
            }

            bool flag = IsSpecificPath(name);
            string key = CreateCacheKey(cacheKeyPrefix, name, flag ? string.Empty : controllerName, themeName);
            if (useCache)
            {
                string viewLocation = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, key);
                if (!Equals(viewLocation, null))
                {
                    return viewLocation;
                }
            }
            return !flag ? GetPathFromGeneralName(controllerContext, locations, name, controllerName, themeName, parentThemeName, key, ref searchedLocations)
                        : GetPathFromSpecificName(controllerContext, name, key, ref searchedLocations);
        }

        private static bool IsSpecificPath(string name)
        {
            char ch = name[0];
            if (!Equals(ch, '~'))
            {
                return (ch == '/');
            }
            return true;
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string themeName)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}",
                new object[] { GetType().AssemblyQualifiedName, prefix, name, controllerName, themeName });
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, string[] locations, string name, string controllerName, string themeName, string parentThemeName, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = string.Empty;
            searchedLocations = new string[(locations.Length * 2)];
            for (int i = 0; i < locations.Length; i++)
            {
                string fileLocation = string.Format(CultureInfo.InvariantCulture, locations[i], new object[] { name, controllerName, themeName });

                if (FileExists(controllerContext, fileLocation))
                {
                    searchedLocations = _emptyLocations;
                    virtualPath = fileLocation;
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
                    return virtualPath;
                }

                searchedLocations[(2 * i)] = fileLocation;

                fileLocation = string.Format(CultureInfo.InvariantCulture, locations[i], new object[] { name, controllerName, parentThemeName });

                if (FileExists(controllerContext, fileLocation))
                {
                    searchedLocations = _emptyLocations;
                    virtualPath = fileLocation;
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
                    return virtualPath;
                }

                searchedLocations[(2 * i) + 1] = fileLocation;
            }
            return virtualPath;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = name;
            if (!FileExists(controllerContext, name))
            {
                virtualPath = string.Empty;
                searchedLocations = new[] { name };
            }
            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
            return virtualPath;
        }
    }
}