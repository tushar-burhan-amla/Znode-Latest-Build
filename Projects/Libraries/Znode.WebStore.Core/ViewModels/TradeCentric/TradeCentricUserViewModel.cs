using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class TradeCentricUserViewModel : BaseViewModel
    {
        public int TradecentricUserId { get; set; }
        public int UserId { get; set; }
        public string ReturnUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string Operation { get; set; }
        public string CookieMappingKey { get; set; }
        public string TokenKey { get; set; }
        public TradeCentricCartViewModel CartModel { get; set; }
    }
}
