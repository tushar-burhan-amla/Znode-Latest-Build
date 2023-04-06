using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public List<SelectListItem> Portal { get; set; }
        public List<SelectListItem> Account { get; set; }
        public int PortalId { get; set; }
        public int AccountId { get; set; }
        public List<SelectListItem> Duration { get; set; }
        public string PortalName { get; set; }
        public string AccountName { get; set; }
    }
}