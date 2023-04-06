using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class BlogNewsListViewModel : BaseViewModel
    {
        public List<BlogNewsViewModel> BlogNewsList { set; get; }

        public string BlogNewsType { get; set; }
    }
}