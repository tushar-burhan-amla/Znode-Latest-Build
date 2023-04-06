namespace Znode.Engine.Api.Models
{
    public class CartItemModel
    {
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProductAmount { get; set; }
    }
}