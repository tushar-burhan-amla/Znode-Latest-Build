using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.Resources.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using System.Linq.Expressions;
using Microsoft.Web.Mvc;
using Znode.Engine.WebStore.Models;
using Newtonsoft.Json;
using Znode.Engine.Exceptions;
using Znode.Libraries.Resources;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using System.Globalization;

namespace Znode.Engine.WebStore.Controllers
{
    public class BaseController : Controller
    {
        #region Private Methods
        private PortalViewModel _currentPortal;
        private PortalViewModel currentPortal
        {
            get
            {
                if (HelperUtility.IsNull(_currentPortal))
                {
                    _currentPortal = PortalAgent.CurrentPortal;
                }
                return _currentPortal;
            }
        }
        #endregion

        #region Protected Methods
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (HelperUtility.IsNotNull(currentPortal))
            {
                ViewBag.Title = currentPortal.WebsiteTitle;
                if (Directory.Exists(Server.MapPath("/Views/Themes/" + currentPortal.Theme)))
                    ViewBag.Theme = currentPortal.Theme;
                else
                    ViewBag.Theme = string.Empty;
                if (Directory.Exists(Server.MapPath("/Views/Themes/" + currentPortal.Theme + "/Content/css/" + currentPortal.Css?.ToLower())))
                    ViewBag.CSS = currentPortal.Css;
                else
                {
                    ViewBag.CSS = string.Empty;
                }
                ViewBag.PortalID = currentPortal.PortalId;
                ViewBag.DefaultMediaPath = currentPortal.MediaServerUrl;
                ViewBag.DefaultThumbnailImagePath = currentPortal.MediaServerThumbnailUrl;
            }
            this.ControllerContext = new ControllerContext(requestContext, this);
        }
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //Get culture name cookies 
            string LocaleId = string.IsNullOrEmpty(Request.Cookies["_WebStoreculture"]?.Value) ? Convert.ToString(currentPortal?.Locales?.FirstOrDefault(x => x.IsDefault)?.LocaleId) : Request.Cookies["_WebStoreculture"].Value;
           
            IPortalAgent _portalAgent = ZnodeDependencyResolver.GetService<IPortalAgent>();
            HttpCookie cultureCookie = CookieHelper.GetCookie("_WebStoreculture");           
            if (cultureCookie == null)
            {                
                _portalAgent.ChangeLocale(DefaultSettingHelper.DefaultLocale);
                LocaleId = Convert.ToString(PortalAgent.LocaleId);                
            }
                
            if (string.IsNullOrEmpty(LocaleId) || Convert.ToInt32(LocaleId) <= 0)
            {
                var _locales = currentPortal.Locales?.Where(x => x.IsDefault == true);
                LocaleId = _locales.Count() > 0 ? _locales.FirstOrDefault().LocaleId.ToString() : currentPortal.Locales.FirstOrDefault().LocaleId.ToString();
                _portalAgent.ChangeLocale(LocaleId);
                LocaleId = Convert.ToString(PortalAgent.LocaleId);
            }
           
            CultureHelper.SetConfigurationSettingSource(currentPortal.Locales?.Where(x => x.LocaleId == Convert.ToInt32(LocaleId)).Select(x => x.Code).ToList());

            // Validate culture name
            string cultureName = CultureHelper.GetImplementedCulture(LocaleId); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            // Set cookie for selected locale culture name
            CookieHelper.SetCookie("culture", CultureInfo.CurrentCulture.TwoLetterISOLanguageName, isCookieHttpOnly: false);

