using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Client
{
    public interface IDiagnosticsClient
    {
        /// <summary>
        /// This method calls diagnostics api controller to get Version details of product from database
        /// </summary>
        /// <returns>Returns the DiagnosticsResponse which contains the version details</returns>
        DiagnosticsResponse GetProductVersionDetails();

        /// <summary>
        /// This method calls diagnostics api controller to sends the diagnostics email
        /// </summary>
        /// <param name="model">DiagnosticsEmailModel which should contain case number and merged text (diagnostics data)</param>
        /// <returns>Returns the DiagnosticsResponse which contains the email sent status</returns>
        DiagnosticsResponse EmailDiagnostics(DiagnosticsEmailModel model);

        /// <summary>
        /// This methods calls diagnostics api controller to get diagnostics status list
        /// </summary>
        /// <returns></returns>
        DiagnosticsListModel GetDiagnosticsList();
    }
}
