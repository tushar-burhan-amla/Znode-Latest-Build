using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
  public class DiagnosticsClient : BaseClient, IDiagnosticsClient
    {

        /// <summary>
        /// This method calls diagnostics api controller to get Version details of product from database
        /// </summary>
        /// <returns>Returns the DiagnosticsResponse which contains the version details</returns>
        public DiagnosticsResponse GetProductVersionDetails()
        {
            string endpoint = DiagnosticsEndpoint.GetProductVersionDetails();

            ApiStatus status = new ApiStatus();
            DiagnosticsResponse response = GetResourceFromEndpoint<DiagnosticsResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return Equals(response, null) ? null : response;
        }

        /// <summary>
        /// This method calls diagnostics api controller to sends the diagnostics email
        /// </summary>
        /// <param name="model">DiagnosticsEmailModel which should contain case number and merged text (diagnostics data)</param>
        /// <returns>Returns the DiagnosticsResponse which contains the email sent status</returns>
        public DiagnosticsResponse EmailDiagnostics(DiagnosticsEmailModel model)
        {
            string endpoint = DiagnosticsEndpoint.EmailDiagnostics();
            ApiStatus status = new ApiStatus();
            DiagnosticsResponse response = PostResourceToEndpoint<DiagnosticsResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return (Equals(response, null)) ? null : response;
        }

        /// <summary>
        /// This methods calls diagnostics api controller to get diagnostics status
        /// </summary>
        /// <returns></returns>
        public DiagnosticsListModel GetDiagnosticsList()
        {
            string endpoint = DiagnosticsEndpoint.GetDiagnosticsList();

            ApiStatus status = new ApiStatus();

            DiagnosticsResponse response = GetResourceFromEndpoint<DiagnosticsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DiagnosticsListModel diagnosticListModel = new DiagnosticsListModel { DiagnosticsList = response?.Diagnostics?.DiagnosticsList };
            return diagnosticListModel;

        }

        #region Private Methods

        private DiagnosticsModel BuildModel(string category, string item, bool status) => new DiagnosticsModel() { Category = category, Item = item, Status = status };

        #endregion
    }
}
