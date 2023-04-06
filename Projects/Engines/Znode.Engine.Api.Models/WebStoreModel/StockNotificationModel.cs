namespace Znode.Engine.Api.Models
{
    public class StockNotificationModel : BaseModel
    {
        public int StockNoticeId { get; set; }
        public string ParentSKU { get; set; }
        public string ProductSKU { get; set; }
        public string EmailId { get; set; }
        public decimal Quantity { get; set; }
        public int PortalId { get; set; }
        public int CatalogId { get; set; }
        public bool IsEmailSent { get; set; }
    }
}
