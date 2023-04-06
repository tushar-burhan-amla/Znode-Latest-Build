using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class TaxRuleListViewModel : BaseViewModel
    {
        public List<TaxRuleViewModel> TaxRuleList { get; set; }
        public GridModel GridModel { get; set; }
        public int TaxClassId { get; set; }
    }
}