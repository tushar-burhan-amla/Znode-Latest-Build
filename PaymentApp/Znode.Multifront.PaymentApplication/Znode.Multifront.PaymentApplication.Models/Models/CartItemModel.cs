
namespace Znode.Multifront.PaymentApplication.Models
{
    /// <summary>
    /// To show product details on paypal express checkout page
    /// </summary>
    public class CartItemModel : BaseModel
    {
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProductAmount { get; set; }

    }
}
