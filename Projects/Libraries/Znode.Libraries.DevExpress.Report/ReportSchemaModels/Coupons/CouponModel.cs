using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("Coupons")]
    [HighlightedClass]
    public class CouponModel : List<CouponUsageInfo>
    {
        [HighlightedMember]
        public CouponModel()
        {

        }
    }
}
