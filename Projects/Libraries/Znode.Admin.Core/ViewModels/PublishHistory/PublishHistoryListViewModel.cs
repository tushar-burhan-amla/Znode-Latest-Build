using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PublishHistoryListViewModel : BaseViewModel
    {
        public List<PublishHistoryViewModel> PublishHistoryList { get; set; }
        public GridModel GridModel { get; set; }

        public PublishHistoryListViewModel()
        {
            PublishHistoryList = new List<PublishHistoryViewModel>();
        }
    }
}
