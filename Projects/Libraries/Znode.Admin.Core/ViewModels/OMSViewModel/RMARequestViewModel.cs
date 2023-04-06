using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMARequestViewModel : BaseViewModel
    {
        public RMARequestViewModel()
        {
            RMARequestItems = new List<RMARequestItemViewModel>();
        }

        public int RMARequestID { get; set; }
        public int OmsOrderId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string StoreName { get; set; }
        public int PortalId { get; set; }
        public string CustomerName { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestStatus { get; set; }
        public int? RmaRequestStatusId { get; set; }
        public string RequestCode { get; set; }
        public string RequestNumber { get; set; }
        public string TaxCost { get; set; }
        public string Discount { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public decimal? TaxCostAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Comments { get; set; }
        public string Flag { get; set; }
        public string OrderLineItems { get; set; }
        public string Quantities { get; set; }
        public List<RMARequestItemViewModel> RMARequestItems { get; set; }
        public bool EmailRMAReport { get; set; }
        public string UserName { get; set; }
        public string OrderNumber { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
    }
}