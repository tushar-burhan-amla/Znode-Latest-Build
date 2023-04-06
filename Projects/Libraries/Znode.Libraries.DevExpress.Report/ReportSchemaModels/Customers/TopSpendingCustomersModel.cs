using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("TopSpendingCustomers")]
    [HighlightedClass]
    public class TopSpendingCustomersModel : List<TopSpendingCustomersInfo>
    {
        [HighlightedMember]
        public TopSpendingCustomersModel()
        {

        }
    }
}
