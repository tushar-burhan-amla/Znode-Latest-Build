using System;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Engine.MvcAdmin.Agents.Agents;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.Diagnostics.Controllers
{
  [AllowAnonymous]
    public class DiagnosticsController : Controller
    {
        #region Variables
        private int errorCount = 0;
        private StringBuilder exceptionLog = new StringBuilder();
        private IDiagnosticsAgent _diagnosticsAgent;

        #endregion
        public DiagnosticsController()
        {
            _diagnosticsAgent = new DiagnosticsAgent();
        }

        // GET: Diagnostics/Diagnostics
        /// <summary>
        /// Get diagnostics status 
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            DiagnosticListViewModel diagnosticList = _diagnosticsAgent.GetDiagnosticsList();
            return View(diagnosticList);
        }

        #region Email diagnostics

        /// <summary>
        /// This method sends the diagnostics email
        /// </summary>
        /// <param name="caseNumber">Case number for diagnostics</param>
        /// <returns>Returns the json result which contains the email sent status and exception log</returns>
        [HttpPost]
        public JsonResult EmailDiagnostics(string caseNumber)
        {
            bool status = false;
            if (string.IsNullOrEmpty(caseNumber))
            {
                return Json(new { Result = status, Log = Admin_Resources.RequiredCaseNumber, IsModelError = true }, JsonRequestBehavior.AllowGet);
            }
            string log = string.Empty;
            try
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                byte[] diagnosticsData = webClient.DownloadData(Request.UrlReferrer.AbsoluteUri);
                if (!_diagnosticsAgent.EmailDiagnostics(new DiagnosticsEmailModel { MergedText = Encoding.Default.GetString(diagnosticsData), CaseNumber = caseNumber }).HasError)
                {
                    log = Admin_Resources.SuccessSendingDiagnosticsMail;
                    status = true;
                }
                else
                {
                    log = string.Format(Admin_Resources.DiagnosticsExceptionLogFormat, ++errorCount, Admin_Resources.ErrorSendingDiagnosticsMail);
                }
            }
            catch (Exception generalException)
            {
                ZnodeLogging.LogMessage(generalException, string.Empty, TraceLevel.Error);
                log = string.Format(Admin_Resources.DiagnosticsExceptionLogFormat, ++errorCount, Convert.ToString(generalException));
            }
            return Json(new { Result = status, Log = log }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        /// <summary>
        /// This method validates the license and get the type of license installed
        /// </summary>
        /// <param name="model">It takes DiagnosticsViewModel in which license status and description is to be set</param>
        private void GetLicenseData(DiagnosticsViewModel model)
        {
            try
            {
                ZnodeLicenseType licenseStatus = _diagnosticsAgent.ValidateLicense();
                model.LicenseStatus = (licenseStatus != ZnodeLicenseType.Invalid);
                model.LicenseDescription = _diagnosticsAgent.GetLicenseStatusDescription();
            }
            catch (Exception generalException)
            {
                ZnodeLogging.LogMessage(generalException, string.Empty, TraceLevel.Error);
                this.exceptionLog.Append(string.Format(ZnodeAdmin_Resources.DiagnosticsExceptionLogFormat, ++errorCount, Convert.ToString(generalException)));
                model.LicenseStatus = false;
                model.LicenseDescription = ZnodeAdmin_Resources.ErrorGettingLicense;
            }
        }

        /// <summary>
        /// Display Version detail of product from database
        /// </summary>
        /// <param name="model">DiagnosticsViewModel in which version details are to be set</param>
        private void GetVersionDetails(DiagnosticsViewModel model)
        {
            try
            {
                model.ProductVersion = _diagnosticsAgent.GetProductVersionDetails()?.ProductVersion;
            }
            catch (Exception generalException)
            {
                ZnodeLogging.LogMessage(generalException, string.Empty, TraceLevel.Error);
                this.exceptionLog.Append(string.Format(ZnodeAdmin_Resources.DiagnosticsExceptionLogFormat, ++errorCount, Convert.ToString(generalException)));
                model.LicenseStatus = false;
                model.LicenseDescription = ZnodeAdmin_Resources.ErrorGettingLicense;
            }
        }


    }
}