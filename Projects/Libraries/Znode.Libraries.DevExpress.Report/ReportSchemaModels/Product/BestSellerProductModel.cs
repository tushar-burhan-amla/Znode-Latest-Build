using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("BestSellerProduct")]
    [HighlightedClass]
    public class BestSellerProductModel : List<BestSellerProductInfo>
    {
        [HighlightedMember]
        public BestSellerProductModel()
        {

        }
    }
}
