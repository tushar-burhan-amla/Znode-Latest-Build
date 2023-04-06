using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CategoryHistoryListResponse : BaseListResponse
    {
        public List<CategoryHistoryModel> CategoryHistories { get; set; }
    }
}
