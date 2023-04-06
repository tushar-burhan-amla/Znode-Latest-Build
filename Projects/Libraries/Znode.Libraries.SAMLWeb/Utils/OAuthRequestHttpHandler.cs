﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Znode.Libraries.SAMLWeb
{
    /// <summary>
    /// Automatically adds the provided OAuthToken to every request
    /// </summary>
    public class OAuthRequestHttpHandler : HttpClientHandler
    {
        private readonly string OAuthToken;

        public OAuthRequestHttpHandler(string OAuthToken)
        {
            this.OAuthToken = OAuthToken;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", "Bearer " + OAuthToken);

            return base.SendAsync(request, cancellationToken);
        }
    }
}