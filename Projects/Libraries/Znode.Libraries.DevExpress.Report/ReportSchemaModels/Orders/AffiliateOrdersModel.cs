using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("AffiliateOrders")]
    [HighlightedClass]
    public class AffiliateOrdersModel : List<AffiliateOrdersInfo>
    {
        [HighlightedMember]
        public AffiliateOrdersModel()
        {

        }
    }
}
