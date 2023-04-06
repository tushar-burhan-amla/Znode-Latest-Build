using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class BlogNewsListViewModel: BaseViewModel
    {
        public List<BlogNewsViewModel> BlogNewsList { get; set; }

        public GridModel GridModel { get; set; }
    }
}
