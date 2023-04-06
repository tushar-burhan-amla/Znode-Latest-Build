using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class BlogNewsCommentListViewModel : BaseViewModel
    {
        public List<BlogNewsCommentViewModel> BlogNewsCommentList { set; get; }

        public int BlogNewsId { get; set; }

        public GridModel GridModel { get; set; }
    }
}
