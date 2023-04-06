using System;
using System.Collections.Generic;
namespace Znode.Engine.Admin.ViewModels
{
    public class QuoteInfoViewModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public string QuoteNumber { get; set; }
        public string OmsQuoteStatus { get; set; }
        public int userId { get; set; }
        public int PortalId { get; set; }
        public string StoreName { get; set; }
        
        public string CreatedByName { get; set; }
        public string QuoteDateWithTime { get; set; }
        public string ShippingTypeDescription { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingTypeClassName { get; set; }
        public DateTime? InHandDate  { get; set; }
        public DateTime? QuoteExpirationDate  { get; set; }

        public int ShippingId { get; set; }
        public string JobName { get; set; }
        public string UserName { get; set; }
        
        public string ShippingConstraintCode { get; set; }
        public IList<ShippingConstraintsViewModel> ShippingConstraints { get; set; }
        public int OmsOrderId { get; set; }
        public string OrderNumber { get; set; }
    }
}

