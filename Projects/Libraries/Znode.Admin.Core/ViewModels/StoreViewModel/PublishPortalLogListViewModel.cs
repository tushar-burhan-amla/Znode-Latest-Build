using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PublishPortalLogListViewModel : BaseViewModel
    {
        public List<PublishPortalLogViewModel> PublishPortalLog { get; set; }
        public GridModel GridModel { get; set; }
        public int PortalId { get; set; }
        public string StoreName { get; set; }
    }
}