            return base.BeginExecuteCore(callback, state);
        }

        /// <summary>
        /// Strongly Type Redirect To Action
        /// </summary>
        /// <typeparam name="TController">Controller Name</typeparam>
        /// <param name="action">Action Name</param>
        /// <returns>Strongly Type Action Result</returns>
        /// <example>
        /// If your controller name is "Dashboard" and Action Mehtod name is "Dashboard"
        /// Then we can redirect to action method using strongly type as
        /// <code>
        /// RedirectToAction<DashboardController>(o => o.Index())
        /// </code>
        /// </example>
        protected ActionResult RedirectToAction<TController>(
                Expression<Action<TController>> action)
                where TController : Controller
        {
            return ControllerExtensions.RedirectToAction(this, action);
        }


        /// <summary>
        /// Get the success notification message.
        /// </summary>
        /// <param name="successMessage">success message.</param>
        /// <returns>Returns serialize MessageBoxModel with notification set to success.</returns>
        protected string GetSuccessNotificationMessage(string successMessage)
            => GenerateNotificationMessages(successMessage, NotificationType.success);

        /// <summary>
        /// Get the error notification message.
        /// </summary>
        /// <param name="errorMessage">error message.</param>
        /// <returns>Returns serialize MessageBoxModel with notification set to error.</returns>
        protected string GetErrorNotificationMessage(string errorMessage)
            => GenerateNotificationMessages(errorMessage, NotificationType.error);

        /// <summary>
        /// Set notification message.
        /// </summary>
        /// <param name="notificationMessage">Message to set.</param>
        protected void SetNotificationMessage(string notificationMessage)
            => TempData[WebStoreConstants.Notifications] = notificationMessage;


        /// <summary>
        /// To show Notification message 
        /// </summary>
        /// <param name="message">string message to show on page</param>
        /// <param name="type">enum type of message</param>
        /// <param name="isFadeOut">bool isFadeOut true/false</param>
        /// <param name="fadeOutMilliSeconds">int fadeOutMilliSeconds</param>
        /// <returns>string Json format of message box</returns>
        protected string GenerateNotificationMessages(string message, NotificationType type, int fadeOutMilliSeconds = 10000)
        {
            MessageBoxModel msgObj = new MessageBoxModel();
            msgObj.Message = message;
            msgObj.Type = type.ToString();
            msgObj.IsFadeOut = CheckIsFadeOut();
            msgObj.FadeOutMilliSeconds = fadeOutMilliSeconds;
            return JsonConvert.SerializeObject(msgObj);
        }

        protected override void OnException(ExceptionContext exceptionContext)
        {
            // Mark exception as handled
            exceptionContext.ExceptionHandled = true;
            ZnodeLogging.LogMessage(exceptionContext.Exception, string.Empty, TraceLevel.Error);
            if (!User.Identity.IsAuthenticated && exceptionContext.Exception is HttpAntiForgeryException)
            {
                exceptionContext.Result = RedirectToAction("Login", "User");
                base.OnException(exceptionContext);
            }
            if (exceptionContext.Exception is FileNotFoundException)
            {
                Response.Clear();
                Response.ContentType = "text/html;charset=UTF-8";
                Response.Write(WebStore_Resources.FormattedThemeNotFoundMessage);
                Server.ClearError();
            }               
            if (User.Identity.IsAuthenticated && HttpContext.Request.UrlReferrer.PathAndQuery.Contains("/User/Login?returnUrl"))
            {
                exceptionContext.Result = RedirectToAction("Index", "Home");
                base.OnException(exceptionContext);
            }
            if (exceptionContext.Exception is ZnodeException)
            {
                ZnodeException productException = (ZnodeException)exceptionContext.Exception;
                if (productException.ErrorCode == ErrorCodes.ProductNotFound)
                {
                    HttpContext.Response.Redirect("/404");
                }
            }
        }

        public new RedirectToRouteResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }
        #endregion

        #region ActionView
        public ActionResult ActionView()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult ActionView(IView view)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(view);
        }

        public ActionResult ActionView(object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult ActionView(string viewName)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewName);
            }
            return View(viewName);
        }

        public ActionResult ActionView(IView view, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(view, model);
        }

        public ActionResult ActionView(string viewName, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewName, model);
            }
            return View(viewName, model);
        }

        public ActionResult ActionView(string viewName, string masterName)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(viewName, masterName);
        }

        public ActionResult ActionView(string viewName, string masterName, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(viewName, masterName, model);
        }
        #endregion

        #region public methods
        /// <summary>
        /// To get IsFadeOut status from web config file, 
        /// if NotificationMessagesIsFadeOut key not found in config then it will returns false 
        /// </summary>
        /// <returns>return true/false</returns>
        public bool CheckIsFadeOut()
        {
            bool isFadeOut = false;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationMessagesIsFadeOut"]))
            {
                isFadeOut = Convert.ToBoolean(ConfigurationManager.AppSettings["NotificationMessagesIsFadeOut"]);
            }
            else
            {
                //To do : need to log this in log file after common log functionality is ready.
                //ZnodeLogging.LogMessage(ZnodeResources.ConfigKeyNotificationMessagesIsFadeOutMissing);
            }
            return isFadeOut;
        }

        //Method converts the Partial view result to string.
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter stringWriter = new StringWriter())
            {
                ThemedViewEngine _viewEngine = new ThemedViewEngine();
                ViewEngineResult viewResult = _viewEngine.FindPartialView(ControllerContext, viewName, false);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View,
                ViewData, TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Log error to Elmah
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="contextualMessage"></param>
        public virtual void ExceptionHandler(Exception ex, string contextualMessage = null)
        {
            if (HelperUtility.IsNotNull(contextualMessage))
            {
                Exception annotatedException = new Exception(contextualMessage, ex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(annotatedException);
            }
            else
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

            if (ex is ZnodeException)
            {
                ZnodeException exception = ex as ZnodeException;
                ModelState.AddModelError(string.Empty, string.IsNullOrEmpty(exception.ErrorMessage) == true ? WebStore_Resources.GenericErrorMessage : exception.ErrorMessage);
            }
            else
                ModelState.AddModelError(string.Empty, WebStore_Resources.GenericErrorMessage);
        }

        //Set notification message based on the status
        protected void SetNotificationMessageByStatus(bool status, string successMessage, string errorMessage)
        {
            SetNotificationMessage(status ? successMessage : errorMessage);
        }
        #endregion
    }
}
