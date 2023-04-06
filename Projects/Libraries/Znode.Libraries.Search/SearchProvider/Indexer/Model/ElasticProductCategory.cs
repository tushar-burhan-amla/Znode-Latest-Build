namespace Znode.Libraries.Search
{
    public class ElasticProductCategory
    {
        public int ProductCategoryId { get; set; }
        public SearchCategory Category { get; set;}
        public bool ActiveInd { get; set; }
    }
}
