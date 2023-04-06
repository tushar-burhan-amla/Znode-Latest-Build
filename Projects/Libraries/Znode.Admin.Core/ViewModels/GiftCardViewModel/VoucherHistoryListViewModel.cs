using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class VoucherHistoryListViewModel : BaseViewModel
    {
        public VoucherHistoryListViewModel()
        {
            GiftCardHistoryList = new List<VoucherHistoryViewModel>();
        }
        public List<VoucherHistoryViewModel> GiftCardHistoryList { get; set; }
        public GridModel GridModel { get; set; }
        public int? GiftCardId { get; set; }
        public int? PortalId { get; set; }
    }
}
