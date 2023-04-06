namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductComparePopUpViewModel
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public bool HideViewComparisonButton { get; set; }
    }
}