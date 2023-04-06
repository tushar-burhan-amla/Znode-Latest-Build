using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RMAReturnLineItemViewModel : BaseViewModel
    {
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
        public DateTime ReturnDate { get; set; }
        public decimal? ShippingCost { get; set; }
        public int RmaReturnStateId { get; set; }
        public bool IsActive { get; set; }
        public bool IsShippingReturn { get; set; }
        public decimal? RefundAmount { get; set; }
        public string ReturnTypeCode { get; set; }

        public string ReturnStatus { get; set; }
        public string ImagePath { get; set; }
        public decimal ShippedQuantity { get; set; }
        public Dictionary<string, object> PersonaliseValueList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
    }
}
