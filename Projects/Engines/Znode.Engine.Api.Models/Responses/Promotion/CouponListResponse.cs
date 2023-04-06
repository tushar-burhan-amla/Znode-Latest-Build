using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CouponListResponse : BaseListResponse
    {
        public List<CouponModel> CouponList { get; set; }
    }
}
