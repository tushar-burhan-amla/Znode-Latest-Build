using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface ISMSService
    { 
        /// <summary>
      ///  Get SMS by portal Id.
      /// </summary>
      /// <param name="portalId">int smsId to get the sms.</param>
      /// <returns>DomainModel</returns>
        SMSModel GetSMSDetails(int portalId, bool isSMSSettingEnabled = false);

        /// <summary>
        ///  Get SMS Provide By PortalId.
        /// </summary>
        /// <param name="portalId">int smsId to get the sms.</param>
        /// <returns>DomainModel</returns>
        List<SMSProviderModel> GetSmsProviderList();

        /// <summary>
        ///  Update SMS details.
        /// </summary>
        /// <param name="smsModel">smsmodel to update sms.</param>
        /// <returns>SmsModel</returns>
        bool InsertUpdateSMSSetting(SMSModel smsModel);
    }


}
