using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;

using Znode.Engine.WebStore;
using Znode.Libraries.ECommerce.Utilities;

namespace System.Web.Mvc.Ajax
{
	public static class AjaxExtensions
	{
		private static ThemedViewEngine engine = new ThemedViewEngine();

		#region Public Methods

		//Get Widgets control.
		public static IHtmlString AjaxWidgetPartial(this AjaxHelper ajaxHelper, string widgetCode, string displayName, string widgetKey, string typeOfMapping, int mappingId = 0, Dictionary<string, object> properties = null)
		{
			ControllerContext controllerContext = ajaxHelper.ViewContext.Controller.ControllerContext;
			ViewEngineResult viewResult = engine.FindPartialView(controllerContext, $"_Widget{widgetCode}", true);

			using (WidgetParameter prm = new WidgetParameter())
			{
				prm.WidgetCode = widgetCode;
				prm.WidgetKey = widgetKey;
				prm.TypeOfMapping = typeOfMapping;
				prm.CMSMappingId = mappingId;
				prm.DisplayName = displayName;
				prm.properties = properties;
				return GetPartial(ajaxHelper, viewResult, controllerContext, prm);
			}
		}

		/// <summary>
		/// /// This method returns a partial view place holder which can then be utilized by an ajax request to dump its html response.
		/// </summary>
		/// <param name="ajaxHelper"></param>
		/// <param name="identifier">A unique name to identify this placeholder within your client side scripts.</param>
		/// <param name="actionName"></param>
		/// <param name="controllerName"></param>
		/// <param name="parameters"></param>
		/// <param name="replaceTargetSelector">Any selector supported by jQuery. This parameter is optional.</param>
		/// <returns></returns>
		public static IHtmlString Partial(this AjaxHelper ajaxHelper, string identifier, string actionName, string controllerName, object parameters, string replaceTargetSelector = null)
		{
			ControllerContext controllerContext = ajaxHelper.ViewContext.Controller.ControllerContext;
			ViewEngineResult viewResult = engine.FindPartialView(controllerContext, $"_AjaxPartial", true);

			string parentView = CurrentViewName(ajaxHelper);

			using (AjaxPartialParameter prm = new AjaxPartialParameter())
			{
				prm.ActionName = actionName;
				prm.ControllerName = controllerName;
				prm.Parameters = JsonConvert.SerializeObject(parameters);
				prm.Identifier = $"{parentView}_{identifier}";
				prm.ReplaceTargetSelector = replaceTargetSelector;
				return GetPartial(ajaxHelper, viewResult, controllerContext, prm);
			}
		}
		
		#endregion

		#region Public Control

		//Available total balance for ECertificate.
		public static IHtmlString ECertTotalBalanceAjaxControl(this AjaxHelper ajaxHelper, string partialViewName, WidgetParameter widgetparameter)
        {
            //Get total balance amount only if specified conditions fulfilled.
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Only authorized users can see ECert widget
                var eCertTotalBalanceWidgetData = WidgetHelper.GetECertTotalBalance(widgetparameter);

                return (HelperUtility.IsNotNull(eCertTotalBalanceWidgetData) 
                    && eCertTotalBalanceWidgetData.AvailableTotal > 0)
                       //If connecter is active
                       //If connecter is connected
                       //If available total is non-zero
                       ? GetPartial(ajaxHelper, partialViewName, eCertTotalBalanceWidgetData)
                       : MvcHtmlString.Empty;
            }
            else return MvcHtmlString.Empty;
        }

        //Available total balance for ECertificate to show on user dashboard.
        public static IHtmlString ECertUserProfileBalanceAjaxControl(this AjaxHelper ajaxHelper, string partialViewName, WidgetParameter widgetparameter, decimal availableBalance = 0)
        {
            //Get total associated balance amount regardless of any condition.
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Only authorized users can see ECert widget
                var eCertTotalBalanceWidgetData = WidgetHelper.GetECertTotalBalance(widgetparameter, availableBalance );

                return (HelperUtility.IsNotNull(eCertTotalBalanceWidgetData))
                       ? GetPartial(ajaxHelper, partialViewName, eCertTotalBalanceWidgetData)
                       : MvcHtmlString.Empty;
            }
            else return MvcHtmlString.Empty;
        }

		#endregion

		#region Private Methods

		//Get razor view's name for the supplied AjaxHelper.
		private static string CurrentViewName(this AjaxHelper ajax)
		{
			string viewName;
			if(ajax.ViewContext.View is RazorView)
			{
				viewName = ((RazorView)ajax.ViewContext.View).ViewPath;
				return !string.IsNullOrEmpty(viewName) ? Path.GetFileNameWithoutExtension(viewName) : string.Empty;
			}

			return string.Empty;
		}

		//Get Partial view of widgets.
		private static IHtmlString GetPartial(AjaxHelper ajaxHelper, string partialViewName, object data = null)
        {
            ControllerContext controllerContext = ajaxHelper.ViewContext.Controller.ControllerContext;
            ViewEngineResult viewResult = engine.FindPartialView(controllerContext, partialViewName, true);
            return BuildAjaxPartial(ajaxHelper, controllerContext, viewResult, data);
        }

        //Get Partial view of widgets.
        private static IHtmlString GetPartial(AjaxHelper ajaxHelper, ViewEngineResult viewResult, ControllerContext controllerContext, object data = null)
        => BuildAjaxPartial(ajaxHelper, controllerContext, viewResult, data);

        //Find and build partial view for widgets render.
        private static IHtmlString BuildAjaxPartial(AjaxHelper ajaxHelper, ControllerContext controllerContext, ViewEngineResult viewResult, object data = null)
        {
            ViewDataDictionary viewData = new ViewDataDictionary(data);
            TempDataDictionary tempData = ajaxHelper.ViewContext.TempData;
            using (StringWriter stringWriter = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, stringWriter);

                viewResult.View.Render(viewContext, stringWriter);

                return MvcHtmlString.Create(stringWriter.GetStringBuilder().ToString());
            }
        }

        #endregion



    }
}
