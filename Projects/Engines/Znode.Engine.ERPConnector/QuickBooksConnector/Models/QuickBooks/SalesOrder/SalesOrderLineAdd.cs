namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// Model required for QuickBooks XML add sales order line items node element
    /// </summary>
    public class SalesOrderLineAdd
    {
        public Ref ItemRef { get; set; }
        public string Desc { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
    }
}