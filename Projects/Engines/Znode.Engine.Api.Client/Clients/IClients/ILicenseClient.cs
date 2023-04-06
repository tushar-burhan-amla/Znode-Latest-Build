using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ILicenseClient: IBaseClient
    {
        /// <summary>
        /// Get Installed Znode License Information.
        /// </summary>
        /// <returns>LicenceInfoModel</returns>
        LicenceInfoModel GetLicenseInformation();
    }
}
