using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class RMAReturnLineItemModel : BaseModel
    {
        public int RmaReturnLineItemsId { get; set; }
        public int RmaReturnDetailsId { get; set; }
        [Required]
        public int OmsOrderLineItemsId { get; set; }
        public int? RmaReasonForReturnId { get; set; }
        public int OrderLineItemRelationshipTypeId { get; set; }
        public string RmaReasonForReturn { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal ExpectedReturnQuantity { get; set; }
        public decimal? ReturnedQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal? Weight { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool ShipSeparately { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? ShippingCost { get; set; }
        public int RmaReturnStateId { get; set; }
        public bool IsActive { get; set; }
        public bool IsShippingReturn { get; set; }
        public decimal? RefundAmount { get; set; }
        public string ReturnTypeCode { get; set; }

        public string ReturnStatus { get; set; }
        public string ImagePath { get; set; }
        public decimal ShippedQuantity { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public decimal TaxCost { get; set; }
        public Dictionary<string, object> PersonaliseValueList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public int? OmsReturnOrderLineItemsId { get; set; }
        public decimal PerQuantityLineItemDiscount { get; set; }
        public decimal PerQuantityCSRDiscount { get; set; }
        public decimal PerQuantityShippingDiscount { get; set; }
        public decimal PerQuantityShippingCost { get; set; }
        public decimal PerQuantityOrderLevelDiscountOnLineItem { get; set; }
        public int? PaymentStatusId { get; set; }
        public decimal PerQuantityVoucherAmount { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public decimal ImportDuty { get; set; }
    }
}
