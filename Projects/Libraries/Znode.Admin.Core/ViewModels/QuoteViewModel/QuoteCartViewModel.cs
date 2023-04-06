using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class QuoteCartViewModel : BaseViewModel
    {

        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }

        public int OmsQuoteId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelSubTotal, ResourceType = typeof(Admin_Resources))]
        public decimal? SubTotal { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ImportDuty { get; set; }
        
        public decimal? Total { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal? EstimateShippingCost { get; set; }
        public decimal ShippingHandlingCharges { get; set; }

        public string CultureCode { get; set; }

        public int ShippingId { get; set; }
        public string ShippingName { get; set; }
        public string ShippingErrorMessage { get; set; }
        public bool IsTaxExempt { get; set; }
        public List<TaxSummaryViewModel> TaxSummaryList { get; set; }
        // To bind the CustomShippingCost data 
        public decimal? CustomShippingCost { get; set; }
    }
}
