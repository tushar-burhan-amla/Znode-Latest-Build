using System.Web.Optimization;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/References/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/cookies").Include(
                        "~/Scripts/References/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/References/jquery.validate.js",
                        "~/Scripts/References/jquery.validate.unobtrusive.js",
                        "~/Scripts/References/DynamicValidation.js",
                        "~/Scripts/References/typeahead.bundle.js",
                        "~/Scripts/References/typeahead.bundle.orig.js",
                        "~/Scripts/References/typeahead.mvc.model.js",
                        "~/Scripts/References/fastselect.mvc.model.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                        "~/Scripts/References/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/loginpagevalidate").Include(
                    "~/Scripts/References/jquery-3.3.1.min.js",
                    "~/Scripts/References/jquery.validate.js",
                    "~/Scripts/References/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/loginpage").Include(
                      "~/Content/bootstrap-3.3.7/js/bootstrap.js",
                      "~/Scripts/Core/Model/Znode.Model.js",
                      "~/Scripts/Core/Common/ZnodeGlobal.js",
                      "~/Scripts/Core/Znode/ZnodeNotification.js",
                      "~/Scripts/Core/Endpoint/ZnodeEndpoint.js",
                      "~/Scripts/Core/Znode/Login.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/References/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Content/bootstrap-3.3.7/js/bootstrap.js",
                     "~/Content/bootstrap-3.3.7/js/datepicker.js",
                     "~/Content/bootstrap-3.3.7/js/jquery-ui-custom.js",
                     "~/Content/bootstrap-3.3.7/js/jquery.dirtyforms.js",
                     "~/Scripts/References/respond.js",
                     "~/Scripts/References/underscore-min.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/JqueryUI").Include(
               "~/Scripts/References/jquery-ui-3.3.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/Core").IncludeDirectory(
                "~/Scripts/Core", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Custom").IncludeDirectory(
               "~/Scripts/Custom", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Controls").IncludeDirectory(
              "~/Scripts/Core/Controls", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/JsTree").Include(
               "~/Scripts/References/JsTree/jstree.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/XmlGenerator")
                .Include("~/Scripts/References/WebGrid/xmlGenerator.js"));

            bundles.Add(new ScriptBundle("~/bundles/MediaUpload")
                .Include("~/Scripts/References/MediaUpload/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/TinymceJs")
               .Include("~/Scripts/tinymce/tinymce.js")
               .Include("~/Scripts/tinymce/inittinymce.js")
               .Include("~/Scripts/tinymce/TinymceCustom.js"));

            bundles.Add(new ScriptBundle("~/bundles/ClientResource").IncludeDirectory(
             "~/Scripts/ClientResource", "*.js", true));


            bundles.Add(new ScriptBundle("~/bundles/Multisortable").Include(
               "~/Scripts/References/Multisortable/jquery-ui.min.js",
               "~/Scripts/References/Multisortable/multisortable.js"));

            bundles.Add(new ScriptBundle("~/bundles/DateTimeRangeFilter").Include(
               "~/Scripts/Core/Controls/DateTimeRangePicker/moment.min.js",
               "~/Scripts/Core/Controls/DateTimeRangePicker/daterangepicker.min.js",
               "~/Scripts/Core/Controls/DropdownImage.js"));

           #endregion

            #region CSS

            bundles.Add(new StyleBundle("~/content/css/AllCss").Include(
                       "~/Content/bootstrap-3.3.7/css/bootstrap.css",
                       "~/Content/bootstrap-3.3.7/css/jquery-ui.css",
                       "~/Content/bootstrap-3.3.7/css/bootstrap-theme.css",
                       "~/Content/css/site.css",
                       "~/Content/css/bootstrap-select.css"));


            #endregion

            BundleTable.EnableOptimizations = ZnodeAdminSettings.EnableScriptOptimizations;

        }
    }
}
