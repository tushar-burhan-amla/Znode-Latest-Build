using System.Web.Optimization;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                   "~/Scripts/lib/jquery-3.3.1.js",
                   "~/Scripts/lib/jquery-ui.js",
                   "~/Scripts/lib/jquery.validate.js",
                   "~/Scripts/lib/jquery.validate.unobtrusive.js",
                     "~/Scripts/lib/DynamicValidation.js",
                     "~/Scripts/lib/jquery.unobtrusive-ajax.js",
                     "~/Scripts/lib/closest.js",
                     "~/Scripts/lib/blazy.js",
                     "~/Scripts/lib/datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/CoreJs").Include(
                     "~/Scripts/Core/Model/Znode.Model.js",
                     "~/Scripts/Core/Common/ZnodeGlobal.js",
                     "~/Scripts/Core/Common/ZnodeHelper.js",
                     "~/Scripts/Core/Common/ZnodeRegExHelper.js",
                     "~/Scripts/Core/Endpoint/ZnodeEndpoint.js",
                     "~/Scripts/Core/Controls/WebGrid/DynamicGrid.js",
                     "~/Scripts/Core/Controls/WebGrid/CustomJurl.js",
                     "~/Scripts/Core/Controls/WebGrid/GridPager.js",
                      "~/Scripts/Core/Controls/WebGrid/EditableDynamicGrid.js",
                       "~/Scripts/Core/Controls/WebGrid/jurl.js",
                        "~/Scripts/Core/Controls/WebGrid/jurl.min.js",
                        "~/Scripts/References/WebGrid/xmlGenerator.js",
                         "~/Scripts/Core/Controls/ZnodeMultiSelect.js",
                         "~/Scripts/Core/Controls/TextBoxEditorFor.js",
                         "~/Scripts/Core/Controls/ZnodeAjaxify.js",
                         "~/Scripts/ClientResource/Resource.en.js",
              "~/Scripts/ClientResource/Resource.de.js",
              "~/Scripts/ClientResource/Resource.fr.js"));

            bundles.Add(new ScriptBundle("~/bundles/validation").Include(
              "~/Scripts/lib/jquery.validate.js",
              "~/Scripts/lib/jquery.validate.unobtrusive.js",
                "~/Scripts/lib/DynamicValidation.js"));

            bundles.Add(new ScriptBundle("~/bundles/ZnodeCoreJs").Include(
                                    "~/Scripts/Core/Znode/Newsletter.js",
                                    "~/Scripts/Core/Znode/Product.js",
                                   "~/Scripts/Core/Znode/Category.js",
                                   "~/Scripts/Core/Znode/QuickOrder.js",
                                   "~/Scripts/Core/Znode/QuickOrderPad.js",
                                   "~/Scripts/Core/Znode/Search.js",
                                   "~/Scripts/Core/Znode/ZSearch.js",
                                   "~/Scripts/Core/Znode/Cart.js",
                                    "~/Scripts/Core/Znode/User.js",
                                     "~/Scripts/Core/Znode/ZnodeNotification.js",
                                     "~/Scripts/Core/Znode/Search.js",
                                     "~/Scripts/Core/Znode/Brand.js",
                                     "~/Scripts/Core/Znode/Checkout.js",
                                     "~/Scripts/Core/Znode/CaseRequest.js",
                                     "~/Scripts/Core/Znode/Home.js",
                                     "~/Scripts/Core/Znode/Config.js",
                                     "~/Scripts/Core/Znode/SiteMap.js",
                                     "~/Scripts/Core/Znode/StoreLocator.js",
                                     "~/Scripts/Core/Znode/Import.js",
                                     "~/Scripts/lib/typeahead.bundle.js",
                                     "~/Scripts/lib/typeahead.bundle.orig.js",
                                     "~/Scripts/lib/typeahead.mvc.model.js",
                                        "~/Scripts/lib/jquery.payment.min.js",
                                     "~/Scripts/Core/Znode/FormBuilder.js",
                                     "~/Scripts/Core/Znode/BlogNews.js",
                                      "~/Scripts/Core/Znode/ContentPage.js",
                                     "~/Scripts/BarcodeScanner/BarcodeReader.js",
                                     "~/Scripts/VoiceRecognition/VoiceRecognition.js",
                                     "~/Scripts/Core/Znode/Quote.js",
                                     "~/Scripts/Core/Znode/RMAReturn.js",
                                     "~/Scripts/Core/Znode/GoogleAnalytics.js",
                                     "~/Scripts/Core/Znode/ConfigurableProduct.js"
                                     ));

            #region Checkout
            bundles.Add(new ScriptBundle("~/bundles/CheckoutCoreJs").Include(
                       "~/Scripts/Core/Model/Znode.Model.js",
                       "~/Scripts/Core/Common/ZnodeGlobal.js",
                       "~/Scripts/Core/Common/ZnodeHelper.js",
                       "~/Scripts/Core/Endpoint/ZnodeEndpoint.js",
                       "~/Scripts/ClientResource/Resource.en.js",
                       "~/Scripts/ClientResource/Resource.de.js",
                       "~/Scripts/ClientResource/Resource.fr.js"));


            bundles.Add(new ScriptBundle("~/bundles/ZnodeCheckoutCustomJs").Include(
                                   "~/Scripts/Core/Znode/Product.js",
                                   "~/Scripts/Core/Znode/Cart.js",
                                   "~/Scripts/Core/Znode/User.js",
                                   "~/Scripts/Core/Znode/ZnodeNotification.js",
                                   "~/Scripts/Core/Znode/Checkout.js",
                                   "~/Scripts/Core/Znode/Quote.js"
                                    ));
            #endregion

            BundleTable.EnableOptimizations = ZnodeWebstoreSettings.EnableScriptOptimizations; 
        }
    }
}