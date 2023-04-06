using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductListViewModel : BaseViewModel
    {
        public string SearchResultCountText { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public long TotalProductCount { get; set; }
        public CategoryViewModel ParentCategoryHierarchy { get; set; }
        public string BreadCrumbHtml { get; set; }
        public string SearchTextName { get; set; }
        public bool IsSearchFromSuggestions { get; set; }
        public string SuggestTerm { get; set; }
        public int PageNumber { get; set; }
        public string SearchKeyword { get; set; }

        public string Location { get; set; }

        public SearchResultViewModel SearchResultViewModel { get; set; }
        public string RedirectToPDP
        {
            get
            {
                if (Products?.Count == 1 && PageNumber == 1)
                {
                    string SEOUrl = Products.First()?.SEOUrl;
                    if (string.IsNullOrEmpty(SEOUrl))
                    {
                        return $"~/Product/Details?id={Products.FirstOrDefault().PublishProductId}";
                    }
                    else
                    {
                        return $"{HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)}/{SEOUrl}";
                    }
                }
                return string.Empty;
            }
        }

        public int SearchProfileId { get; set; }
        public int TotalCMSPageCount { get; set; }

        //Returns the list of Products to be sent to the datalayer.
        public virtual List<ProductImpressionsViewModel> GetProductImpressionsData()
        {
            List<ProductImpressionsViewModel> impressions = Products?
                .Select(x => new ProductImpressionsViewModel() { SKU = x.SKU, Name = x.Name, CategoryName = x.CategoryName, BrandName = x.Attributes?.ValueFromSelectValue(Libraries.ECommerce.Utilities.ZnodeConstant.Brand), SearchKeyword = SearchKeyword, ProductPrice = x.RetailPrice})
                .ToList();
            return impressions ?? new List<ProductImpressionsViewModel>();
        }
    }
}