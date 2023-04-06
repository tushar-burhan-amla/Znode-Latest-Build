namespace Znode.Libraries.Cloudflare
{
    public class CloudflareDomainModel
    {
        public int DomainId { get; set; }
        public string DomainName { get; set; }
        public int GlobalAttributeId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
        public string ApiKey { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int PortalId { get; set; }
        public string ApplicationType { get; set; }
        public string StoreName { get; set; }
        public string ZoneId { get; set; }
    }
}
