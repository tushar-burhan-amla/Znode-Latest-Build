using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CurrencyListModel : BaseListModel
    {
        public List<CurrencyModel> Currencies { get; set; }

        public CurrencyListModel()
        {
            Currencies = new List<CurrencyModel>();
        }
    }
}
