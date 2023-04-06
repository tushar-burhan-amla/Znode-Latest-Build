using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class RMAReturnModel : BaseModel
    {
        public RMAReturnModel()
        {
            ReturnLineItems = new List<RMAReturnLineItemModel>();
        }
        public int RmaReturnDetailsId { get; set; }
        public int OmsOrderId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        [Required]
        public string OrderNumber { get; set; }
        public string ReturnNumber { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int RmaReturnStateId { get; set; }
        public decimal TotalExpectedReturnQuantity { get; set; }
        public string EmailId { get; set; }
        [Required]
        public int PortalId { get; set; }
        [Required]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? AddressId { get; set; }
        public int? ShippingId { get; set; }
        public string ShippingNumber { get; set; }
        public bool IsTaxCostEdited { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? ReturnShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ReturnTaxCost { get; set; }
        public decimal CSRDiscount { get; set; }
        public bool IsActive { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }

        public string ReturnStatus { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public string StoreLogo { get; set; }
        public string Notes { get; set; }
        public bool IsSubmitReturn { get; set; }
        public string CreatedByName { get; set; }
        public List<RMAReturnLineItemModel> ReturnLineItems { get; set; }
        public List<RMAReturnNotesModel> ReturnNotes { get; set; }
        public List<RMAReturnHistoryModel> ReturnHistoryAndNotesList { get; set; }
        public string ShippingToAddressHtml { get; set; }
        public string ShippingFromAddressHtml { get; set; }
        public string BarcodeImage { get; set; }
        public OrderModel RMAOrderModel { get; set; }
        public bool IsAdminRequest { get; set; }

        public string OldReturnStatus { get; set; }
        public List<RMAReturnLineItemModel> OldReturnLineItems { get; set; }
        public Dictionary<string, RMAReturnLineItemHistoryModel> ReturnLineItemHistory { get; set; }
        public Dictionary<string, string> ReturnHistory { get; set; }
        public decimal? OverDueAmount { get; set; }
        public bool IsRefundProcess { get; set; }
        public decimal ReturnShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public virtual decimal TotalReturnAmount => (SubTotal + ReturnShippingCost.GetValueOrDefault() + ReturnTaxCost + ReturnImportDuty - DiscountAmount - CSRDiscount - ReturnShippingDiscount - ReturnCharges);
        public bool IsCalculateTaxAfterDiscount { get; set; }
        public bool IsPricesInclusiveOfTaxes { get; set; }        
        public decimal? VoucherAmount { get; set; }
        public bool HasError { get; set; }
        public decimal ReturnImportDuty { get; set; }
        public string PhoneNumber { get; set; }
    }
}