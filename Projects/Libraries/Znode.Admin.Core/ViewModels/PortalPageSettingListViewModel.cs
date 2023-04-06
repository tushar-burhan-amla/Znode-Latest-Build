using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalPageSettingListViewModel : BaseViewModel
    {
        public PortalPageSettingListViewModel()
        {
            PageSettings = new List<PortalPageSettingViewModel>();
        }
        public List<PortalPageSettingViewModel> PageSettings { get; set; }
        public GridModel GridModel { get; set; }
        public string PortalName { get; set; }
        public int PortalId { get; set; }
    }
}