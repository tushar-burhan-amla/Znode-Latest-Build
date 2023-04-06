using System.Diagnostics;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class LicenseService : ILicenseService
    {
        protected string DomainName = string.Empty;
        private readonly string _customerIpAddress = HttpContext.Current.Request.UserHostAddress;

        //This method is used to install License on machine.
        public bool InstallLicense(string licenseType, string serialNumber, string name, string email, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Input Parameters licenseType,serialNumber,name,email:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { licenseType, serialNumber, name, email });
            DomainName = HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath;

            // Install the license
            ZnodeLicenseManager licenseMgr = new ZnodeLicenseManager();

            ZnodeLogging.LogMessage("Parameter for getting licenseInstalled", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { "method GetLicenseType", serialNumber , name , email });

            bool licenseInstalled = licenseMgr.InstallLicense(GetLicenseType(licenseType), serialNumber, name, email, out errorMessage);

            if (licenseInstalled)
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.ActivationSuccess, _customerIpAddress, DomainName, name, email, "Your Znode license has been successfully activated.");
            else
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.ActivationFailed, _customerIpAddress, DomainName, name, email, "Failed to activate license.");
            ZnodeLogging.LogMessage("licenseInstalled:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, new object[] { licenseInstalled });
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
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.ActivationSuccess, _customerIpAddress, "Your Znode license has been successfully updated.");
            else
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.ActivationFailed, _customerIpAddress, "Failed to update license.");
            return licenseUpdated;
        }

        //Get the Installed License Information.
        public LicenceInfoModel GetLicenceInformation()
        {
            LicenceInfoModel licenceModel = new LicenceInfoModel();

            ZnodeLicenseManager manager = new ZnodeLicenseManager();
            //Get License Information
            AdditionalInfo licenceInfo = manager.GetAdditionalInfo();
            if (HelperUtility.IsNotNull(licenceInfo) && !string.IsNullOrEmpty(licenceInfo.LicenseType))
            {
                licenceModel.LicenseType = licenceInfo.LicenseType;
                licenceModel.InstallationDate = licenceInfo.InstallationDate;
                licenceModel.SerialKey = licenceInfo.SerialKey;
                licenceModel.ExpirationDate = licenceInfo.ExpirationDate;
            }
            return licenceModel;
        }

        //Checks licensetype received and returns licencetype value saved in enum.
        private static ZnodeLicenseType GetLicenseType(string licenseType)
        {
            ZnodeLogging.LogMessage("Input Parameters licenseType:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, licenseType);
            ZnodeLicenseType trialLicense = ZnodeLicenseType.Trial; // Default

            if (Equals(licenseType, "MallAdmin"))
                trialLicense = ZnodeLicenseType.MallAdmin;
            else if (Equals(licenseType, "Marketplace"))
                trialLicense = ZnodeLicenseType.Marketplace;
            else if (Equals(licenseType, "Multifront"))
                trialLicense = ZnodeLicenseType.Multifront;
            else if (Equals(licenseType, "SingleFront"))
                trialLicense = ZnodeLicenseType.SingleFront;
            return trialLicense;
        }
    }
}