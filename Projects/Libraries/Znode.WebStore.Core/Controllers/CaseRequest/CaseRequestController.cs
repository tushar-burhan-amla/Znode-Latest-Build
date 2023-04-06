using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.WebStore.Core.Extensions;

namespace Znode.Engine.WebStore.Controllers
{
    public class CaseRequestController : BaseController
    {
        #region Private Variables
        private ICaseRequestAgent _caseRequestAgent;
        private const string contactUsView = "ContactUs";
        #endregion

        #region Public Constructor

        public CaseRequestController(ICaseRequestAgent caseRequestAgent)
        {
            _caseRequestAgent = caseRequestAgent;
        }

        #endregion

        #region Public Methods

        #region Contact Us
        //Get : Create CaseRequest for contact us form.
        public virtual ActionResult ContactUs() => View();

        //Post : Create CaseRequest for contact us form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaAuthorization]
        public virtual ActionResult ContactUs(CaseRequestViewModel caseRequestViewModel)
        {
            //bool isValidCaptcha = this.IsCaptchaValid(WebStore_Resources.ErrorWrongCaptcha);
            if (ModelState.IsValid && HelperUtility.IsNotNull(caseRequestViewModel))
            {
                caseRequestViewModel = _caseRequestAgent.CreateContactUs(caseRequestViewModel);

                SetNotificationMessage(caseRequestViewModel?.CaseRequestID > 0
                   ? GetSuccessNotificationMessage(WebStore_Resources.SuccessMessageContactUs)
                    : GetErrorNotificationMessage(WebStore_Resources.ErrorMessageContactUs));
                ModelState.Clear();
                return View();
            }
            caseRequestViewModel.ErrorMessage = WebStore_Resources.ErrorCaptchaCode;
            return View(caseRequestViewModel);
        }

      
        #endregion

        #region Customer Feedback.
        //Get : Create CaseRequest for customer feedback form.
        [HttpGet]
        public virtual ActionResult CustomerFeedback() => View();

        //Post : Create CaseRequest for customer feedback form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaAuthorization]
        public virtual ActionResult CustomerFeedback(CaseRequestViewModel caseRequestViewModel)
        {
           // bool isValidCaptcha = this.IsCaptchaValid(WebStore_Resources.ErrorWrongCaptcha);
            if (ModelState.IsValid && HelperUtility.IsNotNull(caseRequestViewModel))
            {
                caseRequestViewModel = _caseRequestAgent.CreateCustomerFeedback(caseRequestViewModel);
                if (caseRequestViewModel?.CaseRequestID > 0)
                    return RedirectToAction<CaseRequestController>(x => x.CustomerFeedbackSuccess());
            }
            caseRequestViewModel.ErrorMessage = WebStore_Resources.ErrorCaptchaCode;
            return View(caseRequestViewModel);
        }

        // Customer Feedback Success Message
        public virtual ActionResult CustomerFeedbackSuccess() => View();
        #endregion

        #endregion
    }
}