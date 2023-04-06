using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class TradeCentricSessionRequestViewModel 
    {        
        public string Operation { get; set; }
        public string Return_Url { get; set; }
        public string Redirect_Url { get; set; }
        public string CookieMappingId { get; set; }
        public string Selected_item { get; set; }
        public TradeCentricContactViewModel Contact { get; set; } = new TradeCentricContactViewModel();
        public TradeCentricCustomDetailViewModel Custom { get; set; } = new TradeCentricCustomDetailViewModel();
        public List<TradeCentricCartItemViewModel> Items { get; set; } = new List<TradeCentricCartItemViewModel>(); 
   
    }
}
