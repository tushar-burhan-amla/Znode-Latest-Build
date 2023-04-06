using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class CreateRequestViewModel : BaseViewModel
    {
        public CreateRequestViewModel()
        {
            RMARequestItems = new List<CreateRequestItemsViewModel>();
        }
        public int RMARequestId { get; set; }
        public string RequestNumber { get; set; }
        public decimal? TaxCost { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public string Comments { get; set; }
        public string Flag { get; set; }
        public List<CreateRequestItemsViewModel> RMARequestItems { get; set; }
        public string Mode { get; set; }
        public string OrderLineItems { get; set; }
        public string Quantities { get; set; }
        public int OrderId { get; set; }
    }

    public class CreateRequestItemsViewModel : BaseViewModel
    {
        public int OrderId { get; set; }
        public int? OrderLineItemId { get; set; }
        public decimal? Quantity { get; set; }
        public string Price { get; set; }
        public int? ReasonForReturnId { get; set; }
    }
}
