using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("PopularSearch")]
    [HighlightedClass]
    public class PopularSearchModel : List<PopularSearchInfo>
    {
        [HighlightedMember]
        public PopularSearchModel()
        {

        }
    }
}
