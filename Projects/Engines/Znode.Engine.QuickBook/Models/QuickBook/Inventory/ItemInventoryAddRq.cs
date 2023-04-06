namespace Znode.Engine.QuickBook
{
    /// <summary>
    /// Model required for QuickBook XML add inventory request node element
    /// </summary>
    public class ItemInventoryAddRq : BaseModel
    {
        public ItemInventoryAdd ItemInventoryAdd { get; set; }
    }
}