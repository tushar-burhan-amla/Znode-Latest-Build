using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("OrderLineItems")]
    [HighlightedClass]
    public class OrderLineItemsModel : List<OrderLineItemsInfo>
    {
        [HighlightedMember]
        public OrderLineItemsModel()
        {

        }
    }
}
