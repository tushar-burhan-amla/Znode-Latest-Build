using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchSynonymsListModel : BaseListModel
    {
        public List<SearchSynonymsModel> SynonymsList { get; set; }
    }
}
