using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportDiscountTypeListResponse : BaseListResponse
    {
        public List<ReportDiscountTypeModel> DiscountTypeList { get; set; }
    }
}
