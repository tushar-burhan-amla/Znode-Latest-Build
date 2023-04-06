namespace Znode.Engine.Api.Models
{
    public class PortalAlternateWarehouseModel : BaseModel
    {
        public int PortalAlternateWarehouseId { get; set; }
        public int PortalWarehouseId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseCode { get; set; }
    }
}
