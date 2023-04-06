using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Engine.Api.Client;

namespace Znode.Engine.WebStore.Agents
{
    public class PowerBIAgent : BaseAgent, IPowerBIAgent
    {
        #region Private Variables

        private readonly IGeneralSettingClient _generalSettingClient;

        #endregion Private Variables
        #region Public Constructor

        public PowerBIAgent(IGeneralSettingClient generalSettingClient)
        {
            _generalSettingClient = GetClient<IGeneralSettingClient>(generalSettingClient);
        }

        #endregion Public Constructor
        #region Public Methods

        //Gets Power BI setting details
        public virtual PowerBISettingsViewModel GetPowerBISettings()
            => _generalSettingClient.GetPowerBISettings()?.ToViewModel<PowerBISettingsViewModel>();

        // Method to get PowerBI Report
        public virtual async Task<PowerBIEmbedViewModel> GetPowerBIReportsData(PowerBISettingsViewModel powerBISettingsViewModel)
        {
            PowerBIEmbedViewModel powerBIEmbedViewModel = new PowerBIEmbedViewModel();
            string errorMessage = ValidatePowerBISettings(powerBISettingsViewModel);

            if (HelperUtility.IsNull(errorMessage))
            {
                try
                {
                    // Create a user password credentials.
                    UserPasswordCredential credential = new UserPasswordCredential(powerBISettingsViewModel.PowerBIUserName, powerBISettingsViewModel.PowerBIPassword);
                    
                    // Authenticate using created credentials
                    AuthenticationContext authenticationContext = new AuthenticationContext(string.Concat(ZnodeWebstoreSettings.PowerBIAuthorityUrl, powerBISettingsViewModel.PowerBITenantId), new TokenCache());
                    AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(ZnodeWebstoreSettings.PowerBIResourceUrl, powerBISettingsViewModel.PowerBIApplicationId, credential);

                    if (HelperUtility.IsNull(authenticationResult))
                    {
                        ZnodeLogging.LogMessage(WebStore_Resources.ErrorPowerBIAuthentication, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                        powerBIEmbedViewModel.ErrorMessage = WebStore_Resources.ErrorPowerBISettings;
                        return powerBIEmbedViewModel;
                    }

                    TokenCredentials tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");

                    // Create a Power BI Client object. It will be used to call Power BI APIs.
                    using (PowerBIClient client = new PowerBIClient(new Uri(ZnodeWebstoreSettings.PowerBIApiUrl), tokenCredentials))
                    {
                        // Get a list of reports.
                        ODataResponseListReport reports = await client.Reports.GetReportsInGroupAsync(powerBISettingsViewModel.PowerBIGroupId);
                        Report report;

                        if (string.IsNullOrEmpty(powerBISettingsViewModel.PowerBIReportId))
                            // Get the first report in the group.
                            report = reports.Value.FirstOrDefault();
                        else
                            report = reports.Value.FirstOrDefault(r => r.Id == powerBISettingsViewModel.PowerBIReportId);

                        if (HelperUtility.IsNull(report))
                        {
                            ZnodeLogging.LogMessage(WebStore_Resources.ErrorPowerBIReportId, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                            powerBIEmbedViewModel.ErrorMessage = WebStore_Resources.ErrorPowerBISettings;
                            return powerBIEmbedViewModel;
                        }

                        Dataset datasets = await client.Datasets.GetDatasetByIdInGroupAsync(powerBISettingsViewModel.PowerBIGroupId, report.DatasetId);
                        
                        // Generate Embed Token for reports without effective identities.
                        GenerateTokenRequest generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                        EmbedToken tokenResponse = await client.Reports.GenerateTokenInGroupAsync(powerBISettingsViewModel.PowerBIGroupId, report.Id, generateTokenRequestParameters);

                        if (HelperUtility.IsNull(tokenResponse))
                        {
                            ZnodeLogging.LogMessage(WebStore_Resources.ErrorPowerBIAccessToken, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                            powerBIEmbedViewModel.ErrorMessage = WebStore_Resources.ErrorPowerBIAccessToken;
                            return powerBIEmbedViewModel;
                        }

                        // Generate Embed Configuration.
                        powerBIEmbedViewModel.EmbedToken = tokenResponse;
                        powerBIEmbedViewModel.EmbedUrl = report.EmbedUrl;
                        powerBIEmbedViewModel.ReportId = report.Id;

                        return powerBIEmbedViewModel;
                    }
                }
                catch (HttpOperationException ex)
                {
                    ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error, ex);
                    powerBIEmbedViewModel.ErrorMessage = WebStore_Resources.ErrorPowerBISettings;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error, ex);
                    powerBIEmbedViewModel.ErrorMessage = WebStore_Resources.ErrorPowerBISettings;
                }

                return powerBIEmbedViewModel;
            }
            else
            {
                powerBIEmbedViewModel.ErrorMessage = errorMessage;
                return powerBIEmbedViewModel;
            }
        }
        
        #endregion Public Methods

        #region Private Methods

        //Validate Power BI Settings
        private string ValidatePowerBISettings(PowerBISettingsViewModel powerBISettingsViewModel)
        {
            if (HelperUtility.IsNull(ZnodeWebstoreSettings.PowerBIAuthorityUrl) || HelperUtility.IsNull(ZnodeWebstoreSettings.PowerBIResourceUrl)
                || HelperUtility.IsNull(ZnodeWebstoreSettings.PowerBIApiUrl))
            {
                ZnodeLogging.LogMessage(WebStore_Resources.PowerBIWebconfigSettings, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                return WebStore_Resources.PowerBIWebconfigSettings;
            }
            else if (HelperUtility.IsNotNull(powerBISettingsViewModel) && (string.IsNullOrEmpty(powerBISettingsViewModel.PowerBIApplicationId)
                || string.IsNullOrEmpty(powerBISettingsViewModel.PowerBIGroupId) || string.IsNullOrEmpty(powerBISettingsViewModel.PowerBIReportId)
                || string.IsNullOrEmpty(powerBISettingsViewModel.PowerBITenantId) || string.IsNullOrEmpty(powerBISettingsViewModel.PowerBIPassword)
                || string.IsNullOrEmpty(powerBISettingsViewModel.PowerBIUserName)))
            {
                return WebStore_Resources.ErrorPowerBISettings;
            }

            return null;
        }

        #endregion Private Methods
    }
}