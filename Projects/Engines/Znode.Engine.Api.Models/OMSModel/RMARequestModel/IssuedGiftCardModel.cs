namespace Znode.Engine.Api.Models
{
    public class IssuedGiftCardModel : BaseModel
    {
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public string ExpirationDate { get; set; }
    }
}
