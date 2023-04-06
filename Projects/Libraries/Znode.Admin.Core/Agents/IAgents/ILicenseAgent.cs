using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface ILicenseAgent
    {
        /// <summary>
        /// Gets the Znode Installed License Information
        /// </summary>
        /// <returns>Return Installed License Information</returns>
        LicenceInfoViewModel GetLicenseInformation();
    }
}
