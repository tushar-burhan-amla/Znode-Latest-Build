using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TaxClassListModel : BaseListModel
    {
        public List<TaxClassModel> TaxClassList { get; set; }
    }
}
