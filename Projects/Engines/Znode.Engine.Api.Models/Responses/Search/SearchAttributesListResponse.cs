using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchAttributesListResponse : BaseListResponse
    {
        public List<SearchAttributesModel> SearchAttributesList { get; set; }
    }
}
