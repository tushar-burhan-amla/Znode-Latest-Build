
namespace Znode.Engine.Api.Cache
{
    public interface IDiagnosticsCache
    {
        /// <summary>
        /// This method calls diagnostics service to check 'Simple Mail Transfer Protocol' Account
        /// </summary>
        /// <returns>DiagnosticsResponse in string format which will contain the status of SMPT account</returns>
        string CheckEmailAccount();

        /// <summary>
        /// This method calls diagnostics service to get Version details of product from database
        /// </summary>
        /// <returns>DiagnosticsResponse in string format which will contain the details of product version</returns>
        string GetProductVersionDetails();
    }
}
