using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PaymentTypeListModel
    {
        public PaymentTypeListModel()
        {
            PaymentTypes = new List<PaymentTypeModel>();
        }
        public List<PaymentTypeModel> PaymentTypes { get; set; }
    }
}
