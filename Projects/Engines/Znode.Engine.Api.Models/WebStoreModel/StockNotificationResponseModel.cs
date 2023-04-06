namespace Znode.Engine.Api.Models
{
    public class StockNotificationResponseModel : PublishProductModel
    {
        public int StockNoticeId { get; set; }
        public string ParentSKU { get; set; }
        public string ProductSKU { get; set; }
        public string EmailId { get; set; }
        public int CatalogId { get; set; }
        public decimal AvailableQuantity { get; set; }
        public bool IsEmailSent { get; set; }
        public string ProductAttributes { get; set; }
    }
}
