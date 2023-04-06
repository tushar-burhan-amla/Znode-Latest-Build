using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchDocumentMappingListResponse : BaseListResponse
    {
        public List<SearchDocumentMappingModel> SearchDocumentMappingList { get; set; }
    }
}
