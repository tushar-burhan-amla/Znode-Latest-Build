using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("AbandonedCart")]
    [HighlightedClass]
    public class AbandonedCartModel: List<AbandonedCartInfo>
    {
        [HighlightedMember]
        public AbandonedCartModel()
        {

        }
    }
}
