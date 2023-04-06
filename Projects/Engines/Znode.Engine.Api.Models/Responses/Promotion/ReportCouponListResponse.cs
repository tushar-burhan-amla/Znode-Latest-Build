using System.Collections.Generic;
namespace Znode.Engine.Api.Models.Responses
{
    public class ReportCouponListResponse : BaseListResponse
    {
        public List<ReportCouponModel> CouponList { get; set; }
    }
}
