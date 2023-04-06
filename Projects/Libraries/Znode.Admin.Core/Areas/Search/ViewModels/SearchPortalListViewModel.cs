using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchProfilePortalListViewModel : BaseViewModel
    {
        public List<SearchProfilePortalViewModel> SearchProfilePortalList { get; set; }
        public GridModel GridModel { get; set; }

        public int SearchProfileId { get; set; }
    }
}
