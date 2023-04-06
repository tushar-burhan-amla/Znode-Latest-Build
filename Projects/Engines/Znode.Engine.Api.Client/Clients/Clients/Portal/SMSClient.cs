using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class PortalSMSClient: BaseClient, IPortalSMSClient
    {
        public virtual SMSModel GetSMSSetting(int portalId,bool isSMSSettingEnabled = false)
        {
            string endpoint = SMSEndpoint.GetSMSSetting(portalId,isSMSSettingEnabled);
            ApiStatus status = new ApiStatus();
            SMSResponse response = GetResourceFromEndpoint<SMSResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Sms;
        }
        public virtual List<SMSProviderModel> GetSmsProviderList()
        {
            string endpoint = SMSEndpoint.GetSMSProviderList();
            ApiStatus status = new ApiStatus();
            SMSResponse response = GetResourceFromEndpoint<SMSResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SmsProviderModelList;
        }
        public virtual SMSModel InsertUpdateSMSSetting(SMSModel smsModel)
        {
            string endpoint = SMSEndpoint.InsertUpdateSMSSetting();
            ApiStatus status = new ApiStatus();
            SMSResponse response = PutResourceToEndpoint<SMSResponse>(endpoint, JsonConvert.SerializeObject(smsModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.Sms;
        }

    }
}
