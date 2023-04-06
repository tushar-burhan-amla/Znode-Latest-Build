using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CurrencyListViewModel : BaseViewModel
    {
        public List<CurrencyViewModel> Currencies { get; set; }
        public GridModel GridModel { get; set; }

        public CurrencyListViewModel()
        {
            Currencies = new List<CurrencyViewModel>();
        }
    }
}