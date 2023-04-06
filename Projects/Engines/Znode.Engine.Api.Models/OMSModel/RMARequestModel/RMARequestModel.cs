using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RMARequestModel : BaseModel
    {
        public RMARequestModel()
        {
            RMARequestItems = new List<RMARequestItemModel>();
        }

        public int OmsOrderId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string StoreName { get; set; }
        public string RequestStatus { get; set; }
        public int PortalId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<RMARequestItemModel> RMARequestItems { get; set; }
        public string CustomerName { get; set; }
        public int RmaRequestId { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Comments { get; set; }
        public int? RmaRequestStatusId { get; set; }
        public string RequestNumber { get; set; }
        public decimal? TaxCost { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public bool EmailRMAReport { get; set; }
        public string RequestCode { get; set; }
        public string UserName { get; set; }
        public string OrderNumber { get; set; }
        public string CurrencyCode { get; set; }
    }
}
