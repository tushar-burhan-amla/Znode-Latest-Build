namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// Model required for QuickBooks XML node element related to all address related information
    /// </summary>
    public class Address
    {
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}