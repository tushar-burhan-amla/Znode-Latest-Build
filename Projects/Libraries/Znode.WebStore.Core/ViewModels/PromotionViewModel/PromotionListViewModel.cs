using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class PromotionListViewModel : BaseViewModel
    {
        public List<PromotionViewModel> PromotionList { get; set; }
    }
}