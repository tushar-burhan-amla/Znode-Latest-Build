using System;

namespace Znode.Multifront.PaymentApplication.Models
{
    public class CardConnectResponseModel : BaseModel
    {
        public string Resptext { get; set; }
        public string Setlstat { get; set; }
        public string Respcode { get; set; }
        public string Respstat { get; set; }
        public string Retref { get; set; }
        public string OrderId { get; set; }
        public string Token { get; set; }
        public string Profileid { get; set; }
        public string Acctid { get; set; }
        public string Refundable { get; set; }
        public string Voidable { get; set; }
    }
}
