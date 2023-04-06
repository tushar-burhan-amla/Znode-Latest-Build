using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        public List<CartItemViewModel> ShoppingCartItems { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSubTotal, ResourceType = typeof(Admin_Resources))]
        public decimal? SubTotal { get; set; }
        public decimal TaxCost { get; set; }
        public decimal TaxRate { get; set; }
        public decimal? Total { get; set; }
        public decimal? Vat { get; set; }
        public decimal? OrderTotalWithoutVoucher { get; set; }
        public decimal? ImportDuty { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelOverDueAmount, ResourceType = typeof(Admin_Resources))]
        public decimal? OverDueAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingCost, ResourceType = typeof(Admin_Resources))]
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public int LocaleId { get; set; }
        public int OmsOrderId { get; set; }
        public int PortalId { get; set; }
        public int CatalogId { get; set; }
        public int PublishedCatalogId { get; set; }
        public int UserId { get; set; }
        public int ShippingId { get; set; }
        public string ShippingName { get; set; }
        public bool GiftCardApplied { get; set; }
        public string GiftCardMessage { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelVoucherAmount, ResourceType = typeof(Admin_Resources))]
        public decimal GiftCardAmount { get; set; }
        public string GiftCardNumber { get; set; }
        public bool GiftCardValid { get; set; }
        public List<CouponViewModel> Coupons { get; set; }
        public bool IsQuote { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCSRDiscount, ResourceType = typeof(Admin_Resources))]
        public decimal CSRDiscountAmount { get; set; }
        public bool CSRDiscountApplied { get; set; }
        public string CSRDiscountDescription { get; set; }
        public string CSRDiscountMessage { get; set; }
        public bool FreeShipping { get; set; }
        public string ShippingErrorMessage { get; set; }
        public string OrderState { get; set; }
        public GridModel GridModel { get; set; }
        public decimal? CustomShippingCost { get; set; }
        public decimal? EstimateShippingCost { get; set; }
        public decimal? TotalAdditionalCost { get; set; }
        public string CustomerName { get; set; }
        public List<VouchersViewModel> Vouchers { get; set; }
        public List<TaxSummaryViewModel> TaxSummaryList { get; set; }
        public CartViewModel()
        {
            Coupons = new List<CouponViewModel>();
            GridModel = new GridModel();
            Vouchers = new List<VouchersViewModel>();
        }
        public string CultureCode { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public decimal ReturnCharges { get; set; }
    }
}