using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Api.Cache
{
    public interface ISMSCache
    {  /// <summary>
       /// Get SMTP details
       /// </summary>
       /// <param name="portalId">int portalId to get smtp</param>
       /// <param name="routeUri">URI to Route.</param>
       /// <param name="routeTemplate">Template to Route.</param>
       /// <param name="isSMSSettingEnabled"></param>
       /// <returns></returns>
        string GetSMSDetails(int portalId, string routeUri, string routeTemplate, bool isSMSSettingEnabled = false);

        /// <summary>
        /// Get SMS details
        /// </summary>
        /// <param name="portalId">int portalId to get smtp</param>
        /// <param name="routeUri">URI to Route.</param>
        /// <param name="routeTemplate">Template to Route.</param>
        /// <returns></returns>
        string GetSmsProviderList(string routeUri, string routeTemplate);
    }
}
