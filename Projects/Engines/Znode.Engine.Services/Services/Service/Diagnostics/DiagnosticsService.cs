using MongoDB.Bson;
using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Libraries.Search;

using ZNode.Libraries.MongoDB.Data.Constants;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class DiagnosticsService : BaseService, IDiagnosticsService
    {
        #region Protected variables     
        
        protected readonly IZnodeRepository<ZnodeMultifront> _znodeMultifront;
        protected readonly string logComponentName = "Diagnostics";

        #endregion

        #region Constructor

        public DiagnosticsService()
        {
            _znodeMultifront = new ZnodeRepository<ZnodeMultifront>();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Check for Database connection String
        /// </summary>
        /// <returns>Returns the status of database connection</returns>
        public virtual bool CheckSqlConnection()
        {
            ZnodeLogging.LogMessage("CheckSqlConnection: Execution started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
            const string settingKey = "ZnodeECommerceDB";
            try
            {
                SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[settingKey].ConnectionString);
                myConnection.Open();
                myConnection.Close();
                myConnection.Dispose();
                ZnodeLogging.LogMessage("Valid SQL Database connection verified.", logComponentName, TraceLevel.Info);
                ZnodeLogging.LogMessage("CheckSqlConnection: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception)
            {
                ZnodeLogging.LogMessage($"Failed to connect to SQL Database. Please check the 'Web.config' to verify that the '{settingKey}' setting is a proper SQL DB connection string.", logComponentName, System.Diagnostics.TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.InvalidSqlConfiguration, Api_Resources.InvalidSqlConfigurationMessage);
            }
        }

        /// <summary>
        /// Check for proper MongoDB Connection String
        /// </summary>
        /// <returns>Returns the status of Mongo database connection</returns>
        public virtual bool CheckMongoDBConnection(string settingKey)
        {
            ZnodeLogging.LogMessage("CheckMongoDBConnection: Execution started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            MongoClient client = new MongoClient(ConfigurationManager.ConnectionStrings[settingKey].ConnectionString);
            //Get mongo database name
            string databaseName = MongoUrl.Create(ConfigurationManager.ConnectionStrings[settingKey].ConnectionString).DatabaseName;
            var database = client.GetDatabase(databaseName);
            //check if the Mongo Server is connected and configured properly
            bool isMongoLive = database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
            if (isMongoLive)
            {
                ZnodeLogging.LogMessage("CheckMongoDBConnection: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage($"Failed to connect to Mongo DB. Please check the 'Web.config' to verify that the '{settingKey}' setting is a proper Mongo DB connection string.", logComponentName, TraceLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// Check for proper MongoDB Connection String for logs
        /// </summary>
        /// <returns>Returns the status of database connection</returns>
        public virtual bool CheckMongoDBLogConnection()
        {
            ZnodeLogging.LogMessage("CheckMongoDBLogConnection: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            string settingKey = MongoSettings.DBLogKey;
            string connectionString = ConfigurationManager.ConnectionStrings[settingKey]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                //Skip this condition if MongoDbForLog connection string not required.
                ZnodeLogging.LogMessage($"No '{settingKey}' setting found in 'Web.Config'. MongoDB logging will be skipped.", logComponentName, TraceLevel.Info);
                return true;
            }

            ZnodeLogging.LogMessage("CheckMongoDBLogConnection: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
            return CheckMongoDBConnection(settingKey);
        }

        /// <summary>
        /// This method get the status of various components
        /// </summary>
        /// <returns></returns>
        public virtual DiagnosticsListModel GetDiagnosticsList()
        {
            ZnodeLogging.LogMessage("GetDiagnosticsList: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            DiagnosticsListModel diagnosticListModel = new DiagnosticsListModel();
            string diagnosticsSettingXml = HttpContext.Current.Server.MapPath(ZnodeConstant.DiagnosticsSettingXml);

            try
            {
                DiagnosticSettings diagnosticSettings = HelperUtility.ConvertXMLStringToModel<DiagnosticSettings>(File.ReadAllText(diagnosticsSettingXml));

                if (diagnosticSettings?.Diagnostic?.Count() > 0)
                {
                    diagnosticSettings.Diagnostic = diagnosticSettings.Diagnostic.Where(z => z.IsVisible).OrderBy(x => x.SortOrder).ToList();
                    foreach (Diagnostic setting in diagnosticSettings.Diagnostic)
                    {
                        FillDiagnosticList(diagnosticListModel, setting);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeDiagnosticsEnum.Diagnostics.ToString(), TraceLevel.Error);

                throw;
            }

            ZnodeLogging.LogMessage("GetDiagnosticsList: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
            return diagnosticListModel;
        }

        /// <summary>
        /// This method sends the diagnostics email
        /// </summary>
        /// <param name="model">DiagnosticsEmailModel which should contain Case number for diagnostics</param>
        /// <returns>Returns true the email sent status otherwise false</returns>
        public virtual bool EmailDiagnostics(DiagnosticsEmailModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("EmailDiagnostics: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

                string senderEmail = ZnodeConfigManager.SiteConfig.CustomerServiceEmail;
                string recipientEmail = Convert.ToString(ConfigurationManager.AppSettings["ZnodeSupportEmail"]);
                string subject = $"Diagnostics for Case {model.CaseNumber}";

                var folderName = DateTime.Now.ToString("yyyy-MM-dd");
                var ZipfolderName = $"logs_{folderName}";
                string path = "~/Data//default//logs//demo";

                GetZipFile(HttpContext.Current.Server.MapPath(path) + "//" + folderName, ZipfolderName);
                string filePath = $"{path}//{folderName}//{ZipfolderName}.zip";

                if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
                {
                    ZnodeEmail.SendEmail(recipientEmail, senderEmail, senderEmail, subject, string.Empty, false, HttpContext.Current.Server.MapPath(filePath));
                }

                ZnodeLogging.LogMessage("EmailDiagnostics: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception)
            {
                ZnodeLogging.LogMessage("Error while sending diagnostics email", logComponentName, TraceLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// This method gets Version details of product from database
        /// </summary>
        /// <returns>Returns the version details</returns>
        public virtual string GetProductVersionDetails()
        {
            try
            {
                ZnodeLogging.LogMessage("GetProductVersionDetails: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

                var productVersion = _znodeMultifront.Table?.OrderByDescending(x => x.MultifrontId)?.FirstOrDefault();

                ZnodeLogging.LogMessage("GetProductVersionDetails: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
                return productVersion?.VersionName + "-" + productVersion?.MajorVersion + "." + productVersion?.MinorVersion + "." + productVersion?.LowerVersion + "." + productVersion?.BuildVersion;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeDiagnosticsEnum.Diagnostics.ToString(), TraceLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// This method checks SMTP Account
        /// </summary>
        /// <returns>status of SMPT account</returns>
        public virtual bool CheckEmailAccount()
        {
            try
            {
                ZnodeLogging.LogMessage("CheckEmailAccount: Execution started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

                ZnodeRepository<ZnodePortal> _znodePortal = new ZnodeRepository<ZnodePortal>();

                string storeAdminEmailId = _znodePortal.Table.Where(x => x.PortalId == ZnodeConfigManager.SiteConfig.PortalId).Select(x => x.AdminEmail).FirstOrDefault();
                ZnodePortalSmtpSetting znodePortalSmtpSetting = ZnodeEmail.GetSMTPSetting();

                if (HelperUtility.IsNotNull(znodePortalSmtpSetting)) {
                    znodePortalSmtpSetting.UserName = new ZnodeEncryption().DecryptData(znodePortalSmtpSetting.UserName);
                }

                // To set the "Store Administrator Email" inside the store assigned in the "To" recipient section and set "fromEmailAddress" property inside the "Portal SMTP Setting" value assigned in the "from recipient" section.                
                if (!string.IsNullOrEmpty(storeAdminEmailId) && !string.IsNullOrEmpty(znodePortalSmtpSetting?.FromEmailAddress))
                {
                    ZnodeEmail.SendEmail(storeAdminEmailId, znodePortalSmtpSetting.FromEmailAddress, string.Empty, "Diagnostic Test", "SMTP Account Test", false, znodePortalSmtpSetting);
                }
                else
                {
                    ZnodeEmail.SendEmail("noreply@WebApp.com", "noreply@WebApp.com", string.Empty, "Diagnostic Test", "SMTP Account Test", false);
                }

                ZnodeLogging.LogMessage("CheckEmailAccount: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error while checking SMTP settings", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Error, ex.Message);
                //No need to throw exception here
                return false;
            }
        }

        /// <summary>
        /// This method check Service working or stopped. 
        /// </summary>
        /// <param name="serviceName">service Name</param>
        /// <returns>return status</returns>
        public virtual string CheckService(string serviceName)
        {
            try
            {
                ZnodeLogging.LogMessage("CheckService: Execution started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

                ServiceController sc = new ServiceController(serviceName);
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.StopPending:
                        return "Stopping";
                    case ServiceControllerStatus.StartPending:
                        return "Starting";
                    default:
                        return "Status Changing";
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error while checking service status", logComponentName, TraceLevel.Error);
                throw;
            }
        }

        #endregion

        #region Protected Methods

        //Get the contents of zip file of theme.
        protected virtual byte[] GetZipFile(string filePath, string themeName)
        {
            ZnodeLogging.LogMessage("GetZipFile: Execution started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            //Create a zip file to download.
            Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
            zip.AddDirectory(filePath);
            zip.Save($"{ filePath}/{themeName}.zip");

            //Read all bytes of zip file.
            byte[] fileBytes = File.ReadAllBytes($"{ filePath}/{themeName}.zip");

            //Dispose the zip file object.
            zip.Dispose();

            //Delete saved zip File.
            if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(""), $"{ themeName}\\{themeName}.zip")))
                File.Delete(new FileInfo(Path.Combine($"{HttpContext.Current.Server.MapPath("")}\\{themeName}\\", themeName + ".zip")).ToString());

            ZnodeLogging.LogMessage("GetZipFile: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
            return fileBytes;
        }
        
        /// <summary>
        /// Check Public Folder for Read + Write Permissions
        /// </summary>
        /// <param name="path">Represents the path </param>
        /// <returns>true if the folder have read+write permission</returns>
        protected virtual bool CheckFolderPermissions(string path)
        {
            ZnodeLogging.LogMessage("CheckFolderPermissions: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            FileInfo file = null;
            FileStream filestream = null;
            try
            {
                file = new FileInfo(HttpContext.Current.Server.MapPath(path) + "test.txt");
                filestream = file.Create();

                ZnodeLogging.LogMessage("CheckFolderPermissions: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error while checking folder permissions", logComponentName, TraceLevel.Error);
                return false;
            }
            finally
            {
                filestream.Close();
                file.Delete();
            }
        }

        /// <summary>
        /// To build diagnostics model
        /// </summary>
        /// <param name="category"></param>
        /// <param name="item"></param>
        /// <param name="status"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        protected virtual DiagnosticsModel BuildModel(string category, string item, bool status) => new DiagnosticsModel() { Category = category, Item = item, Status = status };

        /// <summary>
        /// Check if elastic search is working
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckElasticSearch()
        {
            try
            {
                ZnodeLogging.LogMessage("CheckElasticSearch: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

                string elasticSearchStatus = GetService<IZnodeSearchProvider>().CheckElasticSearch();
                return string.IsNullOrWhiteSpace(elasticSearchStatus) ||
                    elasticSearchStatus.ToLower().Equals(ZnodeDiagnosticsEnum.Red.ToString(), StringComparison.OrdinalIgnoreCase)
                    ? false : true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeDiagnosticsEnum.Diagnostics.ToString(), TraceLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// Method to check if Mongo DB cluster is active
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        protected virtual bool CheckMongoDb(string cluster)
        {
            try
            {
                ZnodeLogging.LogMessage("CheckMongoDb: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

                MongoClient client = new MongoClient(cluster);
                var state = client.Cluster.Description.State;

                ZnodeLogging.LogMessage("CheckMongoDb: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
                return client?.Cluster?.Description?.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected ? true : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeDiagnosticsEnum.Diagnostics.ToString(), TraceLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// To check if host in URL is active or not
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual bool CheckIfHostIsActive(string url)
        {
            try
            {
                ZnodeLogging.LogMessage("CheckIfHostIsActive: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

                return new Ping().Send(new Uri(url).Host).Status != IPStatus.Success ? false : true;
            }
            catch (Exception)
            {
                //No need to throw exception here
                return false;
            }
        }

        /// <summary>
        /// To check status of load balancer nodes
        /// </summary>
        /// <param name="diagnosticListModel"></param>
        /// <param name="featureSubValues"></param>
        /// <param name="domain"></param>
        protected virtual void CheckNodesStatus(DiagnosticsListModel diagnosticListModel, GlobalSettingValues featureSubValues, Diagnostic domain)
        {
            ZnodeLogging.LogMessage("CheckNodesStatus: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            if (!string.IsNullOrEmpty(featureSubValues?.Value1))
            {
                string[] array = featureSubValues.Value1.Split('~');
                for (int i = 0; i < array.Length; i++)
                {
                    if (!string.IsNullOrEmpty(array[i]))
                    {
                        diagnosticListModel.DiagnosticsList.Add(BuildModel(domain.Category, $"{domain.Description}  {i + 1}", CheckIfHostIsActive(array[i])));
                    }
                }
            }
            else
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(domain.Category, $"Load balancer is not configured for {domain.Type}", false));
            }

            ZnodeLogging.LogMessage("CheckNodesStatus: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
        }

        /// <summary>
        /// To check status of MongoDB 
        /// </summary>
        /// <param name="diagnosticListModel"></param>
        /// <param name="setting"></param>
        protected virtual void CheckMongoDb(DiagnosticsListModel diagnosticListModel, Diagnostic setting)
        {
            ZnodeLogging.LogMessage("CheckMongoDb: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            List<string> clusters = ConfigurationManager.ConnectionStrings["ZnodeMongoDBForLog"].ConnectionString.Split(',').ToList();
            if (clusters.Any())
            {
                for (int i = 0; i < clusters.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(clusters[i]))
                    {
                        if (clusters.Count > 1)
                            diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, $"{setting.Description} { i + 1}", CheckMongoDb(clusters[i])));
                        else
                            diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, $"{setting.Description}", CheckMongoDb(clusters[i])));
                    }
                }
            }

            ZnodeLogging.LogMessage("CheckMongoDb: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
        }

        /// <summary>
        /// Fill return list for diagnostics model
        /// </summary>
        /// <param name="diagnosticListModel"></param>
        /// <param name="setting"></param>
        protected virtual void FillDiagnosticList(DiagnosticsListModel diagnosticListModel, Diagnostic setting)
        {
            ZnodeLogging.LogMessage("FillDiagnosticList: Execution Started.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);

            if (setting.Type.Equals(ZnodeDiagnosticsEnum.SqlServer.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckSqlConnection()));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.MongoDb.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                CheckMongoDb(diagnosticListModel, setting);
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.Es.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckElasticSearch()));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.Permission.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckFolderPermissions("~/Data/Default/")));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.Smtp.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckEmailAccount()));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.ApiRoot.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckIfHostIsActive(ZnodeAdminSettings.ZnodeApiRootUri)));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.PaymentRoot.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckIfHostIsActive(ZnodeAdminSettings.PaymentApplicationUrl)));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.ArtifiRoot.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckIfHostIsActive(ConfigurationManager.AppSettings["ArtifiImageURLStartPath"])));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.CalenderCenter.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckIfHostIsActive(ConfigurationManager.AppSettings["CalendarCenterApi"])));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.Pec.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diagnosticListModel.DiagnosticsList.Add(BuildModel(setting.Category, setting.Description, CheckIfHostIsActive(ConfigurationManager.AppSettings["PECApi"])));
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.Webstore.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                CheckNodesStatus(diagnosticListModel, DefaultGlobalConfigSettingHelper.DefaultClearLoadBalancerWebStoreCacheIPs, setting);
            }
            else if (setting.Type.Equals(ZnodeDiagnosticsEnum.Api.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                CheckNodesStatus(diagnosticListModel, DefaultGlobalConfigSettingHelper.DefaultClearLoadBalancerAPICacheIPs, setting);
            }

            ZnodeLogging.LogMessage("FillDiagnosticList: Execution Done.", ZnodeLogging.Components.Diagnostics.ToString(), TraceLevel.Info);
        }

        #endregion
    }
}
