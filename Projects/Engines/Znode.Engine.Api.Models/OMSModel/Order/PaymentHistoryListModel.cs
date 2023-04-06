using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Api.Models
{
    public class PaymentHistoryListModel : BaseListModel
    {
        public List<PaymentHistoryModel> PaymentHistoryList { get; set; }

        public PaymentHistoryListModel()
        {
            PaymentHistoryList = new List<PaymentHistoryModel>();
        }
    }
}
