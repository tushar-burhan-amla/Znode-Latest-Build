using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportBestSellerProductResponse : BaseListResponse
    {
        public List<ReportBestSellerProductModel> ReportBestSellerProductList { get; set; }
    }
}
