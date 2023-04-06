using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RMAReturnOrderNumberListModel : BaseModel
    {
        public List<string> ReturnEligibleOrderNumberList { get; set; }

        public RMAReturnOrderNumberListModel()
        {
            ReturnEligibleOrderNumberList = new List<string>();
        }
    }
}
