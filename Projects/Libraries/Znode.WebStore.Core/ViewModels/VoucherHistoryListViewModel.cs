using System;
using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
  public  class VoucherHistoryListViewModel : BaseViewModel
    {
        public List<GiftCardHistoryViewModel> List { get; set; }
        public VoucherViewModel voucherViewModel { get; set; }
        public VoucherHistoryListViewModel()
        {
            List = new List<GiftCardHistoryViewModel>();
            GridModel = new GridModel();
            voucherViewModel = new VoucherViewModel();
        }

        public GridModel GridModel { get; set; }

       
    }
}
