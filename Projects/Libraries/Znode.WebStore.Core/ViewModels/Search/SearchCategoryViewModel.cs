using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class SearchCategoryViewModel : BaseViewModel
    {
        public long Count { get; set; }
        public List<SearchCategoryViewModel> Hierarchy { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Categoryurl { get; set; }
    }
}