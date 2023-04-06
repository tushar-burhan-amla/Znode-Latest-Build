namespace Znode.Engine.WebStore.ViewModels
{
    public class SEOUrlViewModel : BaseViewModel
    {
        public string SeoUrl { get; set; }

        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int? ContentPageId { get; set; }
        public string ContentPageName { get; set; }

        public int? BrandId { get; set; }
        public string BrandName { get; set; }

        public string Name { get; set; }
        public int SEOId { get; set; }

        public bool IsActive { get; set; }
        public string SeoCode { get; set; }
        public string SEOTitle { get; set; }
        public string SEOKeywords { get; set; }
        public string SEODescription { get; set; }
        public string CanonicalURL { get; set; }
        public string RobotTag { get; set; }
    }
}