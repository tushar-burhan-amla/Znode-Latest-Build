using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderNotesListViewModel : BaseViewModel
    {
        public List<OrderNotesViewModel> OrderNoteList { get; set; }

        public OrderNotesListViewModel()
        {
            OrderNoteList = new List<OrderNotesViewModel>();
        }
    }
}