using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductFeedListViewModel : BaseViewModel
    {
        public List<ProductFeedViewModel> ProductFeeds { get; set; }
        public GridModel GridModel { get; set; }
    }
}
