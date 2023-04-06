using Microsoft.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Libraries.Resources.Helpers;

namespace Znode.Engine.Admin.Controllers
{
    public class BaseController : Controller
    {
        #region Public variable
        public const string Notifications = "Notifications";
        #endregion

        #region Property
        public int? PortalId { get; set; }
        public bool? EnableCustomerPricing { get; set; }
        #endregion

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this.ControllerContext = new ControllerContext(requestContext, this);
            // PortalId = PortalAgent.CurrentPortal.PortalId;
            //  EnableCustomerPricing = PortalAgent.CurrentPortal.EnableCustomerPricing;
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string localeId = null;

            //Get culture name cookies
            localeId = string.IsNullOrEmpty(Request.Cookies["_culture"]?.Value) ? DefaultSettingHelper.DefaultLocale : Request.Cookies["_culture"].Value;

            // Save culture in a cookie     
            CookieHelper.SetCookie("_culture", localeId);


            if (CookieHelper.IsCookieExists("_culture"))
                localeId = CookieHelper.GetCookieValue<string>("_culture");
            else
                localeId = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

            CultureHelper.SetConfigurationSettingSource(DefaultSettingHelper.GetActiveLocaleList()?.Locales?.Where(x => x.LocaleId == Convert.ToInt32(localeId)).Select(x => x.Code).ToList());

            // Validate culture name
            string cultureName = DefaultSettingHelper.GetActiveLocaleList()?.Locales?.Where(x => x.LocaleId == Convert.ToInt32(localeId)).Select(x => x.Code).FirstOrDefault();
            string validCultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(validCultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        #region ActionView
        public virtual ActionResult ActionView()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public virtual ActionResult ActionView(IView view)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(view);
        }

