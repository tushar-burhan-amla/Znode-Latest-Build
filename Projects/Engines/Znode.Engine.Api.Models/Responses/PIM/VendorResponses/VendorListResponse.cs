using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class VendorListResponse : BaseListResponse
    {
        public List<VendorModel> Vendors { get; set; }
        public List<PIMAttributeDefaultValueModel> VendorCodes { get; set; }
    }
}
