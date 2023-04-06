namespace Znode.Engine.Admin.ViewModels
{
    public class ProductImageViewModel : BaseViewModel
    {
        public int PimProductImageId { get; set; }
        public int? PimProductId { get; set; }
        public int? PimAttributeId { get; set; }
        public string PimAttributeCode { get; set; }
        public int? MediaId { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public int? LastSelectedMediaId { get; set; }

        public Property ControlProperty { get; set; }

        public ProductImageViewModel()
        {
            ControlProperty = new Property();
        }
    }
}