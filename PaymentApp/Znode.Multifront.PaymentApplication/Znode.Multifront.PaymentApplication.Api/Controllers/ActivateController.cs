using System;
using System.Web.Mvc;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Api.Controllers
{
    public class ActivateController : Controller
    {
        public ActionResult Index() => View();

        public ActionResult LicenseInformation()
        {
            return View("LicenseInformation", new ActivateLicenceModel());
        }

        [HttpPost]
        public ActionResult Create(ActivateLicenceModel model)
        {
            if (!Equals(model.LicenseType, "FreeTrial") && string.IsNullOrEmpty(model.SerialNumber))
            {
                ModelState.AddModelError("SerialNumber", "Serial number is required");
            }

            if (ModelState.IsValid)
            {
                string message = String.Empty;
                LicenceHelper _service = new LicenceHelper();
                if (_service.InstallLicense(model.LicenseType, model.SerialNumber, model.FullName, model.Email, out message))
                {
                    // Confirmation page 
                    model.Message = "Your Znode license has been successfully activated.";
                }
                else
                {
                    model.ErrorMessage = "Failed to activate license. Additional info: " + message;
                }
            }
            return View("LicenseInformation", model);
        }

        /// <summary>
        /// Update License
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateLicense()
        {
            string message = String.Empty;
            LicenceHelper _service = new LicenceHelper();         

            return (_service.UpdateLicense(out message))
                ? Content("Your Znode license has been updated successfully. ")
                : Content(message);
        }
    }
}