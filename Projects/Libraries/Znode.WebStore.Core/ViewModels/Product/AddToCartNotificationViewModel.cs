namespace Znode.Engine.WebStore.ViewModels
{
    public class AddToCartNotificationViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal Quantity { get; set; }
        public string SKU { get; set; }
        public string GroupProductSKUs { get; set; }
        public string ProductImage { get; set; }
        public bool IsEnabled { get; set; }
    }
}
