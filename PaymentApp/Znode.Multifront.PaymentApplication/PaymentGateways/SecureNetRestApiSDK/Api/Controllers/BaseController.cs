using System;
using System.Configuration;
using System.Collections.Generic;
using SecureNetRestApiSDK.Api.Requests;
using Newtonsoft.Json;
using SNET.Core;

namespace SecureNetRestApiSDK.Api.Controllers
{
    public abstract class BaseController
    {
        #region Attributes
        #endregion

        #region Methods

        public T ProcessRequest<T>(APIContext apiContext, SecureNetRequest secureNetRequest)
        {
            if (secureNetRequest == null)
            {
                throw new ArgumentNullException("secureNetRequest");
            }

            string payLoad = JsonConvert.SerializeObject(secureNetRequest);

            return Helper.ConfigureAndExecute<T>(apiContext, secureNetRequest.GetMethod(), secureNetRequest.GetUri(), payLoad);
        }

        #endregion
    }
}
