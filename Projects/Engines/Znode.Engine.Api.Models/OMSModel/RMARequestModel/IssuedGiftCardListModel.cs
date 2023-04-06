using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class IssuedGiftCardListModel : BaseListModel
    {
        public IssuedGiftCardListModel()
        {
            IssuedGiftCardModels = new List<IssuedGiftCardModel>();
        }
        public List<IssuedGiftCardModel> IssuedGiftCardModels { get; set; }
    }
}
