using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class IssuedGiftCardListResponse : BaseResponse
    {
        public List<IssuedGiftCardModel> IssuedGiftCards { get; set; }
    }
}
