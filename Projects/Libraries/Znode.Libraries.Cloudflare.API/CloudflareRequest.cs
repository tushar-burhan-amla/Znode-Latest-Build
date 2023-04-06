using System;
using System.Diagnostics;
using Znode.Libraries.Cloudflare.API.Endpoints;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Cloudflare.API
{
    public class CloudflareRequest : ICloudflareRequest
    {
        readonly ClientRequest client;
        public CloudflareRequest()
        {
            client = new ClientRequest();
        }

        #region Public Methods
        /// <summary>
        /// Purge everything from cloudflare.
        /// </summary>
        /// <param name="zoneId">ZoneId which will send to Cloudflare URL</param>
        /// <returns>PurgeResponseModel</returns>
        public PurgeResponseModel PurgeEverything(string zoneId)
        {
            try
            {
                if (string.IsNullOrEmpty(zoneId))
                {
                    return new PurgeResponseModel() { Success = false, Errors = new PurgeResponseErrorModel[1] { new PurgeResponseErrorModel() { Message = "zoneId is empty.", Code = "000" } } };
                }
                string destinationURL = CloudflareEndpoints.CloudflareEndpoint(zoneId);
                PurgeAllModel purgeAll = new PurgeAllModel();
                return client.PostRequest<PurgeResponseModel, PurgeAllModel>(destinationURL, purgeAll);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.API.CloudflareRequest.PurgeEverything -> error " + ex.Message, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Purge cloudflare cache by Host Name.
        /// </summary>
        /// <param name="HostName">List of host names which needs to be purge</param>
        /// <param name="zoneId">ZoneId which will send to Cloudflare URL</param>
        /// <returns>PurgeResponseModel</returns>
        public PurgeResponseModel PurgeByHostName(HostListModel hostNames, string zoneId)
        {
            try
            {
                if (hostNames?.Hosts == null && hostNames?.Hosts.Count > 0)
                {
                    ZnodeLogging.LogMessage("No hosts were passed for Cloudflare purge.", "CloudflareRequest_PurgeByHostName", TraceLevel.Error);
                    return new PurgeResponseModel() { Success = false, Errors = new PurgeResponseErrorModel[1] { new PurgeResponseErrorModel() { Message = "No hosts were passed for Cloudflare purge.", Code = "000" } } };
                }
                string destinationURL = CloudflareEndpoints.CloudflareEndpoint(zoneId);
                return client.PostRequest<PurgeResponseModel, HostListModel>(destinationURL, hostNames);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.API.CloudflareRequest.PurgeByHostName error " + ex.Message, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Purge cloudflare cache by URLs.
        /// </summary>
        /// <param name="filesList">List of URLs which needs to be purge</param>
        /// <param name="zoneId">ZoneId which will send to Cloudflare URL</param>
        /// <returns>PurgeResponseModel</returns>
        public PurgeResponseModel PurgeByURLs(FilesListModel filesList, string zoneId)
        {
            try
            {
                if (filesList?.Files == null && filesList?.Files.Count > 0)
                {
                    ZnodeLogging.LogMessage("No hosts were passed for Cloudflare purge.", "CloudflareRequest_PurgeByHostName", TraceLevel.Error);
                    return new PurgeResponseModel() { Success = false, Errors = new PurgeResponseErrorModel[1] { new PurgeResponseErrorModel() { Message = "No URLs were passed for Cloudflare purge.", Code = "000" } } };
                }
                string destinationURL = CloudflareEndpoints.CloudflareEndpoint(zoneId);
                return client.PostRequest<PurgeResponseModel, FilesListModel>(destinationURL, filesList);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.API.CloudflareRequest.PurgeByURLs error " + ex.Message, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        #endregion
    }
}
