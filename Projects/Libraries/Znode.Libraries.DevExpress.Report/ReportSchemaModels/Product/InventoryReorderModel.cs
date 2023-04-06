using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("InventoryReorder")]
    [HighlightedClass]
    public class InventoryReorderModel : List<InventoryReorderInfo>
    {
        [HighlightedMember]
        public InventoryReorderModel()
        {

        }
    }
}
