using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class KeywordSearchViewModel : BaseViewModel
    {
        public List<ProductViewModel> Products { get; set; }
        public int CategoryId { get; set; }
        public int OldPageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
        public int SelectedCategoryId { get; set; }
        public string NextPageUrl { get; set; }
        public string PreviousPageurl { get; set; }
        public string Category { get; set; }
        public string SearchTerm { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoTitle { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryBanner { get; set; }
        public string CategoryLongDescription { get; set; }        
    }
}