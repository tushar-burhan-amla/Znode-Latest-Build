using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.ViewModels;
namespace Znode.Engine.Admin.Controllers
{
    public class LicenseController : BaseController
    {
        private readonly ILicenseAgent _licenseAgent;

        public LicenseController(ILicenseAgent licenseAgent)
        {
            _licenseAgent = licenseAgent;
        }

        #region Public Methods
        [HttpGet]
        public virtual ActionResult GetLicenseInformation()
        {
            //Get the Znode Installed License Information.
            LicenceInfoViewModel licenseInfo = _licenseAgent.GetLicenseInformation();
            return PartialView("_LicenseInformation", licenseInfo);
        }
        #endregion
    }
}