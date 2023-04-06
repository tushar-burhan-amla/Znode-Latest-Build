using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMAReturnViewModel : BaseViewModel
    {
        public RMAReturnViewModel()
        {
            ReturnLineItems = new List<RMAReturnLineItemViewModel>();
        }

        public int RmaReturnDetailsId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelReturnNumber, ResourceType = typeof(Admin_Resources))]
        public string ReturnNumber { get; set; }
        public string ReturnStatus { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalExpectedReturnQuantity { get; set; }

        public int OmsOrderId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelOrderNumber, ResourceType = typeof(Admin_Resources))]
        public string OrderNumber { get; set; }
        public int RmaReturnStateId { get; set; }
        public string EmailId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsTaxCostEdited { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public string StoreLogo { get; set; }
        public List<RMAReturnLineItemViewModel> ReturnLineItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? ReturnShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ReturnTaxCost { get; set; }
        public decimal TotalReturnAmount { get; set; }
        public string ReturnTotalWithCurrency { get; set; }
        public string ReturnDateWithTime { get; set; }
        public string CreatedByName { get; set; }
        public string PaymentDisplayName { get; set; }
        public string PaymentType { get; set; }
        public string CreditCardNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalReturnNotes { get; set; }
        public string ShippingToAddressHtml { get; set; }
        public string ShippingFromAddressHtml { get; set; }
        public RMAReturnTotalViewModel ReturnTotalModel { get; set; }
        public List<RMAReturnHistoryViewModel> ReturnHistoryAndNotesList { get; set; }
        public OrderModel RMAOrderModel { get; set; }
        public bool IsAdminRequest { get; set; }
        public string OldReturnStatus { get; set; }
        public int OldRmaReturnStateId { get; set; }    
        public List<RMAReturnLineItemViewModel> OldReturnLineItems { get; set; }
        public Dictionary<string, RMAReturnLineItemHistoryModel> ReturnLineItemHistory { get; set; }
        public Dictionary<string, string> ReturnHistory { get; set; }
        public decimal OverDueAmount { get; set; }
        public bool IsRefundProcess { get; set; }
        public string BarcodeImage { get; set; }

        public string CustomerServiceEmail { get; set; }
        public string CustomerServicePhoneNumber { get; set; }
        public decimal CSRDiscount { get; set; }
        public decimal ReturnShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public decimal? VoucherAmount { get; set; }
        public bool IsReturnDetailsReceipt { get; set; }
        public List<RMAReturnNotesViewModel> ReturnNotes { get; set; }
        public DateTime orderdate { get; set; }
        public decimal ReturnImportDuty { get; set; }
    }
}