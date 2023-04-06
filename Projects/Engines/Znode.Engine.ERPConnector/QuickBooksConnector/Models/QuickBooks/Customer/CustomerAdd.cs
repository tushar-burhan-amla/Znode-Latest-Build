namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// Model required for QuickBooks XML add customer node element
    /// </summary>
    public class CustomerAdd
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Address BillAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
    }
}