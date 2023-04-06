using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class GiftCardListViewModel : BaseViewModel
    {
        public GiftCardListViewModel()
        {
            GiftCardList = new List<GiftCardViewModel>();
        }
        public List<GiftCardViewModel> GiftCardList { get; set; }
        public GridModel GridModel { get; set; }
        public int UserId { get; set; }
        public bool ReferralCommissionCount { get; set; }
        public bool IsExcludeExpired { get; set; }
    }
}