namespace Znode.Engine.Api.Models
{
    public class UnassociateInventoryWarehouseModel : BaseModel
    {
        public int InventoryListId { get; set; }
        public int WarehouseId { get; set; }
        public string InventoryWarehouseIds { get; set; }
    }
}
