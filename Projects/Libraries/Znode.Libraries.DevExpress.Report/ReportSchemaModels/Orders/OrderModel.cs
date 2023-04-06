using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("Orders")]
    [HighlightedClass]
    public class OrderModel : List<OrderInfo>
    {
        [HighlightedMember]
        public OrderModel()
        {

        }

    }
}
