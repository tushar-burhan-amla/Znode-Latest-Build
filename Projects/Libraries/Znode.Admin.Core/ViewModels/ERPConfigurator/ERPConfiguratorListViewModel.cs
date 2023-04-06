 using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ERPConfiguratorListViewModel : BaseViewModel
    {
        public List<ERPConfiguratorViewModel> ERPConfiguratorList { get; set; }
        public GridModel GridModel { get; set; }
        public int ERPConfiguratorId { get; set; }
        public ERPConfiguratorListViewModel()
        {
            ERPConfiguratorList = new List<ERPConfiguratorViewModel>();
        }
    }
}