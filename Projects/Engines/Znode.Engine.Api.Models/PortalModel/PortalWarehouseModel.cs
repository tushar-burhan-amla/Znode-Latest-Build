using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalWarehouseModel : BaseModel
    {
        public int PortalWarehouseId { get; set; }
        public int PortalId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseCode { get; set; }
        public List<PortalAlternateWarehouseModel> AlternateWarehouses { get; set; }
        public List<WarehouseModel> WarehouseList { get; set; }
    }
}
