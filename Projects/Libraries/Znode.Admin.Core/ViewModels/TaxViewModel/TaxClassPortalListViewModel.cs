using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class TaxClassPortalListViewModel : BaseViewModel
    {
        public List<TaxClassPortalViewModel> TaxClassPortalList { get; set; }
        public GridModel GridModel { get; set; }
        public int TaxClassId { get; set; }
        public string Name { get; set; }
    }
}