using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class BundleProductViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public List<PublishAttributeViewModel> Attributes { get; set; }
        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public decimal? AssociatedProductBundleQuantity { get; set; }
    }
}