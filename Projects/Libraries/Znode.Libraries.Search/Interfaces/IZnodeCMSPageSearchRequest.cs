using Nest;
using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    public interface IZnodeCMSPageSearchRequest
    {
        int PortalId { get; set; }

        int PageFrom { get; set; }

        int PageSize { get; set; }

        int StartIndex { get; set; }

        int LocaleId { get; set; }

        int ProfileId { get; set; }

        string SearchText { get; set; }

        string IndexName { get; set; }

        string QueryClass { get; set; }

        string Operator { get; set; }

        string[] Source { get; set; }

        Dictionary<string, List<string>> FilterDictionary { get; set; }

        List<QueryContainer> FilterValues { get; set; }

        List<ElasticSearchAttributes> SearchableAttribute { get; set; }
    }
}
