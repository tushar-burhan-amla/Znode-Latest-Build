using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RMAReturnStateListModel : BaseListModel
    {
        public List<RMAReturnStateModel> ReturnStates { get; set; }

        public RMAReturnStateListModel()
        {
            ReturnStates = new List<RMAReturnStateModel>();
        }
    }
}
