namespace Znode.Engine.WebStore.ViewModels
{
    public class HighlightsViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public string SKU { get; set; }
        public int HighlightId { get; set; }
        public bool DisplayPopup { get; set; }
        public string Hyperlink { get; set; }
        public int HighlightTypeId { get; set; }
        public int DisplayOrder { get; set; }
        public string ImageAltTag { get; set; }
        public string HighlightName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public int? LocaleId { get; set; }
        public string HighlightType { get; set; }
        public string MediaFileName { get; set; }
        public string MediaPath { get; set; }
        public string SEOUrl { get; set; }
        public string HighlightCode { get; set; }
    }
}