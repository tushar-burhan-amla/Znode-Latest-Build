using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderNotesListModel : BaseListModel
    {
        public List<OrderNotesModel> OrderNotes { get; set; }

        public OrderNotesListModel()
        {
            OrderNotes = new List<OrderNotesModel>();
        }
    }
}
