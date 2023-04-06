using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class EditCartItemViewModel : AddToCartViewModel
    {
        public int OrderLineItemStatusId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public int OmsOrderId { get; set; }
        public int? OmsSavedCartLineItemId { get; set; }
        public int? ParentOmsSavedcartLineItemId { get; set; }

        public string OrderLineItemStatus { get; set; }
        public string TrackingNumber { get; set; }
        public string TrackingUrl { get; set; }
        public int ShippingId { get; set; }
        public List<SelectListItem> ShippingStatusList { get; set; }

        public List<CouponModel> Coupons { get; set; }
        public decimal? PartialRefundAmount { get; set; }
        public string GroupId { get; set; }
    }
}
