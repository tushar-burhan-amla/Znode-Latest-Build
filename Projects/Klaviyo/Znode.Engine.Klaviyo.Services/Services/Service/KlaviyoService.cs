using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Znode.Engine.Exceptions;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Abstract.Services;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Klaviyo;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Klaviyo.Services
{
    public class KlaviyoService : BaseService, IKlaviyoService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalKlaviyoSetting> _znodeKlaviyoRepository;
        private readonly IZnodeRepository<ZnodeEmailProvider> _znodeEmailProviderRepository;
        #endregion

        #region Constructor
        public KlaviyoService()
        {
            _znodeKlaviyoRepository = new ZnodeRepository<ZnodePortalKlaviyoSetting>(HelperMethods.KlaviyoContext);
            _znodeEmailProviderRepository = new ZnodeRepository<ZnodeEmailProvider>(HelperMethods.KlaviyoContext);
        }
        #endregion

        #region Public Methods
        //Get Klaviyo Configuration details for the Portal.
        public virtual KlaviyoModel GetKlaviyo(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters portalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, portalId);
            //Get Klaviyo Configuration details for the portal.
            if (portalId <= 0)
                return null;

            KlaviyoModel klaviyoModel = KlaviyoMap.ToModel(_znodeKlaviyoRepository.Table?.FirstOrDefault(x => x.PortalId == portalId));

            if (HelperUtility.IsNotNull(klaviyoModel))
            {
                ZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();

                //Bind store name.
                klaviyoModel.PortalName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName;
            }
            ZnodeLogging.LogMessage("Klaviyo PortalName:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, klaviyoModel?.PortalName);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return klaviyoModel;

        }

        //Save the Klaviyo Configuration details for the Portal.
        public virtual bool UpdateKlaviyo(KlaviyoModel klaviyoModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters KlaviyoModel:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, klaviyoModel);
            if (HelperUtility.IsNull(klaviyoModel))
                throw new ZnodeException(ErrorCodes.NullModel, "Model cannot be null.");

            if (klaviyoModel?.PortalId > 0 && klaviyoModel?.PortalKlaviyoSettingId > 0)
            {
                //If Already Klaviyo Configuration Exists, then Update the Klaviyo Configuration.
                if (_znodeKlaviyoRepository.Update(KlaviyoMap.ToEntity(klaviyoModel)))
                {
                    ZnodeLogging.LogMessage(string.Format("Klaviyo Configuration for Portal Id is {0} Updated Successfully.", klaviyoModel.PortalId), string.Empty, TraceLevel.Info);
                    klaviyoModel = KlaviyoMap.ToModel(_znodeKlaviyoRepository.Table?.FirstOrDefault(x => x.PortalId == klaviyoModel.PortalId));
                    return true;
                }
            }
            else
            {
                //Insert the Klaviyo Configuration Details.
                ZnodePortalKlaviyoSetting klaviyo = _znodeKlaviyoRepository.Insert(KlaviyoMap.ToEntity(klaviyoModel));
                ZnodeLogging.LogMessage(string.Format("Klaviyo Configuration for Portal Id is {0} Inserted Successfully.", klaviyoModel.PortalId), string.Empty, TraceLevel.Info);
                klaviyoModel = KlaviyoMap.ToModel(klaviyo);
                return true;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return false;
        }

        // Check the response for the Klaviyo and Get the details for the Klaviyo
        public virtual bool KlaviyoIdentify(IdentifyModel userModel)
        {
            KlaviyoModel klaviyoModel = GetKlaviyo(userModel.PortalId.GetValueOrDefault());

            if (HelperUtility.IsNull(klaviyoModel) || HelperUtility.IsNull(userModel))
                return false;

            if (CheckValidKlaviyoDetails(klaviyoModel))
            {
                IConnectionHelper _connectionHelper = GetService<IConnectionHelper>();
                return _connectionHelper.GetKlaviyoIdentify(userModel, klaviyoModel.PublicApiKey);
            }
            return false;
        }

        //// Check the response for the Klaviyo and Get the details for the Klaviyo
        public virtual bool KlaviyoTrack(KlaviyoProductDetailModel klaviyoProductDetailModel)
        {
            KlaviyoModel klaviyoModel = GetKlaviyo(klaviyoProductDetailModel.PortalId.GetValueOrDefault());

            if (HelperUtility.IsNull(klaviyoModel) || HelperUtility.IsNull(klaviyoProductDetailModel))
                return false;

            if (CheckValidKlaviyoDetails(klaviyoModel))
            {
                IConnectionHelper _connectionHelper = GetService<IConnectionHelper>();
                _connectionHelper.GetKlaviyoTrack(KlaviyoMapper.ToOrderDetailsModel(klaviyoProductDetailModel), klaviyoModel.PublicApiKey);
            }
            return false;
        }

        // Check the Validation Details for the Klaviyo Configuration
        public virtual bool CheckValidKlaviyoDetails(KlaviyoModel klaviyoModel)
        {
            return klaviyoModel.IsActive && HelperUtility.IsNotNull(klaviyoModel.PublicApiKey);
        }
        // save Email provider details for klaviyo configuration
        public virtual List<EmailProviderModel> GetEmailProviderList()
        {
            var _entity = _znodeEmailProviderRepository.GetEntityList(string.Empty);

            return KlaviyoMap.ToEmailProviderModel(_entity);
        }

        #endregion
    }
}