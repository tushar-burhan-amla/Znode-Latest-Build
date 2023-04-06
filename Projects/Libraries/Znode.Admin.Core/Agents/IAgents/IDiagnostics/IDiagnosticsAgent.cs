using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
  public interface IDiagnosticsAgent
    {

        /// <summary>
        /// This method calls diagnostics client to get Version details of product from database
        /// </summary>
        /// <returns>Returns the DiagnosticsResponse which contains the version details</returns>
        DiagnosticsResponse GetProductVersionDetails();

        /// <summary>
        /// This method calls the diagnostics client to sends the diagnostics email
        /// </summary>
        /// <param name="caseNumber">Case number for diagnostics</param>
        /// <returns>Returns the DiagnosticsResponse which contains the email sent status</returns>
        DiagnosticsResponse EmailDiagnostics(DiagnosticsEmailModel model);

        /// <summary>
        /// This method validates the license (whether the license is installed on the calling server or not)
        /// This method calls the Znode.Libraries.Framework.Business.ZnodeLicenseManager class to validate the license
        /// </summary>
        /// <returns>Returns ZnodeLicenseType which will contain the status true if the license is valid otherwise false</returns>
        ZnodeLicenseType ValidateLicense();

        /// <summary>
        /// This method validates the license (whether the license is installed on the calling server or not)
        /// This method calls the Znode.Libraries.Framework.Business.ZnodeLicenseManager class to get the license status description
        /// </summary>
        /// <returns>Returns License Status Description</returns>
        string GetLicenseStatusDescription();

        /// <summary>
        ///  This method calls diagnostics api client to get diagnostics status
        /// </summary>
        /// <returns></returns>
        DiagnosticListViewModel GetDiagnosticsList();
    }
}