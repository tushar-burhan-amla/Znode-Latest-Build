using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IDiagnosticsService
    {
        /// <summary>
        /// This method checks SMTP Account
        /// </summary>
        /// <returns>status of SMPT account</returns>
        bool CheckEmailAccount();

        /// <summary>
        /// This method gets Version details of product from database
        /// </summary>
        /// <returns>Returns the version details</returns>
        string GetProductVersionDetails();

        /// <summary>
        /// This method sends the diagnostics email
        /// </summary>
        /// <param name="model">DiagnosticsEmailModel which should contain Case number for diagnostics</param>
        /// <returns>Returns true the email sent status otherwise false</returns>
        bool EmailDiagnostics(DiagnosticsEmailModel model);


        /// <summary>
        /// This method check Service working or stopped. 
        /// </summary>
        /// <param name="serviceName">service Name</param>
        /// <returns>return status</returns>
        string CheckService(string serviceName);

        /// <summary>
        /// This method get the status of various components
        /// </summary>
        /// <returns></returns>
        DiagnosticsListModel GetDiagnosticsList();

        /// <summary>
        /// Check for Database connection String
        /// </summary>
        /// <returns>Returns the status of database connection</returns>
        bool CheckSqlConnection();

        /// <summary>
        /// Check for proper MongoDB Connection String
        /// </summary>
        /// <returns>Returns the status of database connection</returns>
        bool CheckMongoDBConnection(string settingKey);

        /// <summary>
        /// Check for proper MongoDB Connection String for logs
        /// </summary>
        /// <returns>Returns the status of database connection</returns>
        bool CheckMongoDBLogConnection();
    }
}
