using System.Web;
using Znode.Multifront.PaymentFramework.Bussiness;

namespace Znode.Multifront.PaymentApplication.Helpers
{
    public class LicenceHelper
    {
        protected string DomainName = string.Empty;
        private string _customerIpAddress = HttpContext.Current.Request.UserHostAddress;

        //This method is used to install License on machine.
        public bool InstallLicense(string licenseType, string serialNumber, string name, string email, out string errorMessage)
        {
            DomainName = HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath;

            // Install the license
            ZnodeLicenseManager licenseMgr = new ZnodeLicenseManager();
            bool licenseInstalled = licenseMgr.InstallLicense(GetLicenseType(licenseType), serialNumber, name, email, out errorMessage);

            if (licenseInstalled)
                ZnodeLogging.LogMessage($"{_customerIpAddress},{DomainName},{ name},{email} Your Znode license has been successfully activated.");
            else
                ZnodeLogging.LogMessage($"{_customerIpAddress},{DomainName},{ name},{email} Failed to activate license.");
            return licenseInstalled;
        }

        //This method is used to Update License on machine.
        public bool UpdateLicense(out string errorMessage)
        {
            string error = string.Empty;
            ZnodeLicenseManager licenseMgr = new ZnodeLicenseManager();
            bool licenseUpdated = licenseMgr.UpdateLicense(out error);
            errorMessage = error;
            if (licenseUpdated)
                ZnodeLogging.LogMessage($"{ _customerIpAddress} Your Znode license has been successfully updated.");
            else
                ZnodeLogging.LogMessage($"{ _customerIpAddress} Failed to update license.");
            return licenseUpdated;
        }

        //Checks licensetype received and returns licencetype value saved in enum.
        private static ZnodeLicenseType GetLicenseType(string licenseType)
        {
            ZnodeLicenseType trialLicense = ZnodeLicenseType.Trial; // Default

            if (Equals(licenseType, "API"))
                trialLicense = ZnodeLicenseType.Production;
            return trialLicense;
        }
    }
}
