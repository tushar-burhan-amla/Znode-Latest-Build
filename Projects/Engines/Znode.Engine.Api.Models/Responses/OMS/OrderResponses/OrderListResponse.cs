namespace Znode.Engine.Api.Models.Responses
{
    public class OrderListResponse : BaseListResponse
    {
        public OrdersListModel OrderList { get; set; }
        public OrderNotesListModel OrderNotesList { get; set; }
    }
}
