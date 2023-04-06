using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TaxClassListResponse : BaseListResponse
    {
        public List<TaxClassModel> TaxClasses { get; set; }
    }
}
