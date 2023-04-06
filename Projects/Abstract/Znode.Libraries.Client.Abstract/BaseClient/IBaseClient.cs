using System.Collections.Generic;

using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Abstract.Client
{
    public interface IBaseClient
    {
        int UserId { get; set; }
        bool RefreshCache { get; set; }
        int LoginAs { get; set; }
        string Custom1 { get; set; }
        string Custom2 { get; set; }
        string Custom3 { get; set; }
        string Custom4 { get; set; }
        string Custom5 { get; set; }
        int RequestTimeout { get; set; }
        /// <summary>
        /// Sets the supplied publish state in the client instances' PublishStateHeader property for the cases when a client class initialization
        /// occurs before the CurrentPortal global variable has it's updated value.
        /// </summary>
        /// <param name="publishState"></param>
        void SetPublishStateExplicitly(ZnodePublishStatesEnum publishState);
        /// <summary>
        /// Sets the supplied locale Id in the client instances' LocaleHeader property for the cases when a client class initialization
        /// occurs before the CurrentPortal global variable has it's updated value.
        /// </summary>
        /// <param name="localeId"></param>
        void SetLocaleExplicitly(int localeId);
        /// <summary>
        /// Sets the supplied domain header in the client instances' DomainHeader property explicitly for Multifront environments to avoid
        /// domain name conflicts in the API requests.
        /// </summary>
        /// <param name="domainName"></param>
        void SetDomainHeaderExplicitly(string domainName);

        /// <summary>
        /// Sets the supplied profile Id in the client instances' ProfileHeader property for the cases when a client class initialization
        /// profile Id is present then the product pricing data will show on the basis of profile.
        /// </summary>
        /// <param name="profileId"></param>
        void SetProfileIdExplicitly(int profileId);

        /// <summary>
        /// Set Custom Headers
        /// </summary>
        /// <param name="headers">Headers for client</param>
        void SetCustomHeadersExplicitly(Dictionary<string, string> headers);
    }
}