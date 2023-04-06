using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Admin.Controllers
{
    public class DemoController : BaseController
    {
        #region Private Variables
        private readonly IDemoAgent _agent;
        #endregion

        #region Constructor
        public DemoController()
        {
            _agent = new DemoAgent();
        }
        #endregion

        // GET: Demo
        public virtual ActionResult Index()
        {

            return View(new UserModel());
        }

        [HttpPost]
        public virtual ActionResult Index(string DemoControl)
        {
            return RedirectToAction("Index");
        }

        ////Handles all application level errors.
        //[AllowAnonymous]
        //public ActionResult ErrorHandler(Exception exception)
        //{
        //    HttpException httpexception = exception as HttpException;

        //    if (IsNotNull(httpexception))
        //    {
        //        int httpCode = httpexception.GetHttpCode();

        //        switch (httpCode)
        //        {
        //            case 404:
        //                {
        //                    ViewBag.ErrorMessage = Admin_Resources.HttpCode_401_AccessDeniedMsg;
        //                    break;
        //                }
        //            case 401:
        //                {
        //                    ViewBag.ErrorMessage = Admin_Resources.HttpCode_401_AccessDeniedMsg;
        //                    break;
        //                }
        //            default:
        //                {
        //                    if (exception is HttpRequestValidationException)
        //                        ViewBag.ErrorMessage = Admin_Resources.HttpCode_500_RequestValidationErrorMsg;
        //                    else
        //                        ViewBag.ErrorMessage = Admin_Resources.HttpCode_500_InternalServerErrorMsg;
        //                    break;
        //                }
        //        }
        //    }
        //    else
        //        ViewBag.ErrorMessage = Admin_Resources.GenericErrorMessage;
        //    return View("ElmahError");
        //}

        [AllowAnonymous]
        public virtual ActionResult TokenError(Exception exception)
        {
            ZnodeException znodeException = exception as ZnodeException;

            if (znodeException.ErrorCode == ErrorCodes.WebAPIKeyNotFound)
                ViewBag.ErrorMessage = "Web Api key not found please enter into web.config file.";

            return View("UnAuthorized");
        }


        [AllowAnonymous]
        public virtual ActionResult ConfigurationError(Exception exception)
        {
            ZnodeException znodeException = exception as ZnodeException;
            ViewBag.ErrorMessage = znodeException?.ErrorMessage ?? exception.Message;
            return View("UnAuthorized");
        }
    }
}