using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalSortSettingListViewModel : BaseViewModel
    {
        public PortalSortSettingListViewModel()
        {
            SortSettings = new List<PortalSortSettingViewModel>();
        }
        public List<PortalSortSettingViewModel> SortSettings { get; set; }
        public GridModel GridModel { get; set; }
        public string PortalName { get; set; }
        public string ProfileName { get; set; }
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
    }
}