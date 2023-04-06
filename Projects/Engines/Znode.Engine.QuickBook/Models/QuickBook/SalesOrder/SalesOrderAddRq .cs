namespace Znode.Engine.QuickBook
{
    /// <summary>
    /// Model required for QuickBook XML add sales order request node element
    /// </summary>
    public class SalesOrderAddRq : BaseModel
    {
        public SalesOrderAdd SalesOrderAdd { get; set; }
    }
}