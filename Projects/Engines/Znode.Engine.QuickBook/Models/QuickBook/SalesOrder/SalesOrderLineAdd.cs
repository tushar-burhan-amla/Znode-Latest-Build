namespace Znode.Engine.QuickBook
{
    /// <summary>
    /// Model required for QuickBook XML add sales order line items node element
    /// </summary>
    public class SalesOrderLineAdd
    {
        public Ref ItemRef { get; set; }
        public string Desc { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
    }
}