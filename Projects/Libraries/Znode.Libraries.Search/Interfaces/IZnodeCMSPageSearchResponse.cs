using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    public interface IZnodeCMSPageSearchResponse
    {
        int TotalCMSPageCount { get; set; }

        List<dynamic> CMSPageDetails { get; set; }

        string RequestBody { get; set; }
    }
}
