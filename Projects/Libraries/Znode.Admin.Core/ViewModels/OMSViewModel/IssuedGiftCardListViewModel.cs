using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class IssuedGiftCardListViewModel : BaseViewModel
    {
        public IssuedGiftCardListViewModel()
        {
            IssuedGiftCardModels = new List<IssuedGiftCardViewModel>();
        }
        public List<IssuedGiftCardViewModel> IssuedGiftCardModels { get; set; }
    }
}