using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.MvcAdmin.Agents.Agents
{
  public class DiagnosticsAgent : BaseAgent, IDiagnosticsAgent
    {
        #region Variables

        private readonly IDiagnosticsClient _client;
        #endregion

        #region Constructor

        public DiagnosticsAgent()
        {
            _client = new DiagnosticsClient();
        }

        #endregion


        /// <summary>
        /// This method calls diagnostics client to get Version details of product from database
        /// </summary>
        /// <returns>Returns the DiagnosticsResponse which contains the version details</returns>
        public DiagnosticsResponse GetProductVersionDetails()
        {
            return _client.GetProductVersionDetails();
        }

        /// <summary>
        /// This method calls the diagnostics client to sends the diagnostics email
        /// </summary>
        /// <param name="caseNumber">Case number for diagnostics</param>
        /// <returns>Returns the DiagnosticsResponse which contains the email sent status</returns>
        public DiagnosticsResponse EmailDiagnostics(DiagnosticsEmailModel model)
        {
            return _client.EmailDiagnostics(model);
        }

        /// <summary>
        /// This method validates the license (whether the license is installed on the calling server or not)
        /// This method calls the Znode.Libraries.Framework.Business.ZnodeLicenseManager class to validate the license
        /// </summary>
        /// <returns>Returns ZnodeLicenseType which will contain the status true if the license is valid otherwise false</returns>
        public ZnodeLicenseType ValidateLicense()
        {
            ZnodeLicenseManager licenseMgr = new ZnodeLicenseManager();
            return licenseMgr.Validate();
        }

        /// <summary>
        /// This method validates the license (whether the license is installed on the calling server or not)
        /// This method calls the Znode.Libraries.Framework.Business.ZnodeLicenseManager class to get the license status description
        /// </summary>
        /// <returns>Returns License Status Description</returns>
        public string GetLicenseStatusDescription()
        {
            ZnodeLicenseManager licenseMgr = new ZnodeLicenseManager();
            licenseMgr.Validate();
            return string.Empty; //licenseMgr.GetStatusDescription();
        }


        /// <summary>
        /// This method calls diagnostics api client to get diagnostics status
        /// </summary>
        /// <returns></returns>
        public DiagnosticListViewModel GetDiagnosticsList()
        {
            try
            {
                DiagnosticsListModel diagnosticModel = _client.GetDiagnosticsList();

                DiagnosticListViewModel diagnosticListViewModel = new DiagnosticListViewModel { DiagnosticsList = diagnosticModel?.DiagnosticsList?.ToViewModel<DiagnosticViewModel>().ToList() };

                return diagnosticListViewModel?.DiagnosticsList?.Count > 0 ? diagnosticListViewModel
        : new DiagnosticListViewModel { DiagnosticsList = new List<DiagnosticViewModel>() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }
        }
    }
}