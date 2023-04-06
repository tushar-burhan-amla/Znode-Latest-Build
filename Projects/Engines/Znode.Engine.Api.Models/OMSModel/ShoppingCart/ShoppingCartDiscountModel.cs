namespace Znode.Engine.Api.Models
{
    public class ShoppingCartDiscountModel
    {
        public int ProductId { get; set; }
        public int ParentProductId { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
