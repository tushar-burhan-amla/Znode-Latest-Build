using Nest;
using System.Collections.Generic;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class ElasticCMSPageSearchRequest : IZnodeCMSPageSearchRequest
    {
        public int PortalId { get; set; }

        public int PageFrom { get; set; } = 0;

        public int PageSize { get; set; } = 10;

        public int StartIndex { get; set; }

        public int LocaleId { get; set; }

        public int ProfileId { get; set; }

        public string SearchText { get; set; }

        public string IndexName { get; set; }

        public string QueryClass { get; set; }

        public string Operator { get; set; }

        public Dictionary<string, List<string>> FilterDictionary { get; set; }

        public List<QueryContainer> FilterValues { get; set; }

        public List<ElasticSearchAttributes> SearchableAttribute { get; set; }

        public string[] Source { get; set; }

    }
}
