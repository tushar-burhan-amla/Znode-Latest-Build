using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImpersonationViewModel
    {
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public bool IsLock { get; set; }
        public List<SelectListItem> AvailablePortals { get; set; }
    }
}
