using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("TopEarningProduct")]
    [HighlightedClass]
    public class TopEarningProductModel : List<TopEarningProductInfo>
    {
        [HighlightedMember]
        public TopEarningProductModel()
        {

        }
    }
}
