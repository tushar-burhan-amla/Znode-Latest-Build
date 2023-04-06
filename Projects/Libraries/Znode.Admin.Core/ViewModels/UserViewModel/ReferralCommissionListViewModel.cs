using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReferralCommissionListViewModel : BaseViewModel
    {
        public ReferralCommissionListViewModel()
        {
            List = new List<ReferralCommissionViewModel>();
            GridModel = new GridModel();
        }
        public List<ReferralCommissionViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public int UserId { get; set; }
    }
}