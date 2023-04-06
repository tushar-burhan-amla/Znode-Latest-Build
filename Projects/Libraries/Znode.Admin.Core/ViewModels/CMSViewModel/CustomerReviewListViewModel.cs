using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CustomerReviewListViewModel : BaseViewModel
    {
        public List<CustomerReviewViewModel> CustomerReviewList { get; set; }

        public GridModel GridModel { get; set; }

        public CustomerReviewListViewModel()
        {
            CustomerReviewList = new List<CustomerReviewViewModel>();
        }
    }
}