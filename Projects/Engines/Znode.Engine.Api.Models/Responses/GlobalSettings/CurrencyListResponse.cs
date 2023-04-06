using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CurrencyListResponse : BaseListResponse
    {
        public List<CurrencyModel> Currencies { get; set; }
    }
}
