using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchableColumnListModel
    {
        public string FilteredValue { get; set; }

        public List<string> SearchableColumn { get; set; }

        public string SearchableColumnName { get; set; }
    }
}
