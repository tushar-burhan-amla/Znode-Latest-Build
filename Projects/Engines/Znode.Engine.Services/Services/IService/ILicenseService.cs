using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface ILicenseService
    {
        /// <summary>
        /// This method is used to install License on machine
        /// </summary>
        /// <param name="licenseType">Type of License u need to install</param>
        /// <param name="serialNumber">License Serial Number</param>
        /// <param name="name">User name</param>
        /// <param name="email">User Email Address</param>
        /// <param name="errorMessage">Error message while installing license</param>
        /// <returns>Boolean result</returns>
        bool InstallLicense(string licenseType, string serialNumber, string name, string email, out string errorMessage);

        /// <summary>
        ///  This method is used to Update License on machine
        /// </summary>
        /// <param name="errorMessage">Error message while updating license</param>
        /// <returns>Boolean result</returns>
        bool UpdateLicense(out string errorMessage);

        /// <summary>
        /// Get the Installed License Information.
        /// </summary>
        /// <returns>Return the License Information in LicenceInfoModel Format.</returns>
        LicenceInfoModel GetLicenceInformation();
    }
}
