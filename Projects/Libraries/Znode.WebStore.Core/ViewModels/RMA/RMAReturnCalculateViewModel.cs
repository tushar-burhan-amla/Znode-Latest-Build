using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RMAReturnCalculateViewModel : BaseViewModel
    {
        [Required]
        public List<RMAReturnCalculateLineItemViewModel> ReturnCalculateLineItemList { get; set; }
        [Required]
        public string OrderNumber { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int PortalId { get; set; }
        public decimal? ReturnSubTotal { get; set; }
        public decimal? ReturnTaxCost { get; set; }
        public decimal? ReturnShippingCost { get; set; }
        public decimal? ReturnTotal { get; set; }
        public string CultureCode { get; set; }
        public decimal CSRDiscount { get; set; }
        public decimal Discount { get; set; }
        public decimal GiftCardAmount { get; set; }
        public decimal ReturnShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public decimal ReturnImportDuty { get; set; }
    }
}