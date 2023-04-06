using System;
using System.Web.Mvc;
using Znode.Engine.Api.Areas.Activate.Models;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Areas.Activate.Controllers
{
    public class ActivateController : Controller
    {
        private readonly ILicenseService _service;

        public ActivateController()
        {
            _service = new LicenseService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LicenseInformation()
        {
            return View("LicenseInformation", new ActivatePageApiModel());
        }

        [HttpPost]
        public ActionResult Create(ActivatePageApiModel model)
        {
            if (!Equals(model.LicenseType, "FreeTrial") && string.IsNullOrEmpty(model.SerialNumber))
            {
                ModelState.AddModelError("SerialNumber", "Serial number is required");
            }

            if (ModelState.IsValid)
            {
                string message = String.Empty;
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
            if (_service.UpdateLicense(out message))
            {
                return Content("Your Znode license has been updated successfully. ");
            }
            else
            {
                return Content(message);
            }
        }
    }
}