namespace Znode.Libraries.Search
{
    public class SortCriteria
    {
        public enum SortNameEnum
        {
            Price,
            ProductName,
            HighestRated,
            MostReviewed,
            OutOfStock,
            InStock,
            DisplayOrder,
            ProductBoost
        }

        public enum SortDirectionEnum
        {
            ASC = 0,
            DESC = 1,
        }

        public SortNameEnum SortName { set; get; }
        public SortDirectionEnum SortDirection { set; get; }

    }
}
