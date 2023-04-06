using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CustomerReviewListModel : BaseListModel
    {
        public List<CustomerReviewModel> CustomerReviewList { get; set; }

        public CustomerReviewListModel()
        {
            CustomerReviewList = new List<CustomerReviewModel>();
        }
    } 
}