        public virtual ActionResult ActionView(object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public virtual ActionResult ActionView(string viewName)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewName);
            }
            return View(viewName);
        }

        public virtual ActionResult ActionView(IView view, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(view, model);
        }

        public virtual ActionResult ActionView(string viewName, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewName, model);
            }
            return View(viewName, model);
        }

        public virtual ActionResult ActionView(string viewName, string masterName)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(viewName, masterName);
        }

        public virtual ActionResult ActionView(string viewName, string masterName, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View(viewName, masterName, model);
        }
        #endregion

        #region Protected Method

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
        /// Get the information notification message.
        /// </summary>
        /// <param name="infoMessage">information message.</param>
        /// <returns>Returns serialize MessageBoxModel with notification set to info.</returns>
        protected string GetInfoNotificationMessage(string infoMessage)
            => GenerateNotificationMessages(infoMessage, NotificationType.info);

        /// <summary>
        /// Set notification message.
        /// </summary>
        /// <param name="notificationMessage">Message to set.</param>
        protected void SetNotificationMessage(string notificationMessage)
            => TempData[AdminConstants.Notifications] = notificationMessage;

        //Sets the global search filter.
        protected virtual void SetGlobalSearchFilter(FilterCollectionDataModel model)
        {
            FilterTuple globalFilter = TempData[DynamicGridConstants.GlobalSearchFilter] as FilterTuple;
            if (!Equals(globalFilter, null))
            {
                //Remove all existing filter with the same filter key.
                model.Filters.RemoveAll(x => string.Equals(x.FilterName, globalFilter.FilterName, StringComparison.InvariantCultureIgnoreCase));
                model.Filters.Add(globalFilter);
            }
        }

        protected override void OnException(ExceptionContext exceptionContext)
        {
            ZnodeLogging.LogMessage(exceptionContext.Exception, string.Empty, TraceLevel.Error);
            if ((HttpStatusCode)exceptionContext.Exception.GetDynamicProperty("StatusCode") == HttpStatusCode.Unauthorized)
                exceptionContext.Result = RedirectToAction("UnAuthorizedErrorRequest", "ErrorPage");

            base.OnException(exceptionContext);
        }

        /// <summary>
        /// a method to return a json with status and message
        /// </summary>
        /// <param name="status">status</param>
        /// <param name="message">message</param>
        /// <returns>returns json with status and message</returns>
        protected virtual JsonResult JsonReturnType(bool status, string message)
        {
            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region public Method
        /// <summary>
        /// Log error to Elmah
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="contextualMessage"></param>
        public virtual void ExceptionHandler(Exception ex, string contextualMessage = null)
        {
            if (contextualMessage != null)
            {
                var annotatedException = new Exception(contextualMessage, ex);
            }
            if (ex is ZnodeException)
            {
                ZnodeException exception = ex as ZnodeException;
                ModelState.AddModelError(string.Empty, string.IsNullOrEmpty(exception.ErrorMessage) == true ? Admin_Resources.GenericErrorMessage : exception.ErrorMessage);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Admin_Resources.GenericErrorMessage);
            }
        }

        //Method converts the Partial view result to string.
        public virtual string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// To add portal Ids in FilterCollection for franchisee admin
        /// </summary>
        /// <param name="filters">FilterCollection filters</param>
        /// <returns>returns FilterCollection with portal ids</returns>
        //public virtual FilterCollection GetAuthorizedPortalIdFilter(FilterCollection filters)
        //{
        //    ProfileCommonAgent profileAgent = new ProfileCommonAgent();
        //    string portalIds = profileAgent.GetProfileStoreAccess(HttpContext.User.Identity.Name).ProfileStoreAccess;
        //    if (!string.IsNullOrEmpty(portalIds))
        //    {
        //        if (!Equals(portalIds, "0"))
        //        {
        //            if (Equals(filters, null))
        //            {
        //                filters = new FilterCollection();
        //            }
        //            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalIds));
        //        }
        //    }
        //    return filters;
        //}

        /// <summary>
        /// To add account Id in FilterCollection for mall admin
        /// </summary>
        /// <param name="filters">FilterCollection filters</param>
        /// <returns>returns FilterCollection with account id</returns>
        //public virtual FilterCollection GetAuthorizedAccountIdFilter(FilterCollection filters)
        //{
        //    AccountAgent accountAgent = new AccountAgent();
        //    var account = accountAgent.GetAccounts();
        //    if (!Equals(account, null))
        //    {
        //        if (account.AccountId > 0)
        //        {
        //            if (Equals(filters, null))
        //            {
        //                filters = new FilterCollection();
        //            }
        //            filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, account.AccountId.ToString()));
        //        }
        //    }
        //    return filters;
        //}

        /// <summary>
        /// To Get the Authorized portal Id for the login user.
        /// </summary>
        /// <returns>Return the portal Id.</returns>
        //public virtual int? GetAuthorizedPortalId()
        //{
        //    int portalId = 0;
        //    ProfileCommonAgent profileAgent = new ProfileCommonAgent();
        //    string portalIds = profileAgent.GetProfileStoreAccess(HttpContext.User.Identity.Name).ProfileStoreAccess;
        //    if (!string.IsNullOrEmpty(portalIds) && int.TryParse(portalIds.Split(',')[0], out portalId))
        //    {
        //        return portalId;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        ///  To Get the Authorized account Id for the login user.
        /// </summary>
        /// <returns>returns AccountId</returns>
        //public virtual int? GetAuthorizedAccountId()
        //{
        //    AccountAgent accountAgent = new AccountAgent();
        //    var account = accountAgent.GetAccounts();
        //    if (!Equals(account, null) && account.AccountId > 0)
        //    {
        //        return account.AccountId;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        public virtual ActionResult DownloadSamples(string sampleFilePath)
        {
            // Set download path in string
            string filePath = HelperMethods.GetImportTemplateFilePath(AdminConstants.CSV, sampleFilePath);

            // Check whether the file exists or not
            if (HelperMethods.FileOrDirectoryExists(Server.MapPath(filePath)))
            {
                //Read the file content.
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(filePath));
                //Downloads the File from the File Path.
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
            }
            else
            {
                SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.ImportTemplateNotFound, NotificationType.info));
                return RedirectToAction(AdminConstants.ListView, ControllerContext.RouteData.Values["controller"].ToString());
            }
        }

        /// <summary>
        /// This action method is used for Save and Close button redirect to back page.
        /// </summary>
        /// <returns>Actin to which redirect</returns>
        public virtual ActionResult GotoBackURL()
        {
            var backURLcookies = Request.Cookies["_backURL"];
            string backURL = string.Empty;
            if (backURLcookies != null)
            {
                backURL = backURLcookies.Value;
                if (!string.IsNullOrEmpty(backURL))
                    return Redirect(HttpUtility.UrlDecode(backURL));
            }
            return null;
        }

        /// <summary>
        /// This method used for converting dictionary to list.
        /// </summary>
        /// <param name="columns">columns dictionary</param>
        /// <returns></returns>
        public virtual List<CustomColumnViewModel> AttrColumn(Dictionary<string, object> columns)
        {
            var _list = new List<CustomColumnViewModel>();
            if (!Equals(columns, null))
            {
                foreach (var item in columns)
                {
                    _list.Add(new CustomColumnViewModel { FieldName = item.Key, DisplayName = Convert.ToString(item.Value) });
                }
            }

            return _list;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// To get IsFadeOut status from web config file, 
        /// if NotificationMessagesIsFadeOut key not found in config then it will returns false 
        /// </summary>
        /// <returns>return true/false</returns>
        private bool CheckIsFadeOut()
        {
            bool isFadeOut = false;
            if (!string.IsNullOrEmpty(ZnodeAdminSettings.NotificationMessagesIsFadeOut))
            {
                isFadeOut = Convert.ToBoolean(ZnodeAdminSettings.NotificationMessagesIsFadeOut);
            }
            else
            {
                //To do : need to log this in log file after common log functionality is ready.
                //ZnodeLogging.LogMessage(ZnodeResources.ConfigKeyNotificationMessagesIsFadeOutMissing);
            }
            return isFadeOut;
        }
        #endregion

    }
}
