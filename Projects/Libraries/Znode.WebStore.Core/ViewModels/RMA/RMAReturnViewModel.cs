using System;
using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RMAReturnViewModel : BaseViewModel
    {
        public int RmaReturnDetailsId { get; set; }
        public string ReturnNumber { get; set; }
        public string ReturnStatus { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal TotalExpectedReturnQuantity { get; set; }

        public int OmsOrderId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string OrderNumber { get; set; }
        public int RmaReturnStateId { get; set; }
        public string EmailId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? AddressId { get; set; }
        public int? ShippingId { get; set; }
        public string ShippingNumber { get; set; }
        public bool IsTaxCostEdited { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ReturnShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ReturnTaxCost { get; set; }
        public decimal TotalReturnAmount { get; set; }
        public bool IsActive { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }

        public string Notes { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }

        public List<RMAReturnLineItemViewModel> ReturnLineItems { get; set; }
        public List<RMAReturnNotesViewModel> ReturnNotes { get; set; }

        public string ShippingToAddressHtml { get; set; }
        public string ShippingFromAddressHtml { get; set; }

        public string BarcodeImage { get; set; }
        public bool IsReturnDetailsReceipt { get; set; }
        public decimal ReturnImportDuty { get; set; }

        public RMAReturnViewModel()
        {
            ReturnLineItems = new List<RMAReturnLineItemViewModel>();
            ReturnNotes = new List<RMAReturnNotesViewModel>();
        }
        public decimal CSRDiscount { get; set; }
        public decimal ReturnShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public decimal? VoucherAmount { get; set; }
        public DateTime orderdate { get; set; }
    }
}