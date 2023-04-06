using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchAttributesListModel : BaseListModel
    {
        public List<SearchAttributesModel> SearchAttributeList { get; set; }
    }
}
