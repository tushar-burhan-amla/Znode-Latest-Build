using Microsoft.SqlServer.ReportingServices2005;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Libraries.Admin
{
    public class ReportUploader
    {
        #region Private Variables
        private readonly string Username = ConfigurationManager.AppSettings["MvcReportViewer.Username"].ToString();
        private readonly string Password = ConfigurationManager.AppSettings["MvcReportViewer.Password"].ToString();
        private readonly string DomainName = ConfigurationManager.AppSettings["ReportServerDomain"].ToString();
        private readonly string Url = ConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"].ToString();
        private readonly string FolderPath = ConfigurationManager.AppSettings["ReportServerDynamicReportFolderName"].ToString(); 
        #endregion

        #region public Method

        /// <summary>
        /// Upload the RDL file on Report Server
        /// </summary>
        /// <param name="doc">XmlDocument</param>
        /// <param name="reportName">reportName</param>
        /// <returns>True if Uploaded else false</returns>
        public virtual bool UploadRDLReport(XmlDocument doc, string reportName, out int errorCode)
        {
            bool isReportDeployed = true;

            ReportingService2005 service = new ReportingService2005();
            service.Credentials = new System.Net.NetworkCredential(Username, Password, DomainName);
            service.Url = $"{Url}{"/reportservice2005.asmx?"}" ;

            Byte[] definition = null;
            Warning[] warnings = null;

            try
            {
                definition = Encoding.Default.GetBytes(doc.OuterXml);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Error,ex);
            }

            try
            {
                warnings = service.CreateReport($"{reportName}{".rdl"}", $"{"/"}{FolderPath}", false, definition, null);

                if (IsNotNull(warnings))
                {
                    isReportDeployed = false;
                    foreach (Warning warning in warnings)
                        ZnodeLogging.LogMessage(warning.Message, ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Error);
                }
                else
                {
                    isReportDeployed = true;
                    ZnodeLogging.LogMessage($"Report: {reportName.Trim()}.rdl created successfully with no warnings", ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Info);
                }
                errorCode = 0;
            }
            catch (SoapException ex)
            {
                isReportDeployed = false;
                if (ex.Message.ToString().IndexOf(reportName) > 0 && ex.Message.ToString().Contains("already"))
                    errorCode = 2;
                else
                    errorCode = 19;
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Error,ex);
            }
            catch (Exception ex)
            {
                isReportDeployed = false;
                errorCode = 20;
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Error);
            }

            return isReportDeployed;
        }

        /// <summary>
        /// Delete the dynamic report from reporting server
        /// </summary>
        /// <param name="reportName">reportName</param>
        /// <returns>bool</returns>
        public virtual bool DeleteReport(string reportName)
        {
            bool isReportDeleted = false;
            ReportingService2005 service = new ReportingService2005();
            service.Credentials = new System.Net.NetworkCredential(Username, Password, DomainName);
            service.Url = $"{Url}{"/reportservice2005.asmx?"}";

            try
            {
                service.DeleteItem($"/{FolderPath}/{reportName}.rdl");
                isReportDeleted = true;
            }
            catch (SoapException ex)
            {
                isReportDeleted = false;
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Error,ex);
            }
            catch (Exception ex)
            {
                isReportDeleted = false;
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.DynamicReports.ToString(),TraceLevel.Error,ex);
            }

            return isReportDeleted;

        }
        #endregion
    }
}
