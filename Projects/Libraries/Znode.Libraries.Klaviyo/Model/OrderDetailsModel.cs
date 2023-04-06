using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Libraries.Klaviyo
{
    public class OrderDetailsModel : BaseModel
    {
        public List<OrderLineItemDetailsModel> OrderLineItems { get; set; }
        public bool IsProductDisplayPage { get; set; }
    }
}
