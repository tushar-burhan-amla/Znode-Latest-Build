namespace Znode.Engine.Api.Models.Responses
{
    public class OmsLineItemListResponse : BaseListResponse
    {
        public OrderLineItemDataListModel LineItemList { get; set; }
    }
}
