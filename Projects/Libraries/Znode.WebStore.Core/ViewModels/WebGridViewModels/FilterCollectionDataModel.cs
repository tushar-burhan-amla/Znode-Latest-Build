using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class FilterCollectionDataModel : BaseViewModel
    {
        public Dictionary<string, string> Params { get; set; }

        public List<string> LinkPermission { get; set; }

        public string ViewMode { get; set; } = "List";

        public bool IsMultiSelectList { get; set; }
        public FilterCollectionDataModel()
        {
            Params = new Dictionary<string, string>();
        }
    }
}