using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalWarehouseViewModel : BaseViewModel
    {
        public int PortalWarehouseId { get; set; }
        public int PortalId { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseCode { get; set; }
        public string PortalName { get; set; }

        public List<PortalAlternateWarehouseViewModel> AlternateWarehouses { get; set; }

        public List<WarehouseViewModel> WarehouseList { get; set; }

        public List<SelectListItem> MainWarehouseList { get; set; }
        public List<SelectListItem> UnassociatedWarehouseList { get; set; }
        public List<SelectListItem> AssociatedWarehouseList { get; set; }

    }
}