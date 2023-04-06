using System.Collections.Generic;
using System.Web.Mvc;

using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMAReturnLineItemViewModel : BaseViewModel
    {
        public RMAReturnLineItemViewModel()
        {
            ExternalId = System.Guid.NewGuid().ToString();
        }

        public int RmaReturnLineItemsId { get; set; }
        public int RmaReturnDetailsId { get; set; }
        public int? OmsOrderLineItemsId { get; set; }
        public int? RmaReasonForReturnId { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public string RmaReasonForReturn { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal ExpectedReturnQuantity { get; set; }
        public decimal? ReturnedQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal? Weight { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool ShipSeparately { get; set; }
        public decimal? ShippingCost { get; set; }
        public int RmaReturnStateId { get; set; }
        public bool IsActive { get; set; }
        public bool IsShippingReturn { get; set; }
        public decimal RefundAmount { get; set; }
        public string ReturnTypeCode { get; set; }

        public string ReturnStatus { get; set; }
        public string ProductImagePath { get; set; }
        public decimal ShippedQuantity { get; set; }
        public string ExternalId { get; set; }
        public bool ShipSeperately { get; set; }
        public string ProductType { get; set; }
        public string ImagePath { get; set; }
        public string CultureCode { get; set; }
        public bool IsOrderPartialRefund { get; set; }
        public decimal Total { get; set; }
        public List<SelectListItem> ReturnStatusList { get; set; }
        public decimal TaxCost { get; set; }
        public int? ProductId { get; set; }
        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public decimal PerQuantityLineItemDiscount { get; set; }
        public decimal PerQuantityCSRDiscount { get; set; }
        public decimal PerQuantityShippingDiscount { get; set; }
        public decimal PerQuantityOrderLevelDiscountOnLineItem { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public decimal PerQuantityShippingCost { get; set; }
        public decimal PerQuantityVoucherAmount { get; set; }
        public decimal ImportDuty { get; set; }
    }
}
