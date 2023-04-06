using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    /// <summary>
    /// Product History List Model.
    /// </summary>
    public class ProductHistoryListModel : BaseListModel
    {
        public List<ProductHistoryModel> ProductHistoryList { get; set; }

        public ProductHistoryListModel()
        {
            ProductHistoryList = new List<ProductHistoryModel>();
        }
    }
}
