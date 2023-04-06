using Znode.Engine.WebStore;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AssociatedProductsViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public int? PimProductId { get; set; }
        public string SKU { get; set; }
        public string OMSColorSwatchText { get; set; }
        public string OMSColorCode { get; set; }
        public string OMSColorValue { get; set; }
        public string OMSColorPath { get; set; }
        public int DisplayOrder { get; set; }
    }
}
