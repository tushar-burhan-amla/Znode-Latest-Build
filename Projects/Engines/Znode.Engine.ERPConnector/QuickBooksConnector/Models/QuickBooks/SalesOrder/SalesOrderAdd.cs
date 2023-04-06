using System.Xml.Serialization;

namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// Model required for QuickBooks XML add sales order node element
    /// </summary>
    public class SalesOrderAdd
    {
        public Ref CustomerRef { get; set; }
        public string TxnDate { get; set; }
        public string RefNumber { get; set; }
        public Address BillAddress { get; set; }
        public Address ShipAddress { get; set; }
        public Ref ItemSalesTaxRef { get; set; }
        public string Memo { get; set; }

        [XmlElement("SalesOrderLineAdd")]
        public SalesOrderLineAdd[] SalesOrderLineAdd { get; set; }
    }
}