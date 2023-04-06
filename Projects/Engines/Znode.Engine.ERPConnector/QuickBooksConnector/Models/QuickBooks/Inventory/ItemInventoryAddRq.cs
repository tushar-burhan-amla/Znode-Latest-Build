namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// Model required for QuickBooks XML add inventory request node element
    /// </summary>
    public class ItemInventoryAddRq : BaseModel
    {
        public ItemInventoryAdd ItemInventoryAdd { get; set; }
    }
}