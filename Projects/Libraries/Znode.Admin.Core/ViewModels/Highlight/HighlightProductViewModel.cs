namespace Znode.Engine.Admin.ViewModels
{
    public class HighlightProductViewModel : BaseViewModel
    {
        public int? HighlightProductId { get; set; }
        public int HighlightId { get; set; }
        public string PublishProductId { get; set; }
        public string PublishProductIds { get; set; }
        public string ProductName { get; set; }
    }
}