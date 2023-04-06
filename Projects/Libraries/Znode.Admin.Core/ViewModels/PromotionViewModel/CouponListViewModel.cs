using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CouponListViewModel : BaseViewModel
    {
        public List<CouponViewModel> CouponList { get; set; }
        public List<CouponExportViewModel> CouponExportList { get; set; }
        public GridModel GridModel { get; set; }

        public int PromotionCouponId { get; set; }
    }
}