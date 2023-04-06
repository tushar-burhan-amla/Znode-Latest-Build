using System.Collections.Generic;

namespace Znode.Engine.Admin.Models
{
    public class TabViewListModel
    {
        public bool MaintainAllTabData { get; set; }
        public List<TabViewModel> Tabs { get; set; }
        public TabViewListModel()
        {
            Tabs = new List<TabViewModel>();
        }
    }
}