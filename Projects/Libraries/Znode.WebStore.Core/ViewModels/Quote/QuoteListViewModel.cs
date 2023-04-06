using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class QuoteListViewModel : BaseViewModel
    {
        public List<QuoteViewModel> Quotes { get; set; }
        public GridModel GridModel { get; set; }
        public int UserId { get; set; }
    }
}
