using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("SalesTax")]
    [HighlightedClass]
    public class SalesTaxModel : List<SalesTaxInfo>
    {
        [HighlightedMember]
        public SalesTaxModel()
        {

        }

    }
}
