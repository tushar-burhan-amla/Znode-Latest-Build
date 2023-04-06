using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    public class ElasticCMSPageSearchResponse : IZnodeCMSPageSearchResponse
    {
        public int TotalCMSPageCount { get; set; }

        public List<dynamic> CMSPageDetails { get; set; }

        public string RequestBody { get; set; }
    }
}



