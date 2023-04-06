namespace Znode.Engine.Api.Models
{
    public class OrderStateModel : BaseModel
    {
        public int OrderStateId { get; set; }
        public string OrderStateName { get; set; }
        public string Description { get; set; }
        public bool IsEdit { get; set; }
        public bool? IsOrderState { get; set; }
        public bool? IsOrderLineItemState { get; set; }
    }
}
