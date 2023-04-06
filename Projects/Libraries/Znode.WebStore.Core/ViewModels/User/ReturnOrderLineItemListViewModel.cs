using System.Collections.Generic;
using Znode.Engine.WebStore;

namespace Znode.WebStore.ViewModels
{
    public class ReturnOrderLineItemListViewModel : BaseViewModel
    {
        public List<ReturnOrderLineItemViewModel> ReturnItemList { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string CurrencyCode { get; set; }
    }
}
