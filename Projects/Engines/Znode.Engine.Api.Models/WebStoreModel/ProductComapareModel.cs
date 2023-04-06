namespace Znode.Engine.Api.Models
{
    public class ProductCompareModel : BaseModel
    {
        public string SenderEmailAddress { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int PortalId { get; set; }
        public string ProductIds { get; set; }
        public string BaseUrl { get; set; }
        public string ProductName { get; set; }
        public bool IsProductDetails { get; set; }
        public int LocaleId { get; set; }
        public int CatalogId { get; set; }
        public string WebstoreDomainName { get; set; }
        public string WebstoreDomainScheme { get; set; }
        public bool IsShowPriceAndInventoryToLoggedInUsersOnly { get; set; }
    }
}
