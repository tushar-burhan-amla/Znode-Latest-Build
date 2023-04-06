using System.Diagnostics;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class LicenseAgent:BaseAgent,ILicenseAgent
    {
        #region Private Variables
        private readonly ILicenseClient _licenseClient;
        #endregion

        #region Constructor
        public LicenseAgent(ILicenseClient licenseClient)
        {
            _licenseClient = GetClient<ILicenseClient>(licenseClient);
        }
        #endregion

        #region Public Methods
        public virtual LicenceInfoViewModel GetLicenseInformation()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            LicenceInfoViewModel licenseModel = null;
            //Get the Installed License Information.
            LicenceInfoModel licenseInfo = _licenseClient.GetLicenseInformation();
            if (HelperUtility.IsNotNull(licenseInfo) && !string.IsNullOrEmpty(licenseInfo.LicenseType))
            {
                licenseModel = licenseInfo.ToViewModel<LicenceInfoViewModel>();
                licenseModel.LicenseType = Admin_Resources.ResourceManager.GetString(licenseModel.LicenseType);
            }
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            return licenseModel;
        }
        #endregion
    }
}