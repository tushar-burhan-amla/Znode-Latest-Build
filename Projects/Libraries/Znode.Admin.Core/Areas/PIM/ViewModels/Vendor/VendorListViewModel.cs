using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class VendorListViewModel : BaseViewModel
    {
        public List<VendorViewModel> Vendors { get; set; }
        public GridModel GridModel { get; set; }

        public List<ProductDetailsViewModel> ProductDetailList { get; set; }
        public int PimVendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
    }
}