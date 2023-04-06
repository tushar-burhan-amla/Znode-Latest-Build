using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class TaxClassSKUListViewModel : BaseViewModel
    {
        public List<TaxClassSKUViewModel> TaxClassSKUList { get; set; }
        public GridModel GridModel { get; set; }
        public int TaxClassId { get; set; }
        public string Name { get; set; }
    }
}