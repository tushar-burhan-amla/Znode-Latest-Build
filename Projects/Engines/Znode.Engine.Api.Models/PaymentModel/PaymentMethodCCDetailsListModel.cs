using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PaymentMethodCCDetailsListModel:BaseListModel
    {
        public PaymentMethodCCDetailsListModel()
        {
            PaymentMethodCCDetails = new List<PaymentMethodCCDetailsModel>();
        }
        public List<PaymentMethodCCDetailsModel> PaymentMethodCCDetails { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public bool IsSuccess { get; set; }
    }
}
