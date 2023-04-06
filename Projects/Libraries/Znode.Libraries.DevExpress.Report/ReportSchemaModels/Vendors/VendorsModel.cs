using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("Vendors")]
    [HighlightedClass]
    public class VendorsModel : List<VendorsInfo>
    {
        [HighlightedMember]
        public VendorsModel()
        {

        }
    }
}
