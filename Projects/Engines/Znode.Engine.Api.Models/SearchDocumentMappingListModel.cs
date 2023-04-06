using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchDocumentMappingListModel : BaseListModel
    {
        public SearchDocumentMappingListModel()
        {
            SearchDocumentMappingList = new List<SearchDocumentMappingModel>();
        }

        public List<SearchDocumentMappingModel> SearchDocumentMappingList { get; set; }
    }
}
