using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Znode.Libraries.SAMLWeb
{
    /// <summary>
    /// Automatically adds the provided OAuthToken to every request
    /// </summary>
    public class OAuthCodeHttpHandler : HttpClientHandler
    {
        private readonly string username;
        private readonly string samlToken;

        public OAuthCodeHttpHandler(string username, string samlToken)
        {
            this.username = username;
            this.samlToken = samlToken;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string credentials = username + ":" + samlToken;

            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials)));

            return base.SendAsync(request, cancellationToken);
        }
    }
}