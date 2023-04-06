using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class HighlightListViewModel : BaseViewModel
    {
        public List<HighlightViewModel> HighlightList { get; set; }
        public GridModel GridModel { get; set; }
        public List<ProductDetailsViewModel> ProductDetailList { get; set; }
        public int HighlightId { get; set; }
        public string HighlightCode { get; set; }
        public int LocaleId { get; set; }
        public string HighlightName { get; set; }
    }
}