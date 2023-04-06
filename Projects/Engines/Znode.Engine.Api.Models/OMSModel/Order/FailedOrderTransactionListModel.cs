using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FailedOrderTransactionListModel : BaseListModel
    {
        public List<FailedOrderTransactionModel> FailedOrderTransactionModel { get; set; }
    }
}
