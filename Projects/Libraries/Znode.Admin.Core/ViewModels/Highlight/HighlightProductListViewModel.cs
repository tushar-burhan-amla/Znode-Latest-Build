using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class HighlightProductListViewModel : BaseViewModel
    {
        public List<HighlightProductViewModel> HighlightProductList { get; set; }
        public GridModel GridModel { get; set; }
        public int HighlightId { get; set; }
        public string Name { get; set; }
        public string HighlightName { get; set; }
        public int? LocaleId { get; set; }
    }
}