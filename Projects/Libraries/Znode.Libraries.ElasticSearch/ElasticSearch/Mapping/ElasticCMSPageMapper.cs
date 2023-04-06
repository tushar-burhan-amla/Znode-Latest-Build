using Nest;
using System.Linq;
using Znode.Libraries.Search;
using System;

namespace Znode.Libraries.ElasticSearch
{
    public static class ElasticCMSPageMapper
    {
        #region Public Method

        //Map data of search result from elastic search to require model
        public static ElasticCMSPageSearchResponse MapSearchResponseToCMSPageSearchResponse(ISearchResponse<dynamic> searchResponse)
        {
            ElasticCMSPageSearchResponse elasticCMSPageSearchResponse = new ElasticCMSPageSearchResponse();

            BindSearchRequestQuery(elasticCMSPageSearchResponse, searchResponse.DebugInformation);
     
            elasticCMSPageSearchResponse.TotalCMSPageCount = Convert.ToInt32(searchResponse.Total);
            elasticCMSPageSearchResponse.CMSPageDetails = searchResponse.Documents?.ToList();

            return elasticCMSPageSearchResponse;
        }

        //Map data of search result from elastic search to require model
        public static ElasticCMSPageSearchResponse MapCountResponseToCMSPageSearchResponse(CountResponse countResponse)
        {
            ElasticCMSPageSearchResponse elasticCMSPageSearchResponse = new ElasticCMSPageSearchResponse();

            BindSearchRequestQuery(elasticCMSPageSearchResponse, countResponse.DebugInformation);
           
            elasticCMSPageSearchResponse.TotalCMSPageCount = Convert.ToInt32(countResponse.Count);

            return elasticCMSPageSearchResponse;
        }
        #endregion

        #region Private Method
        //Bind request query from search response
        private static void BindSearchRequestQuery(ElasticCMSPageSearchResponse elasticCMSPageSearchResponse, string debugInformation)
        {
            string startTag = "# Request:";
            int startIndex = debugInformation.IndexOf(startTag) + startTag.Length;
            int endIndex = debugInformation.IndexOf("# Response:", startIndex);
            string requestbody = debugInformation.Substring(startIndex, endIndex - startIndex);

            elasticCMSPageSearchResponse.RequestBody = requestbody;
        }
        #endregion
    }
}
