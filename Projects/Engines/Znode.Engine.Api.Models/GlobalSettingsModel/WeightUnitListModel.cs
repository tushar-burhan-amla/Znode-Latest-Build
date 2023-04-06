using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WeightUnitListModel : BaseListModel
    {
        public List<WeightUnitModel> WeightUnits { get; set; }

        public WeightUnitListModel()
        {
            WeightUnits = new List<WeightUnitModel>();
        }
    }
}
