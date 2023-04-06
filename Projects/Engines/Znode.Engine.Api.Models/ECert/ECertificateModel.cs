namespace Znode.Engine.Api.Models
{
    public class ECertificateModel : BaseModel
    {
        public int ECertificateId { get; set; }
        public string IssuedByName { get; set; }
        public string IssuedById { get; set; }
        public string CertificateKey { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string CertificateType { get; set; }
        public string IssuedCYMD { get; set; }
        public decimal IssuedAmt { get; set; }
        public decimal Balance { get; set; }
        public string LastUsedCYMD { get; set; }
        public decimal RedemptionApplied { get; set; }
        public decimal CurrentBalance { get; set; }

        public string PortalId { get; set; }
    }
}
