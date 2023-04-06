using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("MostFrequentCustomer")]
    [HighlightedClass]
    public class MostFrequentCustomerModel : List<MostFrequentCustomerInfo>
    {
        [HighlightedMember]
        public MostFrequentCustomerModel()
        {

        }
    }
}
