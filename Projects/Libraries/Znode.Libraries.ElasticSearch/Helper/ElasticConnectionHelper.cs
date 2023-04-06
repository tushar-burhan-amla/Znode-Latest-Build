using Elasticsearch.Net;
using Nest;
using System;
using Znode.Libraries.ECommerce.Utilities;
using System.Web;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public static class ElasticConnectionHelper 
    {
        /// <summary>
        /// Get the Elastic client based on the number of connections.
        /// </summary>
        /// <returns>ConnectionSettings</returns>
        public static ConnectionSettings GetElasticConnectionSetting()
        {
            string uri = ZnodeApiSettings.ElasticSearchRootUri;
            var uris = uri.Split(',');
            ConnectionSettings setting = null;
            if (uris.Length > 1)
            {
                var elasticUris = new Uri[uris.Length];
                for (int iUrls = 0; iUrls < uris.Length; iUrls++)
                {
                    elasticUris[iUrls] = new Uri(uris[iUrls]);
                }
                var connectionPool = new StaticConnectionPool(elasticUris);
                setting = new ConnectionSettings(connectionPool).DisableDirectStreaming();
            }
            else
            {
                Uri node = new Uri(string.IsNullOrEmpty(uri) ? "http://localhost:9200" : uri);
                setting = new ConnectionSettings(node).DisableDirectStreaming();
            }
            if (HelperUtility.IsNotNull(setting))
                setting.BasicAuthentication(ZnodeApiSettings.ElasticSearchUsername, ZnodeApiSettings.ElasticSearchPassword);
            return setting;
        }


        public static ElasticClient ElasticSearchClient
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string objectContextKey = "search_" + HttpContext.Current.GetHashCode().ToString("x");
                    if (!HttpContext.Current.Items.Contains(objectContextKey))
                    {
                        ConnectionSettings settings = GetElasticConnectionSetting().DefaultMappingFor<SearchCMSPages>(m => m.IdProperty(p => p.indexid)).RequestTimeout(TimeSpan.FromMinutes(20));
                        ElasticClient _client = new ElasticClient(settings);
                        HttpContext.Current.Items.Add(objectContextKey, _client);
                        return HttpContext.Current.Items[objectContextKey] as ElasticClient;
                    }
                    else
                        return HttpContext.Current.Items[objectContextKey] as ElasticClient;

                }
                else
                    return new ElasticClient(GetElasticConnectionSetting().DefaultMappingFor<SearchCMSPages>(m => m.IdProperty(p => p.indexid)).RequestTimeout(TimeSpan.FromMinutes(20)));
            }
        }
    }
}
