using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IPortalSMSClient : IBaseClient
    {
        /// <summary>
        /// Get SMSDetail by portalId
        /// </summary>
        /// <param name="portalId">int portalId to get SMS</param>
        /// <returns>SMS Model</returns>
        SMSModel GetSMSSetting(int portalId,bool isSMSSettingEnabled = false);

        /// <summary>
        /// Get SMS Provide List by portalId
        /// </summary>
        /// <param name="portalId">int portalId to get SMS</param>
        /// <returns>List SMS Model</returns>
       List<SMSProviderModel> GetSmsProviderList();

        /// <summary>
        /// Update SMSDetail
        /// </summary>
        /// <param name="smsModel">SMSModel to update SMS</param>
        /// <returns>SMS Model</returns>
        SMSModel InsertUpdateSMSSetting(SMSModel smsModel);


    }
}
