namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// Model required for QuickBooks XML add sales order request node element
    /// </summary>
    public class SalesOrderAddRq : BaseModel
    {
        public SalesOrderAdd SalesOrderAdd { get; set; }
    }
}