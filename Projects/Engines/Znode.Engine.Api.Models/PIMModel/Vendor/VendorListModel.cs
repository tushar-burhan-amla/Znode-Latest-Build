using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class VendorListModel : BaseListModel
    {
        public List<VendorModel> Vendors { get; set; }

        public List<PIMAttributeDefaultValueModel> VendorCodes { get; set; }
    }
}
