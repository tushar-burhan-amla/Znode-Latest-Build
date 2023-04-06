using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CatalogHistoryListResponse : BaseListResponse
    {
        public List<CatalogHistoryModel> CatalogHistories { get; set; }
    }
}
