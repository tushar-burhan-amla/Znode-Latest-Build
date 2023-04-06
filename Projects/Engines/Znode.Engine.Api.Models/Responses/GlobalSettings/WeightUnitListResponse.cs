using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WeightUnitListResponse : BaseListResponse
    {
        public List<WeightUnitModel> WeightUnits { get; set; }
    }
}
