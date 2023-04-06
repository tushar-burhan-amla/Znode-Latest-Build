using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchTriggersListViewModel : BaseViewModel
    {
        public List<SearchTriggersViewModel> SearchTriggerList { get; set; }
        public GridModel GridModel { get; set; }
        public int SearchProfileId { get; set; }
    }
}
